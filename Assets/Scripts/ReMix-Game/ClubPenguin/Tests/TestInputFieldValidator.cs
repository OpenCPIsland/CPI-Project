using ClubPenguin.UI;

namespace ClubPenguin.Tests
{
	public class TestInputFieldValidator : InputFieldValidator
	{
		public void StartTest(string testString)
		{
			TextInput.text = testString;
			StartValidation();
		}
	}
}
