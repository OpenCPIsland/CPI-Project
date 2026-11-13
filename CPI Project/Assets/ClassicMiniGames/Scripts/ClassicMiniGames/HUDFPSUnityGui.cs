using System.Collections;
using UnityEngine;

public class HUDFPSUnityGui : MonoBehaviour
{
	public Rect startRect = new Rect(10f, 10f, 75f, 50f);

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 0.5f;

	public int nbDecimal = 1;

	private float accum = 0f;

	private int frames = 0;

	private Color color = Color.white;

	private string sFPS = "";

	private GUIStyle style;

	public bool forceToLowerLeft = true;

	public float offsetFromBottomWhenForcedToLowerLeft = -10f;

	public static HUDFPSUnityGui Instance = null;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (forceToLowerLeft)
		{
			Rect rect = startRect = new Rect(0f, (float)Screen.height - startRect.height - offsetFromBottomWhenForcedToLowerLeft, startRect.width, startRect.height);
		}
		StartCoroutine(FPS());
	}

	private void Update()
	{
		accum += Time.timeScale / Time.deltaTime;
		frames++;
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			float fps = accum / (float)frames;
			sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
			int frameRate = (Application.targetFrameRate > 0) ? Application.targetFrameRate : 60;
			if (fps >= (float)frameRate)
			{
				color = Color.green;
			}
			else if (fps >= (float)(frameRate / 2))
			{
				color = Color.yellow;
			}
			else
			{
				color = Color.red;
			}
			accum = 0f;
			frames = 0;
			yield return new WaitForSeconds(frequency);
		}
	}

	private void OnGUI()
	{
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
		}
		GUI.color = (updateColor ? color : Color.white);
		startRect = GUI.Window(0, startRect, DoMyWindow, "");
	}

	private void DoMyWindow(int windowID)
	{
		GUI.Label(new Rect(0f, 0f, startRect.width, startRect.height), sFPS + " fps", style);
		if (allowDrag)
		{
			GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
		}
	}
}
