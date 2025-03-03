using System.Collections;

namespace ClubPenguin.Tests
{
	public class UsernameFieldValidatorTests : FormFieldValidatorTests
	{
		protected override IEnumerator runTest()
		{
			yield return StartCoroutine(base.runTest());
			yield return StartCoroutine(testStep("", true, "Empty Username"));
			yield return StartCoroutine(testStep("hCx", true, "Too Short Boundary (3 char long) Username"));
			yield return StartCoroutine(testStep("VBZk", false, "Too Short Boundary (4 char long) Username"));
			yield return StartCoroutine(testStep("9ViMl", false, "Too Short Boundary (5 char long) Username"));
			yield return StartCoroutine(testStep("K4fR0VfK4MToQslVupGkGvAKFqw3HBXOfkpXalYUX1Kv5kbKL08MNxk3W2gfjk0", false, "Too Long Boundary (63 char long) Username"));
			yield return StartCoroutine(testStep("T3uyzjtGIcoHGmNeW2sO3ikBN9rgiX3oNiMCnJ6Wc3CmqxCBxtNqMICsF4bvLDDE", false, "Too Long Boundary (64 char long) Username"));
			yield return StartCoroutine(testStep("EhqzerN6iee3cfH1AMrOvmCuUPHWwv3B69SbEMmqs3JHaC8CuYqgtmtxuD6Oyeeql", true, "Too Long Boundary (65 char long) Username"));
			yield return StartCoroutine(testStep("abcdefghij41", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("klmnopqrst32", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("uvwxyzABCD56", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("EFGHIJCLMN78", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("OPQRSTUVWX90", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("YZŒœßÀÁÂÃÄ1", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("ÆÇÈÉÊËÌÍÎÏ2", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("ÑÒÓÔÕÖÙÚÛÜù3", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("Ýàáâãäæçèéõ4", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("úûüýîïñòóôö5", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("êëìí397", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("АБВГДЕЖЗИЙ01", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("КЛМНОПРСТУ23", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("ФХЦЧШЩЪЫЬЭ45", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("ЮЯабвгдежз67", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("ийклмнопрс89", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("туфхцчшщъыь", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("эюяЁё635", false, "CP Valid Char Check Username"));
			yield return StartCoroutine(testStep("Danielle[", true, "Invalid Contains ["));
			yield return StartCoroutine(testStep("Dani]elle", true, "Invalid Contains ]"));
			yield return StartCoroutine(testStep("Danielle}", true, "Invalid Contains }"));
			yield return StartCoroutine(testStep("Dani{elle", true, "Invalid Contains {"));
			yield return StartCoroutine(testStep("Danielle|I", true, "Invalid |"));
			yield return StartCoroutine(testStep("Danielle&I", true, "Invalid &"));
			yield return StartCoroutine(testStep("Danielle%20", true, "Invalid %"));
			yield return StartCoroutine(testStep("Danielle<I", true, "Invalid <"));
			yield return StartCoroutine(testStep("Danielle>20", true, "Invalid >"));
			yield return StartCoroutine(testStep("<script>alert('Danielle');</script>", true, "Invalid XSS"));
			yield return StartCoroutine(testStep("dtdevmouse", true, "Already Used Username"));
		}
	}
}
