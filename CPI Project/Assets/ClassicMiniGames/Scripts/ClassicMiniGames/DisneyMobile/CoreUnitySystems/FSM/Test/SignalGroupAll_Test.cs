using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM.Test
{
	[TestFixture]
	public class SignalGroupAll_Test
	{
		private SignalGroupAll mSignalGroup = null;

		private GameObject mGameObject = null;

		[SetUp]
		public void SetUp()
		{
			mGameObject = new GameObject();
			mGameObject.AddComponent<SignalGroupAll>();
			Assert.NotNull(mGameObject);
			mSignalGroup = mGameObject.GetComponent<SignalGroupAll>();
			Assert.NotNull(mSignalGroup);
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(mSignalGroup);
			Object.DestroyImmediate(mGameObject);
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mSignalGroup);
		}

		[Test]
		public void TestNoSignalsActive()
		{
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
		}

		[Test]
		public void TestPartialSignalsActive()
		{
			List<Signal> signals = new List<Signal>();
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			AddSignalsToList(ref signals, 5);
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			signals[0].ActivateSignal();
			signals[2].ActivateSignal();
			signals[4].ActivateSignal();
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			foreach (Signal item in signals)
			{
				Object.DestroyImmediate(item.gameObject);
				Object.DestroyImmediate(item);
			}
		}

		[Test]
		public void TestAllSignalsActive()
		{
			List<Signal> signals = new List<Signal>();
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			AddSignalsToList(ref signals, 5);
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			foreach (ManualSignal item in signals)
			{
				item.ActivateSignal();
			}
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(true));
			foreach (Signal item2 in signals)
			{
				Object.DestroyImmediate(item2.gameObject);
				Object.DestroyImmediate(item2);
			}
		}

		[Test]
		public void TestManualActivation()
		{
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			List<Signal> signals = new List<Signal>();
			AddSignalsToList(ref signals, 4);
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			mSignalGroup.ActivateSignal();
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(true));
			foreach (ManualSignal item in signals)
			{
				Assert.That(item.IsSignaled(), Is.EqualTo(true));
			}
			foreach (Signal item2 in signals)
			{
				Object.DestroyImmediate(item2.gameObject);
				Object.DestroyImmediate(item2);
			}
		}

		[Test]
		public void TestSignalReset()
		{
			List<Signal> signals = new List<Signal>();
			AddSignalsToList(ref signals, 10);
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(false));
			foreach (ManualSignal item in signals)
			{
				item.ActivateSignal();
			}
			Assert.That(mSignalGroup.IsSignaled(), Is.EqualTo(true));
			mSignalGroup.Reset();
			foreach (ManualSignal item2 in signals)
			{
				Assert.That(item2.IsSignaled(), Is.EqualTo(false));
			}
			foreach (Signal item3 in signals)
			{
				Object.DestroyImmediate(item3.gameObject);
				Object.DestroyImmediate(item3);
			}
		}

		private void AddSignalsToList(ref List<Signal> signals, int amountToAdd)
		{
			for (int i = 0; i < amountToAdd; i++)
			{
				GameObject gameObject = new GameObject();
				Assert.NotNull(gameObject);
				gameObject.AddComponent<ManualSignal>();
				ManualSignal component = gameObject.GetComponent<ManualSignal>();
				Assert.NotNull(component);
				signals.Add(component);
				gameObject.transform.parent = mGameObject.transform;
			}
			mSignalGroup.RefreshChildrenSignalMapping();
		}
	}
}
