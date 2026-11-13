using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin
{
    internal class ReplayIntroButtonAutoBinder : MonoBehaviour
    {
        private const string ReplayButtonName = "ReplayButton";

        private static ReplayIntroButtonAutoBinder instance;

        private readonly HashSet<int> wiredButtons = new HashSet<int>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (instance != null)
            {
                return;
            }

            GameObject host = new GameObject("ReplayIntroButtonAutoBinder");
            DontDestroyOnLoad(host);
            instance = host.AddComponent<ReplayIntroButtonAutoBinder>();
            SceneManager.sceneLoaded += instance.OnSceneLoaded;
            instance.StartCoroutine(instance.WireButtonsNextFrame());
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                instance = null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(WireButtonsNextFrame());
        }

        private IEnumerator WireButtonsNextFrame()
        {
            yield return null;
            Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = buttons[i];
                if (button == null)
                {
                    continue;
                }

                if (!string.Equals(button.gameObject.name, ReplayButtonName, System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int id = button.GetInstanceID();
                if (wiredButtons.Contains(id))
                {
                    continue;
                }

                button.onClick.AddListener(OnReplayClicked);
                wiredButtons.Add(id);
                Debug.Log("Replay intro button wired: " + button.gameObject.name);
            }
        }

        private void OnReplayClicked()
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
