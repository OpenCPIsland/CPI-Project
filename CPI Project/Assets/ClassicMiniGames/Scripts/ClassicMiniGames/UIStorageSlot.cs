using UnityEngine;

[AddComponentMenu("NGUI/Examples/UI Storage Slot")]
public class UIStorageSlot : UIItemSlot
{
	public UIItemStorage storage;

	public int slot = 0;

	protected override InvGameItem observedItem
	{
		get
		{
			return (storage != null) ? storage.GetItem(slot) : null;
		}
	}

	protected override InvGameItem Replace(InvGameItem item)
	{
		return (storage != null) ? storage.Replace(slot, item) : item;
	}
}
