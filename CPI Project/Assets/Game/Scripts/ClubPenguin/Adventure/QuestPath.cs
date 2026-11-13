using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestPath : MonoBehaviour
	{
		public float NodeRadius = 1f;

		public float ForwardDistance = 1f;

		public float TrailingDistance = 1f;

		public GameObject ParticleEffectPrefab;

		public bool DEBUG_ShowAllEmitters = false;

		private bool DEBUG_isShowingEmitters = false;

		private int currentNode = 0;

		private QuestPathNode[] nodes;

		private QuestService questService;

		private void Awake()
		{
			nodes = GetComponentsInChildren<QuestPathNode>();
		}

		private void Start()
		{
			questService = Service.Get<QuestService>();
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i].ColliderRadius = NodeRadius;
				nodes[i].NodeIndex = i;
				if (i > 0)
				{
					nodes[i - 1].NextNode = nodes[i];
				}
			}
		}

		private void Update()
		{
			if (!Debug.isDebugBuild)
			{
				return;
			}
			if (DEBUG_ShowAllEmitters && !DEBUG_isShowingEmitters)
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					nodes[i].EnableParticles(ParticleEffectPrefab);
				}
				DEBUG_isShowingEmitters = true;
			}
			else if (!DEBUG_ShowAllEmitters && DEBUG_isShowingEmitters)
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					nodes[i].DisableParticles();
				}
				DEBUG_isShowingEmitters = false;
			}
		}

		private void OnDrawGizmos()
		{
			QuestPathNode[] componentsInChildren = GetComponentsInChildren<QuestPathNode>();
			if (componentsInChildren.Length != 0)
			{
				renameAllNodes();
			}
		}

		public GameObject GetCurrentNode()
		{
			return nodes[currentNode].gameObject;
		}

		public GameObject GetLastActiveNode()
		{
			int num = 0;
			bool flag = false;
			for (int i = 0; i < nodes.Length && (!flag || nodes[i].IsNodeActive); i++)
			{
				if (nodes[i].IsNodeActive)
				{
					flag = true;
					num = i;
				}
			}
			return nodes[num].gameObject;
		}

		public void StartPath()
		{
			changeNode(0);
		}

		public void StopPath()
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i].IsNodeActive = false;
				nodes[i].DisableParticles();
			}
			currentNode = 0;
		}

		public void OnNodeTriggered(int nodeIndex)
		{
			if (nodeIndex == currentNode)
			{
				sendTriggeredEvent(nodeIndex);
			}
			changeNode(nodeIndex + 1);
		}

		private void renameAllNodes()
		{
			QuestPathNode[] componentsInChildren = GetComponentsInChildren<QuestPathNode>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				string text = "Node " + i;
				if (!componentsInChildren[i].name.Equals(text))
				{
					componentsInChildren[i].name = text;
				}
			}
		}

		private void changeNode(int newNode)
		{
			if (newNode > currentNode + 1)
			{
				int num = newNode - currentNode;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					num2 = currentNode + i;
					nodes[num2].IsNodeActive = false;
					sendTriggeredEvent(num2);
				}
			}
			else
			{
				nodes[currentNode].IsNodeActive = false;
			}
			currentNode = newNode;
			if (currentNode >= nodes.Length)
			{
				completePath();
				return;
			}
			int pathNodeCountWithinDistance = GetPathNodeCountWithinDistance(currentNode, ForwardDistance, true);
			for (int i = 0; i <= pathNodeCountWithinDistance; i++)
			{
				nodes[currentNode + i].IsNodeActive = true;
			}
			updateNodeEffects();
		}

		private void completePath()
		{
			StopPath();
			questService.SendEvent("PathComplete");
		}

		private void updateNodeEffects()
		{
			int pathNodeCountWithinDistance = GetPathNodeCountWithinDistance(currentNode, ForwardDistance, true);
			for (int i = 0; i < pathNodeCountWithinDistance; i++)
			{
				nodes[currentNode + i].EnableParticles(ParticleEffectPrefab);
			}
			int pathNodeCountWithinDistance2 = GetPathNodeCountWithinDistance(currentNode, TrailingDistance, false);
			for (int num = currentNode - 1; num >= 0; num--)
			{
				if (num >= currentNode - pathNodeCountWithinDistance2)
				{
					nodes[num].EnableParticles(ParticleEffectPrefab);
				}
				else
				{
					nodes[num].DisableParticles();
				}
			}
		}

		private int GetPathNodeCountWithinDistance(int startIndex, float distance, bool forward)
		{
			int num = 0;
			float num2 = 0f;
			int num3 = startIndex;
			if (forward)
			{
				if (startIndex < nodes.Length)
				{
					for (num3 = startIndex; num3 < nodes.Length - 1; num3++)
					{
						QuestPathNode questPathNode = nodes[num3];
						num2 += questPathNode.DistanceToNextNode;
						if (num2 < distance)
						{
							num++;
							continue;
						}
						break;
					}
				}
			}
			else if (startIndex > 0)
			{
				num3 = startIndex - 1;
				num2 = 0f;
				while (num3 >= 0)
				{
					QuestPathNode questPathNode = nodes[num3];
					num2 += questPathNode.DistanceToNextNode;
					if (num2 < distance)
					{
						num++;
						num3--;
						continue;
					}
					break;
				}
			}
			return num;
		}

		private void sendTriggeredEvent(int nodeIndex)
		{
			string eventName = string.Format("PathNode{0}Triggered", nodeIndex);
			questService.SendEvent(eventName);
		}

		public void OnValidate()
		{
			NodeRadius = Mathf.Max(0f, NodeRadius);
			ForwardDistance = Mathf.Max(0f, ForwardDistance);
			TrailingDistance = Mathf.Max(0f, TrailingDistance);
		}
	}
}
