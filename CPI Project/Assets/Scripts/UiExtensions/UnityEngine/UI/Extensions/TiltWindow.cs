using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.UI.Extensions
{
    public class TiltWindow : MonoBehaviour
    {
        public Vector2 range = new Vector2(5f, 3f);

        private Transform mTrans;
        private Quaternion mStart;
        private Vector2 mRot = Vector2.zero;

        private void Start()
        {
            mTrans = transform;
            mStart = mTrans.localRotation;
        }

        private void Update()
        {
            // Use the new Input System for mouse position
            Vector3 mousePosition = Mouse.current != null ? (Vector3)Mouse.current.position.ReadValue() : Vector3.zero;
            float num = Screen.width * 0.5f;
            float num2 = Screen.height * 0.5f;
            float x = Mathf.Clamp((mousePosition.x - num) / num, -1f, 1f);
            float y = Mathf.Clamp((mousePosition.y - num2) / num2, -1f, 1f);
            mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);
            mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * range.y, mRot.x * range.x, 0f);
        }
    }
}