using ClubPenguin.Cinematography;
using Disney.Kelowna.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenContinuousLoop
{
	private Vector3 destinationPosition;

	private Vector3 originPosition;

	private ICoroutine TweenRoutine;

	private List<GameObject> objectPool;

	private GameObject particlePrefab;

	private float tweenTime;

	private float tweenInterval;

	private PrefabContentKey prefabContentKey;

	public TweenContinuousLoop(Vector3 originPosition, Vector3 destinationPosition, PrefabContentKey prefabContentKey, float tweenTime, float tweenInterval)
	{
		this.originPosition = originPosition;
		this.destinationPosition = destinationPosition;
		this.tweenInterval = tweenInterval;
		this.prefabContentKey = prefabContentKey;
		this.tweenTime = tweenTime;
		objectPool = new List<GameObject>();
		CoroutineRunner.Start(startContinuousTween(), this, "TweenCoinCommand.startContinuousTween");
	}

	public void StopTween()
	{
		CoroutineRunner.Start(stopTweenRoutine(), this, "TweenContinuousLoop.StopTweenRoutine");
	}

	private IEnumerator stopTweenRoutine()
	{
		TweenRoutine.Cancel();
		yield return new WaitForSeconds(tweenTime);
		for (int i = 0; i < objectPool.Count; i++)
		{
			UnityEngine.Object.Destroy(objectPool[i]);
		}
		objectPool.Clear();
	}

	private IEnumerator startContinuousTween()
	{
		yield return CoroutineRunner.Start(createParticlePrefab(prefabContentKey), this, "TweenCoinCommand.CreatePaticlePrefab");
		TweenRoutine = CoroutineRunner.Start(tweenObjectsToHUD(), this, "TweenCoinCommand.CreatePaticlePrefab");
	}

	private IEnumerator tweenObjectsToHUD()
	{
		YieldInstruction waitforInterval = new WaitForSeconds(tweenInterval);
		while (true)
		{
			GameObject particle = getParticleFromPool();
			particle.transform.position = originPosition;
			Tweenable tweenable = particle.GetComponent<Tweenable>();
			tweenable.TweenCompleteAction = (Action<GameObject>)Delegate.Combine(tweenable.TweenCompleteAction, (Action<GameObject>)delegate(GameObject go)
			{
				addParticleToPool(go);
			});
			tweenable.TweenPosition(destinationPosition, tweenTime);
			yield return waitforInterval;
		}
	}

	private GameObject getParticleFromPool()
	{
		GameObject gameObject = null;
		for (int i = 0; i < objectPool.Count; i++)
		{
			if (!objectPool[i].activeSelf)
			{
				gameObject = objectPool[i];
			}
		}
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(particlePrefab);
			CameraCullingMaskHelper.SetLayerIncludingChildren(gameObject.transform, "UI");
			objectPool.Add(gameObject);
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void addParticleToPool(GameObject particle)
	{
		particle.SetActive(false);
	}

	private IEnumerator createParticlePrefab(PrefabContentKey prefabContentKey)
	{
		AssetRequest<GameObject> assetRequest = Content.LoadAsync(prefabContentKey);
		yield return assetRequest;
		particlePrefab = assetRequest.Asset;
	}
}
