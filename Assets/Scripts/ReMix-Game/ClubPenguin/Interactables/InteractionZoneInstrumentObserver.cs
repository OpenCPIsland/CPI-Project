using System;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace ClubPenguin.Interactables
{
	public class InteractionZoneInstrumentObserver : InteractionZoneObserver
	{
		[Serializable]
		public struct InteractionZoneInstrumentData
		{
			public string InstrumentName;

			public string[] MixFunctionNames;

			public InteractionZoneInstrumentData(string instrumentName, string[] mixFunctionNames)
			{
				InstrumentName = instrumentName;
				MixFunctionNames = mixFunctionNames;
			}
		}

		[Tooltip("The audio mixer for the instruments")]
		public AudioMixer MainMixer;

		[Tooltip("100% volume in dB")]
		public float MuteVolumedB = -80f;

		[Tooltip("Muted volume in dB")]
		public float FullVolumedB = 0f;

		public InteractionZoneInstrumentData[] InstrumentData;

		private readonly Dictionary<string, int> instrumentPlayCountDictionary = new Dictionary<string, int>();

		private static InteractionZoneInstrumentObserver badSingleton;

		public void Awake()
		{
			badSingleton = this;
		}

		public void OnValidate()
		{
		}

		[PublicTweak]
		[Invokable("InteractiveZones.Beach.Stage.SetGuitarCount", Description = "Directly set the number of guitars on stage")]
		public static void SetGuitarCount([ArgDescription("Number of guitars")] int count)
		{
			SetInstrumentCount("Guitar", count);
		}

		[Invokable("InteractiveZones.Beach.Stage.SetKeytarCount", Description = "Directly set the number of keytars on stage")]
		[PublicTweak]
		public static void SetKeytarCount([ArgDescription("Number of keytars")] int count)
		{
			SetInstrumentCount("Keytar", count);
		}

		[PublicTweak]
		[Invokable("InteractiveZones.Beach.Stage.SetDrumsCount", Description = "Directly set the number of drums on stage")]
		public static void SetDrumsCount([ArgDescription("Number of drums")] int count)
		{
			SetInstrumentCount("Drums", count);
		}

		[PublicTweak]
		[Invokable("InteractiveZones.Town.Park.SetDrumCount", Description = "Directly set the number of bongo drums")]
		public static void SetBongoDrumsCount([ArgDescription("Number of drums")] int count)
		{
			SetInstrumentCount("BongoDrums", count);
		}

		[Invokable("InteractiveZones.Town.Park.SetGuitarCount", Description = "Directly set the number of acooustic guitars")]
		[PublicTweak]
		public static void SetAcousticGuitarCount([ArgDescription("Number of guitars")] int count)
		{
			SetInstrumentCount("AcousticGuitar", count);
		}

		private static void SetInstrumentCount(string instrument, int count)
		{
			if (badSingleton != null)
			{
				badSingleton.instrumentPlayCountDictionary[instrument] = 0;
				badSingleton.StopLoop(instrument);
				for (int i = 0; i < count; i++)
				{
					badSingleton.StartLoop(instrument);
				}
				badSingleton.instrumentPlayCountDictionary[instrument] = count;
				badSingleton.dispatcher.DispatchEvent(new InteractionZoneEvents.IteractiveItemCountEvent(badSingleton.instrumentPlayCountDictionary));
			}
		}

		public new void Start()
		{
			base.Start();
			dispatcher.AddListener<InteractionZoneEvents.InteractivePropEquipEvent>(OnInteractiveItemLoop);
		}

		private bool OnInteractiveItemLoop(InteractionZoneEvents.InteractivePropEquipEvent evt)
		{
			EnsureTypeInDictionary(evt.ItemType);
			switch (evt.ItemAction)
			{
			case InteractiveItemAction.Equip:
				StartLoop(evt.ItemType);
				break;
			case InteractiveItemAction.UnEquip:
				StopLoop(evt.ItemType);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			dispatcher.DispatchEvent(new InteractionZoneEvents.IteractiveItemCountEvent(instrumentPlayCountDictionary));
			return false;
		}

		private void EnsureTypeInDictionary(string itemType)
		{
			if (!instrumentPlayCountDictionary.ContainsKey(itemType))
			{
				instrumentPlayCountDictionary[itemType] = 0;
			}
		}

		private void StartLoop(string itemType)
		{
			instrumentPlayCountDictionary[itemType]++;
			int count = instrumentPlayCountDictionary[itemType];
			for (int i = 0; i < InstrumentData.Length; i++)
			{
				if (InstrumentData[i].InstrumentName == itemType)
				{
					StartLoop(InstrumentData[i].MixFunctionNames, count);
				}
			}
		}

		private void StartLoop(string[] group, int count)
		{
			if (group != null)
			{
				for (int i = 0; i < count && group.Length != i; i++)
				{
					MainMixer.SetFloat(group[i], FullVolumedB);
				}
			}
		}

		private void StopLoop(string itemType)
		{
			int num = instrumentPlayCountDictionary[itemType];
			if (num <= 0)
			{
				num = 1;
			}
			for (int i = 0; i < InstrumentData.Length; i++)
			{
				if (InstrumentData[i].InstrumentName == itemType)
				{
					StopLoop(InstrumentData[i].MixFunctionNames, num);
				}
			}
			instrumentPlayCountDictionary[itemType] = num - 1;
		}

		private void StopLoop(string[] group, int count)
		{
			if (group != null && group.Length >= count)
			{
				int num = count - 1;
				for (int num2 = group.Length - 1; num2 >= num; num2--)
				{
					MainMixer.SetFloat(group[num2], MuteVolumedB);
				}
			}
		}

		protected override bool OnPlayerTriggerInteractionZone(InteractionZoneEvents.InteractionZoneEvent evt)
		{
			return false;
		}

		public new void OnDestroy()
		{
			base.OnDestroy();
			List<string> list = new List<string>();
			for (int i = 0; i < InstrumentData.Length; i++)
			{
				list.AddRange(InstrumentData[i].MixFunctionNames);
			}
			foreach (string item in list)
			{
				MainMixer.SetFloat(item, MuteVolumedB);
			}
			dispatcher.RemoveListener<InteractionZoneEvents.InteractivePropEquipEvent>(OnInteractiveItemLoop);
			instrumentPlayCountDictionary.Clear();
		}
	}
}
