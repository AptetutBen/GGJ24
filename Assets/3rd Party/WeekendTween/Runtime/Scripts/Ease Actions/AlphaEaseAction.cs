using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

namespace noWeekend
{
	[System.Serializable]
	public class AlphaEaseAction : EaseAction
	{
		public override string EaseName => "Ease Position";
		public Vector3 fromPosition;
		public Vector3 toPosition;
		public override WeekendTween.EaseActionType EaseActionType => WeekendTween.EaseActionType.Position;

		public AlphaEaseAction(Vector3 positions)
		{
			this.fromPosition = Vector3Int.RoundToInt(positions);
			this.toPosition = Vector3Int.RoundToInt(positions);
		}

		public AlphaEaseAction(Vector3 fromPosition, Vector3 toPosition)
		{
			this.fromPosition = Vector3Int.RoundToInt(fromPosition);
			this.toPosition = Vector3Int.RoundToInt(toPosition);
		}

		protected override void ApplyEase(Transform targetTransform, float value)
		{
			targetTransform.localPosition = Vector3.LerpUnclamped(fromPosition, toPosition, Ease.GetEase(easeType, value));
		}
		protected override void ApplyEase(EaseRefererceTransform targetTransform, float value)
		{
			float lossyScale = targetTransform.lossyScale.x;

			targetTransform.localPosition = Vector3.LerpUnclamped(fromPosition * lossyScale, toPosition * lossyScale, Ease.GetEase(easeType, value));
		}

		public override void SetStart(Transform targetTransform)
		{
			targetTransform.localPosition = fromPosition;
		}

		public override void SetStart(EaseRefererceTransform targetTransform)
		{
			targetTransform.localPosition = fromPosition;
		}

		public override void SetEnd(Transform targetTransform)
		{
			targetTransform.localPosition = toPosition;
		}
		public override void SetEnd(EaseRefererceTransform targetTransform)
		{
			targetTransform.localPosition = toPosition;
		}

		public void SetFromPositionToCurrent(Transform targetTransform)
		{
			fromPosition = targetTransform.localPosition;

		}

		public void SetToPositionToCurrent(Transform targetTransform)
		{
			toPosition = targetTransform.localPosition;
		}
	}
}