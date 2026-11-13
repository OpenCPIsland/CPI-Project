namespace ClubPenguin.LOD
{
	public class LODWeighting : LODWeightingRule
	{
		public float ConstantWeighting;

		protected override float UpdateWeighting()
		{
			return ConstantWeighting;
		}
	}
}
