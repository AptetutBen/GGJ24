using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using noWeekend;
using System;

public class ButtonsPanel : MonoBehaviour
{
	public WeekendTween tween = null;

	public void Show(Action andThen = null)
	{
		tween.Activate(andThen);
	}

	public void Hide(Action andThen = null)
	{
		tween.Deactivate(andThen);
	}
}
