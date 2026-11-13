using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
    [RequireComponent(typeof(Button))]
    public class WebViewButton : MonoBehaviour
    {
        public string URLToken;
        public string TitleToken;

        private void Start()
        {
            Button component = GetComponent<Button>();
            component.onClick.AddListener(onClicked);
        }

        private void onClicked()
        {
        }
    }
}