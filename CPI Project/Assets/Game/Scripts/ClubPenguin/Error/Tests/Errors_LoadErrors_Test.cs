using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Error.Tests
{
	public class Errors_LoadErrors_Test : MonoBehaviour
	{
		private void Awake()
		{
			Service.ResetAll();
			Service.Set(new EventDispatcher());
			Service.Set((JsonService)new LitJsonService());
			GameObject gameObject = GameObject.Find("CoroutineRunner");
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			gameObject = new GameObject("CoroutineRunner");
			gameObject.AddComponent<CoroutineRunner>();
			ContentManifest manifest = ContentManifestUtility.FromDefinitionFile("Configuration/embedded_content_manifest");
			Content instance = new Content(manifest);
			Service.Set(instance);
			ErrorsMap errorsMap = new ErrorsMap();
			errorsMap.LoadErrorJson("Errors/errors.json");
			Service.Set(errorsMap);
		}

		private void Start()
		{
			Service.Get<ErrorsMap>().LoadErrorJson("test_errors.json");
			ErrorsMap errorsMap = Service.Get<ErrorsMap>();
			errorsMap.EUpdateComplete = (System.Action)Delegate.Combine(errorsMap.EUpdateComplete, new System.Action(updateComplete));
		}

		private void updateComplete()
		{
			ErrorsMap errorsMap = Service.Get<ErrorsMap>();
			errorsMap.EUpdateComplete = (System.Action)Delegate.Remove(errorsMap.EUpdateComplete, new System.Action(updateComplete));
			IntegrationTestEx.FailIf(Service.Get<ErrorsMap>() == null);
			IntegrationTestEx.FailIf(!Service.Get<ErrorsMap>().IsValidError("100"));
			IntegrationTest.Pass();
		}
	}
}
