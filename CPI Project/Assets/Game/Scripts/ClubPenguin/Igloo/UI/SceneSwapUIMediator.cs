using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Igloo.UI
{
	public class SceneSwapUIMediator : AbstractIglooUIMediator
	{
		public PrefabContentKey DefaultLoadingScene;

		private SceneTransitionService sceneTransitionService;

		private Action transitionCompleteCallback = null;

		protected override void Awake()
		{
			base.Awake();
			sceneTransitionService = Service.Get<SceneTransitionService>();
			contextListener.OnContextAdded += onContextAdded;
			eventChannel.AddListener<IglooUIEvents.SwapScene>(onSwapScene);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			contextListener.OnContextAdded -= onContextAdded;
		}

		private void onContextAdded(StateMachineContext context)
		{
			contextListener.OnContextAdded -= onContextAdded;
			base.context = context;
		}

		public void ReloadCurrentScene(Action callback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), DefaultLoadingScene.Key);
			LoadScene(sceneTransitionService.CurrentScene, dictionary, callback);
		}

		public void LoadScene(string sceneName, Dictionary<string, object> sceneArgs = null, Action callback = null)
		{
			if (callback != null)
			{
				if (transitionCompleteCallback != null)
				{
				}
				eventChannel.AddListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
				transitionCompleteCallback = callback;
			}
			sceneTransitionService.LoadScene(sceneName, "Loading", sceneArgs);
		}

		private bool onSceneTransitionComplete(SceneTransitionEvents.TransitionComplete evt)
		{
			eventChannel.RemoveListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
			if (transitionCompleteCallback != null)
			{
				transitionCompleteCallback();
				transitionCompleteCallback = null;
			}
			return false;
		}

		private bool onSwapScene(IglooUIEvents.SwapScene evt)
		{
			SwapScene(evt.SceneName, evt.SceneArgs, delegate
			{
				eventDispatcher.DispatchEvent(default(IglooUIEvents.SwapSceneComplete));
			});
			return false;
		}

		public void SwapScene(string sceneName, Dictionary<string, object> sceneArgs = null, Action callback = null)
		{
			if (!Service.Get<SceneTransitionService>().IsTransitioning || Service.Get<SceneTransitionService>().CurrentScene != sceneName)
			{
				PausedStateData component;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
				{
					component.ShouldSkipResume = true;
				}
				CoroutineRunner.Start(swapScene(sceneName, sceneArgs, callback), this, "swapScene");
			}
		}

		private IEnumerator swapScene(string scene, Dictionary<string, object> sceneArgs, Action callback)
		{
			context.SendEvent(new ExternalEvent("IglooUIModalLoadingSpinner", "enable"));
			context.SendEvent(new ExternalEvent("IglooUIModalLoadingSpinner_BG", "enable"));
			yield return Service.Get<SceneTransitionService>().SwapScene(scene, sceneArgs);
			context.SendEvent(new ExternalEvent("IglooUIModalLoadingSpinner", "disable"));
			context.SendEvent(new ExternalEvent("IglooUIModalLoadingSpinner_BG", "disable"));
			if (callback != null)
			{
				callback();
			}
		}
	}
}
