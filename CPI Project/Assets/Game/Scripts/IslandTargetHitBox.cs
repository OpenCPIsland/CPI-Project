#define UNITY_ASSERTIONS
using UnityEngine;
using UnityEngine.Assertions;

public class IslandTargetHitBox : MonoBehaviour
{
	private IslandTarget target;

	public void Awake()
	{
		target = base.transform.parent.GetComponentInChildren<IslandTarget>();
		Assert.IsNotNull(target, "The target component could not be found.");
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (target != null)
		{
			target.CollisionFromHitBox(collision);
		}
	}
}
