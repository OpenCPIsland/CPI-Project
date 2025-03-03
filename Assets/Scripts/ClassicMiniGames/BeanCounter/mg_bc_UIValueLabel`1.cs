using UnityEngine.UI;

namespace BeanCounter
{
	public class mg_bc_UIValueLabel<T> : mg_bc_UIValueDisplayer<T>
	{
		protected Text m_label;

		public override void Start()
		{
			m_label = GetComponent<Text>();
		}

		public override void SetValue(T _value)
		{
			m_label.text = string.Concat(_value);
		}
	}
}
