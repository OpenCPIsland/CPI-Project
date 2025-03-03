using JetpackReboot.EnumExtensions;
using MinigameFramework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Environment : MonoBehaviour
	{
		private Dictionary<EnvironmentLayer, mg_jr_ParallaxLayer> m_parallaxLayers;

		private mg_jr_ScrollingSpeed m_scrolling;

		public mg_jr_EnvironmentID Id
		{
			get
			{
				return new mg_jr_EnvironmentID(Type, Variant);
			}
		}

		public EnvironmentType Type
		{
			get;
			private set;
		}

		public EnvironmentVariant Variant
		{
			get;
			private set;
		}

		public static mg_jr_Environment CreateEnvironment(EnvironmentType _environmentType, EnvironmentVariant _variant, mg_jr_ScrollingSpeed _scrolling)
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			GameObject gameObject = new GameObject("mg_jr_environment_" + _environmentType.ToString().ToLowerInvariant());
			mg_jr_Environment mg_jr_Environment = gameObject.AddComponent<mg_jr_Environment>();
			mg_jr_Environment.Type = _environmentType;
			mg_jr_Environment.Variant = _variant;
			mg_jr_Environment.m_scrolling = _scrolling;
			foreach (EnvironmentLayer value in Enum.GetValues(typeof(EnvironmentLayer)))
			{
				if (value != EnvironmentLayer.MAX)
				{
					mg_jr_ParallaxLayer instancedParallaxLayer = active.Resources.GetInstancedParallaxLayer(_environmentType, _variant, value);
					instancedParallaxLayer.Init(active.GameLogic.TurboPlayArea);
					Assert.NotNull(instancedParallaxLayer, "There should be a parallaxlayer for every environment and layer combination");
					mg_jr_Environment.SetLayer(value, instancedParallaxLayer);
				}
			}
			return mg_jr_Environment;
		}

		public static mg_jr_Environment CreateEnvironment(mg_jr_EnvironmentID _id, mg_jr_ScrollingSpeed _scrolling)
		{
			return CreateEnvironment(_id.Type, _id.Variant, _scrolling);
		}

		private void Awake()
		{
			m_parallaxLayers = new Dictionary<EnvironmentLayer, mg_jr_ParallaxLayer>(5);
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			if (m_scrolling.ScrollingEnabled)
			{
				foreach (mg_jr_ParallaxLayer value in m_parallaxLayers.Values)
				{
					value.MinigameUpdate(_deltaTime);
				}
			}
		}

		private void SetLayer(EnvironmentLayer _layer, mg_jr_ParallaxLayer _layerRenderer)
		{
			Assert.NotNull(_layerRenderer);
			if (m_parallaxLayers.ContainsKey(_layer))
			{
				UnityEngine.Object.Destroy(m_parallaxLayers[_layer]);
				m_parallaxLayers[_layer] = null;
			}
			m_parallaxLayers[_layer] = _layerRenderer;
			_layerRenderer.transform.parent = base.transform;
			if (_layer == EnvironmentLayer.TOP_BORDER && Type == EnvironmentType.CAVE)
			{
				_layerRenderer.DrawingLayer = mg_jr_SpriteDrawingLayers.DrawingLayers.BOTTOM_BORDER;
			}
			else
			{
				_layerRenderer.DrawingLayer = _layer.ConvertToDrawingLayer();
			}
		}
	}
}
