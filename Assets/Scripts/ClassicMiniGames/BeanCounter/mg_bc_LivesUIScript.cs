using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeanCounter
{
	public class mg_bc_LivesUIScript : mg_bc_UIValueDisplayer<int>
	{
		private GameObject m_firstLifeIcon;

		private List<GameObject> m_lifeIcons;

		public override void Start()
		{
			Transform transform = base.transform.Find("mg_bc_LifeSprite");
			m_firstLifeIcon = transform.gameObject;
			m_lifeIcons = new List<GameObject>();
			m_lifeIcons.Add(m_firstLifeIcon);
			MinigameManager.GetActive<mg_BeanCounter>().GameLogic.Penguin.Lives.SetDisplayer(this);
		}

		public override void SetValue(int _value)
		{
			while (_value > m_lifeIcons.Count)
			{
				GameObject gameObject = Object.Instantiate(m_firstLifeIcon);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localScale = m_firstLifeIcon.transform.localScale;
				gameObject.transform.localPosition = m_firstLifeIcon.transform.localPosition + new Vector3(40f * (float)m_lifeIcons.Count, 0f);
				m_lifeIcons.Add(gameObject);
			}
			int num = 0;
			foreach (GameObject lifeIcon in m_lifeIcons)
			{
				bool flag = num < _value;
				lifeIcon.GetComponent<Image>().enabled = (flag ? true : false);
				num++;
			}
		}
	}
}
