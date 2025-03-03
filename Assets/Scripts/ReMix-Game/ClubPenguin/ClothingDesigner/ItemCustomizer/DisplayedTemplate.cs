namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public struct DisplayedTemplate
	{
		public readonly TemplateDefinition Definition;

		public readonly int Level;

		public readonly string MascotName;

		public DisplayedTemplate(TemplateDefinition definition, int level, string mascotName)
		{
			Definition = definition;
			Level = level;
			MascotName = mascotName;
		}
	}
}
