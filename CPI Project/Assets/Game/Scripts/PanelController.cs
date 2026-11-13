using UnityEngine;

public class PanelController : MonoBehaviour
{
	public GameObject StartPanel;

	public GameObject[] Panels;

	protected virtual void Awake()
	{
		ShowPanel(StartPanel);
	}

	public void ShowPanel(GameObject screen)
	{
		for (int i = 0; i < Panels.Length; i++)
		{
			Panels[i].SetActive(false);
		}
		screen.SetActive(true);
	}
}
