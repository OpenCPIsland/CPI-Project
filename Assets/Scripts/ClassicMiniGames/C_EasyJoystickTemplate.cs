using UnityEngine;

public class C_EasyJoystickTemplate : MonoBehaviour
{
	private void OnEnable()
	{
		EasyJoystick.On_JoystickTouchStart += On_JoystickTouchStart;
		EasyJoystick.On_JoystickMoveStart += On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove += On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;
		EasyJoystick.On_JoystickTouchUp += On_JoystickTouchUp;
		EasyJoystick.On_JoystickTap += On_JoystickTap;
		EasyJoystick.On_JoystickDoubleTap += On_JoystickDoubleTap;
	}

	private void OnDisable()
	{
		EasyJoystick.On_JoystickTouchStart -= On_JoystickTouchStart;
		EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove -= On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
		EasyJoystick.On_JoystickTouchUp -= On_JoystickTouchUp;
		EasyJoystick.On_JoystickTap -= On_JoystickTap;
		EasyJoystick.On_JoystickDoubleTap -= On_JoystickDoubleTap;
	}

	private void OnDestroy()
	{
		EasyJoystick.On_JoystickTouchStart -= On_JoystickTouchStart;
		EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove -= On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
		EasyJoystick.On_JoystickTouchUp -= On_JoystickTouchUp;
		EasyJoystick.On_JoystickTap -= On_JoystickTap;
		EasyJoystick.On_JoystickDoubleTap -= On_JoystickDoubleTap;
	}

	private void On_JoystickDoubleTap(MovingJoystick move)
	{
	}

	private void On_JoystickTap(MovingJoystick move)
	{
	}

	private void On_JoystickTouchUp(MovingJoystick move)
	{
	}

	private void On_JoystickMoveEnd(MovingJoystick move)
	{
	}

	private void On_JoystickMove(MovingJoystick move)
	{
	}

	private void On_JoystickMoveStart(MovingJoystick move)
	{
	}

	private void On_JoystickTouchStart(MovingJoystick move)
	{
	}
}
