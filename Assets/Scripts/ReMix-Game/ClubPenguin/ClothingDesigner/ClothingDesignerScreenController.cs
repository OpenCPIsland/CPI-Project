using ClubPenguin.ClothingDesigner.ItemCustomizer;
using ClubPenguin.Tutorial;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	public class ClothingDesignerScreenController : MonoBehaviour
	{
		public PrefabContentKey AndroidPrefabContentKey;

		private Animator animator;

		public TutorialDefinitionKey Catalog1TutorialDefinition;

		private EventChannel customizationEventChannel;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			customizationEventChannel = new EventChannel(CustomizationContext.EventBus);
			customizationEventChannel.AddListener<CustomizerUIEvents.StartPurchaseMoment>(onStartPurchaseMoment);
			customizationEventChannel.AddListener<CustomizerUIEvents.EndPurchaseMoment>(onEndPurchaseMoment);
			if (Service.Get<CatalogServiceProxy>().GetActiveThemeScheduleId() > 0)
			{
				Service.Get<TutorialManager>().TryStartTutorial(Catalog1TutorialDefinition.Id);
			}
			Content.LoadAsync(onPrefabLoaded, AndroidPrefabContentKey);
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.transform.SetParent(base.transform);
		}

		private bool onStartPurchaseMoment(CustomizerUIEvents.StartPurchaseMoment evt)
		{
			animator.SetBool("IsOpen", false);
			return false;
		}

		private bool onEndPurchaseMoment(CustomizerUIEvents.EndPurchaseMoment evt)
		{
			animator.SetBool("IsOpen", true);
			return false;
		}

		private void OnDestroy()
		{
			if (customizationEventChannel != null)
			{
				customizationEventChannel.RemoveAllListeners();
			}
		}
	}
}
