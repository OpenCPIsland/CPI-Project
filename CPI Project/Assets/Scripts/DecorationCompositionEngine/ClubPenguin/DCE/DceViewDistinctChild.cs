using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.DCE
{
	[DisallowMultipleComponent]
	public class DceViewDistinctChild : DceBaseAsync
	{
		private Renderer rend;

		private DceLoadingService loadingService;

		private readonly ViewPart partView = new ViewPart();

		private DceLoadingService.Request<BasePartDefinition> eqRequest;

		private DceLoadingService.Request<Texture2D> decalRequest;

		public int SlotIndex
		{
			get;
			internal set;
		}

		public int PartIndex
		{
			get;
			internal set;
		}

		public DceModel Model
		{
			get;
			internal set;
		}

		public Rig Rig
		{
			get;
			internal set;
		}

		public Renderer Renderer
		{
			get
			{
				return rend;
			}
		}

		public ViewPart ViewPart
		{
			get
			{
				return partView;
			}
		}

		public void Awake()
		{
			loadingService = Service.Get<DceLoadingService>();
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
		}

		public void OnDestroy()
		{
			cleanup();
		}

		public void Apply(DceModel.Part newPart)
		{
			cleanup();
			startWork();
		}

		public void Update()
		{
			if (base.IsBusy && eqRequest.Finished && decalRequest.Finished)
			{
				setupRenderer();
				stopWork();
			}
		}

		private void setupRenderer()
		{
			partView.SetupRenderer(base.gameObject, Model, ref rend);
		}

		private void cleanup()
		{
			partView.CleanUp(base.gameObject);
			if (eqRequest != null)
			{
				loadingService.Unload(eqRequest);
				eqRequest = null;
			}
			if (decalRequest != null)
			{
				loadingService.Unload(decalRequest);
				decalRequest = null;
			}
		}
	}
}
