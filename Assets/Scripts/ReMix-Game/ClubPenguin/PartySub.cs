using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class PartySub : MonoBehaviour
	{
		public List<GameObject> DiscoFxs = new List<GameObject>();

		public List<GameObject> SceneObjs = new List<GameObject>();

		public GameObject InteractTrigger;

		public Animator DiscoBallAnim;

		public Animator HandleAnim;

		public float DiscoTimeout;

		private List<GameObject> DiscoFxInstances = new List<GameObject>();

		public void OnActionGraphActivation()
		{
			StartCoroutine(StartDiscoFX());
		}

		private IEnumerator StartDiscoFX()
		{
			InteractTrigger.SetActive(false);
			foreach (GameObject sceneObj in SceneObjs)
			{
				if (sceneObj != null)
				{
					sceneObj.SetActive(false);
				}
			}
			foreach (GameObject discoFx in DiscoFxs)
			{
				if (discoFx != null)
				{
					GameObject gameObject = Object.Instantiate(discoFx);
					gameObject.transform.SetParent(base.transform, false);
					DiscoFxInstances.Add(gameObject);
				}
			}
			DiscoBallAnim.SetTrigger("Drop");
			HandleAnim.SetTrigger("Pull");
			yield return new WaitForSeconds(DiscoTimeout);
			DiscoBallAnim.SetTrigger("Raise");
			yield return new WaitForSeconds(1.5f);
			foreach (GameObject sceneObj2 in SceneObjs)
			{
				sceneObj2.SetActive(true);
			}
			foreach (GameObject discoFxInstance in DiscoFxInstances)
			{
				Object.Destroy(discoFxInstance);
			}
			InteractTrigger.SetActive(true);
		}
	}
}
