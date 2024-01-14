using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using noWeekend;
using TMPro;
using System;

public class NetworkErrorPanel : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI messageText;
    private Action andThen;

    public WeekendTween tween;

    public void Show(string message, Action andThen)
    {
        gameObject.SetActive(true);
        messageText.text = message;
		this.andThen = andThen;
		tween.Activate();
	}

    public void Hide()
    {
		tween.Deactivate();
	}

    public void OnOkButtonPress()
    {
		tween.Deactivate(() => andThen?.Invoke());
	}
}
