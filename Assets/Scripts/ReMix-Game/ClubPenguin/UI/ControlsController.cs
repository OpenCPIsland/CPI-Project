using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ControlsController : MonoBehaviour
	{
		public RectTransform LeftContainer;

		public RectTransform RightContainer;

		public InputButtonGroupContentKey InputButtonsContentKey;

		private TrayInputButtonGroup trayInputButtonGroup;

		private ControlsScreenData controlsScreenData;

		private Dictionary<string, int> pathToButtonIndexMap;

		private InputButtonGroupDefinition currentGroupDefinition;

		private Dictionary<int, InputButtonDefinition> overrideDefinitions;

		private Dictionary<int, InputButtonDefinition> pendingInputButtonDefinitions;

		private Dictionary<int, TrayInputButton.ButtonState> pendingButtonStateOverrides;

		private ICoroutine replaceButtonsCoroutine;

		public void Awake()
		{
			pathToButtonIndexMap = new Dictionary<string, int>();
			overrideDefinitions = new Dictionary<int, InputButtonDefinition>();
			pendingInputButtonDefinitions = new Dictionary<int, InputButtonDefinition>();
			pendingButtonStateOverrides = new Dictionary<int, TrayInputButton.ButtonState>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle entityByType = cPDataEntityCollection.GetEntityByType<ControlsScreenData>();
			controlsScreenData = cPDataEntityCollection.GetComponent<ControlsScreenData>(entityByType);
			controlsScreenData.IsControlsScreenActive = true;
			if (controlsScreenData.LeftOptionContentKey != null)
			{
				setLeftOption(controlsScreenData.LeftOptionContentKey);
			}
			else
			{
				setUpLeftOptionPrefab(controlsScreenData.DefaultLeftOptionPrefab);
			}
			if (controlsScreenData.ButtonGroupContentKey != null)
			{
				setRightOption(controlsScreenData.ButtonGroupContentKey);
			}
			else
			{
				setRightOption(InputButtonsContentKey);
			}
			controlsScreenData.OnSetLeftOption += onSetLeftOption;
			controlsScreenData.OnReturnToDefaultLeftOption += onReturnToDefaultLeftOption;
			controlsScreenData.OnSetRightOption += onSetRightOption;
			controlsScreenData.OnReturnToDefaultRightOption += onReturnToDefaultRightOption;
			CoroutineRunner.Start(loadOverrides(), this, "loadOverrides");
		}

		private IEnumerator loadOverrides()
		{
			if (controlsScreenData.ButtonOverrides.Count > 0)
			{
				while (trayInputButtonGroup == null)
				{
					yield return null;
				}
				for (int i = 0; i < controlsScreenData.ButtonOverrides.Count; i++)
				{
					if (controlsScreenData.ButtonOverrides[i] != null)
					{
						setButtonDefinition(controlsScreenData.ButtonOverrides[i], i);
					}
				}
			}
			controlsScreenData.OnSetButton += onSetButton;
			controlsScreenData.OnSetButtonToDefault += onSetButtonToDefault;
		}

		private void onSetLeftOption(PrefabContentKey leftOptionContentKey)
		{
			setLeftOption(leftOptionContentKey);
		}

		private void onReturnToDefaultLeftOption()
		{
			setUpLeftOptionPrefab(controlsScreenData.DefaultLeftOptionPrefab);
		}

		private void onSetRightOption(InputButtonGroupContentKey buttonGroupContentKey)
		{
			setRightOption(buttonGroupContentKey);
		}

		private void onReturnToDefaultRightOption()
		{
			setRightOption(InputButtonsContentKey);
		}

		private void onSetButton(InputButtonContentKey buttonContentKey, int buttonIndex)
		{
			setButtonDefinition(buttonContentKey, buttonIndex);
		}

		private void onSetButtonToDefault(int buttonIndex)
		{
			setButtonToDefault(buttonIndex);
		}

		private void setLeftOption(PrefabContentKey leftOptionContentKey)
		{
			Content.LoadAsync(onSetLeftOptionLoaded, leftOptionContentKey);
		}

		private void onSetLeftOptionLoaded(string path, GameObject leftOptionPrefab)
		{
			setUpLeftOptionPrefab(leftOptionPrefab);
		}

		private void setUpLeftOptionPrefab(GameObject leftOptionPrefab)
		{
			if (LeftContainer != null)
			{
				if (LeftContainer.childCount > 0)
				{
					clearChildren(LeftContainer);
				}
				if (leftOptionPrefab != null)
				{
					UnityEngine.Object.Instantiate(leftOptionPrefab, LeftContainer);
				}
			}
		}

		private void setRightOption(InputButtonGroupContentKey contentKey)
		{
			Content.LoadAsync(onInputButtonSetLoaded, contentKey);
		}

		private void onInputButtonSetLoaded(string path, InputButtonGroupDefinition groupDefinition)
		{
			bool flag = false;
			if (RightContainer != null && doesGroupDefinitionMatchDataModel(groupDefinition))
			{
				if (shouldUseButtonOverrides(groupDefinition))
				{
					for (int i = 0; i < groupDefinition.InputButtonDefinitions.Length; i++)
					{
						bool flag2 = false;
						InputButtonDefinition currentButtonDefinition = GetCurrentButtonDefinition(i);
						if (currentButtonDefinition != groupDefinition.InputButtonDefinitions[i])
						{
							setUpButton(groupDefinition.InputButtonDefinitions[i], i);
							flag2 = true;
						}
						if (groupDefinition.ButtonStateOverrides != null && groupDefinition.ButtonStateOverrides.Length > i && groupDefinition.ButtonStateOverrides[i] != TrayInputButton.ButtonState.None)
						{
							if (flag2)
							{
								pendingButtonStateOverrides[i] = groupDefinition.ButtonStateOverrides[i];
							}
							else
							{
								trayInputButtonGroup.Buttons[i].SetState(groupDefinition.ButtonStateOverrides[i]);
							}
							flag2 = true;
						}
						if (!flag2 && currentButtonDefinition != null)
						{
							setButtonToDefault(i);
						}
						restartButtonComponents(trayInputButtonGroup.Buttons[i]);
					}
				}
				else
				{
					flag = true;
					if (replaceButtonsCoroutine != null && !replaceButtonsCoroutine.Disposed)
					{
						replaceButtonsCoroutine.Stop();
					}
					replaceButtonsCoroutine = CoroutineRunner.Start(replaceButtons(groupDefinition), this, "");
				}
			}
			if (!flag)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ControlsScreenEvents.ControlLayoutLoadComplete));
			}
		}

		private IEnumerator replaceButtons(InputButtonGroupDefinition newButtonGroupDefinition)
		{
			AssetRequest<TrayInputButtonGroup> assetRequest = Content.LoadAsync(newButtonGroupDefinition.TemplateContentKey);
			yield return assetRequest;
			if (!(RightContainer == null))
			{
				if (RightContainer.childCount > 0)
				{
					clearChildren(RightContainer);
				}
				yield return new WaitForEndOfFrame();
				currentGroupDefinition = newButtonGroupDefinition;
				overrideDefinitions.Clear();
				trayInputButtonGroup = newButtonGroupDefinition.Create(assetRequest.Asset, RightContainer);
				Service.Get<EventDispatcher>().DispatchEvent(default(ControlsScreenEvents.ControlLayoutLoadComplete));
			}
		}

		private void restartButtonComponents(TrayInputButton trayInputButton)
		{
			IRestartable[] componentsInChildren = trayInputButton.GetComponentsInChildren<IRestartable>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Restart();
			}
		}

		private void setButtonDefinition(InputButtonContentKey buttonContentKey, int buttonIndex)
		{
			string key = buttonContentKey.Key.ToLower();
			if (pathToButtonIndexMap.ContainsKey(key))
			{
			}
			pathToButtonIndexMap[key] = buttonIndex;
			Content.LoadAsync(onInputButtonLoaded, buttonContentKey);
		}

		private void onInputButtonLoaded(string path, InputButtonDefinition definition)
		{
			int value;
			if (pathToButtonIndexMap.TryGetValue(path, out value))
			{
				if (value >= trayInputButtonGroup.Buttons.Length)
				{
					throw new IndexOutOfRangeException("The button index supplied is out of bounds for the current template");
				}
				setUpButton(definition, value);
				pathToButtonIndexMap.Remove(path);
			}
			else
			{
				Log.LogErrorFormatted(this, "There was no index specified for button {0}", path);
			}
		}

		private void setUpButton(InputButtonDefinition definition, int index)
		{
			overrideDefinitions[index] = definition;
			if (definition != null)
			{
				pendingInputButtonDefinitions[index] = definition;
				trayInputButtonGroup.Buttons[index].OnReady += onTrayInputButtonReady;
				trayInputButtonGroup.Buttons[index].ResetButton();
			}
			else
			{
				trayInputButtonGroup.Buttons[index].ResetButton(false);
				trayInputButtonGroup.Buttons[index].gameObject.SetActive(false);
			}
		}

		private void onTrayInputButtonReady(int index)
		{
			trayInputButtonGroup.Buttons[index].OnReady -= onTrayInputButtonReady;
			if (pendingInputButtonDefinitions.ContainsKey(index))
			{
				pendingInputButtonDefinitions[index].SetUpButton(trayInputButtonGroup.Buttons[index]);
				pendingInputButtonDefinitions.Remove(index);
			}
			if (pendingButtonStateOverrides.ContainsKey(index))
			{
				trayInputButtonGroup.Buttons[index].SetState(pendingButtonStateOverrides[index]);
				pendingButtonStateOverrides.Remove(index);
			}
		}

		private void setButtonToDefault(int buttonIndex)
		{
			trayInputButtonGroup.Buttons[buttonIndex].SetState(trayInputButtonGroup.Buttons[buttonIndex].DefaultState);
		}

		private void clearChildren(Transform container)
		{
			for (int num = container.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(container.GetChild(num).gameObject);
			}
		}

		private bool doesGroupDefinitionMatchDataModel(InputButtonGroupDefinition groupDefinition)
		{
			InputButtonGroupContentKey key = controlsScreenData.ButtonGroupContentKey ?? InputButtonsContentKey;
			return AssetContentKey.GetAssetName(key).ToLower() == groupDefinition.name.ToLower();
		}

		private bool shouldUseButtonOverrides(InputButtonGroupDefinition groupDefinition)
		{
			if (currentGroupDefinition != null && currentGroupDefinition.TemplateContentKey.Key == groupDefinition.TemplateContentKey.Key)
			{
				for (int i = 0; i < groupDefinition.InputButtonDefinitions.Length; i++)
				{
					InputButtonDefinition y = groupDefinition.InputButtonDefinitions[i];
					if (overrideDefinitions.ContainsKey(i))
					{
						if (overrideDefinitions[i] == y)
						{
							return true;
						}
					}
					else if (currentGroupDefinition.InputButtonDefinitions.Length > i && currentGroupDefinition.InputButtonDefinitions[i] == y)
					{
						return true;
					}
				}
			}
			return false;
		}

		public InputButtonDefinition GetCurrentButtonDefinition(int index)
		{
			if (overrideDefinitions.ContainsKey(index))
			{
				return overrideDefinitions[index];
			}
			if (currentGroupDefinition.InputButtonDefinitions.Length <= index)
			{
				throw new IndexOutOfRangeException("The index requested is outside the range of the current group definition");
			}
			return currentGroupDefinition.InputButtonDefinitions[index];
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (controlsScreenData != null)
			{
				controlsScreenData.OnSetLeftOption -= onSetLeftOption;
				controlsScreenData.OnReturnToDefaultLeftOption -= onReturnToDefaultLeftOption;
				controlsScreenData.OnSetRightOption -= onSetRightOption;
				controlsScreenData.OnReturnToDefaultRightOption -= onReturnToDefaultLeftOption;
				controlsScreenData.OnSetButton -= onSetButton;
				controlsScreenData.IsControlsScreenActive = false;
			}
		}
	}
}
