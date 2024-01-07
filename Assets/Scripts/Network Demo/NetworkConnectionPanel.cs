using System;
using System.Text.RegularExpressions;
using noWeekend;
using TMPro;
using UnityEngine;

public class NetworkConnectionPanel : MonoBehaviour
{
    public TMP_InputField addressInputField;
    public TMP_InputField portInputField;
	public TMP_InputField playerNameInputField;
    public TouchButton confirmButton;
    public FlexibleColorPicker colourPicker;
	public WeekendTween tween;
    private bool isHost;

    private void Start()
    {
#if UNITY_EDITOR
        addressInputField.text = "127.0.0.1";
#endif
    }

    public void Show(bool isHost)
    {
        this.isHost = isHost;
        addressInputField.gameObject.SetActive(!isHost);
        portInputField.gameObject.SetActive(!isHost);

        colourPicker.color = Tools.RandomColour();

        confirmButton.UpdateText(isHost ? "Create" : "Connect");

        if(playerNameInputField.text == "")
        {
            playerNameInputField.text = Tools.GenerateRandomName();
		}

		tween.Activate();
    }

    public void Hide(Action andThen)
    {
		tween.Deactivate(andThen);
	}

    public void OnConnectClientButtonPress()
    {
		string playerName = playerNameInputField.text;

		if (string.IsNullOrEmpty(playerName))
		{
            playerName = Tools.GenerateRandomName();
		}

        if (isHost)
        {
            GameFlowController.SetHost(playerName, colourPicker.color);
            return;
		}

		string ipAddress = addressInputField.text;
        string portString = portInputField.text;
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

        GameFlowController.SetClient(ipAddress, portShort, playerName,colourPicker.color);
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
