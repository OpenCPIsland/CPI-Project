using Tweaker.Core;
using UnityEngine;
using UnityEngine.AI;

namespace ClubPenguin.Tests
{
	public class BotManager : MonoBehaviour
	{
		public int BotCount = 0;

		public GameObject BotPrefab;

		public GameObject BotContainer;

		public float DestinationThreshold = 0.2f;

		private static BotManager instance;

		private float[] navWeights;

		private Mesh navMesh;

		private NavMeshAgent[] bots;

		public void Awake()
		{
			instance = this;
			NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();
			navMesh = new Mesh();
			navMesh.vertices = navMeshTriangulation.vertices;
			navMesh.triangles = navMeshTriangulation.indices;
			int num = navMeshTriangulation.indices.Length / 3;
			navWeights = new float[num];
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				Vector3 a = navMesh.vertices[navMesh.triangles[i * 3]];
				Vector3 vector = navMesh.vertices[navMesh.triangles[i * 3 + 1]];
				Vector3 b = navMesh.vertices[navMesh.triangles[i * 3 + 2]];
				float magnitude = (a - vector).magnitude;
				float magnitude2 = (a - b).magnitude;
				float magnitude3 = (vector - b).magnitude;
				float num3 = (magnitude + magnitude2 + magnitude3) * 0.5f;
				float num4 = Mathf.Sqrt(num3 * (num3 - magnitude) * (num3 - magnitude2) * (num3 - magnitude3));
				navWeights[i] = num4;
				num2 += num4;
			}
			for (int i = 0; i < num; i++)
			{
				navWeights[i] /= num2;
			}
		}

		public void Start()
		{
			ConfigureBots();
		}

		private void ConfigureBots()
		{
			int i;
			if (bots != null)
			{
				for (i = 0; i < bots.Length; i++)
				{
					GameObject gameObject = bots[i].gameObject;
					Object.Destroy(gameObject);
					bots[i] = null;
				}
				bots = null;
			}
			if (BotCount <= 0)
			{
				return;
			}
			bots = new NavMeshAgent[BotCount];
			i = 0;
			while (true)
			{
				if (i < bots.Length)
				{
					GameObject gameObject = Object.Instantiate(BotPrefab);
					gameObject.name = "Bot " + i;
					gameObject.transform.parent = BotContainer.transform;
					bots[i] = gameObject.GetComponent<NavMeshAgent>();
					bots[i].destination = GetRandomPosition();
					Vector3 randomPosition = GetRandomPosition();
					NavMeshHit hit;
					if (!NavMesh.SamplePosition(randomPosition, out hit, 2f, -1))
					{
						break;
					}
					bots[i].transform.position = hit.position;
					i++;
					continue;
				}
				return;
			}
			throw new UnityException("Could not find starting position for bot " + i);
		}

		public void Update()
		{
			if (bots == null)
			{
				return;
			}
			float num = DestinationThreshold * DestinationThreshold;
			for (int i = 0; i < bots.Length; i++)
			{
				if ((bots[i].destination - bots[i].transform.position).sqrMagnitude < num)
				{
					bots[i].destination = GetRandomPosition();
				}
			}
		}

		private Vector3 GetRandomPosition()
		{
			float value = Random.value;
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < navWeights.Length; i++)
			{
				num += navWeights[i];
				if (num >= value)
				{
					num2 = i;
					break;
				}
			}
			Vector3 vector;
			do
			{
				vector = new Vector3(Random.value, Random.value, Random.value);
			}
			while (vector.x + vector.y + vector.z == 0f);
			vector /= vector.x + vector.y + vector.z;
			Vector3 a = navMesh.vertices[navMesh.triangles[num2 * 3]];
			Vector3 a2 = navMesh.vertices[navMesh.triangles[num2 * 3 + 1]];
			Vector3 a3 = navMesh.vertices[navMesh.triangles[num2 * 3 + 2]];
			return a * vector.x + a2 * vector.y + a3 * vector.z;
		}

		[Invokable("Tests.SetBotCount")]
		public static void SetBotCount(int botCount)
		{
			instance.BotCount = botCount;
			instance.ConfigureBots();
		}
	}
}
