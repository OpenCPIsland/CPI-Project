using MinigameFramework;
using UnityEngine;

namespace PuffleRoundup
{
	public class mg_pr_InputHandler : MonoBehaviour
	{
		public GameObject snowPuff;

		public Component[] m_puffles;

		public GameObject m_PuffleContainer;

		public mg_PuffleRoundup Minigame;

		private void Awake()
		{
			Minigame = MinigameManager.GetActive<mg_PuffleRoundup>();
		}

		private void Start()
		{
			m_PuffleContainer = Minigame.transform.Find("mg_pr_GameContainer/mg_pr_PuffleContainer").gameObject;
		}

		private void FixedUpdate()
		{
			if (!MinigameManager.IsPaused)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				vector.z = 0f;
				RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
				if (Input.GetMouseButtonDown(0) && (bool)hit && hit.transform.gameObject.GetComponent<Collider2D>().name == "mg_pr_PlayArea")
				{
					Object.Instantiate(snowPuff, vector, Quaternion.identity);
				}
				if (Input.GetMouseButton(0))
				{
					PuffleHandler(vector);
				}
			}
		}

		private void PuffleHandler(Vector3 myPosition)
		{
			m_puffles = m_PuffleContainer.GetComponentsInChildren<mg_pr_PuffleController>();
			Component[] puffles = m_puffles;
			for (int i = 0; i < puffles.Length; i++)
			{
				mg_pr_PuffleController mg_pr_PuffleController = (mg_pr_PuffleController)puffles[i];
				float num = Vector3.Distance(myPosition, mg_pr_PuffleController.transform.position);
				float range = mg_pr_PuffleController.gameObject.GetComponent<mg_pr_PuffleController>().m_range;
				bool escaped = mg_pr_PuffleController.gameObject.GetComponent<mg_pr_PuffleController>().m_escaped;
				if (num <= range && !escaped)
				{
					float speed = mg_pr_PuffleController.GetComponent<mg_pr_PuffleController>().m_speed;
					Vector3 b = (-(myPosition - mg_pr_PuffleController.transform.position)).normalized * speed * Time.deltaTime;
					Vector3 v = mg_pr_PuffleController.transform.position + b;
					mg_pr_PuffleController.GetComponent<Rigidbody2D>().MovePosition(v);
				}
			}
		}
	}
}
