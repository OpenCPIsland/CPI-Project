using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Graphic))]
	public class MaterialSelector : MonoBehaviour
	{
		public Material[] Materials;

		private Graphic graphic;

		public void SelectMaterial(int index)
		{
			if (graphic == null)
			{
				graphic = GetComponent<Graphic>();
			}
			if (Materials[index] != null)
			{
				graphic.material = Materials[index];
			}
		}
	}
}
