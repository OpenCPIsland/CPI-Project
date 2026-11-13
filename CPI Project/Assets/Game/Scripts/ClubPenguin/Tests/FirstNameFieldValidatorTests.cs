using System.Collections;

namespace ClubPenguin.Tests
{
	public class FirstNameFieldValidatorTests : FormFieldValidatorTests
	{
		protected override IEnumerator runTest()
		{
			yield return StartCoroutine(base.runTest());
			yield return StartCoroutine(testStep("", true, "Empty First Name"));
			yield return StartCoroutine(testStep("Danielle[", true, "Invalid Contains ["));
			yield return StartCoroutine(testStep("Dani]elle", true, "Invalid Contains ]"));
			yield return StartCoroutine(testStep("Danielle}", true, "Invalid Contains }"));
			yield return StartCoroutine(testStep("Dani{elle", true, "Invalid Contains {"));
			yield return StartCoroutine(testStep("Danielle|I", true, "Invalid |"));
			yield return StartCoroutine(testStep("Danielle/20", true, "Invalid /"));
			yield return StartCoroutine(testStep("Danielle&I", true, "Invalid &"));
			yield return StartCoroutine(testStep("Danielle%20", true, "Invalid %"));
			yield return StartCoroutine(testStep("Danielle<I", true, "Invalid <"));
			yield return StartCoroutine(testStep("Danielle>20", true, "Invalid >"));
			yield return StartCoroutine(testStep("<script>alert('Danielle');</script>", true, "Invalid XSS"));
		}
	}
}
