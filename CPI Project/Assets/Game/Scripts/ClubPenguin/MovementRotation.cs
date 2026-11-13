using ClubPenguin.Net;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
    public class MovementRotation : ProximityBroadcaster
    {
        [Tooltip("Transform to use as center of rotation")]
        public Transform rotationCenter;

        [Tooltip("Seconds to complete 1 revolution")]
        public float secondsPerRotation = 10f;

        [Tooltip("Keep object facing original rotation")]
        public bool doNotRotateObject = true;

        public bool isControlledByParent = false;

        public bool isActive = false;

        private Vector3 centerPosition;
        private Vector3 centerDistance;

        private INetworkServicesManager network;

        public override void Awake()
        {
            base.Awake();

            centerPosition = rotationCenter.position;
            centerDistance = transform.position - centerPosition;
        }

        private void OnEnable()
        {
            network = Service.Get<INetworkServicesManager>();
        }

        private void Update()
        {
            if (network != null && isActive)
            {
                float cycle = secondsPerRotation * 1000f;

                float t = (network.GameTimeMilliseconds % (int)cycle) / 1000f;

                float angle = (360f / secondsPerRotation) * t;

                transform.position =
                    centerPosition +
                    Quaternion.AngleAxis(angle - 90f, Vector3.forward) * centerDistance;

                if (!doNotRotateObject)
                {
                    transform.rotation =
                        Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }

        public override void OnProximityEnter(ProximityListener other)
        {
            if (!isControlledByParent)
            {
                isActive = true;
            }
        }

        public override void OnProximityExit(ProximityListener other)
        {
            if (!isControlledByParent)
            {
                isActive = false;
            }
        }

        public override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (rotationCenter != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(
                    rotationCenter.position,
                    Vector3.Distance(transform.position, rotationCenter.position)
                );
            }
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (rotationCenter != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, rotationCenter.position);
            }
        }

        public void SetActive(bool active)
        {
            isActive = true;
        }
    }
}