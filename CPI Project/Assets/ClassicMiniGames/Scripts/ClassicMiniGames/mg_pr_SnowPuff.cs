using UnityEngine;

public class mg_pr_SnowPuff : MonoBehaviour
{
	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}
