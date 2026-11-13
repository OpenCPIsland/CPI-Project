using System;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class DropDownListItem
	{
		[SerializeField]
		private string _caption;

		[SerializeField]
		private Sprite _image;

		[SerializeField]
		private bool _isDisabled;

		[SerializeField]
		private string _id;

		public Action OnSelect;

		internal Action OnUpdate;

		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption = value;
				if (OnUpdate != null)
				{
					OnUpdate();
				}
			}
		}

		public Sprite Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
				if (OnUpdate != null)
				{
					OnUpdate();
				}
			}
		}

		public bool IsDisabled
		{
			get
			{
				return _isDisabled;
			}
			set
			{
				_isDisabled = value;
				if (OnUpdate != null)
				{
					OnUpdate();
				}
			}
		}

		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public DropDownListItem(string caption = "", string inId = "", Sprite image = null, bool disabled = false, Action onSelect = null)
		{
			_caption = caption;
			_image = image;
			_id = inId;
			_isDisabled = disabled;
			OnSelect = onSelect;
		}
	}
}
