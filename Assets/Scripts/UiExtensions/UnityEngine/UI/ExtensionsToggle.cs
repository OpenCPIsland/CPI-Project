using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Extensions/Extensions Toggle", 31)]
	[RequireComponent(typeof(RectTransform))]
	public class ExtensionsToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement, IEventSystemHandler
	{
		public enum ToggleTransition
		{
			None,
			Fade
		}

		[Serializable]
		public class ToggleEvent : UnityEvent<bool>
		{
		}

		[Serializable]
		public class ToggleEventObject : UnityEvent<ExtensionsToggle>
		{
		}

		public string UniqueID;

		public ToggleTransition toggleTransition = ToggleTransition.Fade;

		public Graphic graphic;

		[SerializeField]
		private ExtensionsToggleGroup m_Group;

		[Tooltip("Use this event if you only need the bool state of the toggle that was changed")]
		public ToggleEvent onValueChanged = new ToggleEvent();

		[Tooltip("Use this event if you need access to the toggle that was changed")]
		public ToggleEventObject onToggleChanged = new ToggleEventObject();

		[FormerlySerializedAs("m_IsActive")]
		[Tooltip("Is the toggle currently on or off?")]
		[SerializeField]
		private bool m_IsOn;

		public ExtensionsToggleGroup Group
		{
			get
			{
				return m_Group;
			}
			set
			{
				m_Group = value;
				SetToggleGroup(m_Group, true);
				PlayEffect(true);
			}
		}

		public bool IsOn
		{
			get
			{
				return m_IsOn;
			}
			set
			{
				Set(value);
			}
		}

		protected ExtensionsToggle()
		{
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetToggleGroup(m_Group, false);
			PlayEffect(true);
		}

		protected override void OnDisable()
		{
			SetToggleGroup(null, false);
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			if (graphic != null)
			{
				Color color = graphic.canvasRenderer.GetColor();
				bool flag = !Mathf.Approximately(color.a, 0f);
				if (m_IsOn != flag)
				{
					m_IsOn = flag;
					Set(!flag);
				}
			}
			base.OnDidApplyAnimationProperties();
		}

		private void SetToggleGroup(ExtensionsToggleGroup newGroup, bool setMemberValue)
		{
			ExtensionsToggleGroup group = m_Group;
			if (m_Group != null)
			{
				m_Group.UnregisterToggle(this);
			}
			if (setMemberValue)
			{
				m_Group = newGroup;
			}
			if (m_Group != null && IsActive())
			{
				m_Group.RegisterToggle(this);
			}
			if (newGroup != null && newGroup != group && IsOn && IsActive())
			{
				m_Group.NotifyToggleOn(this);
			}
		}

		private void Set(bool value)
		{
			Set(value, true);
		}

		private void Set(bool value, bool sendCallback)
		{
			if (m_IsOn != value)
			{
				m_IsOn = value;
				if (m_Group != null && IsActive() && (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.AllowSwitchOff)))
				{
					m_IsOn = true;
					m_Group.NotifyToggleOn(this);
				}
				PlayEffect(toggleTransition == ToggleTransition.None);
				if (sendCallback)
				{
					onValueChanged.Invoke(m_IsOn);
					onToggleChanged.Invoke(this);
				}
			}
		}

		private void PlayEffect(bool instant)
		{
			if (!(graphic == null))
			{
				graphic.CrossFadeAlpha((!m_IsOn) ? 0f : 1f, (!instant) ? 0.1f : 0f, true);
			}
		}

		protected override void Start()
		{
			PlayEffect(true);
		}

		private void InternalToggle()
		{
			if (IsActive() && IsInteractable())
			{
				IsOn = !IsOn;
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				InternalToggle();
			}
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			InternalToggle();
		}

		/*Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}*/

		bool ICanvasElement.IsDestroyed()
		{
			return IsDestroyed();
		}
	}
}
