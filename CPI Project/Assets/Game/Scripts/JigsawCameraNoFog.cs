using UnityEngine;

public class JigsawCameraNoFog : MonoBehaviour
{
	private bool originalFogState = false;

	private void OnPreRender()
	{
		originalFogState = RenderSettings.fog;
		RenderSettings.fog = false;
	}

	private void OnPostRender()
	{
		RenderSettings.fog = originalFogState;
	}
}
