using System.Collections;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(RunController))]
	public class PivotTestScript : MonoBehaviour
	{
		private RunController controller;

		public float MoveTime = 2f;

		public float StopTime = 0.5f;

		private Vector3 startPos;

		private Quaternion startRot;

		public void Awake()
		{
			controller = GetComponent<RunController>();
		}

		public void Start()
		{
			StartCoroutine(TestRoutine());
		}

		private IEnumerator TestRoutine()
		{
			startPos = base.transform.position;
			startRot = base.transform.rotation;
			Vector2 downleft = (Vector2.down + Vector2.left).normalized;
			Vector2 downright = (Vector2.down + Vector2.right).normalized;
			while (true)
			{
				controller.Steer(Vector2.up);
				yield return new WaitForSeconds(MoveTime);
				controller.Steer(downleft);
				yield return new WaitForSeconds(MoveTime);
				controller.Steer(Vector2.up);
				yield return new WaitForSeconds(MoveTime);
				controller.Steer(downright);
				yield return new WaitForSeconds(MoveTime);
				controller.Steer(Vector2.zero);
				yield return new WaitForSeconds(StopTime);
				base.transform.position = startPos;
				base.transform.rotation = startRot;
			}
		}
	}
}
