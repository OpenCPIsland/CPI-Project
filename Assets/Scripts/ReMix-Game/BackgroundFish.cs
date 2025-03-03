using ClubPenguin;
using UnityEngine;

public class BackgroundFish : MonoBehaviour
{
	public Vector3 PlayerOffset = Vector3.zero;

	private GameObject penguinObj;

	private bool particleStarted = false;

	private ParticleSystem psFish;

	private void Awake()
	{
		psFish = base.gameObject.GetComponentInChildren<ParticleSystem>();
	}

	private void LateUpdate()
	{
		if (penguinObj != null)
		{
			if (!particleStarted)
			{
				if (psFish != null)
				{
					psFish.Play();
				}
				particleStarted = true;
			}
			Vector3 position = base.gameObject.transform.position;
			position.y = penguinObj.transform.position.y + PlayerOffset.y;
			position.x = penguinObj.transform.position.x + PlayerOffset.x;
			position.z = PlayerOffset.z;
			base.gameObject.transform.position = position;
		}
		else
		{
			penguinObj = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
		}
	}
}
