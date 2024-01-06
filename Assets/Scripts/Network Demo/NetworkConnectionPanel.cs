using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class NetworkConnectionPanel : MonoBehaviour
{
    public TMP_InputField addressInputField;
    public TMP_InputField portInputField;
    public TMP_InputField playerNameInputField;

    private void Start()
    {
        //addressInputField.text = networkController.IPAddress;
        //portInputField.text = networkController.Port;
#if UNITY_EDITOR
        addressInputField.text = "127.0.0.1";
#endif
    }

    public void OnHostButtonPress()
    {
        GameFlowController.SetHost();
    }

    public void OnConnectClientButtonPress()
    {
        string ipAddress = addressInputField.text;
        string portString = portInputField.text;
        string playerName = playerNameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Non Valid playerName");
            return;
        }

        ushort portShort;

        if (!ushort.TryParse(portString, out portShort) || !IsValidPortNumber(portShort))
        {
            Debug.LogError("Non Valid port number");
            return;
        }

        if (!IsValidIpAddress(ipAddress))
        {
            Debug.LogError("Non Valid IP address");
            return;
        }

        GameFlowController.SetClient(ipAddress, portShort, playerName);
    }

    private bool IsValidIpAddress(string ipAddress)
    {
        string pattern = @"^(25[0-5]|2[0-4][0-9]|[0-1]?[0-9]?[0-9])(\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9]?[0-9])){3}$";
        Regex regex = new Regex(pattern);

        return regex.IsMatch(ipAddress);
    }

    private bool IsValidPortNumber(ushort port)
    {
        return port >= 1 && port <= 65535;
    }
}
