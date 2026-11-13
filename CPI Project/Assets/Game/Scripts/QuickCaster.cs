using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class QuickCaster : BaseRaycaster
{
	[Header("UI elements require RaycastTarget enabled. No overlaps allowed. Canvas groups are slow.")]
	public bool supportCanvasGroupsAndDragging = false;

	private Graphic fallbackElement;

	private Canvas canvas;

	private Camera uiCamera;

	private Graphic previousResult;

	private Vector2 screenScale;

	public override Camera eventCamera
	{
		get
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
			{
				return null;
			}
			return (canvas.worldCamera != null) ? canvas.worldCamera : Camera.main;
		}
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		Vector2 position = eventData.position;
		RaycastResult item = RaycastAll(position);
		if (item.isValid)
		{
			item.index = resultAppendList.Count;
			resultAppendList.Add(item);
		}
	}

	protected override void Start()
	{
		base.Start();
		canvas = GetComponent<Canvas>();
		uiCamera = canvas.worldCamera;
		screenScale = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		if (canvas.renderMode != RenderMode.ScreenSpaceCamera || uiCamera == null)
		{
			Log.LogError(this, "QuickCaster canvas must be set to ScreenSpace - Camera!");
		}
	}

	private RaycastResult RaycastAll(Vector2 ptInScreen)
	{
		Matrix4x4 matWorldClip = uiCamera.projectionMatrix * uiCamera.worldToCameraMatrix;
		if (previousResult != null && previousResult.raycastTarget && graphicContainsPoint(ptInScreen, previousResult, uiCamera, matWorldClip, screenScale, supportCanvasGroupsAndDragging))
		{
			return createRaycastResult(ptInScreen, previousResult);
		}
		previousResult = null;
		fallbackElement = null;
		IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
		int count = graphicsForCanvas.Count;
		for (int i = 0; i < count; i++)
		{
			Graphic graphic = graphicsForCanvas[i];
			if (graphic.raycastTarget && graphicContainsPoint(ptInScreen, graphic, uiCamera, matWorldClip, screenScale, supportCanvasGroupsAndDragging))
			{
				EmptyGraphic emptyGraphic = graphic as EmptyGraphic;
				if (!(emptyGraphic != null) || !emptyGraphic.lowPriorityTarget)
				{
					previousResult = graphic;
					return createRaycastResult(ptInScreen, graphic);
				}
				fallbackElement = emptyGraphic;
			}
		}
		if (fallbackElement != null)
		{
			return createRaycastResult(ptInScreen, fallbackElement);
		}
		return default(RaycastResult);
	}

	private RaycastResult createRaycastResult(Vector2 ptInScreen, Graphic graphic)
	{
		Transform transform = graphic.transform;
		Vector3 forward = transform.forward;
		Ray ray = uiCamera.ScreenPointToRay(ptInScreen);
		float distance = Vector3.Dot(forward, transform.position - ray.origin) / Vector3.Dot(forward, ray.direction);
		RaycastResult result = default(RaycastResult);
		result.gameObject = graphic.gameObject;
		result.module = this;
		result.distance = distance;
		result.screenPosition = ptInScreen;
		result.depth = graphic.depth;
		result.sortingLayer = canvas.sortingLayerID;
		result.sortingOrder = canvas.sortingOrder;
		return result;
	}

	private static bool graphicContainsPoint(Vector2 ptInScreen, Graphic graphic, Camera camera, Matrix4x4 matWorldClip, Vector2 screenScale, bool supportCanvasGroups)
	{
		if (graphic.depth == -1)
		{
			return false;
		}
		Vector3[] array = new Vector3[4];
		graphic.rectTransform.GetWorldCorners(array);
		array[0] = matWorldClip.MultiplyPoint(array[0]);
		array[0].x = (array[0].x + 1f) * screenScale.x;
		array[0].y = (array[0].y + 1f) * screenScale.y;
		if (ptInScreen.x < array[0].x || ptInScreen.y < array[0].y)
		{
			return false;
		}
		array[2] = matWorldClip.MultiplyPoint(array[2]);
		array[2].x = (array[2].x + 1f) * screenScale.x;
		array[2].y = (array[2].y + 1f) * screenScale.y;
		if (ptInScreen.x > array[2].x || ptInScreen.y > array[2].y)
		{
			return false;
		}
		return !supportCanvasGroups || graphic.Raycast(ptInScreen, camera);
	}
}
