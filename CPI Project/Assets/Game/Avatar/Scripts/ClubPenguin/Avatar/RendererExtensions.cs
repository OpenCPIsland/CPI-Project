using System.Diagnostics;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	public static class RendererExtensions
	{
		[Conditional("UNITY_EDITOR")]
		public static void RebindShaders(this Renderer renderer)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Shader shader = Shader.Find(sharedMaterials[i].shader.name);
				sharedMaterials[i].shader = shader;
			}
		}
	}
}
