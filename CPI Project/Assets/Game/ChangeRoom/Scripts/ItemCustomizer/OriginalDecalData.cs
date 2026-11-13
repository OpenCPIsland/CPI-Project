using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class OriginalDecalData : OriginalFabricData
	{
		public Vector2 ActualRedUVOffset;

		public Vector2 ActualGreenUVOffset;

		public Vector2 ActualBlueUVOffset;

		public Vector2 OriginalRedUVOffset;

		public Vector2 OriginalGreenUVOffset;

		public Vector2 OriginalBlueUVOffset;

		public Vector2 UpdatedUVOffset;

		public Renderer ActualRedRenderer;

		public Renderer ActualGreenRenderer;

		public Renderer ActualBlueRenderer;

		public Renderer OriginalRedRenderer;

		public Renderer OriginalGreenRenderer;

		public Renderer OriginalBlueRenderer;

		public Renderer UpdatedRenderer;

		public override void Clear()
		{
			base.Clear();
			ActualRedUVOffset = Vector2.zero;
			ActualGreenUVOffset = Vector2.zero;
			ActualBlueUVOffset = Vector2.zero;
			OriginalRedUVOffset = Vector2.zero;
			OriginalGreenUVOffset = Vector2.zero;
			OriginalBlueUVOffset = Vector2.zero;
			UpdatedUVOffset = Vector2.zero;
			ActualRedRenderer = null;
			ActualGreenRenderer = null;
			ActualBlueRenderer = null;
			OriginalRedRenderer = null;
			OriginalGreenRenderer = null;
			OriginalBlueRenderer = null;
			UpdatedRenderer = null;
		}
	}
}
