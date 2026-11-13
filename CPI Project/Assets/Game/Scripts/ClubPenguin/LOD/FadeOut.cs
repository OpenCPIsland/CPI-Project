using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class FadeOut : MonoBehaviour
	{
		private const float FADE_DURATION = 0.8f;

		private List<Material> materials = new List<Material>();

		private float timeElapsed;

		private int alphaPropId;

		public event Action ECompleted;

		public void Start()
		{
			timeElapsed = 0f;
			alphaPropId = Shader.PropertyToID("_Alpha");
			SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				materials.AddRange(componentsInChildren[i].materials);
			}
		}

		public void Update()
		{
			if (materials.Count > 0)
			{
				float value = Mathf.Lerp(1f, 0f, timeElapsed / 0.8f);
				bool flag = false;
				for (int i = 0; i < materials.Count; i++)
				{
					if (materials[i] != null && materials[i].HasProperty(alphaPropId))
					{
						materials[i].SetFloat(alphaPropId, value);
						flag = true;
					}
				}
				if (!flag)
				{
				}
				if (timeElapsed >= 0.8f)
				{
					onFinished();
				}
				timeElapsed += Time.deltaTime;
			}
			else
			{
				onFinished();
			}
		}

		private void onFinished()
		{
			if (this.ECompleted != null)
			{
				this.ECompleted();
				this.ECompleted = null;
			}
			UnityEngine.Object.Destroy(this);
		}

		public void OnDestroy()
		{
			if (this.ECompleted != null)
			{
				this.ECompleted();
				this.ECompleted = null;
			}
		}
	}
}
