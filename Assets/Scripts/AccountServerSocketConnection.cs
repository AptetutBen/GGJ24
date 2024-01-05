using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class AccountServerSocketConnection 
{

    public AccountServerSocketConnection(){

    }

    public async void ConnectToAccountServer(){

        using TcpClient client = new();
        await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 7776);
        await using NetworkStream stream = client.GetStream();

        var buffer = new byte[1024];
        int received = await stream.ReadAsync(buffer);

        var message = Encoding.UTF8.GetString(buffer, 0, received);
        UnityEngine.Debug.Log($"Message received: \"{message}\"");
    }
}