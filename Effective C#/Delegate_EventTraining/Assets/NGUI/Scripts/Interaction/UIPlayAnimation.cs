//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using AnimationOrTween;

/// <summary>
/// Play the specified animation on click.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Play Animation")]
public class UIPlayAnimation : MonoBehaviour
{
	static public UIPlayAnimation current = null;

	/// <summary>
	/// Target animation to activate.
	/// </summary>

	public Animation target;

	/// <summary>
	/// Target animator system.
	/// </summary>

	public Animator animator;

	/// <summary>
	/// Optional clip name, if the animation has more than one clip.
	/// </summary>

	public string clipName;

	/// <summary>
	/// Which event will trigger the animation.
	/// </summary>

	public Trigger trigger = Trigger.OnClick;

	/// <summary>
	/// Which direction to animate in.
	/// </summary>

	public Direction playDirection = Direction.Forward;

	/// <summary>
	/// Whether the animation's position will be reset on play or will continue from where it left off.
	/// </summary>

	public bool resetOnPlay = false;

	/// <summary>
	/// Whether the selected object (this button) will be cleared when the animation gets activated.
	/// </summary>

	public bool clearSelection = false;

	/// <summary>
	/// What to do if the target game object is currently disabled.
	/// </summary>

	public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

	/// <summary>
	/// What to do with the target when the animation finishes.
	/// </summary>

	public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

	/// <summary>
	/// Event delegates called when the animation finishes.
	/// </summary>

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Deprecated functionality, kept for backwards compatibility
	[HideInInspector][SerializeField] GameObject eventReceiver;
	[HideInInspector][SerializeField] string callWhenFinished;

	bool mStarted = false;
	bool mActivated = false;
	bool dragHighlight = false;

	bool dualState { get { return trigger == Trigger.OnPress || trigger == Trigger.OnHover; } }

	void Awake ()
	{
		UIButton btn = GetComponent<UIButton>();
		if (btn != null) dragHighlight = btn.dragHighlight;

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

	/// <summary>
	/// Automatically find the necessary components.
	/// </summary>

	void Start ()
	{
		mStarted = true;

		// Automatically try to find the animator
		if (target == null && animator == null)
		{
			animator = GetComponentInChildren<Animator>();
#if UNITY_EDITOR
			if (animator != null) NGUITools.SetDirty(this);
#endif
		}

		if (animator != null)
		{
			// Ensure that the animator is disabled as we will be sampling it manually
			if (animator.enabled) animator.enabled = false;

			// Don't continue since we already have an animator to work with
			return;
		}

		if (target == null)
		{
			target = GetComponentInChildren<Animation>();
#if UNITY_EDITOR
			if (target != null) NGUITools.SetDirty(this);
#endif
		}

		if (target != null && target.enabled)
			target.enabled = false;
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

	void OnHover (bool isOver)
	{
		if (!enabled) return;
		if ( trigger == Trigger.OnHover ||
			(trigger == Trigger.OnHoverTrue && isOver) ||
			(trigger == Trigger.OnHoverFalse && !isOver))
			Play(isOver, dualState);
	}

	void OnPress (bool isPressed)
	{
		if (!enabled) return;
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3) return;
		if ( trigger == Trigger.OnPress ||
			(trigger == Trigger.OnPressTrue && isPressed) ||
			(trigger == Trigger.OnPressFalse && !isPressed))
			Play(isPressed, dualState);
	}

	void OnClick ()
	{
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3) return;
		if (enabled && trigger == Trigger.OnClick) Play(true, false);
	}

	void OnDoubleClick ()
	{
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3) return;
		if (enabled && trigger == Trigger.OnDoubleClick) Play(true, false);
	}

	void OnSelect (bool isSelected)
	{
		if (!enabled) return;
		if (trigger == Trigger.OnSelect ||
			(trigger == Trigger.OnSelectTrue && isSelected) ||
			(trigger == Trigger.OnSelectFalse && !isSelected))
			Play(isSelected, dualState);
	}

	void OnToggle ()
	{
		if (!enabled || UIToggle.current == null) return;
		if (trigger == Trigger.OnActivate ||
			(trigger == Trigger.OnActivateTrue && UIToggle.current.value) ||
			(trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
			Play(UIToggle.current.value, dualState);
	}

	void OnDragOver ()
	{
		if (enabled && dualState)
		{
			if (UICamera.currentTouch.dragged == gameObject) Play(true, true);
			else if (dragHighlight && trigger == Trigger.OnPress) Play(true, true);
		}
	}

	void OnDragOut ()
	{
		if (enabled && dualState && UICamera.hoveredObject != gameObject)
			Play(false, true);
	}

	void OnDrop (GameObject go)
	{
		if (enabled && trigger == Trigger.OnPress && UICamera.currentTouch.dragged != gameObject)
			Play(false, true);
	}
	
	/// <summary>
	/// Start playing the animation.
	/// </summary>

	public void Play (bool forward) { Play(forward, true); }

	/// <summary>
	/// Start playing the animation.
	/// </summary>

	public void Play (bool forward, bool onlyIfDifferent)
	{
		if (target || animator)
		{
			if (onlyIfDifferent)
			{
				if (mActivated == forward) return;
				mActivated = forward;
			}

			if (clearSelection && UICamera.selectedObject == gameObject)
				UICamera.selectedObject = null;

			int pd = -(int)playDirection;
			Direction dir = forward ? playDirection : ((Direction)pd);
			ActiveAnimation anim = target ?
				ActiveAnimation.Play(target, clipName, dir, ifDisabledOnPlay, disableWhenFinished) :
				ActiveAnimation.Play(animator, clipName, dir, ifDisabledOnPlay, disableWhenFinished);

			if (anim != null)
			{
				if (resetOnPlay) anim.Reset();
				for (int i = 0; i < onFinished.Count; ++i)
					EventDelegate.Add(anim.onFinished, OnFinished, true);
			}
		}
	}

	/// <summary>
	/// Play the tween forward.
	/// </summary>

	public void PlayForward () { Play(true); }

	/// <summary>
	/// Play the tween in reverse.
	/// </summary>

	public void PlayReverse () { Play(false); }

	/// <summary>
	/// Callback triggered when each tween executed by this script finishes.
	/// </summary>

	void OnFinished ()
	{
		if (current == null)
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
