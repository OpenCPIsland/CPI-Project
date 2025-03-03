using MinigameFramework;
using NUnit.Framework;
using SpriteRendererExtensionMethods;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_ParallaxLayer : MonoBehaviour
	{
		private mg_jr_ScrollingSpeed m_scrollingSpeed = null;

		private mg_jr_SpriteDrawingLayers.DrawingLayers m_drawingLayer;

		private int m_orderInLayer = 0;

		private List<SpriteRenderer> m_spriteSetForLayer;

		private List<SpriteRenderer> m_activeSprites;

		private int m_spritesNeeded;

		private Bounds m_screenAreaInWorld = new Bounds(Vector3.zero, Vector3.zero);

		private float m_leftScreenEdgeInWorld;

		public mg_jr_SpriteDrawingLayers.DrawingLayers DrawingLayer
		{
			get
			{
				return m_drawingLayer;
			}
			set
			{
				m_drawingLayer = value;
				OrderInLayer = (int)DrawingLayer;
			}
		}

		public float VerticalPosition
		{
			get
			{
				return base.transform.position.y;
			}
			set
			{
				Vector3 position = new Vector3(base.transform.position.x, value, 0f);
				base.transform.position = position;
			}
		}

		private int OrderInLayer
		{
			get
			{
				return m_orderInLayer;
			}
			set
			{
				m_orderInLayer = value;
				if (m_activeSprites != null)
				{
					foreach (SpriteRenderer activeSprite in m_activeSprites)
					{
						activeSprite.sortingOrder = value;
					}
				}
			}
		}

		public void Init(Bounds _areaToFill)
		{
			m_screenAreaInWorld = _areaToFill;
		}

		private void Start()
		{
			Assert.IsFalse(m_screenAreaInWorld.size == Vector3.zero, "Call Init before start");
			m_leftScreenEdgeInWorld = m_screenAreaInWorld.min.x;
			float x = m_screenAreaInWorld.max.x;
			m_spriteSetForLayer = new List<SpriteRenderer>(16);
			foreach (Transform item in base.transform)
			{
				SpriteRenderer component = item.GetComponent<SpriteRenderer>();
				m_spriteSetForLayer.Add(component);
				component.gameObject.SetActive(false);
			}
			Assert.That(m_spriteSetForLayer.Count > 0);
			foreach (SpriteRenderer item2 in m_spriteSetForLayer)
			{
				item2.sortingOrder = m_orderInLayer;
			}
			m_activeSprites = new List<SpriteRenderer>(32);
			int expected = (int)m_spriteSetForLayer[0].sprite.rect.width;
			foreach (SpriteRenderer activeSprite in m_activeSprites)
			{
				Assert.AreEqual(expected, (int)activeSprite.sprite.rect.width, "Sprites in parallax layer are not all of the same width");
			}
			float x2 = m_spriteSetForLayer[0].sprite.bounds.size.x;
			float num = x - m_leftScreenEdgeInWorld;
			float num2 = num / x2;
			int num3 = Mathf.FloorToInt(num2);
			float num4 = num2 - (float)num3;
			m_spritesNeeded = num3 + 1;
			if (num4 > 0.001f)
			{
				m_spritesNeeded++;
			}
			while (m_activeSprites.Count < m_spritesNeeded)
			{
				AddRandomSpriteToQueue();
			}
			m_scrollingSpeed = MinigameManager.GetActive<mg_JetpackReboot>().GameLogic.ScrollingSpeed;
		}

		private void AddRandomSpriteToQueue()
		{
			int index = Random.Range(0, m_spriteSetForLayer.Count);
			SpriteRenderer spriteRenderer = m_spriteSetForLayer[index];
			GameObject gameObject = Object.Instantiate(spriteRenderer.gameObject);
			gameObject.transform.position += base.transform.position;
			gameObject.transform.SetParent(base.transform);
			gameObject.SetActive(true);
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			if (m_activeSprites.Count == 0)
			{
				component.transform.position = new Vector3(m_leftScreenEdgeInWorld, component.transform.position.y, 0f);
			}
			else
			{
				component.AlignSpriteToRightOf(m_activeSprites[m_activeSprites.Count - 1]);
				component.transform.Translate(new Vector3(-0.01f, 0f, 0f));
			}
			m_activeSprites.Add(component);
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			float num = m_scrollingSpeed.CurrentSpeedFor(DrawingLayer);
			float d = num * _deltaTime;
			for (int i = 0; i < m_activeSprites.Count; i++)
			{
				m_activeSprites[i].transform.Translate(Vector3.left * d);
			}
			SpriteRenderer spriteRenderer = m_activeSprites[0];
			float x = spriteRenderer.bounds.max.x;
			if (x < m_leftScreenEdgeInWorld)
			{
				m_activeSprites.RemoveAt(0);
				Object.Destroy(spriteRenderer.gameObject);
				AddRandomSpriteToQueue();
			}
		}
	}
}
