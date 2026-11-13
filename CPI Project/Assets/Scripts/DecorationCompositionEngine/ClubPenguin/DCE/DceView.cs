using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.DCE
{
	[RequireComponent(typeof(Animator))]
	[DisallowMultipleComponent]
	public abstract class DceView : DceBaseAsync
	{
		public static readonly AssetContentKey DECAL_KEYPATTERN = new AssetContentKey("decals/*");

		public DceModel Model;

		protected DceLoadingService outfitService;

		public abstract Bounds GetBounds();

		public void Awake()
		{
			outfitService = Service.Get<DceLoadingService>();
			if (Model == null)
			{
				Model = GetComponent<DceModel>();
			}
			Animator component = GetComponent<Animator>();
			component.avatar = Model.Definition.UnityAvatar;
			onAwake();
		}

		public void OnDestroy()
		{
			cleanup();
		}

		protected abstract void onAwake();

		protected abstract void cleanup();
	}
}
