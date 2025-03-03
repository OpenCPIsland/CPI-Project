using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InWorldText : MonoBehaviour
	{
		public enum TextType
		{
			Text,
			TextMesh
		}

		private Text textComponent;

		private TextMesh textMeshComponent;

		private bool isInitialized = false;

		public TextType Type
		{
			get;
			private set;
		}

		public void SetText(string text)
		{
			if (!isInitialized)
			{
				getComponents();
				isInitialized = true;
			}
			if (Type == TextType.Text)
			{
				textComponent.text = text;
			}
			else
			{
				textMeshComponent.text = text;
			}
		}

		private void getComponents()
		{
			textComponent = GetComponent<Text>();
			if (textComponent != null)
			{
				Type = TextType.Text;
				return;
			}
			textMeshComponent = GetComponent<TextMesh>();
			if (textMeshComponent != null)
			{
				Type = TextType.TextMesh;
			}
			else
			{
				Log.LogError(this, "InWorldText component requires either a Text component or a TextMesh component");
			}
		}
	}
}
