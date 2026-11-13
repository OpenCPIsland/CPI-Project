using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_Order
	{
		public List<mg_ss_OrderStep> Steps
		{
			get;
			private set;
		}

		public mg_ss_SpecialOrderData SpecialOrder
		{
			get;
			private set;
		}

		public int LastStepQueued
		{
			get;
			private set;
		}

		public bool OrderCompleted
		{
			get;
			private set;
		}

		public bool IsSpecial
		{
			get
			{
				return SpecialOrder != null;
			}
		}

		public string SpecialCustomer
		{
			get
			{
				string result = null;
				if (IsSpecial)
				{
					result = SpecialOrder.Tag;
				}
				return result;
			}
		}

		private mg_ss_OrderStep FirstIncompleteStep
		{
			get
			{
				return Steps.Find((mg_ss_OrderStep step) => step.State == mg_ss_EOrderStepState.INCOMPLETE);
			}
		}

		public mg_ss_Order()
		{
			Steps = new List<mg_ss_OrderStep>();
			for (int i = 0; i < 8; i++)
			{
				Steps.Add(new mg_ss_OrderStep());
			}
		}

		public bool ContainsFruit(mg_ss_EItemTypes p_fruitType)
		{
			return Steps.Find((mg_ss_OrderStep step) => step.State == mg_ss_EOrderStepState.INCOMPLETE && step.FruitType == p_fruitType) != null;
		}

		public void CopyOrder(mg_ss_Order p_orderToCopy)
		{
			ClearOrder();
			SpecialOrder = p_orderToCopy.SpecialOrder;
			LastStepQueued = p_orderToCopy.LastStepQueued;
			OrderCompleted = p_orderToCopy.OrderCompleted;
			mg_ss_OrderStep mg_ss_OrderStep = null;
			mg_ss_OrderStep mg_ss_OrderStep2 = null;
			for (int i = 0; i < Steps.Count; i++)
			{
				mg_ss_OrderStep = p_orderToCopy.Steps[i];
				mg_ss_OrderStep2 = Steps[i];
				mg_ss_OrderStep2.State = mg_ss_OrderStep.State;
				mg_ss_OrderStep2.FruitType = mg_ss_OrderStep.FruitType;
			}
		}

		private void ClearOrder()
		{
			foreach (mg_ss_OrderStep step in Steps)
			{
				step.State = mg_ss_EOrderStepState.INVALID;
			}
			SpecialOrder = null;
			LastStepQueued = -1;
			OrderCompleted = false;
		}

		public void GenerateGenericOrder(int p_length)
		{
			ClearOrder();
			mg_ss_OrderStep mg_ss_OrderStep = null;
			for (int i = 0; i < p_length; i++)
			{
				mg_ss_OrderStep = Steps[i];
				mg_ss_OrderStep.FruitType = (mg_ss_EItemTypes)Random.Range(0, 12);
				mg_ss_OrderStep.State = mg_ss_EOrderStepState.INCOMPLETE;
			}
		}

		public void GenerateSpecialOrder(mg_ss_SpecialOrderData p_orderData)
		{
			ClearOrder();
			SpecialOrder = p_orderData;
			mg_ss_OrderStep mg_ss_OrderStep = null;
			for (int i = 0; i < SpecialOrder.Order.Count; i++)
			{
				mg_ss_OrderStep = Steps[i];
				mg_ss_OrderStep.FruitType = SpecialOrder.Order[i];
				mg_ss_OrderStep.State = mg_ss_EOrderStepState.INCOMPLETE;
			}
		}

		public mg_ss_EItemTypes GetNextFruitToQueue(mg_ss_OrderSystem p_orderSystem)
		{
			mg_ss_EItemTypes result = mg_ss_EItemTypes.NULL;
			mg_ss_OrderStep firstIncompleteStep = FirstIncompleteStep;
			if (p_orderSystem.IsItemSpawnedOnConveyor(firstIncompleteStep.FruitType))
			{
				LastStepQueued++;
				mg_ss_OrderStep mg_ss_OrderStep = null;
				if (LastStepQueued < Steps.Count && LastStepQueued < Steps.Count)
				{
					mg_ss_OrderStep = Steps[LastStepQueued];
				}
				if (mg_ss_OrderStep != null && mg_ss_OrderStep.State == mg_ss_EOrderStepState.INCOMPLETE)
				{
					result = mg_ss_OrderStep.FruitType;
				}
			}
			else
			{
				LastStepQueued = Steps.IndexOf(firstIncompleteStep);
				result = firstIncompleteStep.FruitType;
			}
			return result;
		}

		public void CompleteStep(mg_ss_EItemTypes p_fruitType)
		{
			mg_ss_OrderStep mg_ss_OrderStep = Steps.Find((mg_ss_OrderStep step) => step.State == mg_ss_EOrderStepState.INCOMPLETE && step.FruitType == p_fruitType);
			if (mg_ss_OrderStep != null)
			{
				mg_ss_OrderStep.State = mg_ss_EOrderStepState.COMPLETE;
				OrderCompleted = (FirstIncompleteStep == null);
			}
		}

		public List<mg_ss_EItemTypes> GetItemTypes()
		{
			List<mg_ss_EItemTypes> list = new List<mg_ss_EItemTypes>();
			List<mg_ss_OrderStep> list2 = Steps.FindAll((mg_ss_OrderStep step) => step.State == mg_ss_EOrderStepState.INCOMPLETE);
			foreach (mg_ss_OrderStep item in list2)
			{
				if (!list.Contains(item.FruitType))
				{
					list.Add(item.FruitType);
				}
			}
			return list;
		}
	}
}
