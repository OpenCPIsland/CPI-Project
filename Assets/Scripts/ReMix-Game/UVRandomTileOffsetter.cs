using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

public class UVRandomTileOffsetter : MonoBehaviour
{
	public int TilesX = 1;

	public int TilesY = 1;

	[Range(0.001f, 100f)]
	public float SecondsPerChange = 1f;

	private Vector2 oldOffset;

	private float tilesXRecip;

	private float tilesYRecip;

	private static readonly string mainTex = "_MainTex";

	private ICoroutine runner;

	private void Awake()
	{
		tilesXRecip = 1f / (float)TilesX;
		tilesYRecip = 1f / (float)TilesY;
		SecondsPerChange = Mathf.Clamp(SecondsPerChange, 0.001f, 100f);
	}

	private void OnEnable()
	{
		runner = CoroutineRunner.Start(repeater(), this, "repeater");
	}

	private void OnDisable()
	{
		if (runner != null)
		{
			if (!runner.Disposed)
			{
				runner.Cancel();
			}
			runner = null;
		}
	}

	private IEnumerator repeater()
	{
		while (true)
		{
			moveUvs();
			yield return new WaitForSeconds(SecondsPerChange);
		}
	}

	private void moveUvs()
	{
		Vector2 vector = generateOffset();
		while (vector == oldOffset)
		{
			vector = generateOffset();
		}
		base.gameObject.GetComponent<Renderer>().material.SetTextureOffset(mainTex, vector);
		oldOffset = vector;
	}

	private Vector2 generateOffset()
	{
		float num = Random.Range(0, TilesX);
		float num2 = Random.Range(0, TilesY);
		float x = Mathf.Round(num * tilesXRecip * 100f) * 0.01f;
		float y = Mathf.Round(num2 * tilesYRecip * 100f) * 0.01f;
		return new Vector2(x, y);
	}
}
