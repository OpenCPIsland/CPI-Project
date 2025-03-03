namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(ToggleGroup))]
	[AddComponentMenu("UI/Extensions/Accordion/Accordion Group")]
	public class Accordion : MonoBehaviour
	{
		public enum Transition
		{
			Instant,
			Tween
		}

		[SerializeField]
		private Transition m_Transition = Transition.Instant;

		[SerializeField]
		private float m_TransitionDuration = 0.3f;

		public Transition transition
		{
			get
			{
				return m_Transition;
			}
			set
			{
				m_Transition = value;
			}
		}

		public float transitionDuration
		{
			get
			{
				return m_TransitionDuration;
			}
			set
			{
				m_TransitionDuration = value;
			}
		}
	}
}
