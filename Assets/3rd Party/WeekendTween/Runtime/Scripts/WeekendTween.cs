using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace noWeekend
{
	public class WeekendTween : MonoBehaviour
    {
        public enum EaseActionType { Position, Rotation, Scale, Rect, CanvasAlpha}
        public Transform targetTransform;

        [SerializeReference] public List<EaseAction> activateEaseActions = new List<EaseAction>();
        [SerializeReference] public List<EaseAction> deactivateEaseActions = new List<EaseAction>();

        public bool activateOnEnable;
        public bool initiliseActiveState = true;
		public bool hideOnEnable;
		public bool hideAfterDisable;
		public bool useUnscaledTime;

		private bool isActive;

		public bool IsActive => isActive;

		private List<CanvasRenderer> canvasRenderers;
		private List<Image> images;

		public UnityEvent onActivateCompleteAction,onDeactivateCompleteAction;
        private Coroutine onActivateCoroutine, onDeactivateCoroutine;

        public float LongestActivateTween => activateEaseActions.Max(item => item.Duration);
		public float LongestDeactivateTween => deactivateEaseActions.Max(item => item.Duration);

        private float timer;

        public float EditorTimer => timer;

		public float ActivateEditorTimer
        {
            set
            {
                if (activateEaseActions.Count == 0) return;
                timer = value % LongestActivateTween;
            }
        }

		public float DeactivateEditorTimer
		{
			set
			{
				if (deactivateEaseActions.Count == 0) return;
				timer = value % LongestDeactivateTween;
			}
		}

        //On enable of the object
		private void OnEnable()
        {
            if (initiliseActiveState) InitiliseActiveState();

			if (hideOnEnable)
			{
				Hide();
			}

			if (activateOnEnable)Activate();
        }

		//Hide all image and renderers
		public void Hide()
		{
			GetAllRenderersIfNeeded();

            foreach (CanvasRenderer cRenderers in canvasRenderers)
            {
				cRenderers.cull = true;
			}

			foreach (Image image in images)
			{
				image.enabled = false;
			}
		}

		//Unhide all images and renderers
		public void Show()
		{

			GetAllRenderersIfNeeded();

			foreach (CanvasRenderer cRenderers in canvasRenderers)
			{
				cRenderers.cull = false;
			}

			foreach (Image image in images)
			{
				image.enabled = true;
			}

		}

        /// <summary>
        /// Sets the target transform to the initial state of any Activate ease Actions
        /// </summary>
        public void InitiliseActiveState()
        {
            foreach (EaseAction easeAction in activateEaseActions)
            {
                if (!IsEaseActionFirstOfType(activateEaseActions, easeAction)) continue;

                easeAction.SetStart(targetTransform);
			}
        }

        //Checks if a given ease action is the first of its type in a sequence
        private bool IsEaseActionFirstOfType(List<EaseAction> easeActions,EaseAction easeAction)
        {
			EaseAction[] actions = easeActions.Where(item => item.EaseActionType == easeAction.EaseActionType).ToArray();

			EaseAction firstOfTypeAction = easeActions.Where(item => item.EaseActionType == easeAction.EaseActionType).OrderBy(item => item.delay).First();

            return easeAction == firstOfTypeAction;
		}

		//Checks if a given ease action is the first of its type in a sequence
		private bool IsEaseActionLastOfType(List<EaseAction> easeActions, EaseAction easeAction)
		{
			EaseAction lastOfTypeAction = easeActions.Where(item => item.EaseActionType == easeAction.EaseActionType).Aggregate((max, next) => next.delay + next.duration > max.delay + max.duration ? next : max);

			return easeAction == lastOfTypeAction;
		}

		//Change the start postion of a Position Ease action
		public void ChangeStartPositionsOfEase(Vector3 startPosition)
		{
			//Get the first position ease action of activate actions
			EaseAction easeActionActivate = activateEaseActions.FirstOrDefault(item => item.EaseActionType == WeekendTween.EaseActionType.Position);

			if(easeActionActivate != null)
			{
				((PositionEaseAction)easeActionActivate).fromPosition = startPosition;
			}

			//Get the first position ease action of activate actions
			EaseAction easeActionDeactivate = deactivateEaseActions.FirstOrDefault(item => item.EaseActionType == WeekendTween.EaseActionType.Position);

			if (easeActionDeactivate != null)
			{
				((PositionEaseAction)easeActionDeactivate).toPosition = startPosition;
			}
		}

		//Change the end postion of a Position Ease action
		public void ChangeEndPositionsOfEase(Vector3 endPosition)
		{
			//Get the first position ease action of activate actions
			EaseAction easeActionActivate = activateEaseActions.FirstOrDefault(item => item.EaseActionType == WeekendTween.EaseActionType.Position);

			if (easeActionActivate != null)
			{
				((PositionEaseAction)easeActionActivate).toPosition = endPosition;
			}

			//Get the first position ease action of activate actions
			EaseAction easeActionDeactivate = deactivateEaseActions.FirstOrDefault(item => item.EaseActionType == WeekendTween.EaseActionType.Position);

			if (easeActionDeactivate != null)
			{
				((PositionEaseAction)easeActionDeactivate).fromPosition = endPosition;
			}
		}

		// -----------------------------------------------------------------
		// Add - Remove Ease Actions 

		public void RemoveEaseAction(EaseAction easeAction, bool isActivateEase)
        {
			if (isActivateEase)
			{
				activateEaseActions.Remove(easeAction);
			}
			else
			{
				deactivateEaseActions.Remove(easeAction);
			}
		}

        public void OnAddPositionEaseButtonPress(bool isActivateEase)
        {
            if (isActivateEase)
            {
                activateEaseActions.Add(new PositionEaseAction(targetTransform.localPosition));
            }
            else
            {
                deactivateEaseActions.Add(new PositionEaseAction(targetTransform.localPosition));
            }
        }

        public void OnAddRotationEaseButtonPress(bool isActivateEase)
        {
            if (isActivateEase)
            {
                activateEaseActions.Add(new RotateEaseAction(targetTransform.localRotation));
            }
            else
            {
                deactivateEaseActions.Add(new RotateEaseAction(targetTransform.localRotation));
            }
            
        }

        public void OnAddScaleEaseButtonPress(bool isActivateEase)
        {
            if (isActivateEase)
            {
                activateEaseActions.Add(new ScaleEaseAction(targetTransform.localScale));
            }
            else
            {
                deactivateEaseActions.Add(new ScaleEaseAction(targetTransform.localScale));
            }
        }

		public void OnAddRectEaseButtonPress(bool isActivateEase)
		{
			if (isActivateEase)
			{
				activateEaseActions.Add(new RectEaseAction(((RectTransform)targetTransform).rect));
			}
			else
			{
				deactivateEaseActions.Add(new RectEaseAction(((RectTransform)targetTransform).rect));
			}
		}

		//Stop all tweens if they are currently happening
		private void StopAllTweens()
		{
			if (onDeactivateCoroutine != null) StopCoroutine(onDeactivateCoroutine);
			if (onActivateCoroutine != null) StopCoroutine(onActivateCoroutine);
		}

		// -----------------------------------------------------------------
		// Activate Eases

		public void Activate() => Activate(null);

		public void Activate(Action onComplete = null, float stopMotionStart = 0)
		{
			Activate(onComplete);
		}
		
        public void Activate(Action onComplete = null)
        {
			StopAllTweens();
			if (activateEaseActions.Count == 0)return;
			onActivateCoroutine = StartCoroutine(ActivateCoroutine(onComplete));
        }

		public IEnumerator ActivateCoroutine(Action onComplete = null)
		{
			if (hideOnEnable)
			{
				Show();
			}

			//Get the length of the longest tween (including delay)
			float longestTween = LongestActivateTween;

			//Loop through each active tween 
			timer = 0;

			while (timer < longestTween)
			{
				
				foreach (EaseAction easeAction in activateEaseActions)
				{
					easeAction.Process(targetTransform, timer);
				}
				
				timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}

			//Finish all Tweens
			foreach (EaseAction easeAction in activateEaseActions)
			{
				if(IsEaseActionLastOfType(activateEaseActions, easeAction))
				{
					easeAction.SetEnd(targetTransform);
				}
			}

			//Run any on complete Actions
			onActivateCompleteAction?.Invoke();
			onComplete?.Invoke();
			isActive = true;
		}

		// -----------------------------------------------------------------
		// Deactivate Eases

		//Deactivate Ease actions 
		public void Deactivate() => Deactivate(null);

		public void Deactivate(Action onComplete = null, float stopMotionStart = 0)
		{
			Deactivate(onComplete);
		}
		
		//Deactivate Ease actions with callback
		public void Deactivate(Action onComplete = null)
        {
            StopAllTweens();
            if(deactivateEaseActions.Count == 0) return;

			if (gameObject.activeSelf)
			{
				StartCoroutine(DeactivateCoroutine(onComplete));
			}

        }

		//Deactivate Ease actions coroutine
		public IEnumerator DeactivateCoroutine(Action onComplete = null)
		{
			isActive = false;

			//Get the length of the longest tween (including delay)
			float longestTween = LongestDeactivateTween;

			//Loop through each active tween 
			float timer = 0;

			while (timer < longestTween)
			{
				foreach (EaseAction easeAction in deactivateEaseActions)
				{
					easeAction.Process(targetTransform, timer);
				}
				
				timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

				yield return null;
			}

			//Finish all Tweens
			foreach (EaseAction easeAction in deactivateEaseActions)
			{
				if (IsEaseActionLastOfType(deactivateEaseActions, easeAction))
				{
					easeAction.SetEnd(targetTransform);
				}
			}

			//Run any on complete Actions
			onDeactivateCompleteAction?.Invoke();
			onComplete?.Invoke();

			if (hideAfterDisable)
			{
				Hide();
			}
		}

		// -----------------------------------------------------------------
		// Helper funcitons

		//Find all renderers as children of the targetTransform
		private void GetAllRenderersIfNeeded()
		{
			if (canvasRenderers != null) return;

			RebuildRederers();
		}

		//Compliles a list of renders and images to hide if needed
		public void RebuildRederers()
		{
			canvasRenderers = new();
			images = new();
			recursiveCheck(targetTransform);

			void recursiveCheck(Transform checkingTransform)
			{
                foreach (CanvasRenderer cRenderer in checkingTransform.GetComponentsInChildren<CanvasRenderer>())
                {
					canvasRenderers.Add(cRenderer);
                }
				
				foreach (Image image in checkingTransform.GetComponentsInChildren<Image>())
				{
					images.Add(image);
				}

				foreach (Transform child in checkingTransform)
				{
					recursiveCheck(child);
				}
			}
		}
	}
}

