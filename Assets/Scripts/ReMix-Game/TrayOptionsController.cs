using UnityEngine;

public class TrayOptionsController : MonoBehaviour
{
	public GameObject[] Panels;

	private void Start()
	{
		if (Panels != null && Panels[0] != null)
		{
			ChangePanel(Panels[0]);
		}
	}

	public void ChangePanel(GameObject activePanel)
	{
		disableAllPanels();
		activePanel.SetActive(true);
	}

	private void disableAllPanels()
	{
		GameObject[] panels = Panels;
		foreach (GameObject gameObject in panels)
		{
			gameObject.SetActive(false);
		}
	}
}
