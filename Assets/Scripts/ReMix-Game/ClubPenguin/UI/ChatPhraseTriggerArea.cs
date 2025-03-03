using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ChatPhraseTriggerArea : MonoBehaviour
	{
		public ChatPhraseDefinitionList TokenDefinition;

		private PhraseChatData data;

		public void OnTriggerEnter(Collider collider)
		{
			if (collider.CompareTag("Player"))
			{
				getPhraseChatData().Push(TokenDefinition);
			}
		}

		public void OnTriggerExit(Collider collider)
		{
			if (collider.CompareTag("Player"))
			{
				getPhraseChatData().Pop(TokenDefinition);
			}
		}

		private PhraseChatData getPhraseChatData()
		{
			if (data == null)
			{
				DataEntityHandle handle;
				if (!Service.Get<CPDataEntityCollection>().ContainsEntityByName("PhraseChatData"))
				{
					handle = Service.Get<CPDataEntityCollection>().AddEntity("PhraseChatData");
					Service.Get<CPDataEntityCollection>().AddComponent<PhraseChatData>(handle);
				}
				else
				{
					handle = Service.Get<CPDataEntityCollection>().FindEntityByName("PhraseChatData");
				}
				data = Service.Get<CPDataEntityCollection>().GetComponent<PhraseChatData>(handle);
			}
			return data;
		}
	}
}
