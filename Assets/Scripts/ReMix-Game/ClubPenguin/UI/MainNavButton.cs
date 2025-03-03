using ClubPenguin.Core;
using ClubPenguin.Input;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ButtonClickListener))]
	public class MainNavButton : MonoBehaviour
	{
		private const string SCREEN_CONTAINER_CONTENT_NAME = "ScreenContainerContent";

		private const string SCREEN_CONTAINER_CONTENT_NONE_STATE = "none";

		public Sprite NormalSprite;

		public Sprite SelectedSprite;

		public Sprite DisabledSprite;

		public Sprite UnavailableSprite;

		public Image Image;

		public bool ChangeAlphaOnDisable = true;

		public bool IsSelectedAtStart;

		public string ScreenContainerContentState;

		protected MainNavButton[] mainNavButtons;

		protected MainNavButtonState state;

		protected ButtonClickListener buttonClickListener;

		private MainNavData mainNavData;

		private bool isSelected;

		public MainNavButtonState State
		{
			get
			{
				return state;
			}
			private set
			{
				state = value;
			}
		}

		public void Awake()
		{
			buttonClickListener = GetComponent<ButtonClickListener>();
			Transform transform = GetComponentInParent<MainNavStateHandler>().transform;
			mainNavButtons = transform.GetComponentsInChildren<MainNavButton>(true);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle entityByType = cPDataEntityCollection.GetEntityByType<MainNavData>();
			mainNavData = cPDataEntityCollection.GetComponent<MainNavData>(entityByType);
			mainNavData.OnCurrentStateChanged += onCurrentStateChanged;
		}

		public void Start()
		{
			if (isSelectedAtStart())
			{
				setState(MainNavButtonState.SELECTED);
			}
		}

		private void OnEnable()
		{
			buttonClickListener.OnClick.AddListener(OnClick);
		}

		private void OnDisable()
		{
			buttonClickListener.OnClick.RemoveListener(OnClick);
		}

		private bool isSelectedAtStart()
		{
			bool result = false;
			StateMachine component = GameObject.Find("ScreenContainerContent").GetComponent<StateMachine>();
			if (component != null && ScreenContainerContentState.Equals(component.CurrentState.Name))
			{
				result = true;
			}
			else if (component == null && IsSelectedAtStart)
			{
				result = true;
			}
			return result;
		}

		public virtual void setState(MainNavButtonState newState)
		{
			isSelected = false;
			switch (newState)
			{
			case MainNavButtonState.NORMAL:
				Image.sprite = NormalSprite;
				break;
			case MainNavButtonState.SELECTED:
			{
				isSelected = true;
				Image.sprite = SelectedSprite;
				for (int i = 0; i < mainNavButtons.Length; i++)
				{
					if (mainNavButtons[i] != this && mainNavButtons[i].State == MainNavButtonState.SELECTED)
					{
						if (!mainNavButtons[i].GetComponent<Button>().IsInteractable())
						{
							mainNavButtons[i].setState(MainNavButtonState.DISABLED);
						}
						else
						{
							mainNavButtons[i].setState(MainNavButtonState.NORMAL);
						}
					}
				}
				break;
			}
			case MainNavButtonState.DISABLED:
				Image.sprite = DisabledSprite;
				setInteractable(false);
				if (ChangeAlphaOnDisable)
				{
					Image.color = new Color(1f, 1f, 1f, 0.4f);
				}
				break;
			case MainNavButtonState.UNAVAILABLE:
				Image.sprite = UnavailableSprite;
				setInteractable(false);
				break;
			}
			if (ChangeAlphaOnDisable && newState != MainNavButtonState.DISABLED)
			{
				Image.color = Color.white;
			}
			state = newState;
		}

		public virtual void SetButtonEnabled(bool enabled, bool changeState = true)
		{
			if (!(buttonClickListener == null))
			{
				setInteractable(enabled);
				if (enabled && (state == MainNavButtonState.DISABLED || state == MainNavButtonState.UNAVAILABLE))
				{
					setState(MainNavButtonState.NORMAL);
				}
				else if (!enabled && changeState)
				{
					setState(MainNavButtonState.DISABLED);
				}
			}
		}

		private void onCurrentStateChanged(MainNavData.State state)
		{
			if (isSelected)
			{
				setState(MainNavButtonState.SELECTED);
			}
		}

		public virtual void OnClick(ButtonClickListener.ClickType clickType)
		{
			isSelected = true;
			if (base.enabled && mainNavData.CurrentState != 0)
			{
				setState(MainNavButtonState.SELECTED);
			}
		}

		private void setInteractable(bool isInteractable)
		{
			if (!(buttonClickListener == null) && buttonClickListener.Button.interactable != isInteractable)
			{
				buttonClickListener.Button.interactable = isInteractable;
				Button[] componentsInChildren = GetComponentsInChildren<Button>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].interactable = isInteractable;
				}
			}
		}

		private void OnDestroy()
		{
			if (mainNavData != null)
			{
				mainNavData.OnCurrentStateChanged -= onCurrentStateChanged;
			}
		}
	}
}
