using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Canvas))]
	public class SetCanvasToMainCamera : MonoBehaviour
	{
		private void Start()
		{
			Canvas component = GetComponent<Canvas>();
			component.worldCamera = Camera.main;
			component.planeDistance = 1f;
			component.renderMode = RenderMode.ScreenSpaceCamera;
		}
	}
}
