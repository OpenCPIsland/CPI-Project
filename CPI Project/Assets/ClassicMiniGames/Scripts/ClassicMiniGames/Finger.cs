using UnityEngine;

public class Finger
{
	public int fingerIndex;

	public int touchCount;

	public Vector2 startPosition;

	public Vector2 complexStartPosition;

	public Vector2 position;

	public Vector2 deltaPosition;

	public Vector2 oldPosition;

	public int tapCount;

	public float deltaTime;

	public TouchPhase phase;

	public EasyTouch.GestureType gesture;

	public GameObject pickedObject;

	public Camera pickedCamera;

	public bool isGuiCamera;
}
