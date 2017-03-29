----------------------------------------------
            NGUI: Next-Gen UI kit
 Copyright © 2011-2016 Tasharen Entertainment
            Version 3.11.1
    http://www.tasharen.com/?page_id=197
            support@tasharen.com
----------------------------------------------

Thank you for buying NGUI!

PLEASE NOTE that NGUI can only be legally downloaded from the following 3 sources:

  1. Unity Asset Store (Standard License)
  2. www.tasharen.com (Standard License)
  3. github.com/tasharen/ngui (Professional and Site Licenses)

If you've obtained NGUI via some other means then note that your license is effectively invalid,
as Tasharen cannot provide support for pirated and/or potentially modified software.

Documentation can be found here: http://www.tasharen.com/forum/index.php?topic=6754.0

If you have any questions, suggestions, comments or feature requests, please
drop by the NGUI forum, found here: http://www.tasharen.com/forum/index.php?board=1.0

--------------------
 How To Update NGUI
--------------------

If you have the Professional or Site License of NGUI that comes with Git access, just pull the latest changes.

If you have a Standard License:

1. In Unity, File -> New Scene
2. Delete the NGUI folder from the Project View.
3. Import NGUI from the updated Unity Package.

---------------------------------------
 Support, documentation, and tutorials
---------------------------------------

All can be found here: http://www.tasharen.com/forum/index.php?topic=6754.0

Using NGUI with JavaScript (UnityScript)? Read this first: http://www.tasharen.com/forum/index.php?topic=6

------------------
 FreeType Library
------------------

NGUI version 3.5.2 onwards includes the pre-compiled C++ FreeType library, which is an open source project (http://freetype.org/)
FreeType license: http://git.savannah.gnu.org/cgit/freetype/freetype2.git/tree/docs/FTL.TXT
This library is used only if you choose the "Generate Bitmap" font option in the Font Maker,
and it will not be included in the build of your game. It's only used in the editor.

-----------------
 Version History
-----------------

3.11.1
- NEW: Added a 'keep value' option to the popup list that will make popup values persist even after it disappears, like it used to work before the change many versions back.
- FIX: Popup list with values defined at edit time no longer has an initial value (unless the new 'keep value' is checked).
- FIX: Capitalized versions of bbcode keywords like [B] will now work in addition to lowercase.
- FIX: Backwards compatibility fixes with Unity 5.3.

3.11.0
- NEW: It's now possible to specify a custom material on regular NGUI sprites and labels.
- NEW: Added UV2 support, specified on the panels. Secondary UVs can be easily used to add multi-texturing effects such as blended tiled backgrounds or simply detail textures, enhancing your UI's look.
- NEW: Draw call now uses a material property block to specify the main texture as it works better at edit time.
- NEW: UIGeometry now has a onCustomWrite delegate that can be used to modify the generated geometry as you see fit.
- NEW: Added camera.FitOnScreen(transform) to simplify adjusting some group of widget's position to be within screen bounds.
- NEW: Tweens can now be set to use FixedUpdate for their animation.
- NEW: Added UIDrawCall.onCreateDrawCall that's called every time a new draw call gets created.
- NEW: Added UIDrawCall.shadowMode that can be used to change the shadow casting mode.
- FIX: NGUI's geometry should now work with one-sided shaders.
- NEW: Added UICamera.lastWorldRay to hold the last ray used to cast into the world.
- NEW: Added UICamera.mouse0, mouse1, mouse2 to access the mouse directly.
- NEW: UICamera.uiHasFocus to return 'true' when there is an active UI interaction happening.
- NEW: NGUI raycasts into the world will now automatically ignore triggers if the event type is set to World_3D.
- FIX: Popup list's panel now keeps the sorting order of its parent panel.
- FIX: Tweener.Begin now always clears the onFinished callback list.
- FIX: Eliminated GC allocations from raycasts.

3.10.2
- NEW: Added UIDrawCall.MoveToScene for Unity 5.4+ to make it easier to move the UI to another scene.
- FIX: Fixed an issue with Unity 5.4.1 in regards to the NGUI menu.
- FIX: DragDropItem will now inverse transform the delta, so it should theoretically work even with rotated panels.
- FIX: DragDropItem now again clears the scroll view reference after the drag operation completes.
- FIX: Unity 5.4 DX9 bug work-around (no longer marking VBOs as dynamic).

3.10.1
- NEW: Added UICamera.ignoreAllEvents to easily disable all NGUI events.
- FIX: Unity 5.4 editor on OSX: fixes for retina-related glitches of 5.4.
- FIX: Gamma to linear conversion now happens in the draw call class and no longer needs to be set in each OnFill function.

3.10.0
- NEW: Added Texture2D.MakeReadable(true/false) -- a convenience extension for the editor. Not sure why this isn't built-in.
- FIX: UIDragScrollView will no longer inform the scroll view of OnPress(false) on disable unless it's actually being dragged.
- FIX: Removed ColorMask RGB from shaders since according to Unity docs it may slow down some mobiles.
- FIX: Fixed Unity 5.4 function deprecation warnings.
- MISC: Added Profiler.BeginSample/EndSample blocks to clarify GC allocations that only happen in the editor.

3.9.9
- NEW: Added customizable GetMouse, GetTouch and RemoveTouch delegates to UICamera replacing fixed Input calls. This makes all of NGUI's events go through user-settable delegates.
- NEW: Sprite Animation script now has a frame offset index you can set if you want it to start at something other than 0.
- NEW: UIScrollView now has a "constrain on drag" as an option rather than always being off.
- FIX: UILabel resizing due to overflow settings will now trigger its UIWidget.onChange notification.
- FIX: Fix for a visibility issue caused by instantiating a panel off-screen then bringing it into view.
- FIX: Fixed a bug with text wrapping not wrapping colors properly in some situations (UITextList).

3.9.8
- NEW: NGUI now uses Color instead of Color32 for colors, which work better with linear space colors.
- NEW: Added a modifier setting to the UILabel that can automatically change the text prior to displaying it, such as making it uppercase/lowercase or calling a custom modifier function.
- FIX: Better handling of Linear space lighting.
- FIX: Changing UIPanel.alpha no longer invalidates the widgets underneath it (performance boost).
- FIX: Font Maker should now work even in the 32-bit version of Unity 5.
- FIX: Forcing OpenGL in Windows will no longer result in a blurry UI.
- FIX: Fix for an issue in Unity 5.4.

3.9.7
- NEW: Added "events go to colliders" option to all UICameras, not just the first one.
- NEW: UICamera now has an option to process events in either Update or LateUpdate.
- NEW: Added a "max width" property to labels set to overflow using Resize Freely.
- NEW: UIPopupList now supports 2D sprites.
- NEW: Added code to UIKeyBinding to convert its key+modifier to text format and back (good for saving a list of key bindings in files).
- NEW: Added Set() functions to UIProgressBar, UIToggle, UIInput that can set a value without triggering the event callbacks.
- NEW: Added Camera.FitOnScreen() extension that can be used to ensure that any UI does not go past the screen's dimensions (ex: tooltip).
- NEW: Added TweenFill that can be used to tween filled sprites' fill value.
- FIX: UIButton no longer fires its OnClick notification from right and middle mouse button clicks.
- FIX: Fix for duration 0 tweens not advancing their time properly.
- FIX: Fix for long press tooltips not showing properly on touch screens half the time.

3.9.6
- NEW: Added NGUITools.Draw<T> function that can be used like GUI.Draw (just much more efficient).
- NEW: NGUITools.AddChild is now an extension method (ie: gameObject.AddChild).
- NEW: Added several new versions of UIRect.SetAnchor.
- NEW: UIRect.SetScreenRect can be used to set the widget's screen rect to be anchored to top-left, identical to how GUI.Draw(rect) would work.
- NEW: Added "hide inactive" option to UIWrapContent.
- NEW: Added the sorting layer name option to UIPanel (contributed by Benzino07).
- NEW: serializedObject.DrawProperty() convenience function(editor)
- NEW: Localization.Set(language, key, text) to add individual localization entries.
- NEW: Added sprite gradient support (contributed by Nicki).
- FIX: NGUITools.AddChild<T>() now caches types, making it faster.
- FIX: Num pad's Enter is now treated just like Return by UIInput.
- FIX: Fixed a deprecation warning on Unity 4.6.8 and 4.6.9.
- FIX: Tweener will no longer use delta time for the first frame of the animation.
- FIX: Drag & drop should no longer prevent mouse wheel from scrolling the scroll view until the next click.
- FIX: Fix for UISpriteAnimation freezing the game after extended time being minimized.
- FIX: Fixed compilation warnings in Unity 5.3.

3.9.4
- FIX: Work-around for a bug in Unity crashing when dynamic fonts are used (Unity also fixed it in 5.2.1p2).
- FIX: Fix for caret appearing in the wrong place for a split second when typing past the end of the label.
- FIX: Multi-line UIInput will now do a submit via Ctrl+Return by default.

3.9.3
- NEW: Toggle is now capable of triggering tweens in addition to animations.
- FIX: UITextList should now wrap colors properly.
- FIX: UIKeyNavigation will now highlight elements properly with Tab.
- FIX: Keyboard input can now be used in Example 4.
- FIX: More tweaks to dynamic fonts. Oh how I hate that particular "feature"...
- FIX: Work-arounds for some new oddities in Unity 5.2.
- FIX: Fix for an old bug that would sometimes cause items inside a tweened scroll view to be offset visually.
- FIX: Minor tweaks related to ensuring that anchors get called properly on start only once.

3.9.2
- NEW: Added an option to UICamera to automatically hide the cursor when controller or touch input is used.
- NEW: Added ellipsis overflow support by Jason Nollan.
- NEW: Added an option to the Popup List to automatically create its popup on a separate panel, ensuring that it's always on top.
- NEW: Added UICamera.first referencing the active NGUI event system.
- FIX: Alpha should now work as expected with Linear lighting.
- FIX: UICamera.isOverUI should now work properly for all types of input.
- FIX: NGUIEditorTools.DrawProperty can now draw arrays.
- FIX: Added mdeletrain's broken dynamic font fix.
- FIX: Drag operation now cancels tooltips properly.
- FIX: UITextList should now wrap colors properly.
- FIX: Flash compilation fixes.

3.9.1
- NEW: NGUI will now automatically disable controller input on stand-alone builds if the game starts up with some joystick axis reporting non-zero.
- NEW: Added command-line arguments that can enable/disable control types, such as -noJoystick.
- NEW: Scene view UI focus is now bound to ALT+F.
- FIX: Unity 5 dynamic font work-around.
- FIX: Panels moving should no longer cause widgets to get marked as moving.
- FIX: UICamera now uses the GetAnyKeyDown delegate.
- FIX: Dragging a window should now be much faster.
- FIX: UICamera.disableController should now report the correct value after a popup list has been closed.
- FIX: UIPopupList will now again trigger callbacks on start when setting the default value.

3.9.0
- NEW: Completely redesigned how controller-based input was handled. It's now much more robust and handless seamless transitions from one method of input to another.
- NEW: New OnNavigate and OnPan events. OnPan events require Pan axes to be set on the UICamera.
- NEW: Scroll views are now scrollable via controller if Pan axes are set.
- NEW: Sliders can now be adjusted via controller by using Pan axes.
- NEW: OnKey event will now send all key press events to the targeted object.
- NEW: Added UICamera.controllerNavigationObject that explicitly tracks controller-based selection.
- NEW: NGUI now automatically finds and focuses on an appropriate UIKeyNavigation object if none has focus while receiving controller-based input.
- NEW: Added a Color Picker.
- NEW: Added a Tab option to the Key Navigation script (visible when it's attached to a UIInput).
- NEW: Added a new "replacement key" feature to localization that lets you replace localization values without changing the localization itself. Useful for user-defined "overrides".
- NEW: Added a proper editor class for the TypewriterEffect.
- NEW: UIViewport will now automatically disable itself if the corner object has been disabled.
- FIX: Various fixes and improvements for controller-based input support.
- FIX: UITextList now properly line-wraps embedded colors.
- FIX: UICenterOnChild should now respect paging through sorted lists.
- FIX: Popup list's "ensure it's visible" code has been redesigned.
- FIX: Optimized code related to widget change detection / buffer rebuilding.
- FIX: Dynamic font labels will now automatically invalidate themselves when the application regains focus.
- FIX: Unity 5 compatibility tweaks.
- DEL: Removed the Keyboard scheme, since it's always Keyboard+Mouse.

3.8.2
- NEW: Localization will now automatically pull mobile version of keys if the current control scheme is touch.
- NEW: UICamera.touchCount was split into GetInputSources() and UICamera.activeTouches.Count.
- FIX: Better handling of touches in UICamera.
- FIX: Tooltip will now show at the last mouse/touch position rather than last mouse position.
- FIX: Fixed UI colors with linear-space lighting.
- FIX: Fixed UI blurriness in the editor when targeting Android on Windows.
- FIX: Fixed UI blurriness when the window's dimensions are not dividable by two.

3.8.1
- NEW: Added Animator to UIToggle in addition to Animation.
- NEW: Added a "long press" tooltip option to UICamera (to show tooltips on touch-based devices).
- NEW: UIKeyBinding now has explicit "none" and "any" modifier key options.
- NEW: UIScrollView's movement dampening strength is now exposed as a property for scripting.
- NEW: Added Transparent Masked shaders for when you want to have clipped masked textures.
- FIX: Label's MakePixelPerfect will no longer snap to dimensions that don't divide by two.
- FIX: UIButton's isEnabled property will now affect all button scripts on the object, not just the first one.
- FIX: Sprite's padding should now be affected by pixel size adjustments.
- FIX: UIPlayAnimation should now work properly with the controller input again.
- FIX: Unity 4.3 compilation fix.

3.8.0
- NEW: Added a way to add TouchScript support to NGUI: http://www.tasharen.com/forum/index.php?topic=12411.0
- NEW: Setting a Unity sprite on a UI2DSprite will now automatically set its border values.
- FIX: Drag & drop script should now again work with press-based activation and cloneOnDrag option.
- FIX: Popup lists should now work better with multi-touch events.
- FIX: Variety of fixes for obscure issues that most would have never encountered.

3.7.9
- NEW: Localization system can now automatically merge localization data coming from multiple sources as well as partial localizations.
- NEW: Command-click = right click is now an option on the UICamera instead of always being on.
- FIX: UIInput's Return key handling was moved to the Update() function to fix an issue with typing quickly and pressing Return causing the last character to be cut off.
- FIX: Forced keyboard and mouse to be turned off for Android devices in UICamera's Awake() function.
- FIX: UICamera will no longer process keyboard events if both keyboard and controller input is turned off.
- FIX: UICamera should now properly handle release and press events being sent in the same frame.
- FIX: Relative anchor slider will no longer get disabled if the user types values outside 0 to 1.

3.7.8
- NEW: Added a validator delegate to the toggle class for when you want to add custom code to prevent state changes before they happen.
- FIX: Some more compile fixes for newer Unity 5 versions.
- FIX: Sliced sprite corners will no longer be drawn if sides were chosen to be hidden.
- FIX: UIPanel no longer makes all of the game objects underneath it be on the same layer. Just widgets.
- FIX: Minor fix for the scroll view recentering.
- FIX: Flash compilation #ifdef.
- FIX: FreeType fix for Unity 5 (64 bit).

3.7.7
- Fix for the drop-down list appearing in the wrong place in some situations.
- You can now choose to merge loaded localization data with existing one. Useful for patching games.
- Added a user-contributed outline8 type label effect.
- Cleaned up new warnings shown in Unity 5.
- Fixes for dynamic font rendering in Unity 5 (underline/strikethrough).

3.7.6
- FIX: Unity 5.0 compilation and functionality compatibility fixes.
- FIX: Work-around for a Unity bug in 4.5.5p3 and p4 (OnValidate not affected by script execution order).
- FIX: Potential work-around for touch events sending mouse events.
- FIX: NGUI texture import will now use automatic true color instead of ARGB32.
- FIX: UIEventTrigger's OnDrag was mistakenly calling the onDragOut callback.

3.7.5
- NEW: UISavedOption now works with progress bars as well.
- NEW: MathiasSoeholm's implementation for Labels with float spacing.
- FIX: UIPopupList opened manually will now close automatically without requiring selection to be set.
- FIX: UICamera will now use proper 'events go to colliders' flag from the first UICamera.
- FIX: Unity 5 compilation tweaks.
- FIX: Flash compilation fixes.

3.7.4
- FIX: UIInput no longer uses OnGUI unless you actually select the input field first (eliminates GC alloc).
- FIX: UIEventTrigger's OnDragStart/OnDragEnd shouldn't have had parameters.
- FIX: UIDragObject should now work properly with a constrained type UIRoot.
- FIX: Key/controller interaction wasn't quite correct with sliders/scroll bars.
- FIX: UIDragObject will now snap to pixels after the movement completes.

3.7.3
- NEW: New clipping option on panels: Texture Mask. Any texture with alpha will work (think round minimap etc).
- NEW: UICamera now has an option for whether events go to colliders or rigidbodies.
- NEW: Added Cell Alignment field to the UITable letting you change the content's alignment.
- NEW: UIGrid now has a "cell snap" arrangement type for when you simply want to have widgets snap as you drag them.
- NEW: 2D Sprites now have a "pixel size" property.
- FIX: Pre-defined texture preview for the Prefab Tool wasn't quite working right.
- FIX: UIPanel now always force-disables the "offset" option if it's on the UIRoot.
- FIX: Handles will now automatically hide when multi-editing widgets, allowing you to use the transform move.

3.7.2
- NEW: You can now explicitly choose the panel used as a drag region for UIDragObject.
- NEW: You can now specify a custom thumbnail texture for each item in the prefab toolbar.
- NEW: TweenAlpha and TweenColor now work on Unity's SpriteRenderers.
- NEW: Added UICamera.currentTouch.deltaTime (time since touch started).
- FIX: UIGrid.Reposition should now work even without the component being started first.
- FIX: Widgets with alpha 0 won't be selectable in the scene view anymore.
- FIX: UIDragDropItem wasn't setting 'dragged' state properly if the item was cloned.
- FIX: Alpha text encoding [Aa] style should no longer try to interpret non-hex characters.
- FIX: Tweaks to how UICenterOnChild works and fixes to its paging functionality.
- FIX: Minor fix to tween/play tween in regards to playing in reverse.
- FIX: Bitmap labels now support thin spaces (U2009).
- FIX: PropertyBinding now respects "editMode" flag properly.
- FIX: Setting UILabel.material at run time on a dynamic font should now work as expected.
- FIX: DX9 half pixel offset will now be ignored properly in /force OpenGL mode.
- FIX: You should now be able to use the stylus on android devices.
- DEL: UIInput.selectOnTab is now deprecated in favor of UIKeyNavigation (will auto-upgrade)

3.7.1
- NEW: Added generic delegates to UICamera you can subscribe to (onClick, onHover, etc) to replace the genericEventHandler.
- NEW: Added a new option to UITexture and UI2DSprite to keep the original texture's aspect ratio.
- NEW: Popup list items can now have arbitrary data associated with each entry.
- NEW: You can now choose what kind of click will open the popup list.
- NEW: New text symbol [c] will cause the text that follows it to ignore the label's color tint.
- NEW: Added the missing OnDragStart and OnDragEnd to UIEventListener.
- NEW: UICamera.onMouseMove notification.
- FIX: UICamera will only send events to rigidbodies if there was no UIPanel present.
- FIX: UIPanel will no longer use handles if the camera drawing it is not 2D.
- FIX: Unity 4.3 compatibility.

3.7.0
- NEW: You can now set UIWidget.onRender to change material properties like in OnWillRenderObject.
- NEW: Changing UITexture.mainTexture and shader is now super-quick if it's not batched.
- NEW: UIRoot now has additional scaling constraints enabling new fill and fit modes.
- NEW: Added the ability to ignore kerning information when making bitmap fonts.
- NEW: NGUI's events sent via "3D" and "2D" event type UICameras will now go to the rigidbody instead of colliders.
- NEW: UIKeyBinding now has a new setting "All" that will trigger both select and press/click logic.
- NEW: UICamera.isOverUI, UICamera.currentTouch.isOverUI.
- NEW: NGUI now uses the new rect transform tool instead of the move transform in Unity 4.6+.
- NEW: UIPlaySound now has an OnEnable play option. For convenience.
- NEW: UIEventListener now has OnTooltip.
- NEW: TweenAlpha now works with renderers as well.
- FIX: Removed code that was snapping draw call positions to pixels, allowing you animate panels smoothly.
- FIX: Calling Reposition() on the grid and table now works even if its Start() hasn't executed.
- FIX: Dynamic font baseline calculation work-around for some partial fonts.
- FIX: Center On Child script got semi-broken in the last version.

3.6.9
- NEW: Added loop, play, pause and reset functionality to UI2DSpriteAnimation.
- NEW: Added new automatic support for linear lighting.
- NEW: Added a pivot point setting to UITable to match UIGrid.
- NEW: Added warnings to UIAnchor and UIStretch components that inform the user about them being deprecated.
- NEW: Property binding can now let you select properties that only have either get or set, depending on what's needed.
- NEW: Added settable delegates to UICamera: GetKey, GetKeyDown, GetKeyUp, GetAxis.
- NEW: UIRect has a new anchor update setting: OnStart. It will only update anchors once.
- NEW: Panels have a new option to ignore soft border when constraining scroll view content.
- FIX: Added alexkring's work-around for the rare "texture destroyed" issue in the Atlas Maker.
- FIX: Dragging 2D UI elements into a scene with a 3D UI should no longer create many UI Roots.
- FIX: Labels will now use the draw region, making them usable as the slider's foreground.
- FIX: Localization with multi-line entries would skip the first word.
- FIX: The layout system's anchors should now work even for 3D UIs.
- FIX: UIWrapContent was not setting the restrict within panel flag correctly.
- FIX: UILabel.GetWordAtPosition now works with line breaks properly.
- FIX: Embedded URL retrieval code wasn't capping when it encountered a /url tag.
- FIX: Embedded URL retrieval is now much more precise.
- FIX: Mouse scroll wheel should again work with UICenterOnChild.
- FIX: UILocalize will now change UIButton's normal sprite.
- FIX: UIToggle transitions will now be instant if the value was changed while the toggle is disabled.
- FIX: UIDragDropitem will now work with 2D colliders (thanks HanzaRu).
- FIX: Bold can now be mixed with underline properly.
- DEL: Removed the old "Shader Quality" script that was messing up the refractive atlas.

3.6.8
- NEW: UIWrapContent now has a range limit you can set for indices (such as -10 to 10).
- NEW: Added Transform.OverlayPosition to make it easy to position widgets using 3D object positions.
- FIX: Progress bars / sliders will no longer show the foreground if the value is 0.
- FIX: Changing UI2DSprite.sprite2D will now immediately re-add the widget to the panel.
- FIX: UIDragDropItem will now delay enabling the drag scroll view script (thanks, slumtrimpet!)
- FIX: Re-added a hack-around for Adreno GPU crashes. Looks like Unity 4.5 did not fix the problem, despite the patch notes.
- FIX: Seeing as BetterList is slower at sorting than List, some instances of BetterList were replaced with List.
- FIX: UIPanel's option to cull widgets while the scroll view is being dragged is now on by default.
- FIX: UIRoot now considers WP8 and BlackBerry to be mobile devices.
- FIX: More tweaks for how camera's region gets calculated.

3.6.7
- NEW: Added a lookup table to the atlas in order to make GetSprite() faster.
- NEW: Added OnPostFill functionality to widgets, in case you want to further modify the geometry.
- NEW: Added OnMomentumMove and OnStoppedMoving notifications to UIScrollView in addition to OnDragFinished.
- NEW: Added NGUI -> Extras -> Align Scene View to UI (thanks NikolayLezhnev)
- FIX: UIGrid's smooth snapping will now ignore time scale.
- FIX: Nicki's optimizations (Shader.PropertyToID instead of by name).
- FIX: Null check for 'mKeyboard' being null in UIInput (rare case).

3.6.6
- NEW: UIWrapContent now has a settable delegate to initialize items, and will call it on Start().
- NEW: Added OnDragStarted to the scroll view for those that needed it.
- NEW: Added the missing OnDragOver/OnDragOut to the UIEventListener.
- FIX: Hiding game view behind the scene view should now work as expected (thanks NikolayLezhnev).
- FIX: Localization was not always handling double quotes properly.
- FIX: Mobile keyboard-related fix (thanks niniane).
- FIX: 2D raycasts were not working quite right...
- FIX: Underline and strike-out should now look better and will be affected by the gradient.
- DEL: Commented out NGUITools.OpenURL since it causes network permissions to be used on mobile. Uncomment them if you need them.
- DEL: Cleaned up pre-Unity 4.3 code.

3.6.5
- NEW: Added a way to show the transform gizmo without disabling the drag handles (NGUI->Options).
- NEW: Added an "onCenter" notification to UICenterOnChild.
- FIX: Drag & drop example wasn't working properly due to a missed line in UICamera.
- FIX: UIToggle.value will now return the starting state if the toggle has not yet been activated.
- FIX: WP8/iOS UIInput fix, and force the keyboard to show up when it's in a 'password' mode.
- FIX: Flash compilation fixes.
- FIX: Nicki's optimizations.

3.6.4
- NEW: Added the way to set the label alignment for popup lists.
- NEW: EventDelegate.Add(list, callback) now returns an EventDelegate to work with.
- NEW: Added an option to execute the UICenterOnChild in the editor via right-click.
- FIX: Fix for a regression bug causing bar view foreground's collider was never adjusted properly.
- FIX: UILabel now automatically clears NGUIText font references after using them.
- FIX: Nested anchors set to update in OnEnable will now work as expected when the hierarchy gets re-enabled.
- FIX: Unified inspector look can now be modified properly.
- FIX: Switching from 3D to 2D UI will now remove the 3D rigidbody.
- FIX: Drag & drop example wasn't working properly due to a missed line in UICamera. (3.6.4b)
- FIX: UIToggle.value will now return the starting state if the toggle has not yet been activated (3.6.4b)

3.6.3
- NEW: Added onFinished and Finish() to the Typewriter script.
- FIX: Changed the way "hide input" logic works in UIInput.
- FIX: UIInput was not setting its starting value correctly in some cases.
- FIX: Hide Input setting on the input field is now a separate field.
- FIX: UIlabel.Wrap was not using the provided height.
- FIX: Flash compile fixes.

3.6.2
- NEW: Added an optional different (minimalistic) look for NGUI's components (change via Options -> Inspector Look).
- NEW: Typewriter script can now fade in letters gradually using alpha (have a look at Tutorial 5).
- NEW: You can now embed overriding alpha in text using [Aa] format.
- NEW: UIButton can now swap 2D sprites as well.
- FIX: Embedded color's alpha now also affects the shadow and outline effects.
- FIX: Typewriter effect should now be able to fade in multiple tags properly.
- FIX: Replaced all usage of UICamera.lastHit.point with UICamera.lastWorldPosition (for 2D events).
- FIX: Certain widget elements should now support 2D colliders properly (sliders and such)
- FIX: Fixed an issue with double space in an input field causing issues.
- FIX: Yet more WP8 stuff.

3.6.1
- NEW: NGUI now fully supports 2D colliders, and will create them by default if UICamera is in 2D UI mode.
- NEW: Added a way to automatically switch the entire UI to use 2D or 3D colliders via the NGUI->Extras.
- NEW: Added support for TouchScreenKeyboard.hideInput (input caret, selection, etc on mobiles)
- NEW: Added pre-generated Prefab Toolbar preview icons for Unity Free.
- NEW: EnvelopContent script will now execute itself every time it's enabled, and will update anchors.
- NEW: You can now see your NGUI's version via the Help menu.
- FIX: NGUIText now supports unicode spaces (contributed by Graham Reeves).
- FIX: Popup list was not highlighting the selection properly in some cases.
- FIX: Popup list will now always be closed when any item gets chosen.
- FIX: UIProgressBar will now work properly with 2D sprites and UITextures.
- FIX: Nested scroll views instantiated at run-time should now be clipped properly.
- FIX: Grid will now sort the list of children in GetChildList() since the hack-around didn't work.
- FIX: Localization will load the data in the Exists() function as well.
- FIX: Still more WP8 fixes.
- DEL: Upgrade tools are no longer a part of the package. Grab them from the website instead.

3.6.0
- NOTE: NGUI now requires Unity 4.3.4 or higher!
- NEW: Added a new tool -- Prefab Toolbar. It lets you drop prefabs onto it for easy preview.
- NEW: Unity2D Sprite now has all the same options as an NGUI sprite (sliced, filled, tiled, etc).
- NEW: UITexture now has all the same options as an NGUI sprite.
- NEW: You can now choose components as parameters for functions via inspector.
- NEW: Added support for full RGBA32 color encoding in text (RrGgBbAa).
- NEW: UISpriteAnimation example script now has pixel snap setting as optional.
- NEW: Extended the Typewriter Effect script with additional functionality.
- FIX: In some cases changing sprites on a prefab wouldn't "take".
- FIX: WP8/WSA fixes, courtesy of LoneCoder from the forums.
- FIX: Pixel-snap a tiled sprite should no longer revert it to single sprite's dimensions.
- FIX: Nested scroll views were not culling widgets properly in some cases when scrolled.
- FIX: Calculating widget dimensions will now ignore widgets in clipped panels.

3.5.9
- NEW: Added an event delegate drawer in case you want to use the Event Delegate in your own scripts.
- NEW: You can now explicitly specify what the Return key will do on the input field regardless of the label's multi-line setting.
- NEW: Added GetIndex() to UIGrid and improved its look in the inspector.
- FIX: Scroll view will no longer jump back and forth by 1 pixel when it's not using momentum.
- FIX: Input fields should respect the "starting value" if the "saved as" is left blank.
- FIX: Text printing issue if the line begins with a double space.
- FIX: You can now call UIButton.ResetDefaultColor to restore the original color, even after setting 'defaultColor' to something else.
- FIX: UIKeyNavigation will now respect UIButton's isEnabled state if it's present.
- FIX: UIPlaySound will now respect UIButton's isEnabled state if it's present.
- FIX: UIDrawCall copy material creation now also copies shader keywords.
- FIX: UICamera.inputHasFocus should now work properly again.
- FIX: The Sorting Order will now always be exposed on the UIPanel in inspector.
- DEL: Moving DataNode over to TNet since it makes a lot more sense to have it there instead of in NGUI.

3.5.8
- NEW: Added a generic node-based class for simple text-based serialization (DataNode).
- NEW: UITexture now has flip options just like UISprite.
- NEW: Moved the SetRect function from UIWidget to UIRect, making it usable by panels as well.
- NEW: Added convenience add and remove functions to the UIGrid.
- NEW: Added NGUIMath.ScreenToPixels for when you need to convert from screen to virtual pixels.
- NEW: UIButton's SetState is now public, in case you need it.
- NEW: UIInput.caret is now exposed in case you need it.
- FIX: Re-added the "New" button to the atlas maker that was removed for no reason.
- FIX: Added [NonSerialized] next to private variables. Unity apparently serialized private variables in prefabs (sigh).
- FIX: "Flip" option is now exposed in inspector with tiled sprites.
- FIX: Changed all "HIDDEN" shaders to be "Hidden" instead, effectively hiding them.
- FIX: Text list should no longer break when a very long line of text is added.
- FIX: DragDropItem script wasn't un-highlighting things quite right.
- FIX: Clip softness can now be 0.
- DEL: Localization is now a static class, and can no longer be included in the scene.

3.5.7
- NEW: Added OnDragOver/OnDragOut to the Event Trigger.
- FIX: Event delegate compilation on platforms that don't support reflection.
- FIX: The example tooltip should no longer go off-screen.
- FIX: Exposed UISprite's 'flip' option to scripting.
- FIX: Context menu 'Attach' options should now work with multiple objects selected.
- FIX: 'Attach' menu options should now all be undo-able via CTRL+Z.
- FIX: Exposed UIButton.state for those that may need it for any reason.
- FIX: UICamera's raycasts can now be clipped by nested panels.
- FIX: MakePixelPerfect should not change the width if the label is set to "resize height".
- FIX: Made UIButton.isEnabled work with a 2D collider.
- FIX: Unity 4.2 compatibility tweaks.
- FIX: Clip softness can now be 0.

3.5.6
- NEW: Added basic built-in data binding support (PropertyBinding script).
- NEW: All delegates now support any number of parameters that you can set in inspector.
- NEW: You can now nest scroll views (scroll views within scroll views). The built-in shaders support up to 3 scroll views, but you can add more.
- NEW: You can now nest non-clipped panels within clipped panels and clipping will still work.
- FIX: Fix for scroll bar size being wrong if the content was smaller than the scroll view.
- FIX: UIInput will now load the saved value properly even if the "starting value" is not empty.
- FIX: Drag & drop item will now always disable the tween or spring effect when it begins dragging.
- FIX: UICamera's 'inputHasFocus' flag is now set when selection changes rather than every frame.
- FIX: Anchors set to update only in OnEnable will now still update while in edit mode.
- FIX: Triggering ActiveAnimation.Play will now immediately sample the animation.
- FIX: Fixed the bug that was causing the atlas maker to eat up CPU.

3.5.5
- NEW: Added built-in support for endless scroll view (UIWrapContent).
- NEW: Added a new example showing how to make endless scroll views.
- NEW: Added an "Pivot" setting to the UIGrid that controls how the content is positioned.
- NEW: Keyboard and controller navigation has been simplified (UIKeyNavigation).
- NEW: Added EnvelopContent example script that shows how to resize a sprite to envelop custom content.
- NEW: Widget anchors now have an option to be executed only when enabled, rather than every update.
- FIX: UIWidget.SetRect will now work properly again.
- FIX: Unity 4.0, 4.1 and 4.2 compile fix.

3.5.4
- NEW: You can now bake basic effects into bitmap fonts via inspector: soft shadow, soft outline, bevel, etc.
- NEW: Added a way to set the Sorting Order on panels using Explicit Render Queues (for Unity 2D).
- NEW: Cached buffers are now per-draw call rather than global, reducing memory allocations.
- NEW: Added a "tall portrait mode" setting to the UIRoot that will shrink the UI if it's in portrait mode.
- NEW: UIGrid and UITable now has the horizontal and vertical sorting options so drag & drop items can stay where you dropped them.
- NEW: Got rid of all the old tutorial scenes and replaced them with some new ones.
- NEW: Added a new experimental option to the UIRoot: "Adjust by DPI".
- NEW: Bitmap Font creation now works on OSX as well.
- FIX: You can now clear sprite states under UIButton.
- FIX: TweenRotation now tweens X, Y and Z values individually, so you can go from 0 to 360 now.
- FIX: OSX character keys resulted from arrow key presses will now be ignored by UIInput.
- FIX: Fixed an issue with scrollviews being anchored to non-centered widgets.
- FIX: Input selection and caret should now be affected by parent alpha properly.
- FIX: Changing the slider value via small increments should now work as expected.
- FIX: Transform inspector will now always display rotation in -180 to 180 range.
- FIX: CSV parser now supports multi-line input without having to insert "\n".
- FIX: A multi-line input field with a lot of spaces will now wrap correctly.
- FIX: Keyboard and controller navigation should again highlight things properly.
- FIX: Disabling a game object with a widget that was just enabled should no longer cause it to remain visible on rare occasions.
- FIX: You can now assign sliders/progress bars for scroll view scroll bars.
- FIX: Event delegate copy will now work for raw (code) delegates as well.
- FIX: Modifying widget dimensions in inspector is now properly undoable.
- FIX: Typewriter effect example script now supports encoded tags.
- FIX: Went through all examples and fixed a few that were wonky.

3.5.3
- NEW: All sprite types can now be flipped, not just simple sprites.
- NEW: Exposed On Change event in UIInput's inspector.
- FIX: UIButton will no longer pixel snap the normal sprite by default, and pixel snap is now off by default.

3.5.2
- NEW: Added the ability to generate bitmap fonts from within Unity using FreeType directly.
- NEW: You can now add transparent, clamped and tiling border to sprites via the Atlas inspector.
- NEW: You can now modify any sprite to bake a shadow or add some visual depth to it (want deeper shadow? add multiple!)
- NEW: UIImageButton's functionality is now a part of UIButton.
- NEW: You can now flip simple sprites horizontally and vertically (contributed by Nicki).
- FIX: Labels using Packed Fonts no longer have the Gradient and Effect options, as they don't work with packed fonts.
- FIX: Moved the Localization file into Examples/Resources so that it doesn't break older localization projects.
- FIX: Buttons that start with disabled colliders will now always assume their disabled state on start.
- FIX: UIProgressBar will no longer send OnChange if the change was limited by the number of steps.
- FIX: It should be possible to set the font to be of Reference type again.
- FIX: UIKeyBinding will no longer leave the button in a highlighted state.
- FIX: Another fix for scenes being marked as edited.
- FIX: Fixed the 2D hit detection logic.
- FIX: Flash compile fix.
- DEL: Removed Pixel Size property from UIFont. Set the label's target font size instead.
- DEL: Removed UICamera's OnInput event as it wasn't being used (as it wasn't reliable).

3.5.1
- NEW: CSV reader will now convert the "\n" character sequence into a new line char.
- FIX: Scenes using NGUI should no longer get marked edited so much.
- FIX: Reduced the size of meshes used by NGUI draw calls.
- FIX: Changing the panel's alpha will now properly inform child panels.
- FIX: Fix for how URL tags get parsed in labels.

3.5.0
- NEW: Localization system now supports CSV type input.
- NEW: UILocalize script now has key lookup and localized preview options.
- NEW: UICamera now has a new event type that supports 2D colliders.
- NEW: Added justified alignment support for labels.
- NEW: Scroll views now have a Content Origin point.
- NEW: You can now freely adjust width and height of anchored widgets.
- NEW: UIDragResize script now has a maximum size limiting option as well.
- FIX: Improved scroll view resizing and logic regarding how it repositions the content.
- FIX: Fixed an issue with how changing panel's alpha would not propagate to children in certain cases.
- FIX: NGUI will no longer intercept RMB events that occur outside the selected widget's area.
- FIX: UICenterOnClick should now work as expected when there is no UICenterOnChild present.
- FIX: UICenterOnClick shouldn't cache the panel anymore, making it work properly with drag & drop.
- FIX: Widget inspector's Dimensions field should no longer be grayed out if the widget is partially anchored.
- FIX: UIRoot's FixedSizeOnMobiles setting should now recognize BB and WP8 as mobile devices
- FIX: UICamera will now clear all active touch events when the application is paused.
- FIX: Work-around for dynamic font delegate subscriptions causing epic GC.
- FIX: Setting label text will now auto-adjust the collider size.
- FIX: Inlined italic text should now look better.

3.4.9
- NEW: You can now embed hidden content in labels using bbcode: [url=link]Click Here[/url]. Retrieve this content via UILabel.GetUrlAtPosition(UICamera.lastHit.point), then do what you want.
- NEW: Labels can now keep references to UIFonts that use dynamic fonts, for easy replacement/swapping.
- FIX: Work-around for a bug in Unity related to dynamic fonts discarding previously requested characters.
- FIX: UIButtonColor/UIButton will set the normal color in Awake instead of Start to avoid conflicts with tweens.
- FIX: Create UI menu option will now let you create a 3D UI if you have a 2D UI present, and vice versa.
- FIX: Input improvements: IME text selection while typing and proper dialog positioning.
- FIX: Parent widget's visibility checks should no longer cause children to be culled.
- FIX: Scaled bitmap fonts should now be correctly affected by the gradient setting.
- FIX: Removed UIAnchor usage from the Scroll View example.
- FIX: UIRoot should be executed before everything else.
- FIX: UIToggle.startsChecked is now be public.

3.4.8
- NEW: Tweens will now display the curve in inspector as a square, making it easier to eyeball.
- FIX: Fixed floating-point precision issues in NGUIText's print-wrapping logic.
- FIX: UIDrawCall will remove all references to materials and textures when it's disabled.
- FIX: Removed UIAnchor from the Drag & Drop scene.

3.4.7
- NEW: You can now set font size even on bitmap labels.
- NEW: UIScrollView can now reference sliders as scroll bars.
- FIX: Adjusting the widget's aspect ratio will now automatically resize the widget.
- FIX: UIImageButton now won't try to swap sprites if a sprite hasn't been set, and "pixel snap" is now optional.
- FIX: Text set to resize freely with positive spacing should no longer wrap the last char.
- FIX: Compile fixes on Unity 4.0, 4.1, and 4.2.

3.4.6 (previously 3.0.9 f7)
- NEW: UIPlayAnimation now supports Animator animations (mecanim).
- NEW: Added UIEventTrigger that can be used to add event delegates via inspector for press, release, select, etc.
- OLD: Deprecated UIButtonMessage and UIForwardEvents (upgrade to UIEventTrigger at your own pace).

3.4.5 (previously 3.0.9 f6)
- FIX: Typo fix in UIEventDelegate.

3.4.4 (previously 3.0.9 f5)
- NEW: UIGrid and UITable now have a virtual Sort function you can overwrite, and are now extensible.
- NEW: You can now use the Component Selector to load more than just prefabs by specifying explicit extensions.
- FIX: The Component Selector should now behave better with dynamic fonts and will hide Lucida Grande (internal Unity font).
- FIX: UICamera's new hit check should now work even if only one widget was hit.
- FIX: You can now remove delegates from the EventDelegate list even while executing its callbacks.
- FIX: Work-around for potential crash on exit on mobiles due to an issue in Unity.

3.4.3 (previously 3.0.9 f4)
- NEW: Added UIWidget.hitCheck delegate you can set for custom hit detection (circular sprites, alpha checks, etc).
- FIX: Caret and selection will now work properly even with one long word that doesn't fit.
- FIX: UITable will now always update the scroll view's scroll bars.

3.4.2 (previously 3.0.9 f3)
- FIX: Work-around for a bug in Unity that was causing prefabs to be marked as edited (version control).
- FIX: Optimized how UIInput works on mobiles, and setting UIInput.value will now force it through validation.
- FIX: UICamera's raycast now always considers cumulative alpha and ignores invisible objects.
- FIX: "Constrain but don't clip" option will no longer cause widgets to get culled.
- FIX: UILocalize should now work properly when attached to UIInput's label.
- FIX: UITextList will no longer die IRL if not even a single line can fit.
- FIX: Text List should now use Y-padding properly for scrolling.
- FIX: Scroll bar should no longer cause NaNs in some situations.
- FIX: Packed fonts fix.

3.4.1 (previously 3.0.9 f2)
- FIX: UITweener will again keep persistent OnFinished delegates.
- FIX: Widgets that are invisible will disable their box collider as needed.
- FIX: Minor tweak related to widget alpha checks.

3.4.0 (previously 3.0.9 f1)
- NEW: Community contribution: bold, italic, underline, strike-through and subscript support for text (Rudy Pangestu).
- NEW: You can now use TweenPosition on anchored widgets and panels.
- NEW: You can now nudge anchored widgets, panels and containers (arrow keys).
- NEW: It's now possible to resize and move anchored panels and widgets in the scene view.
- FIX: You can now re-activate a tween in its OnFinished callback and set a new OnFinished callback without having it execute immediately.
- FIX: Force-replace the GUI/Text shader with Unlit/Text inside UIDrawCall, seeing as GUI/Text was still used for dynamic text (ugh!)
- FIX: Create Scroll view option from the NGUI menu should now correctly add the UIScrollView script.
- FIX: Orange outline showing scroll view content should now update while dragging content around at edit time.
- FIX: Widget and panel undo should now work properly even when it's anchored.
- FIX: Fix for the issue with panels starting with alpha of 0.
- FIX: Dragging using the slider's thumb should now reach 0 and 1 properly.
- FIX: UIPlaySound set to trigger on hover will no longer play after the button was clicked.
- FIX: Clicking a scroll view set to center on children should no longer conflict with Center On Child logic.
- FIX: Widget aspect ratio will now automatically update when dragging the widget's dimensions even when it's not used.
- FIX: Added a few extra null checks to avoid edge case issues such as destroying draw calls on quit.
- FIX: Component selector (atlas / font selection) now has a scroll bar.
- FIX: FindInParents should now work as expected in Unity 4.3 (Unity regression bug work-around).
- FIX: 'Delete' key is now able to delete the last character correctly.
- FIX: Some extra checks to eliminate possible NaN issues.
- FIX: Gradient on labels should now look correct with fixed size UIRoot.
- FIX: Draw calls from non-automatic Render Q panels will now be more careful with their Z position.

3.3.6 (previously 3.0.8 f7)
- FIX: UIPanel's "explicit" render queue option should now work correctly.
- FIX: UITweener.Play should behave better with duration of 0.
- FIX: NGUITools.FindCamera will prioritize the Main Camera over others (fix for Unity Water).
- FIX: Null exception fix in UIKeyBinding.

3.3.5 (previously 3.0.8 f6)
- FIX: Labels using atlassed fonts will again correctly use the pixel size setting.

3.3.4 (previously 3.0.8 f5)
- NEW: Added a flag to UIDragDropItem that lets you drag a clone of the object rather than the object itself.
- FIX: Labels limited by number of lines with resizable height were not wrapped properly.
- FIX: Added UITable's "keep within panel" checkbox to the UIGrid as well.
- FIX: UIButtonKeys will now respect disabled objects.
- FIX: UIPlayAnimation will now respect UIButton's "Drag Over" state if UIButton is present.
- FIX: UIKeyBinding will now set the UICamera.currentTouch.current properly.
- FIX: UIWidget.CreatePanel will now also invalidate the parent reference.
- FIX: More changes related to how dynamic text is drawn...

3.3.3 (previously 3.0.8 f4)
- NEW: Added a script that can animate Unity 2D sprite (UI2DSpriteAnimation).
- FIX: Tweaks to how PlayAnimation works in regards to dragging over/out.
- FIX: Labels will always be created with even dimensions.
- FIX: More text printing related tweaks.

3.3.2 (previously 3.0.8 f3)
- FIX: Dynamic fonts should now be positioned better.
- FIX: Fixing how fonts behave with a pixel size of non-1.
- FIX: Sliders should no longer shrink the foreground sliced sprite beyond its minimum dimensions.
- FIX: Couple of fixes related to how anchors work, making them work better with prefabs.
- FIX: Grid and table scripts were updating the scroll views even though they shouldn't have been.
- FIX: Removed the UIRect requirement from TweenAlpha.

3.3.1 (previously 3.0.8 f2)
- FIX: Fix for widgets not adding themselves to draw calls when enabled in some cases.

3.3.0 (previously 3.0.8)
- NEW: Input field has been redesigned and now has caret, multi-line selection, click-move, drag select, arrow key navigation, and full copy/paste.
- NEW: Widgets now have a new "aspect ratio" field, in case you want them to keep a specific aspect ratio.
- NEW: Community contribution (Nicki): Sliced & Tiled sprite via the Advanced sprite type setting.
- NEW: All panels now manage their own draw calls rather than working with one giant list, improving performance.
- NEW: Widgets no longer have a global list, and are always managed per-panel.
- NEW: Enabling/disabling widgets no longer affects other panels.
- NEW: Optimization pass. Significantly reduced the time spent in UIPanel.LateUpdate.
- NEW: Added a delegate to the widget class that gets called when the widget's dimensions or position changes.
- FIX: Center-aligned odd width multi-line labels will now always have pixel-perfect lines.
- FIX: Draw calls were not added correctly to the list of active draw calls.
- FIX: Scroll wheel scrolling is now affected by the transform's rotation properly.

3.2.3 (previously 3.0.7 f3)
- NEW: Added an option for anchors to be offset by the panel's position.
- NEW: Made it possible to anchor directly to a Camera, without having to use panels.
- NEW: Made "Keep crisp" option always show up for dynamic fonts.
- FIX: Anchoring to a 3D object at edit time will no longer move the widget's initial position.
- FIX: Account for objects being behind the camera (and thus not visible) when anchoring to 3D game objects.
- FIX: Invisible widgets with colliders will now auto-resize them correctly.
- FIX: Improved how baseline is calculated (with a hack!), making fonts be positioned better.
- FIX: Filled sprites should now ignore the padding.

3.2.2 (previously 3.0.7 f2)
- NEW: You can now right-click on tweens to set the 'from' and 'to' values using the current.
- FIX: Tweens no longer reset the object to its default value when first added (current value is now used instead).
- FIX: Non-clipped panels will no longer use their position when calculating dimensions for anchors.
- FIX: Panels can now use advanced anchors properly (partial anchoring).
- FIX: Anchoring to a transform should no longer reposition the widgets and panels.
- FIX: Cleanup of warnings that don't show up on the Windows version of Unity.
- FIX: Button should now keep the highlighted state correctly when using controller input.
- FIX: Unity has a bug related to input on BB10, apparently (backspace).

3.2.1 (previously 3.0.7 f1)
- NEW: Further improved the layout system's presentation, making it less daunting.
- NEW: Enabling anchoring will automatically anchor to the first parent by default.
- NEW: It's now possible to automatically anchor to the mid-points (sides, center).
- NEW: Made it possible to move and scale anchored widgets.
- FIX: Rotating a widget should no longer hide its side handles.
- FIX: Mobile keyboard will now have the multi-line option.
- FIX: Re-added support for packed fonts.

3.2.0 (previously 3.0.7 rc1 & 2)
- NEW: Created a new layout system. All widgets and panels can now anchor to each other, the screen, and even 3D game objects.
- NEW: You can now create resizable scroll views and anchor them to UI elements.
- NEW: Re-created the Anchor Example to use the new anchoring system.
- NEW: Updated all controls to use the new anchoring system.
- NEW: You can now specify an explicit Render Queue on each panel.
- NEW: Improved the Text List's functionality, adding support for touch interaction and having a scroll bar.
- NEW: Recreated the Chat Window example -- it now features a resizable chat window.
- NEW: Recreated the Drag & Drop example, adding two scroll views resized with screen height, and the ability to move items from one to the other.
- NEW: Holding CTRL will now show the dimensions of the selected widget in the scene view.
- NEW: Resizing the widget now automatically displays width and height guides in the scene view.
- NEW: Selected anchored widgets and panels now show the calculated distance in the scene view.
- NEW: Widget alpha is now fully cumulative (parents affect children).
- NEW: UIDragObject script now ensures that the dragged object remains pixel-perfect.
- NEW: UIDragObject script now can restrict the widget from being dragged off-screen.
- NEW: Added a script that makes it possible to resize a widget by dragging on its corner or side.
- NEW: UICamera.currentScheme tells you the current control scheme -- mouse, touch, or controller.
- NEW: Button scripts have been modified to use the new OnDragOver/Out events
- NEW: Added an option to the widget anchor to hide itself if it's off-screen.
- NEW: Drag Object script now lets you specify an explicit bounds rectangle and has an improved inspector.
- NEW: Added a button to UIButtonColor that can automatically replace it with a UIButton.
- NEW: Added the ability to copy/paste all values of the sprites and labels via right-click on the component.
- NEW: Added a "next page threshold" value to UICenterOnChild for when you want to swipe to move to the next page.
- NEW: If the mouse events are off and touch events are on, NGUI will now fake touches using the mouse in the editor.
- FIX: Changing panel depth in inspector will now reflect the change correctly.
- FIX: Atlas/font selection dialog will now make searching of the entire project optional.
- FIX: UICamera events will once again work independently of time scale.
- FIX: Fixed the glitch that was causing widgets to jump into the middle of nowhere sometimes when resizing them.
- FIX: UIDragScrollView will no longer try to find the scroll view if you set it manually.
- FIX: Enabling and disabling textures and Unity 2D sprites will now again set the correct texture.
- FIX: Adjusting depths via shortcut keys should now work consistently.
- FIX: Draw call viewer will now display the correct triangle count.
- FIX: NGUITools.SetActive will now automatically call CreatePanel on widgets, ensuring that there is no frame delay (read: blinking).
- FIX: UICamera selected object change should now work multiple times per frame.
- FIX: Added a new clause to panel depth comparison that uses panel instance IDs if the panel depth matches (to avoid depth collisions).
- FIX: Max line count on labels should now work again.
- FIX: Fixed the Drag Objects script on mobile devices. It was not applying momentum properly.
- DEL: OnHover is no longer sent via selection changes. Listen to OnSelect and check (UICamera.currentScheme == ControlScheme.Controller).
- DEL: "PMA Shader" option is now going to be permanently hidden once the atlas has been created.
- DEL: Eliminated the half-pixel offset setting from anchors.
- DEL: Removed anchor and stretch scripts from the menus.

3.1.0 (previously 3.0.6)
- NEW: NGUI now has new written documentation.
- NEW: NGUI now has an abundance of context-sensitive help. Just right click on an NGUI component and choose the Help option.
- NEW: NGUI now has robust context menus letting you add, create and modify widgets by right-clicking on stuff in the Scene View.
- NEW: Added snapping support for widget placement. Edge selection restricted to siblings and parent.
- NEW: You can now find an assortment of ready-made controls ready to be drag & dropped into your scenes (search for "Wooden").
- NEW: You can now drag & drop GUI prefabs from your Project Folder right into the Scene View. No need to create the UI beforehand.
- NEW: You can now copy/paste label styles by right-clicking the UILabel script in Inspector.
- NEW: Redesigned the draggable panel class a bit, and renamed it to UIScrollView. UIDragPanelContents is now UIDragScrollView.
- NEW: Labels can now have gradients.
- NEW: Clipped panels now have handles you can drag around instead of adjusting clipping in inspector.
- NEW: Added a new widget type capable of drawing Unity 4.3 sprites.
- NEW: Added UIToggle.GetActiveToggle.
- NEW: You can now specify a material on dynamic font-using UILabels.
- NEW: You can now specify character spacing on labels, and it works with both bitmap and dynamic fonts.
- NEW: Labels set to maintain their crispness will now take UIRoot's size into account, resulting in crisp labels with fixed size UIs.
- NEW: Added a simple script that makes it possible to center a scrollable panel on a child when clicked on.
- NEW: Redesigned the scroll bar and the slider components. They now also derive from a new common class (Progress Bar).
- NEW: UIButtonKeyBinding has been replaced with UIKeyBinding and its functionality has been enhanced.
- NEW: Added the ability to extract sprites from the atlas.
- NEW: Added a progress bar to the atlas maker when it's updating the atlas.
- NEW: You can edit and delete sprites within the sprite selector window via right-click.
- NEW: Created a separate Draw Call Tool window instead of displaying draw calls on the panels.
- FIX: Sprite selection is now cohesive and updates the atlas maker, sprite selector, and inspector.
- FIX: Sprite selection window should now handle large lists of sprites better.
- FIX: Panels will now add rigidbodies to themselves since Unity 4.3 mentions it should improve performance.
- FIX: UIScrollView's movement restriction now makes sense (no more weird 'scale')
- FIX: Draggable panels should no longer move on Play.
- FIX: Improved performance by reducing GC allocations and mesh assignments.
- FIX: Typewriter and text list scripts can now be used with dynamic fonts.
- FIX: Reference atlas references should no longer get broken when modifying sprites.
- FIX: Popup list now offers a way to change the font's size even for fixed size fonts.
- FIX: Popup list was not enveloping the content correctly.
- FIX: Atlas and font selector will now show all existing assets, not just recently used ones.
- FIX: Atlas maker should no longer throw an exception when trying to edit old atlases (SciFi etc).
- FIX: Enabling and disabling widgets will no longer cause the draw call list to be rebuilt unless it's necessary.
- FIX: Improving the process of NGUI remembering the last used values.
- FIX: Drag handles will now hide if there is not enough space to draw them.
- FIX: Anchors and stretch scripts set to "run only once" will still respond to screen size changes.
- FIX: Switching panel to clipped mode then back should refresh the shader correctly.
- FIX: Moving widgets around no longer causes their buffers to get rebuilt. Just re-transformed.
- FIX: Added extra code to ensure that draw calls won't get orphaned.
- FIX: Panel alpha is now cumulative (parents affect children).
- FIX: Got rid of old double-buffering code that was causing issues.
- FIX: More Win8 tweaks.
- DEL: UIPopupList no longer has a 'textLabel' option. Instead use label's SetCurrentSelection for OnValueChanged.
- DEL: UIGrid and UITable no longer have 'repositionNow' member variable. Right-click it to execute it instead.

3.0.5
- NEW: Added a way to set Localization's language using specified name and dictionary combo
- NEW: Added UIInput.onChange that gets called whenever the input field's text changes for any reason.
- NEW: Right-clicking in the scene view with a UI element selected now presents the hierarchy list underneath.
- FIX: Widget selection logic had a bug in it that would select the wrong widget in some cases.
- FIX: Label "Max Lines" setting now works correctly with the "Shrink Content" overflow setting.
- FIX: Draggable panel now uses whole numbers, keeping itself pixel-perfect.
- FIX: UIPlayAnimation will now work fine with multiple OnFinished calls.
- FIX: Made UITextList work with dynamic font-using labels.
- FIX: Popup list was not serializing dynamic fonts correctly.
- FIX: UILabel will no longer use minimum size of zero.
- FIX: Color symbols were not recognized quite right.
- FIX: BetterList.Sort now uses Array.Sort.
- FIX: Removed warnings in Unity 4.5.
- FIX: Null check on UIInput.Append.
- FIX: Flash platform compile fixes.

3.0.4 (merged 3.0.3 letter updates)
- NEW: UIPanel will now show the render queue number used to draw the geometry.
- NEW: You can now specify a bitmap font in the widget wizard.
- NEW: Added the ability to auto-resize the widget's box collider.
- FIX: Draggable panel's scroll bars will now hide correctly when they should be hidden.
- FIX: Scroll bar will no longer force the sprite to be pixel perfect.
- FIX: UIInput was not hiding the password characters on deselect.
- FIX: Additional checks to ensure the UILabel cleans up texture rebuild callbacks.
- FIX: Adjusting the depth of a panel via NGUITools.AdjustDepth will now affect child panels.
- FIX: UILabel.ProcessAndRequest was not calling ProcessText for labels using bitmap fonts.
- FIX: Labels with encoded colors will now wrap properly.
- FIX: It's no longer possible to set the sprite width and height to zero before assigning a sprite.
- FIX: Raycasts that hit no widgets will now be ignored.
- FIX: Fixed out of bounds exception when labels ended with [-].
- FIX: UIWidget.ResizeCollider no longer does anything if the widget is disabled.
- FIX: UIInput will no longer clear the text of multiple labels on mobile platforms.
- FIX: UIInput.Submit() now sets the UIInput.current correctly.
- FIX: Backwards compatibility additions.
- FIX: Event delegate setting fix.
- FIX: Unity 3.5 fixes.
- FIX: WP8 fixes.

3.0.3:
- NEW: You no longer need to create UIFonts for dynamic fonts and can now specify font size and style directly on your labels.
- NEW: As dynamic font-using label shrinks, it can automatically print with lower font size, maintaining its crispness.
- NEW: You can now multi-edit sprites and labels.
- NEW: UIInput has been redone, and now supports moving the caret as well as copy/paste keys in the editor.
- NEW: UIInputValidator script's functionality is now a part of UIInput.
- NEW: You can now create invisible widgets in case you want a simple way of intercepting events.
- NEW: You can now use anonymous delegates with the EventDelegate.
- FIX: UICamera.selectedObject changes are now delayed until end of frame.
- FIX: GUI/Text shader is no longer used, replaced with an Unlit/Text shader instead.
- FIX: Added a by-material sorting clause to widgets with conflicting depth, automatically reducing draw calls.
- FIX: Some UITextures were still mistakenly referencing the Unlit/Texture shader.
- FIX: Mouse events will no longer be processed if there are active touch events.
- FIX: Popup list was not respecting the text scale correctly.
- FIX: CalculateRaycastDepth will now ignore disabled widgets.
- FIX: WP8 compile fix.
- EDT: UILabel.font is now UILabel.bitmapFont, for clarity.
- DEL: UILabel no longer has the 'password' option since it never made sense to have it there to begin with.
- DEL: Got rid of the UpdateManager. It really should have been killed 2 years ago.

3.0.2:
- NEW: Added a "depth" property to the panels to make it possible to easily order panels.
- NEW: UICamera now has "world" and "UI" event types that affect how raycasts are processed.
- NEW: Collider's Z position no longer needs to be adjusted for widgets if the UICamera is set to the "UI" event type.
- NEW: UICamera's raycasts now go by widget and panel depth rather than by distance to the colliders.
- NEW: UIPanels now can show all draw calls instead of just their own.
- NEW: UIStretch can now stretch clipped panels.
- FIX: UITable was bugged with the "Up" direction.
- FIX: Labels will process their text before returning the corners.
- FIX: UIAnchor was not calculating widget-related anchoring properly.

3.0.1:
- FIX: Variety of regression fixes from NGUI 3.0.0 that culminated into alphabetic (bug-fix) micro-updates.
- FIX: MakePixelPerfect on selection no longer stops if it finds a UIWidget. It continues on to children.
- FIX: Atlas Maker will keep the border setting of sprites being replaced.
- FIX: UILabels will no longer MakePixelPerfect when their text is assigned.
- FIX: Marking widgets as changed will now mark them as edited in Unity.
- FIX: Sliced sprite border will again take pixel size into consideration.

*** WARNING ***
PLEASE BACK UP YOUR PROJECT BEFORE UPDATING!
3.0.0 is a major changeset. You will need to open and re-save all of your scenes and prefabs after updating!
After updating, expect some things to no longer work the same way they used to. Widgets scale is no longer
used as its size, so any code that you had relying on this will need to change to use 'width' and 'height'.
You can also expect compile errors related to delegate usage. The following links may help you:

http://www.youtube.com/watch?v=uNSZsMnhS1o&list=UUQGZdUwzE8gmvgjomZSNFJg
http://www.tasharen.com/forum/index.php?topic=11.msg27296#msg27296

3.0.0:
- NEW: Changed the way widgets get batched, properly fixing all remaining Z/depth issues.
- NEW: Draw calls are now automatically split up as needed (no more sandwiching issues!)
- NEW: Re-designed the way widget width & height gets specified. The values are now explicit, and scale is no longer used.
- NEW: NGUI will now automatically replace UITextures with Sprites when they get added to an atlas.
- NEW: It's now possible to have clipped panels in 3D and not have them break when tilting the camera.
- NEW: It's now possible to nest widgets.
- NEW: It's now possible to have multiple widgets on the same object.
- NEW: It's now possible to change the selection handles color via the NGUI menu.
- NEW: UICheckbox is now a UIToggle, and you now specify a 'group' ID rather than a common root object.
- NEW: Added TweenWidth and TweenHeight to tween widgets width and height properties.
- NEW: You can now specify the label overflow method: shrink content, clamp content, resize height, or resize freely.
- NEW: When labels are in "resize label" overflow mode, the drag handles will be greyed out.
- NEW: Added a simple EventDelegate class and improved all generic components to use it.
- NEW: Added a Widget Container class that can be used to easily select and move groups of widgets (think: buttons, windows).
- NEW: Added the RealTime helper class that removed IgnoreTimeScale. Usage: RealTime.time, RealTime.deltaTime.
- NEW: Improved the inspector look of just about every component.
- NEW: UIPanel now shows a list of all of its draw calls and the widgets causing them.
- NEW: Added a way to auto-normalize the depth hierarchy from the NGUI menu.
- NEW: You can now hide explicit draw calls by collapsing the draw call fold-outs on the panel.
- NEW: Sprite selection window now shows sprite names as well.
- NEW: Atlas maker will now automatically sort the sprites, saving them in an alphabetical order.
- NEW: UICamera now has a better inspector, and it automatically hides properties if it's not the main one.
- FIX: CTRL+ and CTRL- now adjust all widgets under the selected object.
- FIX: Labels will now again align vertically properly.
- FIX: Atlas maker will now respect textures that were imported with a non-native size.
- FIX: Atlas maker will no longer change so many import settings on source textures.
- FIX: Make Pixel Perfect is now undoable.
- FIX: You can once again rename sprites in the atlas.
- DEL: Removed the long-ago deprecated UISlicedSprite, UITiledSprite, and UIFilledSprite classes and some other legacy code.

2.7.0:
- NEW: Added a way to resolve all Depth/Z issues. Check your UIPanels and enable Depth Sorting.
- FIX: DownloadTexture no longer leaves a shadow of the previous texture behind.
- FIX: UIDragObject will no longer behave oddly with multiple touches.
- FIX: Popup menu will now correctly trigger OnChange functions on the same selection.
- FIX: UITexture will now default to the Unlit/Transparent Colored shader.
- FIX: Atlas Maker will no longer default sprite list to be hidden.

2.6.5:
- FIX: Labels now have "Max Height", and "Max Lines" again works as expected.
- FIX: Widgets no longer store a reference to texture and material.
- FIX: Fix for some issues with the dynamic fonts.
- FIX: Removed the "password" setting from UILabel in order to clear up some confusion.
- FIX: Transparent colored shader no longer has fixed function pipeline code inside.
- FIX: Atlas maker will now be more perforce-friendly.
- FIX: Popup list will no longer show on Click when the Popup List script is disabled.
- FIX: NGUIMath.Calculate functions will now ignore recently disabled widgets.
- FIX: UIWidget will no longer attempt to create a panel until after Start().
- FIX: UICamera.touchCount and UICamera.dragCount will now work correctly with multi-touch turned off.
- FIX: WP8 and BB10 tweaks.

2.6.4:
- NEW: UIStretch now has the 'run once' option matching UIAnchor.
- FIX: Non-sticky press was not working quite right...
- FIX: Rewrote the transform inspector.
- FIX: Removed the "depth pass" option from the panel's inspector since 99.9% of the people were mis-using it.
- FIX: UIButtonKeys.startsSelected got broken at some point.
- FIX: UIPopupList now respects atlas pixel size and again works correctly for menu style popups.
- FIX: UIPanel will no longer keep references to materials when disabled.

2.6.3:
- NEW: Noticeably improved performance and garbage collection when using Unity 4.1+
- NEW: It's now possible to select sprites in the Atlas Maker for preview purposes.
- NEW: Transform inspector will now warn you when widget panel is marked as 'static'.
- NEW: You can now toggle the panel's "widgets are static" flag from within the panel tool.
- FIX: Widgets will no longer be constantly checking for layer changes in update.
- FIX: Shrink-to-fit labels will now auto-grow when possible.
- FIX: Labels can no longer be resized using handles (but can still be moved and rotated).
- FIX: Labels will now auto-adjust their size properly when the max width gets adjusted.
- FIX: Creating an atlas would rarely throw a null exception. This has been fixed.
- FIX: Draggable panel + non-sticky keys will now mix properly.
- FIX: Drag & drop should now work with non-sticky press.
- FIX: Flash export should now work again.
- DEL: Dropped all remaining support for Unity 3.4.

2.6.2:
- NEW: You can now automatically apply alpha pre-multiplication to textures when creating an atlas.
- NEW: Added UIWidget.Raycast to perform a raycast without using colliders.
- NEW: Added a texture preview to UITexture.
- NEW: Added an option to UIAnchor to run only once, and then destroy itself. Also optimized it slightly.
- NEW: Transform inspector will now gray out fields that are not commonly used by the UI when a widget is selected.
- FIX: Transform multi-object editing was not quite right for widgets...
- FIX: "Shrink to fit" option on labels now works vertically, not just horizontally.
- FIX: Changing a sprite in inspector will no longer auto-resize it. Use MakePixelPerfect to resize it.

2.6.1:
- FIX: Dynamic font-related fixes.
- FIX: Depth pass will now be force-disabled when the panel is clipped.
- FIX: Sticky press option on the UICamera no longer breaks OnDrop events.
- FIX: UIInput's useLabelTextAtStart should now work again.
- FIX: UICamera.touchCount should now be accurate.
- FIX: Fixed a typo in the image button inspector.
- FIX: UIWidget.UpdateGeometry will now check for object's disabled state prior to filling the geometry.

2.6.0
- NEW: Added dynamic font support for Unity 4.0.
- NEW: Handles can now be toggled on/off from the NGUI menu.
- NEW: Atlas maker will now be limited by max texture size, and will no longer make it possible to corrupt an atlas.
- NEW: Warning will be shown on the panel if clipping is not possible (GLES 1.1).
- NEW: Toggle can now have fade in the checkmark instantly.
- NEW: You can now leave C++ style comments (//) in the localization files.
- NEW: You can now paste into input fields in stand-alone builds.
- NEW: Added disabled state to UIImageButton (Nicki)
- FIX: UISlider will now use the sprite size rather than collider size to determine the touch effect area.
- FIX: Resetting the tween will now mark it as not started.
- FIX: Blank labels will no longer be localized.
- FIX: Resetting the sprite animation will also reset the sprite back to 0.

2.5.1
- NEW: Added a "shrink to fit" option for labels that will scale down the text if it doesn't fit.
- FIX: Re-added the "import font" field in the font inspector.

2.5.0
- DEL: Deprecated Unity 3.5.4 and earlier support. If you are using 3.5.4 or earlier, DO NOT UPDATE!
- OLD: Sliced, tiled, and filled sprites have been deprecated.
- NEW: Regular sprite now has options for how the sprite is drawn.
- NEW: NGUI widgets now have visual placement handles.
- NEW: Adding a widget now automatically creates a UI hierarchy if one is not present.
- NEW: NGUI menu has been redesigned with new options and shortcut keys.
- FIX: Widget selection box now takes padding into account properly.
- FIX: Changing the pivot no longer moves the widget visually.
- FIX: Font symbols now use padding instead of inner rect for offset.
- FIX: Font symbols no longer need to be used in the editor before they are usable in-game.
- FIX: More fixes to how tweens get initialized/started.
- FIX: Re-added UISlider.fullSize property for better backwards compatibility.
- FIX: Unity 4.1-related fixes.
- FIX: Variety of other minor tweaks and changes.

2.3.6
- NEW: Added a much easier way to add symbols and emoticons (select the font, you will see it).
- NEW: Added a couple of conditional warnings to the UIPanel warning of common mistakes.
- NEW: Various improvements to widget and sprite inspectors.
- FIX: There is no need to display the "symbols" option on the labels if the font doesn't have any.
- FIX: Removed the hard-coded screen height-based touch threshold on the UICamera.
- FIX: Removed the need for sliders to have a "full size" property.

2.3.5:
- NEW: Font symbols can now have an offset for easier positioning.
- FIX: UISlider will now set the 'current' property before calling the delegate.
- FIX: Fixed the toggle animation issue that was brought to light as a result of 2.3.4.
- FIX: Minor other tweaks, nothing important.

2.3.4:
- NEW: Added the ability to easily copy/paste widget colors in the inspector.
- FIX: Random fixes for minor issues noted on the forums.
- FIX: Minor performance improvements.

2.3.3
- NEW: UIPanels now have alpha for easy fading, and TweenAlpha can now tween panels.
- NEW: Added UICamera.debug mode for when you want to know what the mouse is hovering over.
- NEW: Added AnimatedColor and AnimatedAlpha scripts in case you want to animate widget color or alpha via Unity's animations.
- NEW: Android devices should now be able to support a keyboard and a controller (OUYA).
- NEW: Added UIFont.pixelSize, making it possible to have HD/UD fonts that are not a part of an atlas.
- FIX: Unity 4.1 optimization fix.
- FIX: Label shadow should now be affected by alpha using PMA shaders.
- FIX: UIToggle.current will now work correctly for toggle event receivers.
- FIX: UIButton series of scripts should now initialize themselves on start, not when they are used.
- FIX: TweenOrthoSize should now tween the size instead of FOV (o_O).
- FIX: Sprite selection window will now show sprites properly when the atlas is not square.
- FIX: UIAnchor should now always maintain the same Z-depth, and once again works in 3D UIs.

2.3.1
- NEW: Added UICamera.touchCount.
- NEW: Added an option on the UIInput to turn on auto-correction on mobiles.
- FIX: Fixed compilation on Unity 3.
- FIX: Font inspector will now display the font in a preview window.

2.3.0:
- NEW: Added Premultiplied Alpha support to NGUI along with the appropriate shaders.
- NEW: Added UIButtonKeyBinding script that makes it easy to do button key bindings.
- NEW: Transform inspector now supports multi-object editing (contribution by Bardelot 'Cripple' Alexandre)
- NEW: UIRoot's 'automatic' flag is now gone, replaced by a more intuitive drop-down list.
- NEW: It's now possible to make UIRoot fixed size on mobiles, but pixel-perfect on desktops (it's an option).
- NEW: You can now specify an animation curve on all tweens.
- NEW: Localization will now attempt to load the starting language automatically.
- NEW: Added UICamera.onCustomInput callback making it possible to add input form custom devices.
- NEW: Support for optimizations in Unity 4.1.
- FIX: Tweaks to Localization to make it easier to use. You can now just do Localization.Localize everywhere.
- FIX: UILocalize attached to a label used by input will now localize its default value instead.
- FIX: Kerning should now get saved properly. You will need to re-import your fonts.
- FIX: UICamera with multi-touch turned off should now work properly when returning from sleep.
- FIX: ActiveAnimation's onFinished callback will no longer wait for all animation states to finish (only the playing one).
- FIX: UICamera's touch detection should now work properly when returning from sleep.
- FIX: Changed the way MakePixelPerfect works a bit, hopefully fixing an issue with sprites moving by a pixel.
- FIX: UIPanel should now display the clipped rectangle correctly.
- FIX: UIInputSaved will now save on submit.
- DEL: Removed UIAnchor.depthOffset seeing as it caused more confusion than anything else. Just use an offset child GameObject.
- DEL: Deprecated hard clipping, seeing as it causes issues on too many devices.

2.2.7:
- NEW: Added UICamera.stickyPress option that makes it possible for multiple objects to receive OnPress notifications from a single touch.
- NEW: UICamera.hoveredObject now works for touch events as well, and will always hold the result of the last Raycast.
- NEW: Added "Edit" buttons to all atlase and font fields, making easy to select the atlas/font for modification.
- NEW: Added Localization.Localize. Was going to change Localization.Get to be static, but didn't want to break backwards compatibility.
- FIX: Inventory example should work correctly in Unity 4.0.
- FIX: You can now set UILabel.text to null.
- FIX: UIPanel was not drawing its rect correctly in some cases.
- FIX: Assortment of tweaks and fixes submitted by Andrew Osborne (community contribution).
- FIX: Switching a mainTexture of a UITexture belonging to a clipped panel will now work properly.

2.2.6:
- NEW: Mouse and touch events now have an option to be clipped by the panel's clipping rect, just like widgets.
- NEW: Made it possible to delete several sprites at once (Atlas Maker).
- FIX: Added proper support for Unity 4-based nested active state while maintaining backwards compatibility.

2.2.5:
- NEW: Double-clicking a sprite in the sprite selection window will now close the window.
- FIX: UIRoot will now only consider min/max clamping in automatic mode.
- FIX: Password fields should now get wrapped properly.
- FIX: MakePixelPerfect() will now preserve negatives.
- FIX: UISlider will no longer jump to 0 when clicked with the controller.

2.2.4:
- NEW: SpringPanel and UICenterOnChild now have an OnFinished callback.
- NEW: UIForwardEvents now has OnScroll.
- FIX: UISavedOption now unregisters the state change delegate when disabled.
- FIX: IgnoreTimeScale clamps time delta at 1 sec maximum, fixing a long pause after returning from sleep.
- FIX: UIWidget now correctly cleans up UITextures that have been re-parented.
- FIX: Tween scripts now sample the tween immediately if the duration is 0.
- FIX: UIFont and UIAtlas MarkAsDirty() function now works correctly with a reference atlas (in the editor).

2.2.3:
- FIX: Small fix for UIAnchor using a clipped panel container (thanks yuewah!)
- FIX: Work-around/fix-ish thing for Unity Remote sending both mouse and touch events.
- FIX: hideInactive on UIGrid should now function correctly.

2.2.2:
- NEW: You can now specify a minimum and maximum height on UIRoot.
- NEW: Label shadow and outline distance can now be modified.
- NEW: Added UIButtonActivate -- an extremely simple script that can be used to activate or deactivate something on click.
- NEW: Creating a new UI will now automatically add a kinematic rigidbody to the UIRoot, as it's supposedly faster for physics checks.
- NEW: Game objects destroyed via NGUITools.Destroy will now automatically get un-parented.
- NEW: UIEventListener now has an OnKey delegate.
- FIX: Sprite preview should now display wide sprites correctly.
- FIX: Fixed copy/paste error in the atlas inspector (thanks athos!).
- FIX: UIGrid will no longer consider destroyed game objects.
- FIX: Couple of other smaller fixes.

2.2.1:
- FIX: Sprite list should now be faster.
- FIX: Sprite border editing should now work properly again.
- FIX: A couple of other minor fixes.

2.2.0:
- NEW: Added a sprite selection window that replaces the drop-down selection list. Think texture selection window for your sprites. The sprite selection window has a search box to narrow down your selection.
- NEW: Sprite preview is now shown in the Preview window, and is affected by the widget's color tint.
- NEW: Added warning messages when more than one widget is using the same depth value, and when more than one atlas is used by the panel.
- NEW: It's now possible to edit a sprite quickly by choosing the "edit" option.
- NEW: When editing a sprite in the atlas, a "Return to ..." button is shown if you've navigated here from a sprite.
- FIX: UIAnchor and UIStretch now work with labels properly.
- FIX: UITexture will no longer occasionally lose the reference to its texture.
- FIX: NGUITools.EncodeColor now works in Flash (created a work-around).

2.1.6:
- NEW: UISavedOption now works on a popup list as well.
- FIX: Replaced ifdefs for Unity 4 with a new helper functions for cleaner code (NGUITools.GetActive and NGUITools.SetActiveSelf).
- FIX: UITable was not properly keeping the contents within the draggable panel.
- FIX: UIDraggablePanel.UpdateScrollbars was not considering soft clipping properly, resulting in some jitterness.
- FIX: SpringPanel was not setting position / clipping when it finished, resulting in floating-point drifting errors.
- FIX: UIInput's "not selected" text can now be localized using UILocalize.

2.1.5:
- NEW: Added support for Unity 4.
- NEW: NGUI now uses Unity 3.5.5's newly-added Color32 for colors instead of Color, reducing the memory bandwidth a bit.
- NEW: UIStretch can now stretch to another widget's bounds, not just the screen.
- FIX: UIImageButton will no longer add a box collider if a non-box collider is present.
- FIX: NGUITools.ParseSymbol will now check to see if the symbol is valid.
- FIX: UITexture-related tweaks to UIWidget.
- FIX: UIAnchor can now anchor to labels.
- FIX: UISlicedSprite no longer uses padding.

2.1.4:
- NEW: UIInput now supports multi-line input if its label is multi-line. Hold Ctrl when hitting Enter.
- FIX: UIToggleControlledComponent will now use delegates by default.
- FIX: UITexture should now work properly again.

2.1.3:
- NEW: Seeing as it was an often-asked question, the Scroll View example now features a toggle that makes the scrolled list center on items.
- NEW: UIAnchor can now anchor to sides of other widgets and panels.
- NEW: UICamera now has "drag threshold" properties. Drag events will only be sent after this threshold has been exceeded.
- NEW: You no longer have to create a material for the UITexture.
- NEW: You can now specify a UV rect for the UITexture if you only wish to display a part of it.
- NEW: All event senders, tweens and animation components now have a delegate callback you can use instead of the SendMessage-based event receiver.
- NEW: Added UICamera.current and UIPopupList.current.
- NEW: SpringPosition now has "on finished" event notifications (both event receiver and delegate).
- NEW: Added a new script that can be used to change the alpha of an entire panel worth of widgets at once: UIPanelAlpha.
- FIX: Replaced most usages of List with BetterList instead in order to significantly reduce memory allocation.
- FIX: Custom texture packer now respects padding correctly.

2.1.2:
- NEW: Selected widgets now show their panel's bounding rect, which is the screen's rect if the panel isn't clipped.
- FIX: Tweens that have not been added dynamically will start playing correctly.
- FIX: Texture packer should now have better packing logic.

2.1.1:
- NEW: New texture packer, alternative to using Unity's built-in one. Default is still Unity for backwards compatibilty.
- NEW: Added a different line wrapping functionality for input fields contributed by MightyM.
- NEW: UILocalize now has a "Localize" function you can trigger to make it force-localize whatever it's on.
- NEW: UITweener now has an option to not ignore timeScale.
- FIX: Fixed a "drifting panel" issue introduced in the last update.
- FIX: Added a warning for slider thumb used with radially filled sliders (not supported).
- FIX: ActiveAnimation will now clear its event receiver and callback on Play.
- FIX: UISpriteAnimation.isDone is now UISpriteAnimation.isPlaying, and is no longer backwards.

2.1.0:
- NEW: Now maintained under Unity 3.5.3.
- NEW: BetterList now has Insert and Contains functions.
- NEW: UITweener now has bounce style tweening methods.
- NEW: UITweener's OnUpdate function now has "isFinished" parameter that's set to 'true' if it's the last update.
- NEW: TweenTransform is now capable of re-parenting the object when finished.
- NEW: Added TweenVolume that can tween an audio source's volume.
- NEW: UICamera now has a new property: "Generic Event Handler". If set, this object will receive a copy of all events regardless of where they go.
- NEW: Widget Wizard now lets you specify an initial pivot point for sprites.
- NEW: UISpriteAnimation now has an option to not loop the animation anymore, and can tell you how many frames it has.
- NEW: Added TweenFOV that can be used to tween camera's field of view.
- NEW: Added a UISoundVolume script that can change the volume of the sounds used by NGUITools.PlaySound when attached to a slider.
- FIX: UIInput will now bring up a proper password keyboard on touch-based devices.
- FIX: UIImageButton will now set the correct sprite when it's enabled while highlighted.
- FIX: DragDropItem example script will now work on touch-based devices.
- FIX: UIButtonPlayAnimation will now clear the event receiver if none was specified.
- FIX: Various changes to UICamera, making it more touch-device friendly.
- FIX: UIPanels marked as static will now update their geometry when new widgets get added.
- FIX: Shaders no longer use "fixed" data type as it seems to have issues on certain devices.
- DEL: Removed old deprecated functions in order to clean up the code.

2.0.9:
- NEW: UITable can now return its list of children (in sorted order) via UITable.children.
- FIX: UISpriteAnimation can now be paused with FPS of 0.
- FIX: UITweener's delay should now work properly.
- FIX: UIPanel should now create draw calls with "dont destroy on load" flag instead of hideflags at run time, resolving a rare warning.
- FIX: Tweaks to how multi-touches are handled when they're disabled.
- FIX: Removed the "#pragma fragmentoption ARB_precision_hint_fastest" which was causing issues due to no support on android, mac mini's and possibly other devices.
- FIX: UIInput carat should be removed upon leaving the field on iOS.
- FIX: UIInput default text should be removed OnSelect on iOS.
- FIX: Inventory example should no longer have its own menu, but will instead be under NGUI.

2.0.8:
- NEW: Packed fonts now have clipped version of shaders, making them work with clipped panels.
- NEW: You can now specify the maximum number of lines on UILabel instead of just multiline / single line option.
- NEW: UIButton's disabled color can now be specified explicitly.
- NEW: Tweens and animations now have OnDoubleClick and OnSelect events to work with as well.
- NEW: It's now possible to control the volume used by all UI sounds: NGUITools.soundVolume.
- NEW: You can now delay a tween by specifying a start time delay.
- NEW: You can now disable multi-touch on UICamera, making all touches be treated as one.
- NEW: MakePixelPerfect is now in NGUITools, not NGUIMenu.
- FIX: UIImageButton won't switch images anymore if the script is disabled.
- FIX: Starting value in Localization will no longer overwrite the explicitly switched languages.

2.0.7:
- NEW: You can now specify what keyboard type will be used on mobile devices.
- NEW: You can now add input validation to your inputs to exclude certain characters (such as make your input numeric-only).
- FIX: Packed fonts no longer tie up the alpha channel, and can now be affected by alpha just fine.
- FIX: Clipped panels will no longer cause the unused material message in the console.
- FIX: 3D UIs should now be created with a proper anchor offset.
- FIX: UISliderColors will now work for more than 3 colors.
- FIX: UIPanel will no longer cause a null exception at run time.

2.0.6:
- NEW: Added support for fonts packed into separate RGBA channels (read: eastern language fonts can now be 75% smaller).
- NEW: UITooltip is now a part of NGUI's core rather than being in examples, allowing you to use it freely.
- NEW: Submit and cancel keys can now be specified on the UICamera (before they were hardcoded to Return and Escape).
- FIX: Unity should no longer crash when a second widget is added to the same game object.
- FIX: UIDrawCall no longer updates the index buffer unless it needs to, resulting in increased performance.
- FIX: UIDrawCall now uses double-buffering, so iOS performance should increase.
- FIX: You can now specify whether symbols are affected by color or not (or if they're processed for that matter).
- FIX: Fixed an issue with highlighting not returning to highlighted state after press.

2.0.5:
- NEW: Added support for custom-defined symbols (emoticons and such) in fonts.
- NEW: Added NGUI menu -> Make Pixel Perfect (Alt+Shift+P), and NGUI Menu -> Add Collider is now Alt+Shift+C.
- NEW: Added OnActivate condition to tweens and active animations.
- NEW: It's now possible to have a UITable position items upwards instead of downwards.
- NEW: It's now possible to have a "sticky" tooltip specified on UICamera, making it easier for tooltips to show up.
- NEW: UIInput will now send out OnInputChanged notifications when typing.
- NEW: Added TweenVolume script you can use to tween AudioSource's volume.
- FIX: Fixed what was causing the "Cleaning up leaked objects in scene" message to show up.

2.0.4:
- NEW: Added UIButton -- same as UIButtonColor, but has a disabled state.
- NEW: Added the OnDoubleClick event. Same as OnClick, just sent on double-click.
- FIX: UIDraggablePanel should now have noticeably better performance with many widgets.
- FIX: All private serializable properties will now be hidden from the inspector.
- FIX: UITooltip is now more robust and automatically uses background border size for padding.
- FIX: UILabel inspector now uses a word-wrapped textbox.
- FIX: UIButtonPlayAnimation and UIButtonTween now have an event receiver (on finished).
- FIX: UIGrid no longer modifies Z of its items on reposition.
- FIX: Only one Localization class is now allowed to be present.
- FIX: UILabel should now have a bit better performance in the editor.
- FIX: UISprite's MakePixelPerfect setting now takes padding into account properly.

2.0.3:
- NEW: UIButtonSound now allows you to specify pitch in addition to volume.
- FIX: UIDraggablePanel will now update the scroll bars on start.
- FIX: UITweenScale will now start with a scale of one instead of zero by default.
- FIX: UIInput will now ignore all characters lower than space, fixing an issue with mac OS input.
- FIX: UITexture will no longer lose its material whenever something changes.
- FIX: Reworked the way the mouse is handled in UICamera, fixing a couple of highlighting issues.

2.0.2:
- FIX: UIButton series of scripts will now correctly disable and re-enable their selected state when the game object is enabled / disabled.
- FIX: SpringPanel will now notify the Draggable Panel script on movement, letting it update scroll bars correctly.
- FIX: UIDraggablePanel will now lose its momentum every frame rather than only when it's being dragged.
- FIX: UIDraggablePanel will no longer reset the panel's position on start.
- FIX: UIDraggablePanel.ResetPosition() now functions correctly.
- FIX: UIDraggablePanel.UpdateScrollbars() will now only adjust the position if the scroll bars aren't being updated (ie: called from a scroll bar).
- FIX: 3D UIs will now be created with a proper anchor offset.

2.0.1:
- NEW: UIDraggablePanel will now display the bounds of the draggable widgets as an orange outline in the Scene View.
- NEW: Added a 'repositionNow' toggle to UIDraggablePanel that will reset the clipping area using the children widget's current bounds.
- NEW: It's now possible to specify horizontal and vertical axis names for UICamera.
- FIX: UICamera will no longer process WASD or Space key events if an Input Field is currently selected.
- FIX: UIDraggablePanel's 'startingDragAmount' was renamed to 'startingRelativePosition', for clarity.
- FIX: UIToggle will now set the checkmark state immediately on start instead of gradually.
- FIX: UISlider will now always force-set its value value on start.
- FIX: UIInput.text will now always return its own text rather than that of the label (works better with captions).
- FIX: Setting UIInput.text now sets the color of the label to the active color.

2.0.0:
- NEW: Redesigned the way UIDragCamera and UIDragPanelContents work, making them much more straightforward.
- NEW: New widget has been added: Scroll Bar. It does exactly what you think it does.
- NEW: UIDraggableCamera script is used on the camera to make it draggable via UIDragCamera.
- NEW: UIDraggablePanel script is used on the panel to make it draggable via UIDragPanelContents.
- NEW: UIDraggablePanel natively supports scroll bars with "always show", "fade out if not needed" and "fade in only when dragging" behaviors.
- NEW: Scroll View (DragPanel) and Quest Log examples have been updated with scroll bars.
- NEW: Reorganized all examples to be in a more logical order -- starting with the basic, common functionality and going up from there.
- NEW: Localization will now try to automatically load the language file via Resources.Load if it wasn't found in the local list.
- NEW: Atlas Maker tool now allows you to turn off trimming of transparent pixels before importing certain sprites.
- NEW: Atlas Maker tool now allows you to specify how much padding is applied in-between of sprites.
- FIX: EditorPrefs are now used instead of PlayerPrefs to store editor-related data.
- FIX: Popup list will no longer try to call SendMessage in edit mode.
- FIX: UIEventListener.Add is now UIEventListener.Get, making the function make more sense with the -= operator.
- DEL: Scroll View example that was using UIDragObject has been removed as it's now obsolete.

1.92:
- NEW: Expanded the Filled Sprite to support radial-based filling. Great for progress indicators, cooldown timers, circular health bars, etc.
- FIX: Eliminated all runtime uses of 'foreach', seeing as it causes memory leaks on iOS.

1.91:
- NEW: Added a new example scene showing how to easily implement drag & drop from 2D UI to the 3D world.
- FIX: UICamera was sending multiple OnDrag events for the mouse. This has now been fixed.
- FIX: UIAnchor changes in 1.90 had a few adverse effects on two of the examples.

1.90:
- NEW: You can now specify an option on the UIDragPanelContents that will prevent dragging if the contents already fit.
- NEW: You can now specify a radio button group root on the toggle instead of always having it be the parent object.
- NEW: You can now easily adjust the widget's alpha by using the new UIWidget.alpha property.
- NEW: UIAnchor script has been redesigned, and the 'stretch to fill' property has been removed. You can now position using relative coordinates.
- NEW: UIStretch script has been added, allowing you to stretch an object in either (or both) directions using relative coordinates.
- NEW: You can now specify a maximum range distance for UICamera's raycasts, allowing you to limit the interaction distance (for first-person cameras).
- FIX: Popup list inspector now shows the "Position" drop-down.
- FIX: Slider now updates correctly when it's first created, and when you change the Full Size property.
- FIX: UIDragCamera now takes the camera's size into consideration.
- FIX: DestroyImmediate calls have been replaced with NGUITools.DestroyImmediate as there seem to be odd issues on certain Android devices.

1.88:
- NEW: Added an option to the tweener to use steeper pow(2) curves for ease in/out tweens.
- NEW: You can now specify the movement threshold that will be used to determine whether button presses are eligible for clicks on UICamera.
- NEW: You can now specify an input field to be password-based, and it will only hide the text once you start typing.
- FIX: UIButtonTween can now disable objects properly after a toggle.
- FIX: UISavedOption can now save the state of a single toggle in addition to a group of togglees.
- FIX: Localization now handles duplicate key entries silently.
- FIX: Widgets not using a texture will now have gizmos.
- FIX: Fix for the OnClick event on touch-based devices.

1.87:
- NEW: UISlider now has an inspector class, and 'rawValue' can no longer be modified (use 'sliderValue'!)
- FIX: An assortment of tweaks and fixes, focusing on stability and ease of use.
- FIX: Reworked the way the UIPopupList was calculating its padding, making it more robust.
- FIX: Disabled widgets will get updated correctly when the atlas gets replaced.
- FIX: Disabling the button on click should no longer make it get stuck in the "clicked" state.
- FIX: UICamera.lastTouchPosition is back.

1.86:
- NEW: UIAtlas now has a "pixel size" property that affects MakePixelPerfect logic as well as sliced sprite's border size.
- FIX: UISprite will now always ensure it has a sprite to work with, if at all possible.
- FIX: UIDragPanelContents should now work correctly on mobile devices.

1.85:
- NEW: Added Example 12: Better Scroll View.
- NEW: Added a script that can be used to efficiently drag the contents of the panel: UIDragPanelContents.
- NEW: Added a function replacement for SetActiveRecursively (NGUITools.SetActive), since the former has rare odd issues.

1.84:
- FIX: Changed the way the font data is stored, resulting in potentially better loading performance on mobile devices.
- FIX: UIPanel.Start() should now find cameras faster.
- FIX: UIPanel will no longer use the clipping softness value unless soft clipping is actually used.
- FIX: The way click / drag was handled has been changed a bit. It should now be easier to click buttons on retina screens.
- FIX: Rebuilding an atlas was not updating fonts correctly.
- FIX: Couple of tweaks to UIAtlas and UIFont's replacement feature.

1.83:
- NEW: Added a simple script that can save the state of the toggle (or a group of togglees) to player prefs.
- FIX: A variety of minor tweaks.

1.82:
- NEW: It's now possible to specify a "replacement" value on UIAtlas and UIFonts, making swapping of atlases and fonts a trivial matter.
- NEW: UIToggle now has an option to allow unchecking the last item within a group.
- FIX: Most cases of FindObjectsOfTypeAll has been replaced with FindSceneObjectsOfType instead.
- FIX: UISliderColors now keeps the slider's alpha.
- FIX: Edit-time modification of UISlider's 'rawValue' in the inspector will now again visibly move the slider.
- FIX: UIWidget will no longer consider its geometry as changed every frame if there is nothing to draw (empty text labels).
- FIX: Atlas Maker will now create a new font if the name of the font doesn't match.
- OLD: NGUITools.ReplaceAtlas and Font functions have been deprecated.

1.81:
- NEW: UIInput can now be multi-line.
- FIX: UILabel will now center-align properly again when a fixed line width was specified.
- FIX: UILabel's effect (shadow, outline) will now be affected by the label's alpha.
- FIX: UILabel's effect will now always be offset consistently, even if the scale changes.
- FIX: Changing the widget's pivot will no longer cause it to become it pixel-perfect.
- FIX: UISlider no longer requires a box collider.
- FIX: Creating sliders via the wizard will now set their full size property.

1.80:
- NEW: You can now add a colored shadow/bevel or an outline effect to your labels via a simple toggle.
- NEW: UICamera now has support for keyboard, joystick and controller input.
- NEW: UICamera can now control what kind of events it will process (only touch, only keyboard, etc).
- NEW: UISlider can now be adjusted via keyboard, joystick and controller input.
- NEW: UIPopupList can now be interacted with using a keyboard or controller input.
- NEW: Added a new script, UIButtonKeys that can be used to set up the UI for keyboard, joystick and controller input.
- NEW: New Example 11 shows how to set up the UI to work with the new input types.

1.70:
- NEW: Right click stuff has been replaced by just 'lastTouchID' with added support for the middle mouse button.
- NEW: UIDragCamera now has scrolling wheel support just like UIDragObject.
- FIX: UTF8 encoding is not supported in Flash. Wrote my own binary parsing function to make Flash work.
- FIX: UILabel will now align to right and center properly when not pixel-perfect.
- FIX: UIFont.WrapText will now trim away space characters when word wrapping.
- FIX: UIFont.Print will no longer draw spaces (padding will still be applied).
- FIX: UIPopupList will highlight the correct item even when localized.
- FIX: UITable will now handle disabled children properly.
- FIX: Fixed a crash on Unity 3.5.0 (sigh!).
- FIX: Tweaked how pixel-perfect calculations work for labels.

1.69:
- NEW: Added right-click support by simply adding an optional integer parameter to the OnClick event.
- NEW: The contents of the UIPopupList can now be localized by enabling a toggle on it.
- NEW: You can now give the UIEventListener an optional parameter that you can retrieve later.

1.68:
- NEW: Added a built-in Localization System.
- NEW: Added a new example (10) - Localization.
- FIX: Widgets can now be modified directly on prefabs.
- FIX: Fixed the window stuttering in example 9 (when dragging it).
- FIX: Widgets will now ensure they're under the right panel after drag & drop in the editor.
- FIX: It's now possible to visibly modify the value of the slider at edit mode.
- FIX: Scaling labels now properly rebuilds them.
- FIX: Scaling labels will no longer affect the widget bounds in odd ways.

1.67:
- FIX: Font Maker's Replace button will now re-import the data file.
- FIX: Fixed all known issues with Undo functionality.
- FIX: Fixed all known issues with prefabs (mainly 3.5.0-related)
- FIX: Fixed clipping in Flash by adding a work-around for a bug in Flash export.
- FIX: Removed 3.5b6 work-arounds for Flash as the bug has since been fixed.

1.66:
- NEW: Added a new script: ShaderQuality. It's used to automatically set shader level of detail as the quality level goes down.
- FIX: All examples have been updated to run properly in Flash.
- FIX: NGUI now has no warnings using Unity 3.5.0.

1.65:
- NEW: Example 9: Quest Log shows how to make a fancy quest log.
- NEW: Added a new feature to UIPanel -- the ability to write to depth before any geometry is drawn. This doubles the draw calls but saves fillrate.
- NEW: Clicking on the items in the panel and camera tools will now select them instead of enable/disable them.
- NEW: UITable can now automatically keep its contents within the parent panel's bounds.
- NEW: New event type: OnScroll(float delta).
- FIX: FindInChildren was not named properly. It's now FindInParents.
- FIX: Eliminated most warnings on Unity 3.5.

1.64:
- NEW: Atlas inspector window now shows "Dimensions" and "Border" instead of "Outer" and "Inner" rects.
- NEW: UIPanel now has an optional property: "showInPanelTool" that determines whether the panel will show up in the Panel Tool.
- FIX: Trimmed sprite-using fonts will now correctly trim the glyphs.
- FIX: The "inner rect" outline now uses a checker texture, making it visible regardless of sprite's color.
- FIX: Selected sprite within the UIAtlas is now persistent.
- FIX: Panel and Camera tools have been improved with additional functionality.

1.63:
- NEW: Added a logo to all examples with some additional shiny functionality (contributed by Hjupter Cerrud).
- NEW: Label template in the Widget Tool now has a default color that will be applied to newly created labels.
- NEW: Added an option to TweenScale to automatically notify the UITable of the change.
- FIX: Updating a texture atlas saved as a non-PNG image will now update the texture correctly.
- FIX: Updating an atlas with a font sprite in it will now correctly mark all fonts using it as dirty.
- FIX: Fixed all remaining known issues with the Atlas Maker.
- FIX: Tiled Sprite will now use an inner rect rather than outer rect, letting you add some padding.
- FIX: UIButtonTween components will now set their target in Awake() rather than Start(), fixing a rare order-of-execution issue.
- FIX: UITable will now consider the item's own local scale when calculating bounds.
- DEL: "Deprecated" folder has been deleted.

1.62:
- NEW: Added a new class -- UITable -- that can be used to organize its children into rows/columns of variable size (think HTML table).
- FIX: Font Maker will make it more obvious when you are going to overwrite a font.
- FIX: Tweener will now set its timestamp on Start(), making tweens that start playing on Play behave correctly.
- FIX: UISlicedSprite will now notice that its scale is changing and will rebuild its geometry properly.
- FIX: Atlas and Font maker will now create new atlases and fonts in the same folder as the selected items.

1.61:
- NEW: UIToggle.current will hold the toggle that triggered the 'functionName' function on the 'eventReceiver'.
- FIX: UIPopupList will now place the created object onto a proper layer.

1.60:
- NEW: Added a built-in atlas-making solution: Atlas Maker, making it possible to create atlases without leaving Unity.
- NEW: Added a tool that makes creation of fonts easier: Font Maker. Works well with the Atlas Maker.
- FIX: UIAtlasInspector will now always force the atlas texture to be of proper size whenever the material or texture packer import gets triggered.
- FIX: Removed the work-around for Flash that disabled sound, seeing the bug has been since fixed.
- FIX: Tweener has been renamed to NTweener to avoid name conflicts with HOTween.
- FIX: An assortment of minor usability tweaks.

1.50:
- NEW: The UI is now timeScale-independent, letting you pause the game via Time.timeScale = 0.
- NEW: Added an UpdateManager class that can be used to programmatically control the order of script updates.
- NEW: NGUITools.PlaySound() now returns an AudioSource, letting you change the pitch.
- FIX: UIAtlas and UIFont now work with Textures instead of Texture2Ds, letting you use render textures.
- FIX: Typewriter effect script will now pre-wrap text before printing it.
- FIX: NGUIEditorTools.SelectedRoot() no longer considers prefabs to be valid.
- FIX: TexturePacker import will automatically strip out the ".png" extension from script names.
- FIX: Tested and working with the Flash export as of 3.5.0 f3.

1.49:
- NEW: UIWidgets now work with Textures rather than Texture2D, making it possible to use render textures if desired.
- FIX: Rewrote the UIFont's WrapText function. It now supports wrapping of long lines properly.
- FIX: Input fields are now multi-line, and will now show the last line when typing past the label's width.
- FIX: Input fields will now update less frequently when IME or iOS/Android keyboard is used.

1.48:
- NEW: Added a new container class -- BetterList<>. It replaced the generic List<> in many cases, eliminating GC spikes.
- FIX: Various performance-related optimizations.
- FIX: UITextList will now handle resized text labels correctly.
- FIX: Parenting and reparenting widgets will now cause their panel to get updated correctly.
- FIX: Eliminated one potential cause of widgets trying to update before being parented.

1.47:
- NEW: Added a new example (8) showing how to create a simple menu system.
- NEW: Added an example script that adds a typewriter effect to labels.
- NEW: Added a 'text scale' property to the UIPopupList.
- FIX: UIPopupList will now choose a more appropriate depth rather than just a high number.
- FIX: UIPopupList labels' colliders will now be properly positioned on the Z.
- FIX: Fix for UISpriteAnimationInspector not handling null strings.
- FIX: Several minor fixes for rare issues (such as playing a sound with no audio listener or main camera in the scene).

1.46:
- NEW: Added a new class (UIEventListener) that can be used to easily register event listener delegates via code without the need to create MonoBehaviours.
- NEW: Added a UIPopupList class that can be used to create drop-down lists and menus.
- NEW: Added the Popup List and Popup Menu templates to the Widget Wizard.
- NEW: UISlider can now move in increments by specifying the desired Number of Steps.
- NEW: Tutorial 11 showing how to use UIPopupLists.

1.45:
- NEW: Text labels will center or right-align their text if such pivot was used.
- NEW: Added an inspector class for the UIImageButton.
- NEW: UIGrid now has the ability to skip deactivated game objects.
- NEW: Font sprite is now imported when the font's data is imported, and will now be automatically selected from the atlas on import.
- FIX: Making widgets pixel-perfect will now make them look crisp even if their dimensions are not even (ex: 17x17 instead of 18x18).
- FIX: Component Selector will now only show actual prefabs as recommended selections. Prefab instances aren't.
- FIX: BMFontReader was not parsing lines quite right...

1.44:
- NEW: UIGrid can now automatically sort its children by name before positioning them.
- NEW: Added momentum and drag to UIDragCamera.
- NEW: Added the Image Button template to the Widget Tool.
- FIX: SpringPosition will now disable itself properly.
- FIX: UIImageButton will now make itself pixel-perfect after switching the sprites.
- FIX: UICamera will now always set the 'lastCamera' to be the camera that received the pressed event while the touch is held.
- FIX: UIDragObject will now drag tilted objects (windows) with a more expected result.

1.43:
- NEW: Added the Input template to the Widget Tool.
- NEW: UIButtonMessage will now pass the button's game object as an optional parameter.
- NEW: Tweener will now pass itself as a parameter to the callWhenFinished function.
- NEW: Tweener now has an 'eventReceiver' parameter you can set for the 'callWhenFinished' function.
- FIX: Tweener will no longer disable itself if one of OnFinished SendMessage callbacks reset it.
- FIX: Updated all tutorials to use 1.42+ functionality.
- FIX: Slider will now correctly mark its foreground widget as having changed on value change.

1.42:
- NEW: Added a new tool: Widget Creation Wizard. It replaces all "Add" functions in NGUI menu.
- NEW: Added new templates to the Widget Wizard: Button, Toggle, Progress Bar, Slider.
- NEW: When adding widgets via the wizard, widget depth is now chosen automatically so that each new widget appears on top.
- NEW: AddWidget<> functionality is now exposed to runtime scripts (found in NGUITools).
- FIX: Widget colliders of widgets layed on top of each other are now offset by wiget's depth.
- FIX: Several minor bug fixes.

1.41:
- NEW: Added a new tool: Camera Tool. It can be used to get a bird's eye view of your cameras and determine what draws the selected object.
- NEW: Added a new tool: Create New UI. You can use it to create an entire UI hierarchy for 2D or 3D layouts with one click of a button.
- NEW: Added a new script: UIRoot. It can be used to scale the root of your UI by 2/ScreenHeight (the opposite of UIOrthoCamera).
- NEW: The NGUI menu has been enhanced. When adding widgets, it will intelligently determine where to add them best.
- NEW: Sliced sprites now have an option to not draw the center, in case you only want the border.
- FIX: Scaling sliced sprites and tiled sprites will now correctly update them again.
- FIX: Changing the depth of the widgets will now correctly update them again.
- FIX: The unnecessary color parameter specified on the material has been removed from the shaders.

1.40:
- NEW: Major performance improvements. The way the geometry was being created has been completely redone.
- NEW: With the new system, moving, rotating and scaling panels no longer causes widgets they're responsible for to be rebuilt.
- NEW: Panel clipping will now actually clip widgets, eliminating them from the draw buffers until they move back into view.
- NEW: Matrix parameter has been eliminated from the clip shaders as it's no longer needed with the new system.
- FIX: Work-around for a rare obscure issue caused by a bug in Unity related to instantiating widgets from prefabs (Case 439372).
- FIX: It's no longer possible to edit widgets directly on prefabs. Bring them into the scene first.
- FIX: Panel tool will now update itself on object selection.

1.33:
- NEW: UIToggle now has a configurable function to call.
- NEW: UIToggle now has an animation parameter it can trigger when checked/unchecked.
- NEW: You can now play remote animations via UIButtonPlayAnimations.
- NEW: Tweener now sends out notifications when it finishes.
- NEW: Tweener now has a 'group' parameter that can be used to only enable/disable only certain tweens on the same object.
- NEW: UIButtonTween has been changed to use more descriptive properties. Check your UIButtonTweens and update their properties after upgrading.
- NEW: Examples 1, 5 and 6 have been adjusted to use the new features.
- FIX: Scrolling speed should now be consistent even at low framerates.
- FIX: Shader fixes.

1.32:
- NEW: Added a 'thumb' parameter to the UISlider.
- NEW: Added UIForwardEvents script that can be used to forward events from one object to another.
- NEW: Added the ability to enable and disable target game objects via UIButtonTween.
- FIX: Input fields now support IME.

1.31:
- NEW: Added a panel tool (NGUI menu -> Panel Tool) to aid with multi-panel development.
- FIX: Variety of tweaks and minor enhancements, mainly related to examples.
- FIX: UIDragObject had a rare bug with how items were springing back into place.

1.30:
- NEW: UIPanels can now specify a clipping area (everything outside this area will not be visible).
- NEW: UIFilledSprite, best used for progress bars, sliders, etc (thanks nsxdavid).
- NEW: UISpriteAnimation for some simple sprite animation (attach to a sprite).
- NEW: UIAnchor can now specify depth offset to be used with perspective cameras.
- NEW: UIDragObject can now restrict dragging of objects to be within the panel's clipping bounds.
- NEW: UIToggle now has an "option" field that lets you create option button groups (Tutorial 11).
- NEW: Example 7 showing how to use the clipping feature.
- NEW: Example 0 (Anchor) has been redone.
- NEW: Most tutorials and examples now explain what they do inside them.
- NEW: Added a variety of new interaction scripts replacing State scripts (UIButton series for example).
- NEW: Added a Drag Effect parameter to UIDragObject with options to add momentum and spring effects.
- FIX: UICamera.lastCamera was not pointing to the correct camera with multi-camera setups (thanks LKIM).
- FIX: UIAnchor now positions objects in the center of the ortho camera rather than at depth of 0.
- FIX: Various usability improvements.
- OLD: 'State' series of scripts have all been deprecated.

1.28:
- NEW: Added a simple tweener and a set of tweening scripts (position, rotation, scale, and color).
- FIX: Several fixes for rare non-critical issues.
- FIX: Flash export bug work-arounds.

1.27:
- FIX: UISlider should now work properly when centered again.
- FIX: UI should now work in Flash after LoadLevel (added some work-arounds for current bugs in the flash export).
- FIX: Sliced sprites should now look properly in Flash again (added another work-around for another bug in the Flash export).

1.26:
- NEW: Added support for trimmed sprites (and fonts) exported via TexturePacker.
- NEW: UISlider now has horizontal and vertical styles.
- NEW: Selected widgets now have their gizmos colored green.
- FIX: UISlider now uses the collider's bounds instead of the target's bounds.
- FIX: Sliced sprite will now behave better when scaled with all pivot points, not just top-left.

1.25:
- NEW: Added a UIGrid script that can be used to easily arrange icons into a grid.
- NEW: UIFont can now specify a UIAtlas/sprite combo instead of explicitly defining the material and pixel rect.

1.24
- NEW: Added character and line spacing parameters to UIFont.
- NEW: Sprites will now be sorted alphabetically, both on import and in the drop-down menu.
- NEW: NGUI menu Add* functions now automatically choose last used font and UI atlases and resize the new elements.
- FIX: UICamera will now be able to handle both mouse and touch-based input on non-mobile devices.
- FIX: 'Add Collider' menu option got semi-broken in 1.23.
- FIX: Changing the font will now correctly update the visible text while in the editor.
- Work-around for a bug in 3.5b6 Flash export.

1.23
- NEW: Added a pivot property to the widget class, replacing the old 'centered' flag.

1.22
- NEW: Example 6: Draggable Window
- FIX: UISlider will now function properly for arbitrarily scaled objects.

1.21
- FIX: Gizmos are now rotated properly, matching the widget's rotation.
- FIX: Labels now have gizmos properly scaled to envelop their entire content.

1.20
- NEW: Added the ability to generate normals and tangents for all widgets via a flag on UIPanel.
- NEW: Added a UITexture class that can be used to draw a texture without having to create an atlas.
- NEW: Example 5: Lights and Refraction.
- Moved all atlases into the Examples folder.

1.12
- FIX: Unicode fonts should now get imported correctly.

1.11
- NEW: Added a new example (4) - Chat Window.

1.10
- NEW: Added support for Unity 3.5 and its "export to Flash" feature.

1.09
- NEW: Added password fields (specified on the label)
- FIX: Working directly with atlas and font prefabs will now save their data correctly
- NEW: Showing gizmos is now an option specified on the panel
- NEW: Sprite inner rects will now be preserved on re-import
- FIX: Disabled widgets should no longer remain visible in play mode
- NEW: UICamera.lastHit will always contain the last RaycastHit made prior to sending OnClick, OnHover, and other events.

1.08
- NEW: Added support for multi-touch
