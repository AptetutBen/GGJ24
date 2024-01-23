using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;

public class PotatoWebServer
{
    private TcpListener myListener;
    private int port = 5050;
    private IPAddress localAddr = IPAddress.Parse("0.0.0.0");

    // Okay so why the heck are we running a web server in Unity?
    // Well!! Basically because it'll make hosting it in Kubernetes a lot easier
    // I don't want to have to deal with persistence, so!
    // Each game server can be queried to see what state it's in
    // Eg how many players are in it etc
    // That way we can do stuffff like clean up empty servers and not allocate people to servers that already have people in them!
    // At least that's the plan, you tell me if it worked lol

    // Also ps it's from here: https://iamsimi.medium.com/writing-a-web-server-in-c-12b93134082b
    // And made into a task
    // Okay cool let's doooooooo

    public void StartWebServerWeeewwwww(CancellationToken cancelToken)
    {
        try
        {
            myListener = new TcpListener(localAddr, port);
            myListener.Start();
            Debug.Log($"Web Server Running on {localAddr.ToString()} on port {port}");
            
            Task task = Task.Run(() => StartListening(cancelToken), cancelToken);
        }

        catch (System.Exception err)
        {
            Debug.LogError(err);
        }
    }

    private async void StartListening(CancellationToken cancelToken)
    {
        while (true)
        {
            
            TcpClient client = myListener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            //read request 
            byte[] requestBytes = new byte[1024];
            int bytesRead = await stream.ReadAsync(requestBytes, 0, requestBytes.Length, cancelToken);

            string request = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);
            var requestHeaders = ParseHeaders(request);

            string[] requestFirstLine = requestHeaders.requestType.Split(" ");

            if (!request.StartsWith("GET"))
            {
                SendHeaders(405, "Method Not Allowed, please don't.", ref stream);
            }
            else
            {
                string requestedPath = requestFirstLine[1];
                if(requestedPath == "/info")
                {
                    SendHeaders(200, "OK", ref stream);
                    stream.Write(Encoding.ASCII.GetBytes(GetServerInfo()));
                }
                else
                {
                    SendHeaders(404, "Page Not Found", ref stream);
                }
            }

            client.Close();
        }
    }

    private string GetServerInfo(){
        return "{}";
    }

    private void SendHeaders(int statusCode, string statusMsg, ref NetworkStream networkStream)
    {
        string responseHeaderBuffer = "";

        responseHeaderBuffer = $"HTTP/1.1 {statusCode} {statusMsg}\r\n" +
            $"Date: {DateTime.UtcNow.ToString()}\r\n" +
            $"Content-Type: application/json\r\n\r\n";

        byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaderBuffer);
        networkStream.Write(responseBytes, 0, responseBytes.Length);
    }

    private (Dictionary<string, string> headers, string requestType) ParseHeaders(string headerString)
    {
        var headerLines = headerString.Split('\r', '\n');
        string firstLine = headerLines[0];
        var headerValues = new Dictionary<string, string>();
        foreach (var headerLine in headerLines)
        {
            var headerDetail = headerLine.Trim();
            var delimiterIndex = headerLine.IndexOf(':');
            if (delimiterIndex >= 0)
            {
                var headerName = headerLine.Substring(0, delimiterIndex).Trim();
                var headerValue = headerLine.Substring(delimiterIndex + 1).Trim();
                headerValues.Add(headerName, headerValue);
            }
        }
        return (headerValues, firstLine);
    }
}