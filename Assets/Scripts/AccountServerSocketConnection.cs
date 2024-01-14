using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class AccountServerSocketConnection 
{
    object queuLock = new object();
    private static Queue<byte[]> readQueue = new Queue<byte[]>();
    public AccountServerSocketConnection(){

    }

    byte[] receiveBuffer = new byte[1024000];
    byte[] reconstructBuffer = new byte[10240000]; // 10 megabytes
    NetworkStream openConnection = null;
    CancellationTokenSource cancelSource;

    public void ConnectToAccountServer(string sessionToken, Action<AccountServerState> onConnectionStateChange){
        cancelSource = new CancellationTokenSource();
        Task task = Task.Run(() => ConnectToAccountServerTask(sessionToken, onConnectionStateChange, cancelSource.Token), cancelSource.Token);
    }

    public async void ConnectToAccountServerTask(string sessionToken, Action<AccountServerState> onConnectionStateChange, CancellationToken cancelToken){
        
        onConnectionStateChange(AccountServerState.Connecting);
        using TcpClient client = new();
        IPAddress[] address = await Dns.GetHostAddressesAsync("ggj24.games.luisvsm.com");
        await client.ConnectAsync(address, 7776);
        await using NetworkStream stream = client.GetStream();
        openConnection = stream;

        stream.Write(Encoding.ASCII.GetBytes(sessionToken + ";"));
        int infiniteLoopCatcherLol = 10000;

        try{
            int received = await stream.ReadAsync(receiveBuffer, cancelToken);
            int readPos = 0;
            int bytesToRead = 0;
            int reconstructPos = 0;

            ushort nextMessageLength = 0;
            onConnectionStateChange(AccountServerState.Connected);
            while(received > 0 && infiniteLoopCatcherLol > 0){ 
                // New message!
                // UnityEngine.Debug.Log($"Got TCP message of length: {received}");
                readPos = 0;
                
                while(readPos < received && infiniteLoopCatcherLol > 0){
                    infiniteLoopCatcherLol--; // Avoid an infinite loop! D:

                    // The first two bytes of each message will be a UInt16 ( The NodeJS side prepares it with writeUInt16LE )
                    if(nextMessageLength == 0){
                        nextMessageLength = BitConverter.ToUInt16(receiveBuffer, readPos);
                        // UnityEngine.Debug.Log($"nextMessageLength: {nextMessageLength}");
                        reconstructPos = 0;
                        readPos += 2; 
                        received = received - 2; // We just read 2 bytes so we need to take 2 off received before the code below runs
                    }

                    // Either the expected message length or the number of bytes received, whichever is lower
                    bytesToRead = Mathf.Min(nextMessageLength - reconstructPos, received);
                    if(bytesToRead == 0)
                        break;

                    
                    // The message might be split over multiple receive reads
                    // Or we could have multiple messages in one receive read
                    // So copy the receieved bytes into a reconstruction buffer that we can use to reconstruct to handle those cases.
                    // UnityEngine.Debug.Log($"Copying {bytesToRead} bytes to reconstructBuffer receiveBuffer[{readPos}] reconstructBuffer[{reconstructPos}]");
                    Array.Copy(receiveBuffer, readPos,  reconstructBuffer, reconstructPos, bytesToRead);

                    // Add the number of bytes that we just read into the reconstruction index position
                    reconstructPos += bytesToRead;
                    // Check if we've reconstructed enough bytes to read the complete message
                    if(reconstructPos == nextMessageLength){
                        // UnityEngine.Debug.Log($"Queuing message");
                        
                        // Queue the message and reset the counters
                        byte[] bufferToQueue = new byte[nextMessageLength];
                        Array.Copy(reconstructBuffer, bufferToQueue, nextMessageLength);
                        lock (queuLock)
                        {
                            readQueue.Enqueue(bufferToQueue);
                        }

                        // UnityEngine.Debug.Log($"Reset counters");
                        // Now that we've read the message we need to reset nextMessageLength to 0
                        // so that the next time this while loop executes it'll read the next message length again and start the process over again
                        nextMessageLength = 0; 
                        reconstructPos = 0;

                        // If we're processing messages then we're not in an infinite loop so reset the counter
                        // This is mainly in case we fuck something up in Unity Editor so that it gracefully exits instead of an infinite loop
                        infiniteLoopCatcherLol = 10000;
                    }

                    readPos += bytesToRead;
                }

                // Read from the stream again
                received = await stream.ReadAsync(receiveBuffer, cancelToken);
            }
        }
        catch (System.IO.IOException err){
            // This exception happens if we use the cancellation token to
            // stop reading from the TCP stream before the server returns some data
            // (There's a blocked background thread until we receive data and this is how we kill that thread)
            WeekendLogger.LogNetworkServer(err);
        }catch(Exception err){
			// Catch all error that should be investigated if it shows up
			WeekendLogger.LogWarning(err);
        }

        if(infiniteLoopCatcherLol == 0){
			WeekendLogger.LogError("InfiniteLoop was caught");
        }

        // Okay so we're done with the TCP connection at this point
        // Clean everything up and empty the read queue
        lock (queuLock)
        {
            readQueue = new Queue<byte[]>();
        }

        CloseConnectionToAccountServer();
		WeekendLogger.LogNetworkServer("Connection to account server closed.");
        onConnectionStateChange(AccountServerState.NotConnected);
     }

    public void CloseConnectionToAccountServer(){
        cancelSource.Cancel();

        if(openConnection != null){
            openConnection.Close();
            openConnection = null;
        }
    }

    public bool ThereThingsToReadFromQueue(){
        return readQueue.Count > 0;
    }

    // The TCP connection runs on a background thread
    // We need to get back to Unity's main thread in order to interact with game code safely
    // So the idea is ReadFromQueue() would be called from Update()
    // Because we know that Update() is run by Unity's main thread
    public byte[] ReadFromQueue(){
        byte[] returnData;

        lock (queuLock)
        {
            // UnityEngine.Debug.Log($"readQueue.Count: {readQueue.Count}");
            if(readQueue.Count > 0){
                returnData = readQueue.Dequeue();
            // UnityEngine.Debug.Log($"readQueue.Count after dequeue: {readQueue.Count}");
            }else{
                returnData = null;
            }
        }

        return returnData;
    }

    public bool SendMessage(AccountServerRequest request){
        if(openConnection == null)
            return false;
            
        UnityEngine.Debug.Log("Sending messasge");
        openConnection.Write(Encoding.ASCII.GetBytes(request.ToJSON()));

        return true;
    }
}