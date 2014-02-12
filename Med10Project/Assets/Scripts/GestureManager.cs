using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//TODO: 
//Calculate distance. If distance bigger or lesser than max and min register swipe.

public class GestureManager : MonoBehaviour {
	
	#region Editor Publics
	[SerializeField] private float tapThreshold = 0.5f;
	[SerializeField] private float tapPrecision = 10f;
//	[SerializeField] private float pressThreshold = 0.51f;
	[SerializeField] private float swipeThreshold = 10.0f;
	[SerializeField] private float swipeOffset = 5.0f;
	[SerializeField] private float DeltaSwipeThreshold = 10.0f;
	[SerializeField] private float DeltaSwipeOffset = 2.5f;
	[SerializeField] private IsGestureEnabled isGestureEnabled;
	#endregion
	
	#region Privates
	private Dictionary<int,float> touchBeganTimes = new Dictionary<int, float>();
	private Dictionary<int,float> touchEndedTimes = new Dictionary<int, float>();
	private Dictionary<int,Vector2> touchBeganPositions = new Dictionary<int, Vector2>();
	private Dictionary<int,Vector2> touchEndedPositions = new Dictionary<int, Vector2>();
	private Dictionary<int,float> touchTravelDistance = new Dictionary<int, float>();
	private Dictionary<int,float> touchLifetime = new Dictionary<int, float>();
//	private Dictionary<int,GameObject> touchBeganObjects = new Dictionary<int, GameObject>();

	private bool isSwiping = false;
	#endregion
	
	#region Delegates & Events
	public delegate void TapAction(Vector2 screenPosition);
	public event TapAction OnTap;
	
//	public delegate void DoubleTapAction(GameObject go);
//	public event DoubleTapAction OnDoubleTap;
//	
//	public delegate void PressAction(GameObject go);
//	public event PressAction OnPress;
	
	public delegate void SwipeAction();
	public event SwipeAction OnSwipeRight;
	public event SwipeAction OnSwipeLeft;
	public event SwipeAction OnSwipeUp;
	public event SwipeAction OnSwipeDown;
	
//	public delegate void DragAction();
//	public event DragAction OnDrag;
//	
//	public delegate void PinchSpreadAction(float deltaDistance);
//	public event PinchSpreadAction OnPinch;
//	public event PinchSpreadAction OnSpread;
	#endregion

	// Use this for initialization
	void Start () {
		#if UNITY_ANDROID || UNITY_WP8
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.AutoRotation;
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
		//Universal Quit Button
		if(Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
		
		#if UNITY_ANDROID || UNITY_WP8
		foreach(Touch touch in Input.touches)
		{
			switch(touch.phase)
			{
			case TouchPhase.Began:
				//Log begin position
				touchBeganPositions[touch.fingerId] = touch.position;
				//Log begin time
				touchBeganTimes[touch.fingerId] = Time.time;
				break;

			case TouchPhase.Moved:
				if(touch.fingerId == 0)
				{
					if(isGestureEnabled.UseDeltaSwipes)
					{
						if(isGestureEnabled.HorizontalSwipes && isSwiping == false)
							CheckDeltaHorizontalSwipe();
						if(isGestureEnabled.VerticalSwipes && isSwiping == false)
							CheckDeltaVerticalSwipe();
					}
					else
					{
						if(isGestureEnabled.HorizontalSwipes && isSwiping == false)
							CheckHorizontalSwipe();
						if(isGestureEnabled.VerticalSwipes && isSwiping == false)
							CheckVerticalSwipe();
					}
				}
				break;

			case TouchPhase.Stationary:
				break;

			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				//Log End Position
				touchEndedPositions[touch.fingerId] = touch.position;
				//Calculate the distance travelled by this touch
				calcTouchTravelDistance(touch.fingerId);
				//Log the time this touch ended
				touchEndedTimes[touch.fingerId] = Time.time;
				//Calculate the time the touch was alive
				calcTouchLifetime(touch.fingerId);

				if(touch.fingerId == 0)
				{
					if(isGestureEnabled.SingleTap && touchLifetime[touch.fingerId] <= tapThreshold)
					{
						CheckSingleTap();
					}
					isSwiping = false;
				}
				break;

			default:
				Debug.Log("Incorrect touchphase in gesturemanager2");
				break;
			}
		}
		#endif

		#if UNITY_WEBPLAYER || UNITY_EDITOR || UNITY_STANDALONE
		if(Input.GetMouseButtonDown(0))
		{			
			//Single Tap Event
			if(OnTap != null)
				OnTap(new Vector2(Input.mousePosition.x,Input.mousePosition.y));
		}
		
		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			//SwipeRight Event
			if(OnSwipeRight != null)
				OnSwipeRight();
		}
		
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			//SwipeLeft Event
			if(OnSwipeLeft != null)
				OnSwipeLeft();
		}
		
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			//SwipeUp Event
			if(OnSwipeUp != null)
				OnSwipeUp();
		}
		
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			//SwipeDown Event
			if(OnSwipeDown != null)
				OnSwipeDown();
		}
	#endif
	}

	private void calcTouchTravelDistance(int fingerId)
	{
		touchTravelDistance[fingerId] = Vector2.Distance(touchBeganPositions[fingerId], 
		                                                 touchEndedPositions[fingerId]);
	}

	private void calcTouchLifetime(int fingerId)
	{
		touchLifetime[fingerId] = touchEndedTimes[fingerId] - touchBeganTimes[fingerId];
	}

	//Single Tap
	private void CheckSingleTap()
	{
		Touch tempTouch = Input.GetTouch(0);
		var tempDistance = Vector2.Distance(touchBeganPositions[0], 
		                                    tempTouch.position);
		
		if(tempTouch.tapCount  == 1 && tempDistance < tapPrecision && touchLifetime[0] <= tapThreshold)
		{
			//Single Tap Event
			if(OnTap != null)
				OnTap(tempTouch.position);
		}
	}

	
	//Horizontal Swipes
	private void CheckHorizontalSwipe()
	{
		Touch tempTouch = Input.GetTouch(0);
		float tempDistance = Vector2.Distance(touchBeganPositions[tempTouch.fingerId], 
		                                      tempTouch.position);

		if(tempDistance >= swipeThreshold)
		{

			if(tempTouch.position.x >= touchBeganPositions[tempTouch.fingerId].x &&
			   Mathf.Abs(tempTouch.position.y - touchBeganPositions[tempTouch.fingerId].y) <= swipeOffset)
			{
				isSwiping = true;
				//SwipeRight Event
				if(OnSwipeRight != null)
					OnSwipeRight();
			}
			else if(tempTouch.position.x <= touchBeganPositions[tempTouch.fingerId].x &&
			        Mathf.Abs(tempTouch.position.y - touchBeganPositions[tempTouch.fingerId].y) <= swipeOffset)
			{
				isSwiping = true;
				//SwipeLeft Event
				if(OnSwipeLeft != null)
					OnSwipeLeft();
			}
		}
	}
	
	//Vertical swipes
	private void CheckVerticalSwipe()
	{
		Touch tempTouch = Input.GetTouch(0);
		float tempDistance = Vector2.Distance(touchBeganPositions[tempTouch.fingerId], tempTouch.position);
		
		if(tempDistance >= swipeThreshold)
		{

			if(tempTouch.position.y >= touchBeganPositions[tempTouch.fingerId].y &&
			   Mathf.Abs(tempTouch.position.x - touchBeganPositions[tempTouch.fingerId].x) <= swipeOffset)
			{
				isSwiping = true;
				//SwipeUp Event
				if(OnSwipeUp != null)
					OnSwipeUp();
			}
			else if(tempTouch.position.y <= touchBeganPositions[tempTouch.fingerId].y &&
			        Mathf.Abs(tempTouch.position.x - touchBeganPositions[tempTouch.fingerId].x) <= swipeOffset)
			{
				isSwiping = true;
				//SwipeDown Event
				if(OnSwipeDown != null)
					OnSwipeDown();
			}
		}
	}

	//Delta Horizontal Swipes
	private void CheckDeltaHorizontalSwipe()
	{
		Touch tempTouch = Input.GetTouch(0);

		if(tempTouch.deltaPosition.x >= DeltaSwipeThreshold &&
		   Mathf.Abs(tempTouch.deltaPosition.y) <= DeltaSwipeOffset)
		{
			isSwiping = true;
			//SwipeRight Event
			if(OnSwipeRight != null)
				OnSwipeRight();
			
		}
		else if(tempTouch.deltaPosition.x <= DeltaSwipeThreshold * -1.0f &&
		        Mathf.Abs(tempTouch.deltaPosition.y) <= DeltaSwipeOffset)
		{
			isSwiping = true;
			//SwipeLeft Event
			if(OnSwipeLeft != null)
				OnSwipeLeft();
		}
	}

	//Delta Vertical Swipes
	private void CheckDeltaVerticalSwipe()
	{
		Touch tempTouch = Input.GetTouch(0);

		if(tempTouch.deltaPosition.y >= DeltaSwipeThreshold &&
		   Mathf.Abs(tempTouch.deltaPosition.x) <= DeltaSwipeOffset)
		{
			isSwiping = true;
			//SwipeUp Event
            if(OnSwipeUp != null)
                OnSwipeUp();
        }
		else if(tempTouch.deltaPosition.y <= DeltaSwipeThreshold * -1.0f &&
		        Mathf.Abs(tempTouch.deltaPosition.x) <= DeltaSwipeOffset)
		{
			isSwiping = true;
			//SwipeDown Event
            if(OnSwipeDown != null)
                OnSwipeDown();
        }
    }



	#region SubClasses
	[System.Serializable]
	public class IsGestureEnabled
	{
		public bool SingleTap = true;
		[HideInInspector] public bool DoubleTap = false;
		public bool HorizontalSwipes = true;
		public bool VerticalSwipes = true;
		public bool UseDeltaSwipes = true;
		[HideInInspector] public bool PinchAndSpread = false;
		[HideInInspector] public bool Drag = false;
	};
	#endregion
}
