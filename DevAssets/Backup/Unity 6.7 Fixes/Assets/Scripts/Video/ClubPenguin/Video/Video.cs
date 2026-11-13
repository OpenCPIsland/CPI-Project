using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ClubPenguin.Video
{
    public class Video
    {
        private class NativeAudio
        {
          //  [DllImport("__Internal")]
        //    public static extern bool _IsMusicPlaying();

        //    [DllImport("__Internal")]
       //     public static extern void DuckOthers();

    //        [DllImport("__Internal")]
     //       public static extern void RestoreDuckOthers();
        }

        public static IEnumerator PlayFullScreenVideo(string videoPath)
        {
          if (string.IsNullOrEmpty(videoPath))
          {
            Debug.LogWarning("PlayFullScreenVideo called with empty path.");
            yield break;
          }

          Debug.Log("PlayFullScreenVideo requested: " + videoPath);
          VideoPlaybackRunner runner = VideoPlaybackRunner.Ensure();
          yield return runner.Play(videoPath);
        }
    }

      internal class VideoPlaybackRunner : MonoBehaviour
      {
        private const int MaxCanvasSortingOrder = 32767;
        private const float StartupTimeoutSeconds = 10f;

        private static VideoPlaybackRunner instance;

        public static VideoPlaybackRunner Ensure()
        {
          if (instance != null)
          {
            return instance;
          }

          GameObject runnerObject = new GameObject("VideoPlaybackRunner");
          DontDestroyOnLoad(runnerObject);
          instance = runnerObject.AddComponent<VideoPlaybackRunner>();
          return instance;
        }

        private void OnDestroy()
        {
          if (instance == this)
          {
            instance = null;
          }
        }

        public IEnumerator Play(string videoPath)
        {
          Debug.Log("Video playback started: " + videoPath);
          VideoClip clip = null;
          string url = null;

#if UNITY_WEBGL && !UNITY_EDITOR
          url = ResolveUrl(videoPath);
          if (string.IsNullOrEmpty(url))
          {
            Debug.LogWarning("Video not found: " + videoPath);
            yield break;
          }
          Debug.Log("Video URL resolved: " + url);
#else
          clip = ResolveClip(videoPath);
          if (clip != null)
          {
            Debug.Log("Video clip resolved from Resources: " + clip.name);
          }
          else
          {
            url = ResolveUrl(videoPath);
            if (string.IsNullOrEmpty(url))
            {
              Debug.LogWarning("Video not found: " + videoPath);
              yield break;
            }
            Debug.Log("Video URL resolved: " + url);
          }
#endif

          GameObject root = new GameObject("VideoPlaybackRoot");
          DontDestroyOnLoad(root);

          Canvas canvas = root.AddComponent<Canvas>();
          canvas.renderMode = RenderMode.ScreenSpaceOverlay;
          canvas.overrideSorting = true;
          canvas.sortingOrder = MaxCanvasSortingOrder;

          root.AddComponent<GraphicRaycaster>();

          if (Object.FindAnyObjectByType<EventSystem>() == null)
          {
            GameObject esObject = new GameObject("VideoEventSystem");
            esObject.transform.SetParent(root.transform, false);
            esObject.AddComponent<EventSystem>();
            esObject.AddComponent<StandaloneInputModule>();
          }

          CanvasScaler scaler = root.AddComponent<CanvasScaler>();
          scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
          scaler.referenceResolution = new Vector2(1920f, 1080f);
          scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
          scaler.matchWidthOrHeight = 0.5f;

          GameObject bgObject = new GameObject("VideoBackground");
          bgObject.transform.SetParent(root.transform, false);
          Image bgImage = bgObject.AddComponent<Image>();
          bgImage.color = Color.black;
          bgImage.raycastTarget = false;
          RectTransform bgRect = bgImage.rectTransform;
          bgRect.anchorMin = Vector2.zero;
          bgRect.anchorMax = Vector2.one;
          bgRect.offsetMin = Vector2.zero;
          bgRect.offsetMax = Vector2.zero;

          GameObject imageObject = new GameObject("VideoImage");
          imageObject.transform.SetParent(root.transform, false);
          RawImage rawImage = imageObject.AddComponent<RawImage>();
          RectTransform imageRect = rawImage.rectTransform;
          imageRect.anchorMin = Vector2.zero;
          imageRect.anchorMax = Vector2.one;
          imageRect.offsetMin = Vector2.zero;
          imageRect.offsetMax = Vector2.zero;

            AspectRatioFitter aspectFitter = imageObject.AddComponent<AspectRatioFitter>();
            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

            GameObject skipButtonObject = new GameObject("VideoSkipButton");
            skipButtonObject.transform.SetParent(root.transform, false);
            Image skipImage = skipButtonObject.AddComponent<Image>();
            skipImage.color = new Color(0f, 0f, 0f, 0f);
            skipImage.raycastTarget = true;
            RectTransform skipRect = skipImage.rectTransform;
            skipRect.anchorMin = Vector2.zero;
            skipRect.anchorMax = Vector2.one;
            skipRect.offsetMin = Vector2.zero;
            skipRect.offsetMax = Vector2.zero;
            Button skipButton = skipButtonObject.AddComponent<Button>();
            skipButtonObject.transform.SetAsLastSibling();

          VideoPlayer player = root.AddComponent<VideoPlayer>();
          if (!string.IsNullOrEmpty(url))
          {
            player.source = VideoSource.Url;
            player.url = url;
          }
          else
          {
            player.source = VideoSource.VideoClip;
            player.clip = clip;
          }
          player.playOnAwake = false;
          player.isLooping = false;
          player.skipOnDrop = true;
          player.renderMode = VideoRenderMode.RenderTexture;
          player.aspectRatio = VideoAspectRatio.NoScaling;
          player.audioOutputMode = VideoAudioOutputMode.Direct;

          bool finished = false;
          bool started = false;
          bool hasError = false;
          bool skipRequested = false;
          bool playRequested = false;

          skipButton.onClick.AddListener(delegate
          {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!started)
            {
              playRequested = true;
              return;
            }
#endif
            skipRequested = true;
          });

          player.loopPointReached += delegate { finished = true; };
          player.started += delegate { started = true; };
          player.errorReceived += delegate (VideoPlayer source, string message)
          {
            Debug.LogError("Video playback error: " + message);
            hasError = true;
            finished = true;
          };

          float previousListenerVolume = AudioListener.volume;
          AudioListener.volume = 0f;

          player.Prepare();
          float prepareTimer = 0f;
          while (!player.isPrepared && !hasError)
          {
            prepareTimer += Time.unscaledDeltaTime;
            if (prepareTimer >= StartupTimeoutSeconds)
            {
              Debug.LogWarning("Video prepare timed out.");
              hasError = true;
              break;
            }
            yield return null;
          }

          if (!hasError)
          {
            int width = (int)player.width;
            int height = (int)player.height;
            if (width <= 0 || height <= 0)
            {
              width = Mathf.Max(Screen.width, 16);
              height = Mathf.Max(Screen.height, 16);
            }

            RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            player.targetTexture = renderTexture;
            rawImage.texture = renderTexture;
            aspectFitter.aspectRatio = (float)width / height;

#if UNITY_WEBGL && !UNITY_EDITOR
            if (player.source == VideoSource.Url)
            {
              float prePlayTimer = 0f;
              while (!playRequested && !hasError)
              {
                bool gamepadPressed = false;
                if (Gamepad.current != null)
                {
                  foreach (var control in Gamepad.current.allControls)
                  {
                    if (control is UnityEngine.InputSystem.Controls.ButtonControl btn && btn.wasPressedThisFrame)
                    {
                      gamepadPressed = true;
                      break;
                    }
                  }
                }

                if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame
                    || Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
                    || Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame
                    || gamepadPressed)
                {
                  playRequested = true;
                  break;
                }

                prePlayTimer += Time.unscaledDeltaTime;
                if (prePlayTimer >= StartupTimeoutSeconds)
                {
                  playRequested = true;
                  break;
                }
                yield return null;
              }
            }
#endif
            player.Play();

            float startupTimer = 0f;
            while (!finished)
            {
              bool gamepadPressed = false;
              if (Gamepad.current != null)
              {
                foreach (var control in Gamepad.current.allControls)
                {
                  if (control is UnityEngine.InputSystem.Controls.ButtonControl btn && btn.wasPressedThisFrame)
                  {
                    gamepadPressed = true;
                    break;
                  }
                }
              }

              bool inputPressed = skipRequested
                  || Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame
                  || Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
                  || Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame
                  || gamepadPressed;

#if UNITY_WEBGL && !UNITY_EDITOR
              if (started && inputPressed)
#else
              if (inputPressed)
#endif
              {
                player.Stop();
                break;
              }

              if (!started)
              {
                startupTimer += Time.unscaledDeltaTime;
                if (startupTimer >= StartupTimeoutSeconds)
                {
                  Debug.LogWarning("Video playback timed out while starting.");
                  break;
                }
              }
              yield return null;
            }

            if (player.targetTexture != null)
            {
              player.targetTexture.Release();
            }
          }

          AudioListener.volume = previousListenerVolume;

          Destroy(root);
        }

        private static string ResolveUrl(string videoPath)
        {
          if (videoPath.Contains("://"))
          {
            return videoPath;
          }

          string streamingAssetsPath = Application.streamingAssetsPath;
          string combinedPath;
          if (streamingAssetsPath.Contains("://"))
          {
            combinedPath = streamingAssetsPath.TrimEnd('/') + "/" + videoPath.TrimStart('/');
          }
          else
          {
            combinedPath = Path.Combine(streamingAssetsPath, videoPath);
          }
          combinedPath = combinedPath.Replace("\\", "/");

    #if UNITY_EDITOR || UNITY_STANDALONE
          if (!File.Exists(combinedPath))
          {
            return null;
          }
    #endif

          return combinedPath;
        }

        private static VideoClip ResolveClip(string videoPath)
        {
          if (string.IsNullOrEmpty(videoPath))
          {
            return null;
          }

          string resourcePath = videoPath.Replace("\\", "/");
          int resourcesIndex = resourcePath.IndexOf("Resources/", System.StringComparison.OrdinalIgnoreCase);
          if (resourcesIndex >= 0)
          {
            resourcePath = resourcePath.Substring(resourcesIndex + "Resources/".Length);
          }
          if (resourcePath.EndsWith(".mp4", System.StringComparison.OrdinalIgnoreCase))
          {
            resourcePath = resourcePath.Substring(0, resourcePath.Length - 4);
          }
          else if (resourcePath.EndsWith(".webm", System.StringComparison.OrdinalIgnoreCase))
          {
            resourcePath = resourcePath.Substring(0, resourcePath.Length - 5);
          }

          return Resources.Load<VideoClip>(resourcePath);
        }
      }
}