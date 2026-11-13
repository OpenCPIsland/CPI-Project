using UnityEngine;

public class ToggleFXOnClick : MonoBehaviour
{
	private ParticleSystem ps;

	private int toggle = 0;

	public void ActivateFXOnClick()
	{
		ps = base.gameObject.GetComponent<ParticleSystem>();
		ps.Stop();
		if (toggle == 0)
		{
			ps.Play();
			toggle = 1;
		}
		else if (toggle == 1)
		{
			ps.Stop();
			toggle = 0;
		}
	}
}
