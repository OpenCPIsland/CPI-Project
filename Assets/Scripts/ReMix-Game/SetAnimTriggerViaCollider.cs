using UnityEngine;

public class SetAnimTriggerViaCollider : MonoBehaviour
{
	public GameObject AnimatorOnThisObject;

	public bool FireOnEnter = true;

	public string TriggerToFireOnEnter = "";

	public bool FireOnExit = false;

	public string TriggerToFireOnExit = "";

	public string TagOnDesiredCollisionObject = "Player";

	private Animator Animator;

	private void Start()
	{
		Animator = AnimatorOnThisObject.GetComponent<Animator>();
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (Animator != null && FireOnEnter && collision.CompareTag(TagOnDesiredCollisionObject) && !string.IsNullOrEmpty(TriggerToFireOnEnter))
		{
			Animator.SetTrigger(TriggerToFireOnEnter);
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		if (Animator != null && FireOnExit && collision.CompareTag(TagOnDesiredCollisionObject) && !string.IsNullOrEmpty(TriggerToFireOnExit))
		{
			Animator.SetTrigger(TriggerToFireOnExit);
		}
	}
}
