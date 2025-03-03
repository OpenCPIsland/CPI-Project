using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	[RequireComponent(typeof(Image))]
	public class mg_jr_TintApplicatorUI : MonoBehaviour
	{
		private void Start()
		{
			Image component = GetComponent<Image>();
			component.color = MinigameManager.Instance.GetPenguinColor();
		}
	}
}
