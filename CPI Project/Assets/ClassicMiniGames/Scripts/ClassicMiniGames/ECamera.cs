using System;
using UnityEngine;

[Serializable]
public class ECamera
{
	public Camera camera;

	public bool guiCamera;

	public ECamera(Camera cam, bool gui)
	{
		camera = cam;
		guiCamera = gui;
	}
}
