using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_VisibilityForwarder : MonoBehaviour
	{
		private mg_jr_IVisibilityReceiver m_receiver;

		private void Start()
		{
			Assert.NotNull(base.transform.parent, "Parent must not be null");
			m_receiver = (mg_jr_IVisibilityReceiver)base.transform.parent.GetComponent(typeof(mg_jr_IVisibilityReceiver));
			Assert.NotNull(m_receiver, "Receiver not found");
		}

		private void OnBecameVisible()
		{
			m_receiver.BecameVisible();
		}

		private void OnBecameInvisible()
		{
			m_receiver.BecameInvisible();
		}
	}
}
