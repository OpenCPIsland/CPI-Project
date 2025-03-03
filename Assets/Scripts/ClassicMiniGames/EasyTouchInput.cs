using UnityEngine;

public class EasyTouchInput
{
	private Vector2[] oldMousePosition = new Vector2[2];

	private int[] tapCount = new int[2];

	private float[] startActionTime = new float[2];

	private float[] deltaTime = new float[2];

	private float[] tapeTime = new float[2];

	private bool bComplex = false;

	private Vector2 deltaFingerPosition;

	private Vector2 oldFinger2Position;

	private Vector2 complexCenter;

	public int TouchCount()
	{
		return getTouchCount(false);
	}

	private int getTouchCount(bool realTouch)
	{
		int result = 0;
		if (realTouch || EasyTouch.instance.enableRemote)
		{
			result = Input.touchCount;
		}
		else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
		{
			result = 1;
			if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(EasyTouch.instance.twistKey) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(EasyTouch.instance.swipeKey))
			{
				result = 2;
			}
			if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(EasyTouch.instance.twistKey) || Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(EasyTouch.instance.swipeKey))
			{
				result = 2;
			}
		}
		return result;
	}

	public Finger GetMouseTouch(int fingerIndex, Finger myFinger)
	{
		Finger finger;
		if (myFinger != null)
		{
			finger = myFinger;
		}
		else
		{
			finger = new Finger();
			finger.gesture = EasyTouch.GestureType.None;
		}
		if (fingerIndex == 1 && (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(EasyTouch.instance.twistKey) || Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(EasyTouch.instance.swipeKey)))
		{
			finger.fingerIndex = fingerIndex;
			finger.position = oldFinger2Position;
			finger.deltaPosition = finger.position - oldFinger2Position;
			finger.tapCount = tapCount[fingerIndex];
			finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerIndex];
			finger.phase = TouchPhase.Ended;
			return finger;
		}
		if (Input.GetMouseButton(0))
		{
			finger.fingerIndex = fingerIndex;
			finger.position = GetPointerPosition(fingerIndex);
			if ((double)(Time.realtimeSinceStartup - tapeTime[fingerIndex]) > 0.5)
			{
				tapCount[fingerIndex] = 0;
			}
			if (Input.GetMouseButtonDown(0) || (fingerIndex == 1 && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(EasyTouch.instance.twistKey) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(EasyTouch.instance.swipeKey))))
			{
				finger.position = GetPointerPosition(fingerIndex);
				finger.deltaPosition = Vector2.zero;
				tapCount[fingerIndex]++;
				finger.tapCount = tapCount[fingerIndex];
				startActionTime[fingerIndex] = Time.realtimeSinceStartup;
				deltaTime[fingerIndex] = startActionTime[fingerIndex];
				finger.deltaTime = 0f;
				finger.phase = TouchPhase.Began;
				if (fingerIndex == 1)
				{
					oldFinger2Position = finger.position;
				}
				else
				{
					oldMousePosition[fingerIndex] = finger.position;
				}
				if (tapCount[fingerIndex] == 1)
				{
					tapeTime[fingerIndex] = Time.realtimeSinceStartup;
				}
				return finger;
			}
			finger.deltaPosition = finger.position - oldMousePosition[fingerIndex];
			finger.tapCount = tapCount[fingerIndex];
			finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerIndex];
			if (finger.deltaPosition.sqrMagnitude < 1f)
			{
				finger.phase = TouchPhase.Stationary;
			}
			else
			{
				finger.phase = TouchPhase.Moved;
			}
			oldMousePosition[fingerIndex] = finger.position;
			deltaTime[fingerIndex] = Time.realtimeSinceStartup;
			return finger;
		}
		if (Input.GetMouseButtonUp(0))
		{
			finger.fingerIndex = fingerIndex;
			finger.position = GetPointerPosition(fingerIndex);
			finger.deltaPosition = finger.position - oldMousePosition[fingerIndex];
			finger.tapCount = tapCount[fingerIndex];
			finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerIndex];
			finger.phase = TouchPhase.Ended;
			oldMousePosition[fingerIndex] = finger.position;
			return finger;
		}
		return null;
	}

	public Vector2 GetSecondFingerPosition()
	{
		Vector2 result = new Vector2(-1f, -1f);
		if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(EasyTouch.instance.twistKey)) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(EasyTouch.instance.swipeKey)))
		{
			if (!bComplex)
			{
				bComplex = true;
				deltaFingerPosition = (Vector2)Input.mousePosition - oldFinger2Position;
			}
			return GetComplex2finger();
		}
		if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(EasyTouch.instance.twistKey))
		{
			result = GetPinchTwist2Finger();
			bComplex = false;
			return result;
		}
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(EasyTouch.instance.swipeKey))
		{
			result = GetComplex2finger();
			bComplex = false;
			return result;
		}
		return result;
	}

	private Vector2 GetPointerPosition(int index)
	{
		if (index == 0)
		{
			return Input.mousePosition;
		}
		return GetSecondFingerPosition();
	}

	private Vector2 GetPinchTwist2Finger()
	{
		Vector2 result = default(Vector2);
		if (complexCenter == Vector2.zero)
		{
			result.x = (float)Screen.width / 2f - (Input.mousePosition.x - (float)Screen.width / 2f);
			result.y = (float)Screen.height / 2f - (Input.mousePosition.y - (float)Screen.height / 2f);
		}
		else
		{
			result.x = complexCenter.x - (Input.mousePosition.x - complexCenter.x);
			result.y = complexCenter.y - (Input.mousePosition.y - complexCenter.y);
		}
		oldFinger2Position = result;
		return result;
	}

	private Vector2 GetComplex2finger()
	{
		Vector2 result = default(Vector2);
		result.x = Input.mousePosition.x - deltaFingerPosition.x;
		result.y = Input.mousePosition.y - deltaFingerPosition.y;
		complexCenter = new Vector2((Input.mousePosition.x + result.x) / 2f, (Input.mousePosition.y + result.y) / 2f);
		oldFinger2Position = result;
		return result;
	}
}
