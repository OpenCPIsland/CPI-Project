using UnityEngine;

[AddComponentMenu("NGUI/Examples/UI Equipment Slot")]
public class UIEquipmentSlot : UIItemSlot
{
	public InvEquipment equipment;

	public InvBaseItem.Slot slot;

	protected override InvGameItem observedItem
	{
		get
		{
			return (equipment != null) ? equipment.GetItem(slot) : null;
		}
	}

	protected override InvGameItem Replace(InvGameItem item)
	{
		return (equipment != null) ? equipment.Replace(slot, item) : item;
	}
}
