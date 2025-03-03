using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InputBarFieldLoader : MonoBehaviour
	{
		public Action<InputBarField> OnInputBarFieldLoaded;

		public Transform InputFieldContainer;

		private static PrefabContentKey standaloneContentKey = new PrefabContentKey("InputBarPrefabs/InputFieldStandalonePrefab");

		public void LoadInputBarField()
		{
			loadInputBarField(standaloneContentKey);
		}

		private void loadInputBarField(PrefabContentKey prefabContentKey)
		{
			Content.LoadAsync(onInputBarFieldLoaded, prefabContentKey);
		}

		private void onInputBarFieldLoaded(string path, GameObject inputBarFieldPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(inputBarFieldPrefab);
			if (gameObject != null)
			{
				InputBarField componentInChildren = gameObject.GetComponentInChildren<InputBarField>();
				gameObject.transform.SetParent(InputFieldContainer, false);
				if (OnInputBarFieldLoaded != null)
				{
					OnInputBarFieldLoaded(componentInChildren);
				}
				return;
			}
			throw new MissingReferenceException("The InputBarField prefab specified could not be found.");
		}

		public void OnDestroy()
		{
			OnInputBarFieldLoaded = null;
		}
	}
}
