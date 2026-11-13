using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(SpriteSelector))]
	[RequireComponent(typeof(Button))]
	public class CyclingButton : MonoBehaviour
	{
		[Serializable]
		public struct CycleSpritePair
		{
			public InputEvents.Cycles Cycle;

			public int SpriteIndex;
		}

		public List<CycleSpritePair> Cycles;

		public CycleSpritePair[] Test;

		public SpriteSelector SpriteSelector;

		public float DoubleTapTime = 0.15f;

		private Button button;

		private EventDispatcher dispatcher;

		private float lastClickTime;

		private int curIndex;

		public int CurIndex
		{
			get
			{
				return curIndex;
			}
			set
			{
				curIndex = value;
				SpriteSelector.SelectSprite(Cycles[curIndex].SpriteIndex);
			}
		}

		public void Awake()
		{
			button = GetComponent<Button>();
			dispatcher = Service.Get<EventDispatcher>();
			lastClickTime = -1f;
			curIndex = 0;
			if (SpriteSelector == null)
			{
				SpriteSelector = GetComponent<SpriteSelector>();
			}
		}

		public void OnEnable()
		{
			button.onClick.AddListener(onClick);
		}

		public void OnDisable()
		{
			button.onClick.RemoveListener(onClick);
		}

		private void onClick()
		{
			if (lastClickTime >= 0f && Time.timeSinceLevelLoad - lastClickTime <= DoubleTapTime)
			{
				curIndex++;
				if (curIndex >= Cycles.Count)
				{
					curIndex = 0;
				}
				dispatcher.DispatchEvent(new InputEvents.CycleChangeEvent(Cycles[curIndex].Cycle));
			}
			lastClickTime = Time.timeSinceLevelLoad;
		}
	}
}
