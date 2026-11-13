using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
    public class ReplayIntroButtonClickHandler : MonoBehaviour
    {
        public void OnReplayClicked()
        {
            GameStateController controller = Service.Get<GameStateController>();
            if (controller == null)
            {
                Debug.LogWarning("GameStateController not found for replay intro.");
                return;
            }

            controller.PlayIntroVideo();
        }
    }
}
