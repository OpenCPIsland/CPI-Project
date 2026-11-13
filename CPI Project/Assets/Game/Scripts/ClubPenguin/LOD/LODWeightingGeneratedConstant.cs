namespace ClubPenguin.LOD
{
	public class LODWeightingGeneratedConstant : LODWeightingRule
	{
		public LODWeightingGeneratedConstantData Data;

		protected override float UpdateWeighting()
		{
			return (request.LODGameObject != null) ? Data.Weighting : 0f;
		}
	}
}
