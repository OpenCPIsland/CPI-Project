namespace UnityEngine.UI.Extensions
{
	public class ExampleSelectable : MonoBehaviour, IBoxSelectable
	{
		private bool _selected = false;

		private bool _preSelected = false;

		private SpriteRenderer spriteRenderer;

		private Image image;

		private Text text;

		public bool selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
			}
		}

		public bool preSelected
		{
			get
			{
				return _preSelected;
			}
			set
			{
				_preSelected = value;
			}
		}

		private void Start()
		{
			spriteRenderer = base.transform.GetComponent<SpriteRenderer>();
			image = base.transform.GetComponent<Image>();
			text = base.transform.GetComponent<Text>();
		}

		private void Update()
		{
			Color color = Color.white;
			if (preSelected)
			{
				color = Color.yellow;
			}
			if (selected)
			{
				color = Color.green;
			}
			if ((bool)spriteRenderer)
			{
				spriteRenderer.color = color;
			}
			else if ((bool)text)
			{
				text.color = color;
			}
			else if ((bool)image)
			{
				image.color = color;
			}
			else if ((bool)GetComponent<Renderer>())
			{
				GetComponent<Renderer>().material.color = color;
			}
		}

		/*Transform IBoxSelectable.get_transform()
		{
			return base.transform;
		}*/
	}
}
