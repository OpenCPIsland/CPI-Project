using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(HutongGames.PlayMaker.Actions.SetCameraFOV))]
    public class SetCameraFOVActionEditor : CustomActionEditor
    {
        private GameObject cachedGameObject;
        private Camera camera;

        public override bool OnGUI()
        {
            return DrawDefaultInspector();
        }

        public override void OnSceneGUI()
        {
            var setCameraFOVAction = target as HutongGames.PlayMaker.Actions.SetCameraFOV;
            if (setCameraFOVAction == null) // shouldn't happen!
            {
                return;
            }

            if (setCameraFOVAction.fieldOfView.IsNone)
            {
                return;
            }

            var go = setCameraFOVAction.Fsm.GetOwnerDefaultTarget(setCameraFOVAction.gameObject);
            var fov = setCameraFOVAction.fieldOfView.Value;

            if (go != null && fov > 0)
            {
                if (go != cachedGameObject || camera == null)
                {
                    camera = go.GetComponent<Camera>();
                    cachedGameObject = go;
                }

                if (camera != null)
                {
                    var originalFOV = camera.fieldOfView;
                    camera.fieldOfView = setCameraFOVAction.fieldOfView.Value;

                    Handles.color = new Color(1, 1, 0, .5f);
                    SceneGUI.DrawCameraFrustrum(camera);

                    camera.fieldOfView = originalFOV;
                }
            }
        }
    }
}
