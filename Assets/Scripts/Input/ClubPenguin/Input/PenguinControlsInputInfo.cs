namespace ClubPenguin.Input
{
	public class PenguinControlsInputInfo : InputInfo
	{
		public string Jump = string.Empty;

		public string Action1 = string.Empty;

		public string Action2 = string.Empty;

		public string Action3 = string.Empty;

		public string Cancel = string.Empty;

		public override void Populate(ControlScheme controlScheme)
		{
			Jump = getKeyCodeTranslation(controlScheme.Jump.PrimaryKey);
			Action1 = getKeyCodeTranslation(controlScheme.Action1.PrimaryKey);
			Action2 = getKeyCodeTranslation(controlScheme.Action2.PrimaryKey);
			Action3 = getKeyCodeTranslation(controlScheme.Action3.PrimaryKey);
			Cancel = getKeyCodeTranslation(controlScheme.Cancel.PrimaryKey);
		}
	}
}
