using System;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class TileView : MonoBehaviour
	{
		public TileBackgroundView BackgroundPrefab;

		public TileUIView UIViewPrefab;

		private Color successColor = Color.green;

		private Color errorColor = Color.red;

		protected TileBackgroundView background;

		protected TileUIView ui;

		protected Vector2 scale;

		protected ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		public virtual Vector2 Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
				ui.GetComponent<RectTransform>().localScale = value;
				background.TileImage.GetComponent<RectTransform>().localScale = value;
			}
		}

		public Color TileColor
		{
			get
			{
				return background.TileImage.color;
			}
			set
			{
				background.TileImage.color = value;
			}
		}

		public float TileAlpha
		{
			get
			{
				return background.TileImage.color.a;
			}
			set
			{
				Color tileColor = TileColor;
				tileColor.a = value;
				TileColor = tileColor;
			}
		}

		public Text NameText
		{
			get
			{
				return ui.NameText;
			}
		}

		public string Name
		{
			get
			{
				return NameText.text;
			}
			set
			{
				NameText.text = value;
			}
		}

		public event Action<TileView> Tapped;

		public event Action<TileView> Selected;

		public event Action<TileView> Deselected;

		public event Action<TileView> LongPressed;

		public void Awake()
		{
			InstatiatePrefabs();
			ParentToThis(background.HitAreaImage);
			background.Tapped += OnTapped;
			background.Selected += OnSelected;
			background.Deselected += OnDeselected;
			background.LongPressed += OnLongPressed;
			OnAwake();
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnAwake()
		{
		}

		public void DestroySelf()
		{
			this.Tapped = null;
			this.Selected = null;
			this.Deselected = null;
			background.Tapped -= OnTapped;
			background.Selected -= OnSelected;
			background.Deselected -= OnDeselected;
			background.LongPressed -= OnLongPressed;
			OnDestroy();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void InstatiatePrefabs()
		{
			background = InstantiateTileComponent(BackgroundPrefab);
			ui = InstantiateTileComponent(UIViewPrefab);
		}

		private TComponent InstantiateTileComponent<TComponent>(TComponent prefab) where TComponent : Component
		{
			TComponent val = UnityEngine.Object.Instantiate(prefab);
			ParentToThis(val);
			return val;
		}

		private void ParentToThis(Component component)
		{
			component.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), false);
		}

		public virtual void OnTapped(TileBackgroundView defaultView)
		{
			if (this.Tapped != null)
			{
				this.Tapped(this);
			}
		}

		public virtual void OnSelected(TileBackgroundView defaultView)
		{
			if (this.Selected != null)
			{
				this.Selected(this);
			}
		}

		public virtual void OnDeselected(TileBackgroundView defaultView)
		{
			if (this.Deselected != null)
			{
				this.Deselected(this);
			}
		}

		public virtual void OnLongPressed(TileBackgroundView defaultView)
		{
			if (this.LongPressed != null)
			{
				this.LongPressed(this);
			}
		}

		public void ShowSuccess()
		{
			TileColor = successColor;
		}

		public void ShowError()
		{
			TileColor = errorColor;
		}
	}
}
