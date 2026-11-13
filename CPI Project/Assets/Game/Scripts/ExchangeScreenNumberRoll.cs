using UnityEngine;

public class ExchangeScreenNumberRoll : MonoBehaviour
{
	public ExchangeScreenReel[] Reels;

	public float ReelDelay = 0.5f;

	public void StartSpin()
	{
		for (int i = 0; i < Reels.Length; i++)
		{
			Reels[i].StartSpin(ReelDelay * (float)i);
		}
	}

	public void StopSpin(int count)
	{
		if (count < 999)
		{
			int[] array = new int[3]
			{
				count / 100,
				count % 100 / 10,
				count % 10
			};
			for (int i = 0; i < Reels.Length; i++)
			{
				Reels[i].SetNumber(array[i]);
				Reels[i].StopSpin();
			}
		}
		else
		{
			for (int i = 0; i < Reels.Length; i++)
			{
				Reels[i].SetNumber(9);
				Reels[i].StopSpin();
				Reels[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, Random.Range(-50f, 50f));
			}
		}
	}
}
