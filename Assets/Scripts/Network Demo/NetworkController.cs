using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Linq;
using System;
#if UNITY_EDITOR
using ParrelSync;
#endif
public class NetworkController : MonoBehaviour
{
    public static NetworkController instance;
    private NetworkManager networkManager;
    private UnityTransport unityTransport;

    public string CurrentIPAddress => unityTransport.ConnectionData.Address;
    public int Port => unityTransport.ConnectionData.Port;
    public bool IsConnectedClient => networkManager.IsConnectedClient;
    public bool IsConnectedHost => networkManager.IsHost;
    public string LocalIP => GetLocalIPv4(NetworkInterfaceType.Ethernet);

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        unityTransport = GetComponent<UnityTransport>();

    }

    private void OnServerStarted()
    {

    }

    private void OnServerStopped(bool _)
    {

    }

    private void Start()
    {
        IsDefaultServer();
        networkManager = NetworkManager.Singleton;
        networkManager.OnServerStarted += OnServerStarted;
        networkManager.OnServerStopped += OnServerStopped;
    }

    public void StartHost()
    {
        networkManager.StartHost();
    }

    public void StartServer()
    {
        networkManager.StartServer();
    }

    public void StartClient()
    {
        StartClient(GameFlowController.ipAddress, GameFlowController.host);
    }

    public void StartClient(string ip, ushort port)
    {
        unityTransport.ConnectionData.Address = ip;
        unityTransport.ConnectionData.Port = port;
        networkManager.StartClient();
    }

    private string GetLocalIPv4(NetworkInterfaceType _type)
    {  // Checks your IP adress from the local network connected to a gateway. This to avoid issues with double network cards
        string output = "";  // default output
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) // Iterate over each network interface
        {  // Find the network interface which has been provided in the arguments, break the loop if found
            if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
            {   // Fetch the properties of this adapter
                IPInterfaceProperties adapterProperties = item.GetIPProperties();
                // Check if the gateway adress exist, if not its most likley a virtual network or smth
                if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                {   // Iterate over each available unicast adresses
                    foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                    {   // If the IP is a local IPv4 adress
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {   // we got a match!
                            output = ip.Address.ToString();
                            break;  // break the loop!!
                        }
                    }
                }
            }
            // Check if we got a result if so break this method
            if (output != "") { break; }
        }
        // Return results
        return output;
    }

    public bool IsDefaultServer()
    {
#if UNITY_EDITOR
        return !ClonesManager.IsClone();
#else
        return false;
#endif

    }
}

