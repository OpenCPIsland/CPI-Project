using System;
using UnityEngine.UI.Extensions.Tweens;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
	[AddComponentMenu("UI/Extensions/Accordion/Accordion Element")]
	public class AccordionElement : Toggle
	{
		[SerializeField]
		private float m_MinHeight = 18f;

		private Accordion m_Accordion;

		private RectTransform m_RectTransform;

		private LayoutElement m_LayoutElement;

		[NonSerialized]
		private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		protected AccordionElement()
		{
			if (m_FloatTweenRunner == null)
			{
				m_FloatTweenRunner = new TweenRunner<FloatTween>();
			}
			m_FloatTweenRunner.Init(this);
		}

		protected override void Awake()
		{
			base.Awake();
			base.transition = Transition.None;
			toggleTransition = ToggleTransition.None;
			m_Accordion = base.gameObject.GetComponentInParent<Accordion>();
			m_RectTransform = (base.transform as RectTransform);
			m_LayoutElement = base.gameObject.GetComponent<LayoutElement>();
			onValueChanged.AddListener(OnValueChanged);
		}

		public void OnValueChanged(bool state)
		{
			if (m_LayoutElement == null)
			{
				return;
			}
			switch ((m_Accordion != null) ? m_Accordion.transition : Accordion.Transition.Instant)
			{
			case Accordion.Transition.Instant:
				if (state)
				{
					m_LayoutElement.preferredHeight = -1f;
				}
				else
				{
					m_LayoutElement.preferredHeight = m_MinHeight;
				}
				break;
			case Accordion.Transition.Tween:
				if (state)
				{
					StartTween(m_MinHeight, GetExpandedHeight());
				}
				else
				{
					StartTween(m_RectTransform.rect.height, m_MinHeight);
				}
				break;
			}
		}

		protected float GetExpandedHeight()
		{
			if (m_LayoutElement == null)
			{
				return m_MinHeight;
			}
			float preferredHeight = m_LayoutElement.preferredHeight;
			m_LayoutElement.preferredHeight = -1f;
			float preferredHeight2 = LayoutUtility.GetPreferredHeight(m_RectTransform);
			m_LayoutElement.preferredHeight = preferredHeight;
			return preferredHeight2;
		}

		protected void StartTween(float startFloat, float targetFloat)
		{
			float duration = (!(m_Accordion != null)) ? 0.3f : m_Accordion.transitionDuration;
			FloatTween floatTween = default(FloatTween);
			floatTween.duration = duration;
			floatTween.startFloat = startFloat;
			floatTween.targetFloat = targetFloat;
			FloatTween info = floatTween;
			info.AddOnChangedCallback(SetHeight);
			info.ignoreTimeScale = true;
			m_FloatTweenRunner.StartTween(info);
		}

		protected void SetHeight(float height)
		{
			if (!(m_LayoutElement == null))
			{
				m_LayoutElement.preferredHeight = height;
			}
		}
	}
}
