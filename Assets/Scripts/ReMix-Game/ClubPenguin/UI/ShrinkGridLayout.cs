using ClubPenguin.ClothingDesigner.Inventory;
using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class ShrinkGridLayout : MonoBehaviour
	{
		public PooledCellsScrollRect pooledCellsScrollRect;

		private GridLayoutGroup gridLayoutGroup;

		private bool isShrunk;

		private Vector2 originalCellSize;

		private int originalConstraintCount;

		private Vector3 restoredPosition;

		private IEnumerator Start()
		{
			isShrunk = false;
			gridLayoutGroup = GetComponent<GridLayoutGroup>();
			originalCellSize = gridLayoutGroup.cellSize;
			originalConstraintCount = gridLayoutGroup.constraintCount;
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
			yield return new WaitForEndOfFrame();
			restoredPosition = base.transform.localPosition;
		}

		private void OnEnable()
		{
			InventoryContext.EventBus.AddListener<InventoryUIEvents.ShrinkClicked>(onShrinkButton);
		}

		private void OnDisable()
		{
			InventoryContext.EventBus.RemoveListener<InventoryUIEvents.ShrinkClicked>(onShrinkButton);
		}

		private bool onShrinkButton(InventoryUIEvents.ShrinkClicked evt)
		{
			if (isShrunk)
			{
				restoreGridElements();
			}
			else
			{
				shrinkGridElements();
			}
			if (pooledCellsScrollRect != null)
			{
				CoroutineRunner.Start(refreshLayoutThenReset(), this, "refreshLayoutTheReset");
			}
			return false;
		}

		private IEnumerator refreshLayoutThenReset()
		{
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
			yield return new WaitForEndOfFrame();
			pooledCellsScrollRect.ResetContent();
		}

		private void restoreGridElements()
		{
			gridLayoutGroup.cellSize = originalCellSize;
			gridLayoutGroup.constraintCount = originalConstraintCount;
			base.transform.localPosition = restoredPosition;
			isShrunk = false;
		}

		private void shrinkGridElements()
		{
			Vector2 cellSize = new Vector2(originalCellSize.x / 2f, originalCellSize.y / 2f);
			int constraintCount = originalConstraintCount + 1;
			gridLayoutGroup.cellSize = cellSize;
			gridLayoutGroup.constraintCount = constraintCount;
			base.transform.localPosition = restoredPosition;
			isShrunk = true;
		}
	}
}
