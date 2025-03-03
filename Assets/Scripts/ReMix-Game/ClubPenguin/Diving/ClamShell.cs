using UnityEngine;

namespace ClubPenguin.Diving
{
	[RequireComponent(typeof(Animator))]
	public class ClamShell : MonoBehaviour
	{
		private const float CLOSING_DISTANCE = 2f;

		private static int HASH_PARAM_MAIN_DISTANCE = Animator.StringToHash("MainDistance");

		private static int HASH_PARAM_HAS_PEARL = Animator.StringToHash("HasPearl");

		private static int HASH_PARAM_ANIM_STATE = Animator.StringToHash("AnimationState");

		private static int HASH_ANIM_CLAM_CLOSE = Animator.StringToHash("Base Layer.ClamClose");

		private static int HASH_ANIM_CLAM_HIDE = Animator.StringToHash("Base Layer.ClamHide");

		private static int HASH_ANIM_CLAM_IDLE = Animator.StringToHash("Base Layer.ClamIdle");

		private static int HASH_ANIM_CLAM_OPEN = Animator.StringToHash("Base Layer.ClamOpen");

		public GameObject ParticlesAppear;

		private Animator anim;

		private GameObject penguinObj;

		private ClamAnimation oldClamAnimation = ClamAnimation.NONE;

		private float idleTimeout = 0f;

		private float idleMinTime = 3f;

		private float idleMaxTime = 5f;

		private Vector3 originalPos;

		private GameObject goPearl;

		private SphereCollider clamCollider;

		private float mainDistance;

		private bool hasPearl;

		private void Awake()
		{
			anim = GetComponent<Animator>();
			originalPos = base.gameObject.transform.position;
			goPearl = base.gameObject.transform.Find("PearlStraightener/pearl").gameObject;
			GameObject gameObject = base.gameObject.transform.Find("clamHinge").gameObject;
			clamCollider = gameObject.GetComponent<SphereCollider>();
			ResetClamShell();
			idleTimeout = Time.time + Random.Range(idleMinTime, idleMaxTime);
		}

		private void Start()
		{
			penguinObj = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
		}

		private void DISABLE_TODO_FixedUpdate()
		{
			if (Time.time >= idleTimeout)
			{
				idleTimeout = Time.time + Random.Range(idleMinTime, idleMaxTime);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!collider.gameObject.CompareTag("Player"))
			{
				return;
			}
			if (hasPearl)
			{
				clamCollider.enabled = false;
				if (goPearl != null)
				{
					goPearl.SetActive(true);
				}
			}
			else
			{
				clamCollider.enabled = true;
				if (goPearl != null)
				{
					goPearl.SetActive(false);
				}
			}
			mainDistance = calcDistance();
			anim.SetFloat(HASH_PARAM_MAIN_DISTANCE, mainDistance);
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!collider.gameObject.CompareTag("Player"))
			{
				return;
			}
			mainDistance = calcDistance();
			anim.SetFloat(HASH_PARAM_MAIN_DISTANCE, mainDistance);
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			float num = currentAnimatorStateInfo.normalizedTime - (float)Mathf.RoundToInt(currentAnimatorStateInfo.normalizedTime);
			if (currentAnimatorStateInfo.fullPathHash == HASH_ANIM_CLAM_CLOSE)
			{
				if (num > 0.2f)
				{
					clamCollider.enabled = true;
				}
			}
			else if (currentAnimatorStateInfo.fullPathHash == HASH_ANIM_CLAM_OPEN)
			{
				if (num < 0.7f)
				{
					clamCollider.enabled = true;
				}
			}
			else if (currentAnimatorStateInfo.fullPathHash == HASH_ANIM_CLAM_IDLE || currentAnimatorStateInfo.fullPathHash == HASH_ANIM_CLAM_HIDE)
			{
				clamCollider.enabled = true;
			}
			else
			{
				clamCollider.enabled = false;
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				anim.SetFloat(HASH_PARAM_MAIN_DISTANCE, 3f);
			}
		}

		public void OnAnimComplete(string animName)
		{
			if (animName != null && !(animName == "Idle") && !(animName == "Open") && !(animName == "IdleOpen") && !(animName == "Close") && animName == "Hide")
			{
			}
		}

		public void OnPickedUp()
		{
			HasBeenCollected();
		}

		public void HasBeenCollected()
		{
			setAnimation(ClamAnimation.ANIM_HIDE);
			hasPearl = false;
			anim.SetBool(HASH_PARAM_HAS_PEARL, hasPearl);
			clamCollider.enabled = true;
			goPearl.SetActive(false);
			anim.SetFloat(HASH_PARAM_MAIN_DISTANCE, 0f);
		}

		private float calcDistance()
		{
			return Vector2.Distance(base.gameObject.transform.position, penguinObj.transform.position);
		}

		private void setAnimation(ClamAnimation newAnim)
		{
			if (oldClamAnimation != newAnim)
			{
				oldClamAnimation = newAnim;
				anim.SetInteger(HASH_PARAM_ANIM_STATE, (int)newAnim);
			}
		}

		public void ResetClamShell()
		{
			if (ParticlesAppear != null)
			{
				Vector3 position = originalPos + new Vector3(0f, 0.3f, 0.5f);
				Object.Instantiate(ParticlesAppear, position, Quaternion.identity);
			}
			hasPearl = true;
			anim.SetBool(HASH_PARAM_HAS_PEARL, true);
			clamCollider.enabled = false;
			goPearl.SetActive(true);
			anim.SetFloat(HASH_PARAM_MAIN_DISTANCE, 3f);
			oldClamAnimation = ClamAnimation.NONE;
			setAnimation(ClamAnimation.ANIM_IDLE);
			anim.Play(HASH_ANIM_CLAM_IDLE);
		}
	}
}
