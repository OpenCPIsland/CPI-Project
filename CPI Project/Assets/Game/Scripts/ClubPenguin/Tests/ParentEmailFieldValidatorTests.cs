using System.Collections;

namespace ClubPenguin.Tests
{
	public class ParentEmailFieldValidatorTests : FormFieldValidatorTests
	{
		protected override IEnumerator runTest()
		{
			yield return StartCoroutine(base.runTest());
			yield return testStep("", true, "Empty Parent Email");
			yield return testStep("@disney.com", true, "Invalid Parent Email");
			yield return testStep("email", true, "Invalid Parent Email");
			yield return testStep("@com", true, "Invalid Parent Email");
			yield return testStep("email.disney@com", true, "Invalid Parent Email");
			yield return testStep("email@disney.com", false, "Valid Parent Email");
		}
	}
}
