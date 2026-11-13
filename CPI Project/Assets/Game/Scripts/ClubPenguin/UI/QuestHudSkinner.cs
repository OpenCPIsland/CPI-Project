using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestHudSkinner : MonoBehaviour
	{
		public Image CommunicatorIcon;

		public Image CommunicatorBG;

		public Image CommunicatorBGShadow;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<QuestEvents.QuestStarted>(onQuestStarted);
			if (Service.Get<QuestService>().ActiveQuest != null)
			{
				skinForMascot();
			}
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<QuestEvents.QuestStarted>(onQuestStarted);
		}

		private bool onQuestStarted(QuestEvents.QuestStarted evt)
		{
			skinForMascot();
			return false;
		}

		private void skinForMascot()
		{
			string name = Service.Get<QuestService>().ActiveQuest.Mascot.Name;
			MascotDefinition definition = Service.Get<MascotService>().GetMascot(name).Definition;
			CoroutineRunner.Start(loadImage(definition.CommunicatorIconContentKey, CommunicatorIcon), this, "QuestHud.loadImage");
			CommunicatorBG.color = definition.CommunicatorBGColor;
			CommunicatorBGShadow.color = definition.CommunicatorBGShadowColor;
		}

		private IEnumerator loadImage(SpriteContentKey imageContentKey, Image image)
		{
			AssetRequest<Sprite> assetRequest = Content.LoadAsync(imageContentKey);
			yield return assetRequest;
			image.sprite = assetRequest.Asset;
		}
	}
}
