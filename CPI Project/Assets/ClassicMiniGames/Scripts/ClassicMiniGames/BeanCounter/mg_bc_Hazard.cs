using MinigameFramework;

namespace BeanCounter
{
	public class mg_bc_Hazard : mg_bc_FlyingObject
	{
		public mg_bc_EHazardType HazardType
		{
			get;
			private set;
		}

		public override void Awake()
		{
			base.Awake();
			SetType(mg_bc_EHazardType.HAZARD_FISH);
		}

		internal void SetType(mg_bc_EHazardType _type)
		{
			HazardType = _type;
			m_animator.SetInteger("hazard_type", (int)_type);
		}

		public override void OnCaught()
		{
			base.OnCaught();
			string name = "";
			switch (HazardType)
			{
			case mg_bc_EHazardType.HAZARD_ANVIL:
				name = "mg_bc_sfx_AnvilImpactPlayer";
				break;
			case mg_bc_EHazardType.HAZARD_FISH:
				name = "mg_bc_sfx_FishImpactPlayer";
				break;
			case mg_bc_EHazardType.HAZARD_FLOWERS:
				name = "mg_bc_sfx_VaseImpactPlayer";
				break;
			}
			MinigameManager.GetActive().PlaySFX(name);
		}

		protected override void OnHitGround()
		{
			base.OnHitGround();
			string name = "";
			switch (HazardType)
			{
			case mg_bc_EHazardType.HAZARD_ANVIL:
				name = "mg_bc_sfx_AnvilImpactGround";
				break;
			case mg_bc_EHazardType.HAZARD_FISH:
				name = "mg_bc_sfx_FishImpactGround";
				break;
			case mg_bc_EHazardType.HAZARD_FLOWERS:
				name = "mg_bc_sfx_VaseImpactGround";
				break;
			}
			MinigameManager.GetActive().PlaySFX(name);
		}
	}
}
