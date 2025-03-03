using ClubPenguin.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(InputFieldValidator))]
	public class ValidationIconController : MonoBehaviour
	{
		[Header("Icon Style")]
		public GameObject Icon;

		public Sprite ErrorIcon;

		public Sprite ValidIcon;

		private InputFieldValidator validator;

		private Image iconImage;

		private void Awake()
		{
			validator = GetComponent<InputFieldValidator>();
			if (validator == null)
			{
				throw new Exception("ValidationInputController requires a Validator (InputFieldValidator) script on the same game object");
			}
		}

		private void Start()
		{
			iconImage = Icon.GetComponent<Image>();
			Icon.SetActive(false);
		}

		private void Update()
		{
			if (validator != null && validator.TextInput != null && Icon != null && validator.TextInput.isFocused && Icon.activeInHierarchy)
			{
				Icon.SetActive(false);
			}
		}

		public void SetError(string errorMessage)
		{
			iconImage.sprite = ErrorIcon;
			Icon.SetActive(true);
		}

		public void SetValid()
		{
			iconImage.sprite = ValidIcon;
			Icon.SetActive(true);
		}

		public void Reset()
		{
			Icon.SetActive(false);
		}
	}
}
