using UnityEngine;

public class MovingJoystick
{
	public string joystickName;

	public Vector2 joystickAxis;

	public Vector2 joystickValue;

	public EasyJoystick joystick;

	public float Axis2Angle(bool inDegree = true)
	{
		float num = Mathf.Atan2(joystickAxis.x, joystickAxis.y);
		if (inDegree)
		{
			return num * 57.29578f;
		}
		return num;
	}
}
