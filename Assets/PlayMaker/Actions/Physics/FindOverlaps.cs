
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Find overlaps with GameObject colliders in the scene.")]
    public class FindOverlaps : ComponentAction<Transform>
    {
        public enum Shape
        {
            Box,
            Sphere,
            Capsule
        }

        //[ActionSection("Inputs")]

        [Tooltip("GameObject position to use for the test shape. Set to none to use world origin (0,0,0).")]
        public FsmOwnerDefault position;

        [Tooltip("Offset position of the shape.")]
        public FsmVector3 offset;

        [Tooltip("Shape to find overlaps against.")]
        public Shape shape;

        [HideIf("HideRadius")]
        [Tooltip("Radius of sphere/capsule.")]
        public FsmFloat radius;

        [HideIf("HideBox")]
        [Tooltip("Size of box.")]
        public FsmVector3 box;

        [HideIf("HideCapsule")]
        [Tooltip("The height of the capsule.")]
        public FsmFloat height;

        [Tooltip("Maximum number of overlaps to detect.")]
        public FsmInt maxOverlaps;

        [ActionSection("Filter")]

        [UIHint(UIHint.LayerMask)]
        [Tooltip("LayerMask name to filter the overlapping objects")]
        public FsmInt layerMask;

        [Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;

        [Tooltip("Include self in the array.")]
        public FsmBool includeSelf;

        [Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause Overlaps can get expensive use the highest repeat interval you can get away with.")]
        public FsmInt repeatInterval;

        [ActionSection("Output")]

        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.GameObject)]
        [Tooltip("Store overlapping GameObjects in an array.")]
        public FsmArray storeOverlapping;

        [Tooltip("Event to send if overlaps were found.")]
        public FsmEvent foundOverlaps;

        [Tooltip("Event to send if no overlaps were found.")]
        public FsmEvent noOverlaps;

        [ActionSection("Debug")]

        [Tooltip("The color to use for the debug line.")]
        public FsmColor debugColor;

        [Tooltip("Draw a gizmo in the scene view to visualize the shape.")]
        public FsmBool debug;

        public Vector3 center { get; private set; }
        public Quaternion orientation { get; private set; }
        public Vector3 capsulePoint1 { get; private set; }
        public Vector3 capsulePoint2 { get; private set; }
        public int targetMask { get; private set; }

        private Collider[] colliders;
        private int repeat;

        public override void Reset()
        {
            position = null;
            offset = null;
            shape = Shape.Box;
            radius = new FsmFloat {Value = 1f};
            box = new FsmVector3 {Value = new Vector3(1, 1, 1)};
            height = new FsmFloat { Value = 1f };
            storeOverlapping = null;
            maxOverlaps = new FsmInt {Value = 50};
            repeatInterval = new FsmInt { Value = 1 };
            foundOverlaps = null;
            includeSelf = null;
            layerMask = null;
            invertMask = null;
            noOverlaps = null;
            debugColor = new FsmColor { Value = Color.yellow };
            debug = null;
        }

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }

        public bool HideBox()
        {
            return shape != Shape.Box;
        }

        public bool HideRadius()
        {
            return shape != Shape.Sphere && shape != Shape.Capsule;
        }

        public bool HideCapsule()
        {
            return shape != Shape.Capsule;
        }

        public override void OnEnter()
        {
            colliders = new Collider[Mathf.Clamp(maxOverlaps.Value, 0, int.MaxValue)];

            DoGetOverlap();

            if (repeatInterval.Value == 0)
            {
                Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            repeat--;

            if (repeat == 0)
            {
                DoGetOverlap();
            }
        }

        private void DoGetOverlap()
        {
            repeat = repeatInterval.Value;

            InitShapeCenter();

            targetMask = layerMask.Value;
            if (invertMask.Value) targetMask = ~targetMask;

            var overlapCount = 0;

            switch (shape)
            {
                case Shape.Box:
                    overlapCount = Physics.OverlapBoxNonAlloc(center, box.Value / 2f, colliders, orientation, targetMask);
                    break;

                case Shape.Sphere:
                    overlapCount = Physics.OverlapSphereNonAlloc(center, radius.Value, colliders, targetMask);
                    break;

                case Shape.Capsule:
                    overlapCount = Physics.OverlapCapsuleNonAlloc(capsulePoint1, capsulePoint2, radius.Value, colliders, targetMask);
                    break;
            }

            // Store overlaps

            if (overlapCount == 0)
            {
                storeOverlapping.Values = new object[0];
            }
            else if (includeSelf.Value)
            {
                // cheaper 

                storeOverlapping.Values = new object[overlapCount];
                for (var i = 0; i < overlapCount; i++)
                {
                    storeOverlapping.Values[i] = colliders[i].gameObject;
                }
            }
            else
            {
                // more expensive

                var gameObjects = new List<object>();

                for (var i = 0; i < overlapCount; i++)
                {
                    var go = colliders[i].gameObject;
                    // check children too?
                    if (go != cachedGameObject)
                        gameObjects.Add(go);
                }

                storeOverlapping.Values = gameObjects.ToArray();
            }

            // Send events

            Fsm.Event(overlapCount > 0 ? foundOverlaps : noOverlaps);
        }

        public void InitShapeCenter()
        {
            center = offset.Value;
            orientation = Quaternion.identity;

            var go = Fsm.GetOwnerDefaultTarget(position);
            if (UpdateCachedTransform(go))
            {
                center = cachedTransform.TransformPoint(offset.Value);
                orientation = cachedTransform.rotation;

                if (shape == Shape.Capsule)
                {
                    var pointOffset = height.Value/2 - radius.Value;
                    capsulePoint1 = cachedTransform.TransformPoint(new Vector3(0, -pointOffset, 0));
                    capsulePoint2 = cachedTransform.TransformPoint(new Vector3(0, pointOffset, 0));
                }
            }
        }
    }
}
