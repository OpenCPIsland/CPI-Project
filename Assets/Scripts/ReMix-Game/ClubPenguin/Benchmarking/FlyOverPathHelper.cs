using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class FlyOverPathHelper : MonoBehaviour
	{
		public GameObject NodeContainer;

		public void Awake()
		{
			if (NodeContainer == null)
			{
				NodeContainer = new GameObject("FlyOverPathContainer");
				NodeContainer.AddComponent<SmoothBezierCurve>();
			}
		}

		public void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.F))
			{
				CreateNode();
			}
		}

		private void CreateNode()
		{
			if (NodeContainer != null)
			{
				GameObject gameObject = new GameObject("Node" + NodeContainer.transform.childCount);
				SmoothBezierNode smoothBezierNode = gameObject.AddComponent<SmoothBezierNode>();
				smoothBezierNode.InHandleLength = 0f;
				smoothBezierNode.OutHandleLength = 0f;
				gameObject.transform.position = base.transform.position;
				gameObject.transform.parent = NodeContainer.transform;
			}
		}
	}
}
