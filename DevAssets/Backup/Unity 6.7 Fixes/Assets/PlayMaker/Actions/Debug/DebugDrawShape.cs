using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Draw a debug Gizmo.\nNote: you can enable/disable Gizmos in the Game View toolbar.")]
    public class DebugDrawShape : ComponentAction<Transform>
    {
        public enum ShapeType { Sphere, Cube, WireSphere, WireCube }

        [RequiredField]
        [Tooltip("Draw the Gizmo at a GameObject's position.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The type of Gizmo to draw:\nSphere, Cube, WireSphere, or WireCube.")]
        public ShapeType shape;
        
        [Tooltip("The color to use.")]
        public FsmColor color;
        
        [HideIf("HideRadius")]
        [Tooltip("Use this for sphere gizmos")]
        public FsmFloat radius;
        
        [HideIf("HideSize")]
        [Tooltip("Use this for cube gizmos")]
        public FsmVector3 size;

        public bool HideRadius()
        {
            return shape != ShapeType.Sphere && shape != ShapeType.WireSphere;
        }

        public bool HideSize()
        {
            return shape != ShapeType.Cube && shape != ShapeType.WireCube;
        }

        public override void Reset()
        {
            gameObject = null;
            shape = ShapeType.Sphere;
            color = new FsmColor {Value = Color.grey};
            radius = new FsmFloat {Value = 1f};
            size = new Vector3(1f, 1f, 1f);
        }

        public override void Awake()
        {
            BlocksFinish = false;
        }

        public override void OnDrawActionGizmos()
        {
            if (Fsm == null) return; 

            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (!UpdateCachedTransform(go)) return;

            Gizmos.color = color.Value;

            switch (shape)
            {
                case ShapeType.Sphere:
                    Gizmos.DrawSphere(cachedTransform.position, radius.Value);
                    break;
                case ShapeType.WireSphere:
                    Gizmos.DrawWireSphere(cachedTransform.position, radius.Value);
                    break;
                case ShapeType.Cube:
                    Gizmos.DrawCube(cachedTransform.position, size.Value);
                    break;
                case ShapeType.WireCube:
                    Gizmos.DrawWireCube(cachedTransform.position, size.Value);
                    break;
                default:
                    break;
            }
        }
    }
}
