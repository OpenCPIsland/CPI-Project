using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Net;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using CpRemixShaders;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class RemotePlayerBot : MonoBehaviour
	{
		private EventDispatcher eventDispatcher;

		public long SessionId;

		private GameObjectReferenceData referenceData;

		private Coroutine randomChatterCoroutine;

		private static readonly string[] chatterMessages = new string[1]
		{
			"Hello"
		};

		public DataEntityHandle Handle
		{
			set
			{
				SessionId = Service.Get<CPDataEntityCollection>().GetComponent<SessionIdData>(value).SessionId;
				referenceData = Service.Get<CPDataEntityCollection>().GetComponent<GameObjectReferenceData>(value);
			}
		}

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
		}

		public IEnumerator Start()
		{
			SetLocoState(LocomotionState.Default);
			yield return checkPosition();
		}

		private IEnumerator checkPosition()
		{
			GameObject remotePlayer = null;
			while (remotePlayer == null)
			{
				yield return new WaitForSeconds(5f);
				remotePlayer = referenceData.GameObject;
			}
			LocomotionActionEvent movement = default(LocomotionActionEvent);
			movement.SessionId = SessionId;
			movement.Type = LocomotionAction.Move;
			movement.Position = remotePlayer.transform.position;
			movement.Direction = Vector3.zero;
			eventDispatcher.DispatchEvent(new PlayerActionServiceEvents.LocomotionActionReceived(movement.SessionId, movement));
		}

		public void Remove(float delay)
		{
			if (randomChatterCoroutine != null)
			{
				StopCoroutine(randomChatterCoroutine);
			}
			Invoke("removeDelay", delay);
		}

		private void removeDelay()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.PlayerLeaveRoomEvent(SessionId));
		}

		public void StartRandomChatter(float maxRandomStartDelay)
		{
			randomChatterCoroutine = StartCoroutine(updateRandomChatter(maxRandomStartDelay));
		}

		private IEnumerator updateRandomChatter(float maxRandomStartDelay)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(0f, maxRandomStartDelay));
			while (true)
			{
				EventDispatcher dispatcher = Service.Get<EventDispatcher>();
				dispatcher.DispatchEvent(new ChatServiceEvents.ChatActivityReceived(SessionId));
				yield return new WaitForSeconds(2f);
				dispatcher.DispatchEvent(new ChatServiceEvents.ChatMessageReceived(message: chatterMessages[UnityEngine.Random.Range(0, chatterMessages.Length)], sessionId: SessionId, sizzleClipID: 0));
				yield return new WaitForSeconds(6f);
			}
		}

		public void EquipConsumable(string type = "PartyBlaster")
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.ConsumableEquipped(SessionId, type));
		}

		public void UseConsumable()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ConsumableServiceEvents.ConsumableUsed(SessionId, SessionId.ToString()));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.HeldObjectDequipped(SessionId));
		}

		public void SetLocoState(LocomotionState state)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerLocoStateChanged(SessionId, state));
		}

		public void RandomizeColor()
		{
			Dictionary<int, AvatarColorDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, AvatarColorDefinition>>();
			Profile profile = new Profile();
			profile.colour = UnityEngine.Random.Range(1, dictionary.Count + 1);
			eventDispatcher.DispatchEvent(new PlayerStateServiceEvents.PlayerProfileChanged(SessionId, profile));
		}

		public void RandomizeClothing()
		{
			PlayerOutfitDetails outfit = default(PlayerOutfitDetails);
			outfit.sessionId = SessionId;
			outfit.parts = randomizeOutfitParts();
			eventDispatcher.DispatchEvent(new PlayerStateServiceEvents.PlayerOutfitChanged(outfit));
		}

		private CustomEquipment[] randomizeOutfitParts()
		{
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			Dictionary<int, FabricDefinition> definitions = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
			Dictionary<int, DecalDefinition> definitions2 = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
			AvatarService avatarService = Service.Get<AvatarService>();
			int num = avatarService.GetDefinitionByName("penguinAvatar").Slots.Length;
			int num2 = Enum.GetNames(typeof(DecalColorChannel)).Length / 2;
			int num3 = 3;
			CustomEquipment[] array = new CustomEquipment[num3];
			List<int> list = new List<int>();
			for (int i = 0; i < num3; i++)
			{
				CustomEquipment customEquipment = default(CustomEquipment);
				bool flag = false;
				int num4 = 0;
				while (!flag)
				{
					num4 = getRandomDefinitionId(dictionary);
					flag = (!dictionary[num4].WorkInProgress && !list.Contains(num4));
				}
				customEquipment.definitionId = num4;
				list.Add(num4);
				customEquipment.equipmentId = UnityEngine.Random.Range(0, int.MaxValue);
				CustomEquipmentCustomization[] array2 = new CustomEquipmentCustomization[num2 * 2];
				CustomEquipmentCustomization customEquipmentCustomization;
				for (int j = 0; j < num2; j++)
				{
					customEquipmentCustomization = default(CustomEquipmentCustomization);
					customEquipmentCustomization.definitionId = getRandomDefinitionId(definitions);
					customEquipmentCustomization.index = j;
					customEquipmentCustomization.repeat = true;
					customEquipmentCustomization.rotation = 0f;
					customEquipmentCustomization.scale = 1f;
					customEquipmentCustomization.type = EquipmentCustomizationType.FABRIC;
					customEquipmentCustomization.uoffset = 0f;
					customEquipmentCustomization.voffset = 0f;
					array2[j] = customEquipmentCustomization;
				}
				for (int j = 0; j < num2; j++)
				{
					customEquipmentCustomization = default(CustomEquipmentCustomization);
					customEquipmentCustomization.definitionId = getRandomDefinitionId(definitions2);
					customEquipmentCustomization.index = j + num2;
					customEquipmentCustomization.repeat = true;
					customEquipmentCustomization.rotation = 0f;
					customEquipmentCustomization.scale = 1f;
					customEquipmentCustomization.type = EquipmentCustomizationType.DECAL;
					customEquipmentCustomization.uoffset = 0f;
					customEquipmentCustomization.voffset = 0f;
					array2[j + num2] = customEquipmentCustomization;
				}
				customEquipment.parts = new CustomEquipmentPart[num];
				for (int k = 0; k < num; k++)
				{
					CustomEquipmentPart customEquipmentPart = default(CustomEquipmentPart);
					customEquipmentPart.slotIndex = k;
					customEquipmentPart.customizations = array2;
					customEquipment.parts[k] = customEquipmentPart;
				}
				array[i] = customEquipment;
			}
			return array;
		}

		public void RandomizePosition(Vector3 position, float radius)
		{
			LocomotionActionEvent action = default(LocomotionActionEvent);
			action.SessionId = SessionId;
			action.Type = LocomotionAction.Move;
			Vector3 position2 = UnityEngine.Random.insideUnitSphere * radius + position;
			position2.y = position.y;
			action.Position = position2;
			action.Direction = Vector3.zero;
			eventDispatcher.DispatchEvent(new PlayerActionServiceEvents.LocomotionActionReceived(action.SessionId, action));
		}

		private int getRandomDefinitionId<T>(Dictionary<int, T> definitions) where T : StaticGameDataDefinition
		{
			int num = -1;
			int count = definitions.Count;
			for (int i = 0; i < 10; i++)
			{
				num = UnityEngine.Random.Range(1, count + 1);
				if (definitions.ContainsKey(num))
				{
					break;
				}
			}
			if (!definitions.ContainsKey(num))
			{
				Dictionary<int, T>.Enumerator enumerator = definitions.GetEnumerator();
				enumerator.MoveNext();
				num = enumerator.Current.Key;
			}
			return num;
		}
	}
}
