using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Tubes
{
	public class TubeLoader : MonoBehaviour
	{
		private const float TUBE_SWAP_DELAY = 0.25f;

		public Transform TubeParent;

		private TubeData tubeData;

		private Dictionary<int, TubeDefinition> tubeDefinitions;

		private SlideController slideController;

		private ICoroutine loadTubeCoroutine;

		private AvatarDataHandle dataHandle;

		public void SetDataHandle(AvatarDataHandle dataHandle, SlideController slideController)
		{
			this.slideController = slideController;
			this.dataHandle = dataHandle;
			getTubeDefinitions();
			if (dataHandle != null && Service.Get<CPDataEntityCollection>().TryGetComponent(dataHandle.Handle, out tubeData))
			{
				tubeData.OnTubeSelected += onTubeSelected;
				onTubeSelected(tubeData.SelectedTubeId);
			}
		}

		private void getTubeDefinitions()
		{
			tubeDefinitions = Service.Get<GameData>().Get<Dictionary<int, TubeDefinition>>();
		}

		private void onTubeSelected(int tubeId)
		{
			slideController.DoAction(LocomotionController.LocomotionAction.Jump);
			if (loadTubeCoroutine != null && !loadTubeCoroutine.Cancelled && !loadTubeCoroutine.Completed && !loadTubeCoroutine.Disposed)
			{
				loadTubeCoroutine.Stop();
			}
			loadTubeCoroutine = CoroutineRunner.Start(loadTube(tubeId), this, "");
		}

		private IEnumerator loadTube(int tubeId)
		{
			TubeDefinition definition;
			if (tubeDefinitions.TryGetValue(tubeId, out definition))
			{
				AssetRequest<GameObject> request = Content.LoadAsync(definition.TubeAssetContentKey);
				yield return request;
				yield return new WaitForSeconds(0.25f);
				removeCurrentTube();
				GameObject tubeObject = Object.Instantiate(request.Asset, TubeParent, false);
				if (!dataHandle.IsLocalPlayer)
				{
					tubeObject.layer = LayerMask.NameToLayer(LayerConstants.RemotePlayer);
				}
			}
		}

		private void removeCurrentTube()
		{
			int childCount = TubeParent.childCount;
			for (int num = childCount - 1; num >= 0; num--)
			{
				Object.Destroy(TubeParent.GetChild(num).gameObject);
			}
		}

		private void OnDestroy()
		{
			if (tubeData != null)
			{
				tubeData.OnTubeSelected -= onTubeSelected;
			}
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
