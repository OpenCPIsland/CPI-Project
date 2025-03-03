using ClubPenguin.Avatar;
using ClubPenguin.BlobShadows;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(BlobShadowCaster))]
	[RequireComponent(typeof(AvatarView))]
	public class PlayerMediator : MonoBehaviour
	{
		private AvatarView avatarView;

		private BlobShadowCaster blobShadowCaster;

		private void Awake()
		{
			avatarView = GetComponent<AvatarView>();
			blobShadowCaster = GetComponent<BlobShadowCaster>();
		}

		private void OnEnable()
		{
			avatarView.OnBusy += startCombining;
			avatarView.OnReady += lodMeshesReady;
		}

		private void OnDisable()
		{
			avatarView.OnBusy -= startCombining;
			avatarView.OnReady -= lodMeshesReady;
		}

		private void lodMeshesReady(AvatarBaseAsync obj)
		{
			blobShadowCaster.SetIsActiveIfVisible();
		}

		private void startCombining(AvatarBaseAsync obj)
		{
			blobShadowCaster.SetIsActive(false);
		}
	}
}
