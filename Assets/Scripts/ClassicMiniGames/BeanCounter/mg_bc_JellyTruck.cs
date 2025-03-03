using MinigameFramework;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_JellyTruck : mg_bc_Truck
	{
		private const float WRONG_BAG_CHANCE = 0.3f;

		public TextAsset NormalXml;

		public TextAsset HardXml;

		public TextAsset ExtremeXml;

		protected mg_bc_Penguin m_penguin;

		private List<SpriteRenderer> m_jellyTints;

		private List<mg_bc_Bag> m_bagsToThrowBack;

		private float m_lastBagThrownBack;

		private List<mg_bc_EJellyColors> m_targetColors;

		private List<mg_bc_EJellyColors> m_unwantedColors;

		private GameObject m_unloadArrow;

		public mg_bc_CandyMachine CandyMachine
		{
			get;
			set;
		}

		public override void Awake()
		{
			m_bagsToThrowBack = new List<mg_bc_Bag>();
			m_jellyTints = new List<SpriteRenderer>();
			m_targetColors = new List<mg_bc_EJellyColors>();
			m_unwantedColors = new List<mg_bc_EJellyColors>();
			m_unloadArrow = base.transform.Find("mg_bc_truck_arrow").gameObject;
			m_unloadArrow.SetActive(false);
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				if (spriteRenderer.gameObject.name == "tint")
				{
					m_jellyTints.Add(spriteRenderer);
				}
			}
			base.Awake();
		}

		public override bool NextLevel()
		{
			bool flag = base.NextLevel();
			if (flag)
			{
				PickObjectives();
			}
			return flag;
		}

		private void PickObjectives()
		{
			m_unwantedColors.Clear();
			int thrownBagTypes = m_activeLevel.ThrownBagTypes;
			int num = m_activeLevel.WrongBagTypes;
			m_targetColors = mg_bc_Constants.GetRandomColors(thrownBagTypes);
			List<mg_bc_EJellyColors> list = new List<mg_bc_EJellyColors>();
			foreach (mg_bc_EJellyColors targetColor in m_targetColors)
			{
				mg_bc_EJellyColors _one;
				mg_bc_EJellyColors _two;
				mg_bc_Constants.Get90DegreeColors(targetColor, out _one, out _two);
				if (!m_targetColors.Contains(_one) && !list.Contains(_one))
				{
					list.Add(_one);
				}
				if (!m_targetColors.Contains(_two) && !list.Contains(_two))
				{
					list.Add(_two);
				}
			}
			while (num > 0 && list.Count > 0)
			{
				int index = Random.Range(0, list.Count);
				m_unwantedColors.Add(list[index]);
				list.RemoveAt(index);
				num--;
			}
			CandyMachine.SetColors(m_targetColors);
			int num2 = 0;
			num = m_activeLevel.WrongBagTypes;
			foreach (SpriteRenderer jellyTint in m_jellyTints)
			{
				num2++;
				num2 %= num;
				jellyTint.color = mg_bc_Constants.GetColorForJelly(m_unwantedColors[num2]);
			}
		}

		protected override void ParseXML()
		{
			TextAsset textAsset = null;
			switch (MinigameManager.GetActive<mg_BeanCounter>().GameMode)
			{
			case mg_bc_EGameMode.JELLY_NORMAL:
				textAsset = NormalXml;
				break;
			case mg_bc_EGameMode.JELLY_HARD:
				textAsset = HardXml;
				break;
			case mg_bc_EGameMode.JELLY_EXTREME:
				textAsset = ExtremeXml;
				break;
			default:
				textAsset = NormalXml;
				break;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			ParseXMLDoc(xmlDocument);
		}

		protected override GameObject GetBag()
		{
			GameObject bag = base.GetBag();
			mg_bc_JellyBag component = bag.GetComponent<mg_bc_JellyBag>();
			if (Random.value <= 0.3f)
			{
				int index = Random.Range(0, m_unwantedColors.Count);
				component.SetColor(m_unwantedColors[index]);
			}
			else
			{
				int index = Random.Range(0, m_targetColors.Count);
				component.SetColor(m_targetColors[index]);
			}
			return bag;
		}

		private void OnTriggerEnter2D(Collider2D _collision)
		{
			mg_bc_Penguin component = _collision.gameObject.GetComponent<mg_bc_Penguin>();
			if (component != null)
			{
				m_penguin = component;
				m_unloadArrow.SetActive(true);
			}
		}

		private void OnTriggerExit2D(Collider2D _collision)
		{
			mg_bc_Penguin component = _collision.gameObject.GetComponent<mg_bc_Penguin>();
			if (component != null)
			{
				m_penguin = null;
				m_unloadArrow.SetActive(false);
			}
		}

		protected override void ObjectUpdate(float _delta)
		{
			base.ObjectUpdate(_delta);
			if (m_bagsToThrowBack.Count > 0 && m_lastBagThrownBack < Time.time - 0.02f)
			{
				foreach (mg_bc_Bag item in m_bagsToThrowBack)
				{
					SpriteRenderer[] componentsInChildren = item.GetComponentsInChildren<SpriteRenderer>();
					SpriteRenderer[] array = componentsInChildren;
					foreach (SpriteRenderer spriteRenderer in array)
					{
						spriteRenderer.enabled = true;
					}
				}
				m_bagsToThrowBack.Clear();
			}
		}

		internal override void TakeBag()
		{
			if (m_penguin != null)
			{
				mg_bc_Bag mg_bc_Bag = m_penguin.RemoveBag();
				if (mg_bc_Bag != null)
				{
					mg_bc_JellyBag component = mg_bc_Bag.GetComponent<mg_bc_JellyBag>();
					mg_bc_EJellyColors color = component.Color;
					mg_bc_ScoreController.Instance.OnBagThrownBack(IsTargetColor(color));
					MinigameManager.GetActive().PlaySFX("mg_bc_sfx_BagTossTruck");
					ThrowBack(mg_bc_Bag);
				}
			}
		}

		private void ThrowBack(mg_bc_Bag _bag)
		{
			Rigidbody2D component = _bag.GetComponent<Rigidbody2D>();
			Rigidbody2D component2 = m_penguin.GetComponent<Rigidbody2D>();
			_bag.gameObject.SetActive(true);
			component.velocity = Vector2.zero;
			component.position = new Vector2(component2.position.x, component2.position.y + 3f);
			component.AddForce(new Vector2(200f, 600f));
			SpriteRenderer[] componentsInChildren = _bag.GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				spriteRenderer.enabled = false;
			}
			m_lastBagThrownBack = Time.time;
			m_bagsToThrowBack.Add(_bag);
		}

		internal bool IsTargetColor(mg_bc_EJellyColors _color)
		{
			return m_targetColors.Contains(_color);
		}
	}
}
