using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public class RemotePlayerBotGenerator : MonoBehaviour
	{
		public bool SpawnOnAwake;

		public bool AutoRemove;

		[Range(0f, 10f)]
		public float SpawnRadius = 1f;

		public int BotsToSpawn;

		private int botsLeftToSpawn;

		public int SpawnDelay;

		private int spawnDelayCounter;

		public long LastSessionID = 1000L;

		public string BotName = "RemotePlayerBot";

		public bool FollowPlayer = false;

		private EventDispatcher eventDispatcher;

		private static RemotePlayerBotGenerator generator;

		public int BotsLeftToSpawn
		{
			get
			{
				return botsLeftToSpawn;
			}
		}

		public int SpawnDelayCounter
		{
			get
			{
				return spawnDelayCounter;
			}
		}

		public void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			if (SpawnOnAwake && SpawnDelay == 0)
			{
				spawnImmediate();
			}
			else
			{
				base.enabled = SpawnOnAwake;
			}
		}

		public void OnEnable()
		{
			eventDispatcher.AddListener<NetworkControllerEvents.RemotePlayerJoinedRoomEvent>(onRemotePlayerJoinedRoom, EventDispatcher.Priority.LAST);
			botsLeftToSpawn = BotsToSpawn;
			spawnDelayCounter = SpawnDelay;
		}

		public void Update()
		{
			base.enabled = trySpawn();
		}

		private bool trySpawn()
		{
			bool result = false;
			if (botsLeftToSpawn > 0)
			{
				spawnDelayCounter--;
				if (spawnDelayCounter <= 0)
				{
					checkPosition();
					spawn();
					spawnDelayCounter = SpawnDelay;
					result = true;
					if (spawnDelayCounter <= 0)
					{
						result = trySpawn();
					}
				}
			}
			return result;
		}

		private void spawnImmediate()
		{
			OnEnable();
			while (botsLeftToSpawn > 0)
			{
				spawn();
			}
			OnDisable();
			base.enabled = false;
		}

		private void checkPosition()
		{
			if (FollowPlayer)
			{
				base.transform.position = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.transform.position;
			}
		}

		private void spawn()
		{
			LastSessionID++;
			string name = BotName + LastSessionID;
			eventDispatcher.DispatchEvent(new WorldServiceEvents.PlayerJoinRoomEvent(LastSessionID, name));
			botsLeftToSpawn--;
		}

		public void OnDisable()
		{
			if (eventDispatcher != null)
			{
				eventDispatcher.RemoveListener<NetworkControllerEvents.RemotePlayerJoinedRoomEvent>(onRemotePlayerJoinedRoom);
			}
		}

		private bool onRemotePlayerJoinedRoom(NetworkControllerEvents.RemotePlayerJoinedRoomEvent evt)
		{
			DisplayNameData component;
			if (Service.Get<CPDataEntityCollection>().TryGetComponent(evt.Handle, out component) && component.DisplayName.StartsWith(BotName))
			{
				Transform botContainer = RemotePlayerBotUtil.GetBotContainer();
				GameObject gameObject = new GameObject(component.DisplayName);
				gameObject.transform.SetParent(botContainer);
				RemotePlayerBot remotePlayerBot = gameObject.AddComponent<RemotePlayerBot>();
				remotePlayerBot.Handle = evt.Handle;
				remotePlayerBot.RandomizeClothing();
				remotePlayerBot.RandomizeColor();
				remotePlayerBot.RandomizePosition(base.transform.position, SpawnRadius);
				if (AutoRemove)
				{
					remotePlayerBot.Remove(UnityEngine.Random.Range(0.2f, 10f));
				}
			}
			return false;
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			float f = 0f;
			float x = SpawnRadius * Mathf.Cos(f);
			float z = SpawnRadius * Mathf.Sin(f);
			Vector3 vector = base.transform.position + new Vector3(x, 0f, z);
			Vector3 vector2 = vector;
			Vector3 to = vector;
			for (f = 0.1f; f < (float)Math.PI * 2f; f += 0.1f)
			{
				x = SpawnRadius * Mathf.Cos(f);
				z = SpawnRadius * Mathf.Sin(f);
				vector2 = base.transform.position + new Vector3(x, 0f, z);
				Gizmos.DrawLine(vector, vector2);
				vector = vector2;
			}
			Gizmos.DrawLine(vector, to);
		}

		private static void initialize()
		{
			if (generator == null)
			{
				GameObject gameObject = new GameObject("RemoteBotGenerator");
				generator = gameObject.AddComponent<RemotePlayerBotGenerator>();
			}
			generator.transform.position = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.transform.position;
		}

		[Invokable("Bots.Generate", Description = "Generate remote player bots around the local player")]
		private static void generateBots([ArgDescription("The number of remote bots to spawn.")] int number = 1, [ArgDescription("The radius around the local player the penguins will spawn at the time of invoking.")] float radius = 5f, [ArgDescription("The number of frames delay before each remote penguin spawns")] int delay = 0)
		{
			initialize();
			generator.FollowPlayer = true;
			generator.BotsToSpawn = number;
			generator.SpawnRadius = radius;
			generator.SpawnDelay = delay;
			generator.enabled = true;
		}
	}
}
