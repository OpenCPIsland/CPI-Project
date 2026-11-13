using EasyTouchPlugin;
using UnityEngine;

public class VirtualScreen : EasyTouchPlugin.MonoSingleton<VirtualScreen>
{
	public delegate void On_ScreenResizeHandler();

	public enum ScreenResolution
	{
		IPhoneTall,
		IPhoneWide,
		IPhone4GTall,
		IPhone4GWide,
		IPadTall,
		IPadWide
	}

	public float virtualWidth = 1024f;

	public float virtualHeight = 768f;

	public static float width = 1024f;

	public static float height = 768f;

	public static float xRatio = 1f;

	public static float yRatio = 1f;

	private float realWidth;

	private float realHeight;

	private float oldRealWidth;

	private float oldRealHeight;

	public static event On_ScreenResizeHandler On_ScreenResize;

	private void Awake()
	{
		realWidth = (oldRealWidth = Screen.width);
		realHeight = (oldRealHeight = Screen.height);
		ComputeScreen();
	}

	private void Update()
	{
		realWidth = Screen.width;
		realHeight = Screen.height;
		if (realWidth != oldRealWidth || realHeight != oldRealHeight)
		{
			ComputeScreen();
			if (VirtualScreen.On_ScreenResize != null)
			{
				VirtualScreen.On_ScreenResize();
			}
		}
		oldRealWidth = realWidth;
		oldRealHeight = realHeight;
	}

	public void ComputeScreen()
	{
		width = virtualWidth;
		height = virtualHeight;
		xRatio = 1f;
		yRatio = 1f;
		float num = 0f;
		float num2 = 0f;
		if (Screen.width > Screen.height)
		{
			num = (float)Screen.width / (float)Screen.height;
			num2 = width;
		}
		else
		{
			num = (float)Screen.height / (float)Screen.width;
			num2 = height;
		}
		float num3 = 0f;
		num3 = num2 / num;
		if (Screen.width > Screen.height)
		{
			height = num3;
			xRatio = (float)Screen.width / width;
			yRatio = (float)Screen.height / height;
		}
		else
		{
			width = num3;
			xRatio = (float)Screen.width / width;
			yRatio = (float)Screen.height / height;
		}
	}

	public static void ComputeVirtualScreen()
	{
		EasyTouchPlugin.MonoSingleton<VirtualScreen>.instance.ComputeScreen();
	}

	public static void SetGuiScaleMatrix()
	{
		GUI.matrix = Matrix4x4.Scale(new Vector3(xRatio, yRatio, 1f));
	}

	public static Rect GetRealRect(Rect rect)
	{
		return new Rect(rect.x * xRatio, rect.y * yRatio, rect.width * xRatio, rect.height * yRatio);
	}
}
