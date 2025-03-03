using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Tests;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Tests
{
	public abstract class FormFieldValidatorTests : BaseLoginCreateIntegrationTest
	{
		public InputFieldValidator TestInputField;

		public GameObject ErrorObject;

		protected IEnumerator testStep(string testString, bool shouldSeeError, string message)
		{
			message = message + " (" + testString + ") Failed -- ";
			string messageFieldSuffix;
			string messageObjectSuffix;
			if (shouldSeeError)
			{
				messageFieldSuffix = "No error triggered";
				messageObjectSuffix = "No error message visible";
			}
			else
			{
				messageFieldSuffix = "Unexpected error triggered";
				messageObjectSuffix = "Unexpected error message visible ";
			}
			TestInputField.TextInput.text = testString;
			TestInputField.StartValidation();
			while (!TestInputField.IsValidationComplete)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(TestInputField.HasError != shouldSeeError, message + messageFieldSuffix);
			IntegrationTestEx.FailIf(ErrorObject.activeInHierarchy != shouldSeeError, message + messageObjectSuffix);
		}

		protected override void tearDown()
		{
		}
	}
}
