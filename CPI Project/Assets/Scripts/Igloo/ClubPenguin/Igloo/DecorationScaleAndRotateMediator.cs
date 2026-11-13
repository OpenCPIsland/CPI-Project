using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.ObjectManipulation.Input;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace ClubPenguin.Igloo
{
    [DisallowMultipleComponent]
    public class DecorationScaleAndRotateMediator : MonoBehaviour
    {
        public GameObject ScaleSliderContainer;
        public GameObject RotationWheelContainer;
        public RotationWheel RotationWheel;

        private ObjectManipulationInputController objectManipulationInputController;
        private Slider scaleSlider;
        private float MaxRotation = 0f;
        private float MinRotation = 0f;
        private bool isProcessingKeyboard = false;

        public void Start()
        {
            scaleSlider = GetComponentInChildren<Slider>();
            objectManipulationInputController = SceneRefs.Get<ObjectManipulationInputController>();

            if (objectManipulationInputController != null)
            {
                objectManipulationInputController.InteractionStateChanged += onObjectManipulationInputControllerInteractionStateChanged;
                objectManipulationInputController.ObjectSelected += onObjectManipulationInputControllerObjectSelected;
            }

            if (RotationWheel != null)
            {
                RotationWheel.SelectedStateChanged = (Action<bool>)Delegate.Combine(RotationWheel.SelectedStateChanged, new Action<bool>(OnRotationWheelSelectionStateChanged));
            }

            if (PlatformUtils.GetPlatformType() == PlatformType.Mobile)
            {
                HideScaleControls();
                HideRotationControls();
            }
            else
            {
                InitializeControls();
            }
        }

        public void OnDestroy()
        {
            CoroutineRunner.StopAllForOwner(this);

            if (objectManipulationInputController != null)
            {
                objectManipulationInputController.InteractionStateChanged -= onObjectManipulationInputControllerInteractionStateChanged;
                objectManipulationInputController.ObjectSelected -= onObjectManipulationInputControllerObjectSelected;
            }

            if (scaleSlider != null)
            {
                scaleSlider.onValueChanged.RemoveAllListeners();
            }

            if (RotationWheel != null)
            {
                RotationWheel.ValueChanged = (Action<float>)Delegate.Remove(RotationWheel.ValueChanged, new Action<float>(OnRotationWheelChanged));
                RotationWheel.SelectedStateChanged = (Action<bool>)Delegate.Remove(RotationWheel.SelectedStateChanged, new Action<bool>(OnRotationWheelSelectionStateChanged));
            }
        }

        public void OnValidate() { }

        public void OnEnable()
        {
            if (!isProcessingKeyboard)
            {
                CoroutineRunner.Start(ProcessKeyBoardShortCuts(), this, "Keyboardshortcuts");
                isProcessingKeyboard = true;
            }
        }

        private IEnumerator ProcessKeyBoardShortCuts()
        {
            while (true)
            {
                KeyboardShortCuts();
                yield return null;
            }
        }

        public void OnScaleSliderChanged(float value)
        {
            if (objectManipulationInputController?.CurrentlySelectedObject == null)
            {
                return;
            }
            ObjectManipulator componentInParent = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
            if (componentInParent == null)
            {
                return;
            }
            ManipulatableObject componentInParent2 = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ManipulatableObject>();
            Dictionary<int, DecorationDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
            if (dictionary.ContainsKey(componentInParent2.DefinitionId))
            {
                DecorationDefinition decorationDefinition = dictionary[componentInParent2.DefinitionId];
                if (value >= decorationDefinition.MinScale && value <= decorationDefinition.MaxScale)
                {
                    componentInParent.ScaleTo(value);
                }
            }
        }

        protected void KeyboardShortCuts()
        {
            if (objectManipulationInputController == null || objectManipulationInputController.CurrentlySelectedObject == null)
            {
                return;
            }
            ObjectManipulator componentInParent = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
            if (componentInParent != null)
            {
                if (Keyboard.current != null)
                {
                    if (Keyboard.current.leftArrowKey.isPressed)
                    {
                        RotatedSelectedItem(componentInParent, -1f);
                    }
                    else if (Keyboard.current.rightArrowKey.isPressed)
                    {
                        RotatedSelectedItem(componentInParent, 1f);
                    }

                    if (Keyboard.current.numpadPlusKey != null && Keyboard.current.numpadPlusKey.isPressed)
                    {
                        ScaleSelectedItem(componentInParent, 0.1f);
                    }
                    else if (Keyboard.current.numpadMinusKey != null && Keyboard.current.numpadMinusKey.isPressed)
                    {
                        ScaleSelectedItem(componentInParent, -0.1f);
                    }
                    else if (Keyboard.current.equalsKey != null && Keyboard.current.equalsKey.isPressed)
                    {
                        ScaleSelectedItem(componentInParent, 0.1f);
                    }
                    else if (Keyboard.current.minusKey != null && Keyboard.current.minusKey.isPressed)
                    {
                        ScaleSelectedItem(componentInParent, -0.1f);
                    }
                }
            }
        }

        private void ScaleSelectedItem(ObjectManipulator m, float delta)
        {
            ManipulatableObject componentInParent = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ManipulatableObject>();
            Dictionary<int, DecorationDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
            if (dictionary.ContainsKey(componentInParent.DefinitionId))
            {
                DecorationDefinition decorationDefinition = dictionary[componentInParent.DefinitionId];
                float num = m.Scale + delta;
                if (num >= decorationDefinition.MinScale && num <= decorationDefinition.MaxScale)
                {
                    m.ScaleTo(num);
                }
            }
        }

        private void RotatedSelectedItem(ObjectManipulator m, float delta)
        {
            ManipulatableObject componentInParent = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ManipulatableObject>();
            Dictionary<int, DecorationDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
            if (dictionary.ContainsKey(componentInParent.DefinitionId))
            {
                DecorationDefinition decorationDefinition = dictionary[componentInParent.DefinitionId];
                float degrees = m.CurrentRotationDegreesAroundUp + delta;
                float degrees2 = RotateClamp360Degrees(degrees);
                if (isValidRotation(degrees2))
                {
                    m.RotateBy(delta);
                }
            }
        }

        private static float RotateClamp360Degrees(float degrees)
        {
            float num = degrees % 360f;
            if (degrees < 0f)
            {
                num += 360f;
            }
            return num;
        }

        private void OnRotationWheelChanged(float radians)
        {
            if (RotationWheel == null || objectManipulationInputController?.CurrentlySelectedObject == null)
            {
                return;
            }
            float degrees = (RotationWheel.Value - radians) * 57.29578f;
            float degrees2 = RotateClamp360Degrees(degrees);
            if (isValidRotation(degrees2))
            {
                ObjectManipulator componentInParent = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
                if (componentInParent != null)
                {
                    componentInParent.RotateBy(degrees2);
                }
            }
        }

        private bool isValidRotation(float degrees)
        {
            if (MaxRotation >= degrees && degrees <= MaxRotation)
            {
                return true;
            }
            float num = 360f + MinRotation;
            return num <= degrees && degrees <= 360f;
        }

        private void onObjectManipulationInputControllerInteractionStateChanged(InteractionState state)
        {
            if (state == InteractionState.ActiveSelectedItem)
            {
                InitializeControls();
                return;
            }
            HideScaleControls();
            HideRotationControls();
        }

        private void onObjectManipulationInputControllerObjectSelected(ManipulatableObject obj)
        {
            if (obj != null && (objectManipulationInputController.CurrentState.State == InteractionState.ActiveSelectedItem || objectManipulationInputController.CurrentState.State == InteractionState.NoSelectedItem))
            {
                InitializeControls();
            }
        }

        private void HideRotationControls()
        {
            if (RotationWheelContainer != null)
            {
                RotationWheelContainer.SetActive(false);
            }
        }

        private void HideScaleControls()
        {
            if (ScaleSliderContainer != null)
            {
                ScaleSliderContainer.SetActive(false);
            }
        }

        private void OnRotationWheelSelectionStateChanged(bool isSelected)
        {
            if (objectManipulationInputController != null)
            {
                objectManipulationInputController.SetPausedState(isSelected);
                objectManipulationInputController.SkipOneFrame = true;
            }
        }

        private void InitializeControls()
        {
            if (objectManipulationInputController?.CurrentlySelectedObject == null)
            {
                return;
            }
            ManipulatableObject componentInParent = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ManipulatableObject>();
            ObjectManipulator componentInParent2 = objectManipulationInputController.CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
            if (componentInParent2 == null)
            {
                return;
            }
            switch (componentInParent.Type)
            {
                case DecorationLayoutData.DefinitionType.Structure:
                    HideRotationControls();
                    HideScaleControls();
                    break;
                case DecorationLayoutData.DefinitionType.Decoration:
                    {
                        IGameData gameData = Service.Get<IGameData>();
                        Dictionary<int, DecorationDefinition> dictionary = gameData.Get<Dictionary<int, DecorationDefinition>>();
                        if (dictionary.ContainsKey(componentInParent.DefinitionId))
                        {
                            DecorationDefinition def = dictionary[componentInParent.DefinitionId];
                            ConfigureScalingOptions(def, componentInParent2);
                            ConfigureRotationOptions(def, componentInParent2);
                        }
                        break;
                    }
            }
        }

        private void ConfigureRotationOptions(DecorationDefinition def, ObjectManipulator m)
        {
            if (RotationWheel == null) return;

            RotationWheel.ValueChanged = (Action<float>)Delegate.Remove(RotationWheel.ValueChanged, new Action<float>(OnRotationWheelChanged));
            if (def.MaxRotation != 0 && def.MinRotation != 0)
            {
                MaxRotation = def.MaxRotation;
                MinRotation = def.MinRotation;
                RotationWheel.Value = m.CurrentRotationDegreesAroundUp * ((float)Math.PI / 180f);
                RotationWheel.ValueChanged = (Action<float>)Delegate.Combine(RotationWheel.ValueChanged, new Action<float>(OnRotationWheelChanged));
                if (RotationWheelContainer != null)
                {
                    RotationWheelContainer.SetActive(true);
                }
            }
            else
            {
                MaxRotation = 0f;
                MinRotation = 0f;
                HideRotationControls();
            }
        }

        private void ConfigureScalingOptions(DecorationDefinition def, ObjectManipulator m)
        {
            if (scaleSlider == null) return;

            scaleSlider.onValueChanged.RemoveAllListeners();
            if (def.MaxScale == def.MinScale)
            {
                HideScaleControls();
                return;
            }
            if (ScaleSliderContainer != null)
            {
                ScaleSliderContainer.SetActive(true);
            }
            scaleSlider.minValue = def.MinScale;
            scaleSlider.maxValue = def.MaxScale;
            scaleSlider.value = m.Scale;
            scaleSlider.onValueChanged.AddListener(OnScaleSliderChanged);
        }
    }
}
