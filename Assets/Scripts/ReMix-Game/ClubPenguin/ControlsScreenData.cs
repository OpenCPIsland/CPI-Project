using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class ControlsScreenData : BaseData
	{
		public List<InputButtonContentKey> ButtonOverrides = new List<InputButtonContentKey>();

		private bool isControlsScreenActive;

		private PrefabContentKey leftOptionContentKey;

		private InputButtonGroupContentKey buttonGroupContentKey;

		public GameObject DefaultLeftOptionPrefab;

		public bool IsControlsScreenActive
		{
			get
			{
				return isControlsScreenActive;
			}
			set
			{
				if (this.OnControlsScreenActiveChanged != null)
				{
					this.OnControlsScreenActiveChanged(value);
				}
				isControlsScreenActive = value;
			}
		}

		public PrefabContentKey LeftOptionContentKey
		{
			get
			{
				if (leftOptionContentKey != null && !string.IsNullOrEmpty(leftOptionContentKey.Key))
				{
					return leftOptionContentKey;
				}
				return null;
			}
			private set
			{
				leftOptionContentKey = value;
			}
		}

		public InputButtonGroupContentKey ButtonGroupContentKey
		{
			get
			{
				if (buttonGroupContentKey != null && !string.IsNullOrEmpty(buttonGroupContentKey.Key))
				{
					return buttonGroupContentKey;
				}
				return null;
			}
			private set
			{
				buttonGroupContentKey = value;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ControlsScreenDataBehaviour);
			}
		}

		public event Action<bool> OnControlsScreenActiveChanged;

		public event Action<PrefabContentKey> OnSetLeftOption;

		public event Action OnReturnToDefaultLeftOption;

		public event Action<InputButtonGroupContentKey> OnSetRightOption;

		public event Action OnReturnToDefaultRightOption;

		public event Action<InputButtonContentKey, int> OnSetButton;

		public event Action<int> OnSetButtonToDefault;

		public void SetLeftOption(PrefabContentKey leftOptionContentKey)
		{
			LeftOptionContentKey = leftOptionContentKey;
			if (this.OnSetLeftOption != null)
			{
				this.OnSetLeftOption(leftOptionContentKey);
			}
		}

		public void ReturnToDefaultLeftOption()
		{
			LeftOptionContentKey = null;
			if (this.OnReturnToDefaultLeftOption != null)
			{
				this.OnReturnToDefaultLeftOption();
			}
		}

		public void SetRightOption(InputButtonGroupContentKey buttonGroupContentKey)
		{
			if (buttonGroupContentKey == null || string.IsNullOrEmpty(buttonGroupContentKey.Key))
			{
				throw new ArgumentNullException("buttonGroupContentKey", "This key cannot be null, and cannot have a null key string");
			}
			ButtonGroupContentKey = buttonGroupContentKey;
			ButtonOverrides.Clear();
			if (this.OnSetRightOption != null)
			{
				this.OnSetRightOption(buttonGroupContentKey);
			}
		}

		public void ReturnToDefaultRightOption()
		{
			ButtonGroupContentKey = null;
			ButtonOverrides.Clear();
			if (this.OnReturnToDefaultRightOption != null)
			{
				this.OnReturnToDefaultRightOption();
			}
		}

		public void SetButton(InputButtonContentKey buttonContentKey, int buttonIndex)
		{
			if (ButtonOverrides.Count > buttonIndex && ButtonOverrides[buttonIndex] != null && ButtonOverrides[buttonIndex].Key == buttonContentKey.Key)
			{
				if (this.OnSetButtonToDefault != null)
				{
					this.OnSetButtonToDefault(buttonIndex);
				}
				return;
			}
			int num = buttonIndex + 1;
			while (ButtonOverrides.Count < num)
			{
				ButtonOverrides.Add(null);
			}
			ButtonOverrides[buttonIndex] = buttonContentKey;
			if (this.OnSetButton != null)
			{
				this.OnSetButton(buttonContentKey, buttonIndex);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
			this.OnSetLeftOption = null;
			this.OnReturnToDefaultLeftOption = null;
			this.OnSetRightOption = null;
			this.OnReturnToDefaultRightOption = null;
			this.OnSetButton = null;
			this.OnSetButtonToDefault = null;
		}
	}
}
