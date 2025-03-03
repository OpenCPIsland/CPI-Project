using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI.Example
{
	public class PooledCellsScrollRect_Example : MonoBehaviour
	{
		public int NumberOfItems;

		public PooledCellsScrollRect ScrollRectPool;

		public GameObject ItemToScroll;

		private string[] itemResources;

		private void Start()
		{
			itemResources = new string[NumberOfItems];
			for (int i = 0; i < itemResources.Length; i++)
			{
				itemResources[i] = i.ToString();
			}
			ScrollRectPool.ObjectAdded += itemAdded;
			ScrollRectPool.Init(NumberOfItems, ItemToScroll);
		}

		private void itemAdded(RectTransform element, int index)
		{
			string text = itemResources[index];
			Text componentInChildren = element.GetComponentInChildren<Text>();
			componentInChildren.text = text;
		}
	}
}
