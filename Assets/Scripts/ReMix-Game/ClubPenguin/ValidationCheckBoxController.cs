using ClubPenguin.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(LegalCheckboxesValidator))]
	public class ValidationCheckBoxController : MonoBehaviour
	{
		[Header("Error/Description Boxes")]
		public GameObject ErrorBox;

		public Text errorText;

		private LegalCheckboxesValidator validator;

		private void Awake()
		{
			validator = GetComponent<LegalCheckboxesValidator>();
			if (validator == null)
			{
				throw new Exception("ValidationCheckBoxController requires a Validator  LegalCheckBoxesValidator) script on the same game object");
			}
		}

		public void SetError(string errorMessage)
		{
			errorText.text = errorMessage;
			ErrorBox.SetActive(true);
		}

		public void SetValid()
		{
			errorText.text = "";
			ErrorBox.SetActive(false);
		}
	}
}
