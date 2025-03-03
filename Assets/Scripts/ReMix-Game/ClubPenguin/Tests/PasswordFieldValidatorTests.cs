using System.Collections;

namespace ClubPenguin.Tests
{
	public class PasswordFieldValidatorTests : FormFieldValidatorTests
	{
		protected override IEnumerator runTest()
		{
			yield return StartCoroutine(base.runTest());
			yield return StartCoroutine(testStep("", true, "Empty Password"));
			yield return StartCoroutine(testStep("hC5RS", true, "Too Short Boundary (5 char long) Password"));
			yield return StartCoroutine(testStep("VB7kEZ", false, "Too Short Boundary (6 char long) Password"));
			yield return StartCoroutine(testStep("9VigZMl", false, "Too Short Boundary (7 char long) Password"));
			yield return StartCoroutine(testStep("WzhKs5aQyWinszrtxEuGE3xa", false, "Too Long Boundary (24 char long) Password"));
			yield return StartCoroutine(testStep("ZiRs5plO0IuyNYvhxgJrOoYnb", false, "Too Long Boundary (25 char long) Password"));
			yield return StartCoroutine(testStep("BagF4FBSZSjsZeKBpetLpOj9Mc", true, "Too Long Boundary (26 char long)Password"));
			yield return StartCoroutine(testStep(" 9VigZM", true, "Space at begining of Password"));
			yield return StartCoroutine(testStep("9VigZM ", true, "Space at end of Password"));
			yield return StartCoroutine(testStep("aVigZM", true, "No Numbers or Special Chars Used Password"));
			yield return StartCoroutine(testStep("123#4%", true, "No Letters Used Password"));
			yield return StartCoroutine(testStep("password1", true, "Common Password"));
			yield return StartCoroutine(testStep("123qwe", true, "Common Password"));
			yield return StartCoroutine(testStep("123qwerasd", true, "Password cannot be phone number"));
		}
	}
}
