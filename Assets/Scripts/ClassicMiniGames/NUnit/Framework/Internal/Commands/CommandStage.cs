namespace NUnit.Framework.Internal.Commands
{
	public enum CommandStage
	{
		Default,
		PostSetUpPreTearDown,
		SetUpTearDown,
		PreSetUpPostTearDown,
		CreateThread,
		Repeat,
		CreateFixture,
		SetContext
	}
}
