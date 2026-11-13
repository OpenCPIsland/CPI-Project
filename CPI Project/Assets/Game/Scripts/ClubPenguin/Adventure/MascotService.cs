using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class MascotService
	{
		public class MascotNameGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
				if (Service.IsSet<MascotService>())
				{
					foreach (Mascot mascot in Service.Get<MascotService>().Mascots)
					{
						list.Add(new NamedToggleValueAttribute.NamedToggleValue(mascot.Name, mascot.Name));
					}
				}
				return list;
			}
		}

		private Dictionary<string, Mascot> mascots = new Dictionary<string, Mascot>();

		private Dictionary<string, GameObject> mascotObjects = new Dictionary<string, GameObject>();

		public Mascot ActiveMascot
		{
			get;
			set;
		}

		public IEnumerable<Mascot> Mascots
		{
			get
			{
				return mascots.Values;
			}
		}

		public MascotService(Manifest manifest)
		{
			ScriptableObject[] assets = manifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				MascotDefinition mascotDefinition = (MascotDefinition)scriptableObject;
				mascots[mascotDefinition.name] = new Mascot(mascotDefinition);
			}
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionTerminatedEvent>(onSessionTerminated);
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionStart>(onTransitionStart);
		}

		public Mascot GetMascot(string id)
		{
			if (mascots.ContainsKey(id))
			{
				return mascots[id];
			}
			return null;
		}

		public GameObject GetMascotObject(string mascotName)
		{
			GameObject result = null;
			if (mascotObjects.ContainsKey(mascotName))
			{
				result = mascotObjects[mascotName];
			}
			return result;
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			clearMascotQuests();
			return false;
		}

		private bool onSessionTerminated(SessionEvents.SessionTerminatedEvent evt)
		{
			clearMascotQuests();
			return false;
		}

		private bool onTransitionStart(SceneTransitionEvents.TransitionStart evt)
		{
			ResetInteractionBehaviours();
			return false;
		}

		private void clearMascotQuests()
		{
			foreach (Mascot value in mascots.Values)
			{
				value.ClearQuests();
			}
		}

		public void ResetInteractionBehaviours()
		{
			foreach (Mascot value in mascots.Values)
			{
				value.InteractionBehaviours.Reset();
			}
		}

		public void RegisterMascotObject(string mascotName, GameObject mascotObject)
		{
			if (!mascotObjects.ContainsKey(mascotName))
			{
				mascotObjects.Add(mascotName, mascotObject);
			}
		}

		public void UnregisterMascotObject(string mascotName)
		{
			mascotObjects.Remove(mascotName);
		}
	}
}
