using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Collider))]
	public class MeshButton : MonoBehaviour
	{
		public Camera mainCamera;

		private Collider buttonCollider;

		public bool IsInteractable
		{
			get;
			set;
		}

		public event Action OnClick;

		private void Awake()
		{
			IsInteractable = true;
			buttonCollider = GetComponent<Collider>();
		}

		private void Update()
		{
			if (IsInteractable && UnityEngine.Input.GetMouseButtonDown(0))
			{
				CheckHit(UnityEngine.Input.mousePosition);
			}
		}

		private void CheckHit(Vector3 position)
		{
			RaycastHit hitInfo;
			if (buttonCollider.Raycast(mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition), out hitInfo, 100f))
			{
				onClick();
			}
		}

		private void onClick()
		{
			if (this.OnClick != null)
			{
				this.OnClick();
			}
		}
	}
}
