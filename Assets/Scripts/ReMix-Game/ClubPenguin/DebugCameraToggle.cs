using ClubPenguin.Cinematography;
using UnityEngine;

namespace ClubPenguin
{
	public class DebugCameraToggle : MonoBehaviour
	{
		public GameObject RailCamGroup;

		public GameObject ContextualCamGroup;

		private bool bIsRail = true;

		public void OnCameraSwap()
		{
			bIsRail = !bIsRail;
			RailCamGroup.SetActive(bIsRail);
			ContextualCamGroup.SetActive(!bIsRail);
			Director director = Object.FindObjectOfType<Director>();
			director.ResetCamera();
		}
	}
}
