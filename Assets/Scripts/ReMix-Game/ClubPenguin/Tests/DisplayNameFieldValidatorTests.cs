using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections;

namespace ClubPenguin.Tests
{
	public class DisplayNameFieldValidatorTests : FormFieldValidatorTests
	{
		protected override IEnumerator runTest()
		{
			yield return StartCoroutine(base.runTest());
			yield return StartCoroutine(login());
			yield return StartCoroutine(testStep("", true, "Empty Display Name"));
			yield return StartCoroutine(testStep("hCx", true, "Too Short Boundary (3 char long) Display Name"));
			yield return StartCoroutine(testStep("kEZl", false, "Too Short Boundary (4 char long) Display Name"));
			yield return StartCoroutine(testStep("gZMBD", false, "Too Short Boundary (5 char long) Display Name"));
			yield return StartCoroutine(testStep("K4fR0VfK4MToQ", false, "Too Long Boundary (13 char long) Display Name"));
			yield return StartCoroutine(testStep("T3uyzjtGIcoHGm", false, "Too Long Boundary (14 char long) Display Name"));
			yield return StartCoroutine(testStep("EhqzerN6iee3cfH", true, "Too Long Boundary (15 char long) Display Name"));
			yield return StartCoroutine(testStep("abcdefghij41", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("klmnopqrst32", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("uvwxyzABCD56", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("EFGHIJCLMN78", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("OPQRSTUVWX90", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("YZŒœßÀÁÂÃÄ 1", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("ÆÇÈÉÊ ËÌÍÎÏ2", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("ÑÒÓÔÕÖÙÚÛÜù3", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("Ýàáâãäæçèéõ4", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("úûüýîïñòóôö5", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("êëìí397", false, "CP Valid Char Check Display Name"));
			yield return StartCoroutine(testStep("АБВГДЕЖЗИЙ01", true, "CP InValid Char Check Display Name"));
			yield return StartCoroutine(testStep("КЛМНОПРСТУ23", true, "CP InValid Char Check Display Name"));
			yield return StartCoroutine(testStep("ФХЦЧШЩЪЫЬЭ45", true, "CP InValid Char Check Display Name"));
			yield return StartCoroutine(testStep("ЮЯабвгдежз67", true, "CP InValid Char Check Display Name"));
			yield return StartCoroutine(testStep("ийклмнопрс89", true, "CP InValid Char Check Display Name"));
			yield return StartCoroutine(testStep("туфхцч шщъыь", true, "CP InValid Char Check Display Name"));
			yield return StartCoroutine(testStep("эюяЁё635", true, "CP InValid Char Check Display Name"));
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
			yield return StartCoroutine(testStep("dtdevmouse", true, "Already Used Display Name"));
			yield return StartCoroutine(testStep("Dick", true, "Profane Display Name"));
		}

		protected IEnumerator login()
		{
			MixLoginCreateService mixLoginCreateService = Service.Get<MixLoginCreateService>();
			bool done = false;
			Action<ISession> onSuccess = null;
			Action<ILoginResult> onFailed = null;
			onSuccess = delegate(ISession s)
			{
				mixLoginCreateService.OnLoginSuccess -= onSuccess;
				mixLoginCreateService.OnLoginFailed -= onFailed;
				Service.Get<SessionManager>().AddMixSession(s);
				done = true;
			};
			onFailed = delegate(ILoginResult r)
			{
				mixLoginCreateService.OnLoginSuccess -= onSuccess;
				mixLoginCreateService.OnLoginFailed -= onFailed;
				IntegrationTest.Fail(string.Concat("Couldn't create a session [", r, "] "));
				done = true;
			};
			mixLoginCreateService.OnLoginSuccess += onSuccess;
			mixLoginCreateService.OnLoginFailed += onFailed;
			mixLoginCreateService.Login("dtdev1011a", "testing123");
			while (!done)
			{
				yield return null;
			}
		}
	}
}
