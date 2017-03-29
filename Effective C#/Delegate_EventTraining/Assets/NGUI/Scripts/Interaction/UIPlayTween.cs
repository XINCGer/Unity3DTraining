//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;
using System.Collections.Generic;

/// <summary>
/// Play the specified tween on click.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Play Tween")]
public class UIPlayTween : MonoBehaviour
{
	static public UIPlayTween current;

	/// <summary>
	/// Target on which there is one or more tween.
	/// </summary>

	public GameObject tweenTarget;

	/// <summary>
	/// If there are multiple tweens, you can choose which ones get activated by changing their group.
	/// </summary>

	public int tweenGroup = 0;

	/// <summary>
	/// Which event will trigger the tween.
	/// </summary>

	public Trigger trigger = Trigger.OnClick;

	/// <summary>
	/// Direction to tween in.
	/// </summary>

	public Direction playDirection = Direction.Forward;

	/// <summary>
	/// Whether the tween will be reset to the start or end when activated. If not, it will continue from where it currently is.
	/// </summary>

	public bool resetOnPlay = false;

	/// <summary>
	/// Whether the tween will be reset to the start if it's disabled when activated.
	/// </summary>

	public bool resetIfDisabled = false;

	/// <summary>
	/// What to do if the tweenTarget game object is currently disabled.
	/// </summary>

	public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

	/// <summary>
	/// What to do with the tweenTarget after the tween finishes.
	/// </summary>

	public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

	/// <summary>
	/// Whether the tweens on the child game objects will be considered.
	/// </summary>

	public bool includeChildren = false;

	/// <summary>
	/// Event delegates called when the animation finishes.
	/// </summary>

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Deprecated functionality, kept for backwards compatibility
	[HideInInspector][SerializeField] GameObject eventReceiver;
	[HideInInspector][SerializeField] string callWhenFinished;

	UITweener[] mTweens;
	bool mStarted = false;
	int mActive = 0;
	bool mActivated = false;

	void Awake ()
	{
		// Remove deprecated functionality if new one is used
		if (eventReceiver != null && EventDelegate.IsValid(onFinished))
		{
			eventReceiver = null;
			callWhenFinished = null;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	void Start()
	{
		mStarted = true;

		if (tweenTarget == null)
		{
			tweenTarget = gameObject;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mStarted) OnHover(UICamera.IsHighlighted(gameObject));

		if (UICamera.currentTouch != null)
		{
			if (trigger == Trigger.OnPress || trigger == Trigger.OnPressTrue)
				mActivated = (UICamera.currentTouch.pressed == gameObject);

			if (trigger == Trigger.OnHover || trigger == Trigger.OnHoverTrue)
				mActivated = (UICamera.currentTouch.current == gameObject);
		}

		UIToggle toggle = GetComponent<UIToggle>();
		if (toggle != null) EventDelegate.Add(toggle.onChange, OnToggle);
	}

	void OnDisable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		UIToggle toggle = GetComponent<UIToggle>();
		if (toggle != null) EventDelegate.Remove(toggle.onChange, OnToggle);
	}

	void OnDragOver () { if (trigger == Trigger.OnHover) OnHover(true); }

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnHover ||
				(trigger == Trigger.OnHoverTrue && isOver) ||
				(trigger == Trigger.OnHoverFalse && !isOver))
			{
				if (isOver == mActivated) return;

				// Hover out action happened on a child object -- we want to maintain the hovered state
				if (!isOver && UICamera.hoveredObject != null && UICamera.hoveredObject.transform.IsChildOf(transform))
				{
					// Subscribe to a global hover listener so we can keep receiving hover notifications
					UICamera.onHover += CustomHoverListener;
					isOver = true;
					if (mActivated) return;
				}

				mActivated = isOver && (trigger == Trigger.OnHover);
				Play(isOver);
			}
		}
	}

	/// <summary>
	/// Wait for the hover event to happen outside the object's hierarchy before removing the hovered state.
	/// </summary>

	void CustomHoverListener (GameObject go, bool isOver)
	{
		if (!this) return;
		var myGo = gameObject;
		var hover = myGo && go && (go == myGo || go.transform.IsChildOf(transform));

		if (!hover)
		{
			OnHover(false);
			UICamera.onHover -= CustomHoverListener;
		}
	}

	void OnDragOut ()
	{
		if (enabled && mActivated)
		{
			mActivated = false;
			Play(false);
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnPress ||
				(trigger == Trigger.OnPressTrue && isPressed) ||
				(trigger == Trigger.OnPressFalse && !isPressed))
			{
				mActivated = isPressed && (trigger == Trigger.OnPress);
				Play(isPressed);
			}
		}
	}

	void OnClick ()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			Play(true);
		}
	}

	void OnDoubleClick ()
	{
		if (enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(true);
		}
	}

	void OnSelect (bool isSelected)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnSelect ||
				(trigger == Trigger.OnSelectTrue && isSelected) ||
				(trigger == Trigger.OnSelectFalse && !isSelected))
			{
				mActivated = isSelected && (trigger == Trigger.OnSelect);
				Play(isSelected);
			}
		}
	}

	void OnToggle ()
	{
		if (!enabled || UIToggle.current == null) return;
		if (trigger == Trigger.OnActivate ||
			(trigger == Trigger.OnActivateTrue && UIToggle.current.value) ||
			(trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
			Play(UIToggle.current.value);
	}

	void Update ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (disableWhenFinished != DisableCondition.DoNotDisable && mTweens != null)
		{
			bool isFinished = true;
			bool properDirection = true;

			for (int i = 0, imax = mTweens.Length; i < imax; ++i)
			{
				UITweener tw = mTweens[i];
				if (tw.tweenGroup != tweenGroup) continue;

				if (tw.enabled)
				{
					isFinished = false;
					break;
				}
				else if ((int)tw.direction != (int)disableWhenFinished)
				{
					properDirection = false;
				}
			}

			if (isFinished)
			{
				if (properDirection) NGUITools.SetActive(tweenTarget, false);
				mTweens = null;
			}
		}
	}

	/// <summary>
	/// Activate the tweeners.
	/// </summary>

	public void Play (bool forward)
	{
		mActive = 0;
		GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;

		if (!NGUITools.GetActive(go))
		{
			// If the object is disabled, don't do anything
			if (ifDisabledOnPlay != EnableCondition.EnableThenPlay) return;

			// Enable the game object before tweening it
			NGUITools.SetActive(go, true);
		}

		// Gather the tweening components
		mTweens = includeChildren ? go.GetComponentsInChildren<UITweener>() : go.GetComponents<UITweener>();

		if (mTweens.Length == 0)
		{
			// No tweeners found -- should we disable the object?
			if (disableWhenFinished != DisableCondition.DoNotDisable)
				NGUITools.SetActive(tweenTarget, false);
		}
		else
		{
			bool activated = false;
			if (playDirection == Direction.Reverse) forward = !forward;

			// Run through all located tween components
			for (int i = 0, imax = mTweens.Length; i < imax; ++i)
			{
				UITweener tw = mTweens[i];

				// If the tweener's group matches, we can work with it
				if (tw.tweenGroup == tweenGroup)
				{
					// Ensure that the game objects are enabled
					if (!activated && !NGUITools.GetActive(go))
					{
						activated = true;
						NGUITools.SetActive(go, true);
					}

					++mActive;

					// Toggle or activate the tween component
					if (playDirection == Direction.Toggle)
					{
						// Listen for tween finished messages
						EventDelegate.Add(tw.onFinished, OnFinished, true);
						tw.Toggle();
					}
					else
					{
						if (resetOnPlay || (resetIfDisabled && !tw.enabled))
						{
							tw.Play(forward);
							tw.ResetToBeginning();
						}
						// Listen for tween finished messages
						EventDelegate.Add(tw.onFinished, OnFinished, true);
						tw.Play(forward);
					}
				}
			}
		}
	}

	/// <summary>
	/// Callback triggered when each tween executed by this script finishes.
	/// </summary>

	void OnFinished ()
	{
		if (--mActive == 0 && current == null)
		{
			current = this;
			EventDelegate.Execute(onFinished);

			// Legacy functionality
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
				eventReceiver.SendMessage(callWhenFinished, SendMessageOptions.DontRequireReceiver);

			eventReceiver = null;
			current = null;
		}
	}
}
