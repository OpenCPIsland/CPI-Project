using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class FadeIn : MonoBehaviour
	{
		private const float FADE_DURATION = 0.8f;

		private readonly string ALPHA_SHADER_PROPERTY = "_Alpha";

		private List<Material> materials = new List<Material>();

		private float timeElapsed;

		private AnimatorCullingMode? previousCullMode;

		private int alphaPropId;

		public event Action ECompleted;

		public void Start()
		{
			timeElapsed = 0f;
			alphaPropId = Shader.PropertyToID(ALPHA_SHADER_PROPERTY);
			Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] array = componentsInChildren[i].materials;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].HasProperty(alphaPropId))
					{
						materials.Add(array[j]);
					}
				}
			}
		}

		public void Update()
		{
			if (materials.Count > 0)
			{
				if (!previousCullMode.HasValue)
				{
					Animator component = GetComponent<Animator>();
					previousCullMode = component.cullingMode;
					component.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				}
				float alpha = Mathf.Lerp(0f, 1f, timeElapsed / 0.8f);
				if (!setAlpha(alpha))
				{
				}
				if (timeElapsed >= 0.8f)
				{
					Animator component = GetComponent<Animator>();
					if (component != null)
					{
						component.cullingMode = previousCullMode.Value;
					}
					setAlpha(1f);
					onFinished();
				}
				timeElapsed += Time.deltaTime;
			}
			else
			{
				onFinished();
			}
		}

		private bool setAlpha(float alpha)
		{
			bool result = false;
			for (int i = 0; i < materials.Count; i++)
			{
				Material material = materials[i];
				if (material != null && material.HasProperty(alphaPropId))
				{
					material.SetFloat(alphaPropId, alpha);
					result = true;
				}
			}
			return result;
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
