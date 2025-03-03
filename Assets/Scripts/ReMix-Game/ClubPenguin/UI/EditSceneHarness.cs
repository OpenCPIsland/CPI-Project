using Disney.Kelowna.Common;
using Disney.Kelowna.Common.GameObjectTree;
using Disney.Kelowna.Common.Manifest;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.UI
{
	public class EditSceneHarness : MonoBehaviour
	{
		[Tooltip("The tree of game object nodes stemming from this node definition will be created on scene load.")]
		public TreeNodeDefinitionContentKey RootNodeDefinitionContentKey;

		public GameObject DestroyOnAwake;

		private void Awake()
		{
			destroyBakedTree();
			StartCoroutine(initRoutine());
		}

		private IEnumerator initRoutine()
		{
			createCamera();
			createEventSystem();
			if (Service.Get<Content>() == null)
			{
				setDependencies();
				setConfigurator();
				yield return StartCoroutine(setContentSystem());
			}
			loadUIRoot();
			Object.Destroy(base.gameObject);
		}

		private void createCamera()
		{
			Camera camera = new GameObject("Camera").AddComponent<Camera>();
			camera.gameObject.tag = "MainCamera";
		}

		private void createEventSystem()
		{
			GameObject gameObject = new GameObject("EventSystem");
			gameObject.AddComponent<EventSystem>();
			gameObject.AddComponent<StandaloneInputModule>();
		}

		private void setDependencies()
		{
			Service.Set(base.gameObject);
			Service.Set(new EventDispatcher());
			Service.Set((JsonService)new LitJsonService());
			Service.Set(base.gameObject.AddComponent<CoroutineRunner>());
		}

		private void setConfigurator()
		{
			Configurator configurator = new Configurator();
			configurator.Init(false);
			Service.Set(configurator);
		}

		private IEnumerator setContentSystem()
		{
			ManifestService manifestService = new ManifestService();
			bool contentComplete = false;
			InitializeManifestDefinitionCommand command = new InitializeManifestDefinitionCommand(manifestService, delegate(ContentManifest manifest, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest, bool requiresAppUpgrade, bool appUpgradeAvailable)
			{
				Service.Set(new Content(manifest));
				contentComplete = true;
			});
			command.Execute();
			while (!contentComplete)
			{
				yield return null;
			}
		}

		private void destroyBakedTree()
		{
			if (RootNodeDefinitionContentKey != null && !string.IsNullOrEmpty(RootNodeDefinitionContentKey.Key) && DestroyOnAwake != null)
			{
				Object.Destroy(DestroyOnAwake);
			}
		}

		private void loadUIRoot()
		{
			if (RootNodeDefinitionContentKey != null && !string.IsNullOrEmpty(RootNodeDefinitionContentKey.Key))
			{
				GameObject gameObject = new GameObject("UIRoot");
				gameObject.SetActive(false);
				gameObject.AddComponent<TreeController>().RootNodeDefinitionContentKey = RootNodeDefinitionContentKey;
				gameObject.SetActive(true);
			}
		}
	}
}
