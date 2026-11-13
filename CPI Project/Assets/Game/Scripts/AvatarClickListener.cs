using ClubPenguin;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarClickListener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private bool isLoading;

	public void OnPointerClick(PointerEventData eventData)
	{
		Click();
	}

	public void Click()
	{
		if (!isLoading)
		{
			isLoading = true;
			AvatarDataHandle component = base.gameObject.GetComponent<AvatarDataHandle>();
			if (component != null)
			{
				openPlayerCard(component.Handle);
			}
		}
	}

	private void openPlayerCard(DataEntityHandle handle)
	{
		OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(handle);
		openPlayerCardCommand.Execute();
		isLoading = false;
	}
}
