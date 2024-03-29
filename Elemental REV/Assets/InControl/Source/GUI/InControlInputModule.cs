﻿#if UNITY_4_6 || UNITY_5
using UnityEngine;
using UnityEngine.EventSystems;
using InControl;


namespace InControl
{
	[AddComponentMenu( "Event/InControl Input Module" )]
	public class InControlInputModule : StandaloneInputModule
	{
		public enum Button : int
		{
			Action1 = InputControlType.Action1,
			Action2 = InputControlType.Action2,
			Action3 = InputControlType.Action3,
			Action4 = InputControlType.Action4
		}

		public new Button submitButton = Button.Action1;
		public new Button cancelButton = Button.Action2;
		[Range( 0.1f, 0.9f )]
		public float analogMoveThreshold = 0.5f;
		public float moveRepeatFirstDuration = 0.8f;
		public float moveRepeatDelayDuration = 0.1f;
		public bool allowMobileDevice = true;
		public bool allowMouseInput = true;
		public bool focusOnMouseHover = false;

		InputDevice inputDevice;
		Vector3 thisMousePosition;
		Vector3 lastMousePosition;
		Vector2 thisVectorState;
		Vector2 lastVectorState;
		bool thisSubmitState;
		bool lastSubmitState;
		bool thisCancelState;
		bool lastCancelState;
		float nextMoveRepeatTime;
		float lastVectorPressedTime;
		TwoAxisInputControl direction;

		public PlayerAction SubmitAction { get; set; }

		public PlayerAction CancelAction { get; set; }

		public PlayerTwoAxisAction MoveAction { get; set; }


		protected InControlInputModule()
		{
			direction = new TwoAxisInputControl();
			direction.StateThreshold = analogMoveThreshold;
		}


		public override void UpdateModule()
		{
			lastMousePosition = thisMousePosition;
			thisMousePosition = Input.mousePosition;
		}


		public override bool IsModuleSupported()
		{
			#if UNITY_WII || UNITY_PS3 || UNITY_PS4 || UNITY_XBOX360 || UNITY_XBOXONE
			return true;
			#endif

			return allowMobileDevice || Input.mousePresent;
		}


		public override bool ShouldActivateModule()
		{
			if (!(enabled && gameObject.activeInHierarchy))
			{
				return false;
			}

			UpdateInputState();

			var shouldActivate = false;
			shouldActivate |= SubmitWasPressed;
			shouldActivate |= CancelWasPressed;
			shouldActivate |= VectorWasPressed;

			#if !UNITY_IOS || UNITY_EDITOR
			if (allowMouseInput)
			{
				shouldActivate |= MouseHasMoved;
				shouldActivate |= MouseButtonIsPressed;
			}
			#endif

			return shouldActivate;
		}


		public override void ActivateModule()
		{
			base.ActivateModule();

			thisMousePosition = Input.mousePosition;
			lastMousePosition = Input.mousePosition;

			var selectObject = eventSystem.currentSelectedGameObject;

			if (selectObject == null)
			{
				selectObject = eventSystem.firstSelectedGameObject;
			}

			eventSystem.SetSelectedGameObject( selectObject, GetBaseEventData() );
		}


		public override void Process()
		{
			var usedEvent = SendUpdateEventToSelectedObject();

			if (eventSystem.sendNavigationEvents)
			{
				if (!usedEvent)
				{
					usedEvent = SendVectorEventToSelectedObject();
				}

				if (!usedEvent)
				{
					SendButtonEventToSelectedObject();
				}
			}

			#if !UNITY_IOS || UNITY_EDITOR
			if (allowMouseInput)
			{
				ProcessMouseEvent();
			}
			#endif
		}


		bool SendButtonEventToSelectedObject()
		{
			if (eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}

			var eventData = GetBaseEventData();

			if (SubmitWasPressed)
			{
//				ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, new PointerEventData( EventSystem.current ), ExecuteEvents.pointerDownHandler );
				ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.submitHandler );
			}
			else
			if (SubmitWasReleased)
			{
//				ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, new PointerEventData( EventSystem.current ), ExecuteEvents.pointerUpHandler );
			}

			if (CancelWasPressed)
			{
				ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.cancelHandler );
			}

			return eventData.used;
		}


		bool SendVectorEventToSelectedObject()
		{
			if (!VectorWasPressed)
			{
				return false;
			}

			var axisEventData = GetAxisEventData( thisVectorState.x, thisVectorState.y, 0.5f );

			if (axisEventData.moveDir != MoveDirection.None)
			{
				if (eventSystem.currentSelectedGameObject == null)
				{
					eventSystem.SetSelectedGameObject( eventSystem.firstSelectedGameObject, GetBaseEventData() );
				}
				else
				{
					ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler );
				}
				SetVectorRepeatTimer();
			}

			return axisEventData.used;
		}


		protected override void ProcessMove( PointerEventData pointerEvent )
		{
			var lastPointerEnter = pointerEvent.pointerEnter;

			base.ProcessMove( pointerEvent );

			if (focusOnMouseHover && lastPointerEnter != pointerEvent.pointerEnter)
			{
				var selectObject = ExecuteEvents.GetEventHandler<ISelectHandler>( pointerEvent.pointerEnter );
				eventSystem.SetSelectedGameObject( selectObject, pointerEvent );
			}
		}


		void Update()
		{
			direction.Filter( Device.Direction, Time.deltaTime );
		}


		void UpdateInputState()
		{
			lastVectorState = thisVectorState;
			thisVectorState = Vector2.zero;

			TwoAxisInputControl dir = MoveAction ?? direction;

			if (Utility.AbsoluteIsOverThreshold( dir.X, analogMoveThreshold ))
			{
				thisVectorState.x = Mathf.Sign( dir.X );
			}

			if (Utility.AbsoluteIsOverThreshold( dir.Y, analogMoveThreshold ))
			{
				thisVectorState.y = Mathf.Sign( dir.Y );
			}

			if (VectorIsReleased)
			{
				nextMoveRepeatTime = 0.0f;
			}

			if (VectorIsPressed)
			{
				if (lastVectorState == Vector2.zero)
				{
					if (Time.realtimeSinceStartup > lastVectorPressedTime + 0.1f)
					{
						nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatFirstDuration;
					}
					else
					{
						nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatDelayDuration;
					}
				}

				lastVectorPressedTime = Time.realtimeSinceStartup;
			}

			lastSubmitState = thisSubmitState;
			thisSubmitState = SubmitAction == null ? SubmitButton.IsPressed : SubmitAction.IsPressed;

			lastCancelState = thisCancelState;
			thisCancelState = CancelAction == null ? CancelButton.IsPressed : CancelAction.IsPressed;
		}


		InputDevice Device
		{
			set
			{
				inputDevice = value;
			}

			get
			{
				return inputDevice ?? InputManager.ActiveDevice;
			}
		}


		InputControl SubmitButton
		{
			get
			{
				return Device.GetControl( (InputControlType) submitButton );
			}
		}


		InputControl CancelButton
		{
			get
			{
				return Device.GetControl( (InputControlType) cancelButton );
			}
		}


		void SetVectorRepeatTimer()
		{
			nextMoveRepeatTime = Mathf.Max( nextMoveRepeatTime, Time.realtimeSinceStartup + moveRepeatDelayDuration );
		}


		bool VectorIsPressed
		{
			get
			{
				return thisVectorState != Vector2.zero;
			}
		}


		bool VectorIsReleased
		{
			get
			{
				return thisVectorState == Vector2.zero;
			}
		}


		bool VectorHasChanged
		{
			get
			{
				return thisVectorState != lastVectorState;
			}
		}


		bool VectorWasPressed
		{
			get
			{
				if (VectorIsPressed && Time.realtimeSinceStartup > nextMoveRepeatTime)
				{
					return true;
				}

				return VectorIsPressed && lastVectorState == Vector2.zero;
			}
		}


		bool SubmitWasPressed
		{
			get
			{
				return thisSubmitState && thisSubmitState != lastSubmitState;
			}
		}


		bool SubmitWasReleased
		{
			get
			{
				return !thisSubmitState && thisSubmitState != lastSubmitState;
			}
		}


		bool CancelWasPressed
		{
			get
			{
				return thisCancelState && thisCancelState != lastCancelState;
			}
		}


		bool MouseHasMoved
		{
			get
			{
				return (thisMousePosition - lastMousePosition).sqrMagnitude > 0.0f;
			}
		}


		bool MouseButtonIsPressed
		{
			get
			{
				return Input.GetMouseButtonDown( 0 );
			}
		}


		#region Unity 5.0 compatibility.

		// Copied from StandaloneInputModule where these are marked private instead of protected in Unity 5.0
		#if UNITY_5_0

		bool SendUpdateEventToSelectedObject()
		{
			if (eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			ExecuteEvents.Execute<IUpdateSelectedHandler>( eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler );
			return baseEventData.used;
		}


		void ProcessMouseEvent()
		{
			var mousePointerEventData = this.GetMousePointerEventData();
			var pressed = mousePointerEventData.AnyPressesThisFrame();
			var Released = mousePointerEventData.AnyReleasesThisFrame();
			var eventData = mousePointerEventData.GetButtonState( PointerEventData.InputButton.Left ).eventData;
			if (!UseMouse( pressed, Released, eventData.buttonData ))
			{
				return;
			}
			ProcessMousePress( eventData );
			ProcessMove( eventData.buttonData );
			ProcessDrag( eventData.buttonData );
			ProcessMousePress( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Right ).eventData );
			ProcessDrag( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Right ).eventData.buttonData );
			ProcessMousePress( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Middle ).eventData );
			ProcessDrag( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Middle ).eventData.buttonData );
			if (!Mathf.Approximately( eventData.buttonData.scrollDelta.sqrMagnitude, 0 ))
			{
				var eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>( eventData.buttonData.pointerCurrentRaycast.gameObject );
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>( eventHandler, eventData.buttonData, ExecuteEvents.scrollHandler );
			}
		}


		void ProcessMousePress( PointerInputModule.MouseButtonEventData data )
		{
			var buttonData = data.buttonData;
			var gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				DeselectIfSelectionChanged( gameObject, buttonData );
				var gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>( gameObject, buttonData, ExecuteEvents.pointerDownHandler );
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>( gameObject );
				}
				var unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress)
				{
					var num = unscaledTime - buttonData.clickTime;
					if (num < 0.3f)
					{
						buttonData.clickCount++;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>( gameObject );
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>( buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag );
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute<IPointerUpHandler>( buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler );
				var eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>( gameObject );
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>( buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler );
				}
				else
				{
					if (buttonData.pointerDrag != null)
					{
						ExecuteEvents.ExecuteHierarchy<IDropHandler>( gameObject, buttonData, ExecuteEvents.dropHandler );
					}
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.Execute<IEndDragHandler>( buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler );
				}
				buttonData.dragging = false;
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					HandlePointerExitAndEnter( buttonData, null );
					HandlePointerExitAndEnter( buttonData, gameObject );
				}
			}
		}


		static bool UseMouse( bool pressed, bool Released, PointerEventData pointerData )
		{
			return pressed || Released || pointerData.IsPointerMoving() || pointerData.IsScrolling();
		}

		#endif

		#endregion
	}
}
#endif

