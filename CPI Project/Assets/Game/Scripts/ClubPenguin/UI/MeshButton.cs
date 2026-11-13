using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
            if (IsInteractable && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                CheckHit(Mouse.current.position.ReadValue());
            }
        }

        private void CheckHit(Vector3 position)
        {
            Ray ray = mainCamera.ScreenPointToRay(position);
            RaycastHit hitInfo;
            if (buttonCollider.Raycast(ray, out hitInfo, 100f))
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