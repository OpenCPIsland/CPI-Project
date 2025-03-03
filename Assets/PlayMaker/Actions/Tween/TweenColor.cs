// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [Tooltip("Tween the color of a GameObject or a Color variable. The GameObject needs a Material, Sprite, Image, Text, or Light component.")]
    public class TweenColor : TweenPropertyBase<FsmColor>
    {
        private const string SupportedComponents = "MeshRenderer, Sprite, Image, Text, Light.";

        private const string OffsetTooltip = "How to apply the Offset Color. " +
                                             "Similar to Photoshop Blend modes. " +
                                             "\nNote: use the color alpha to fade the blend.";

        public enum Target { GameObject, Variable }
        public enum TargetType { None, Material, Sprite, Image, Text, Light}

        [Tooltip("What to tween.")]
        public Target target = Target.Variable;

        [Tooltip("A GameObject with a Material, Sprite, Image, Text, or Light component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The variable to tween.")]
        [UIHint(UIHint.Variable)]
        public FsmColor variable;

        [Tooltip(OffsetTooltip)]
        public ColorBlendMode fromOffsetBlendMode;

        [Tooltip(OffsetTooltip)]
        public ColorBlendMode toOffsetBlendMode;

        private GameObject cachedGameObject;
        private Component cachedComponent;

        public TargetType type
        {
            get { return targetType; }
        }

        private TargetType targetType;
        private Material material;
        private SpriteRenderer spriteRenderer;
        private Text text;
        private Image image;
        private Light light;

        public override void Reset()
        {
            base.Reset();

            fromOffsetBlendMode = ColorBlendMode.Normal;
            toOffsetBlendMode = ColorBlendMode.Normal;

            gameObject = null;
            cachedGameObject = null;
            cachedComponent = null;
        }

        private void UpdateCache(GameObject go)
        {
            cachedGameObject = go;
            if (go == null)
            {
                cachedComponent = null;
                return;
            }

            // Down the line we should make this expandable,
            // e.g., register a component type and get/set callbacks

            cachedComponent = go.GetComponent<MeshRenderer>();
            if (cachedComponent != null) return;
            cachedComponent = go.GetComponent<Image>();
            if (cachedComponent != null) return;
            cachedComponent = go.GetComponent<Text>();
            if (cachedComponent != null) return;
            cachedComponent = go.GetComponent<Light>();
            if (cachedComponent != null) return;
            cachedComponent = go.GetComponent<SpriteRenderer>();
        }

        private void CheckCache()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (cachedGameObject != go)
            {
                UpdateCache(go);
            }

            Init();
        }

        private void Init()
        {
            targetType = TargetType.None;

            var renderer = cachedComponent as MeshRenderer;
            if (renderer != null)
            {
                targetType = TargetType.Material;
                material = renderer.material;
                return;
            }

            image = cachedComponent as Image;
            if (image != null)
            {
                targetType = TargetType.Image;
                return;
            }

            spriteRenderer = cachedComponent as SpriteRenderer;
            if (spriteRenderer != null)
            {
                targetType = TargetType.Sprite;
                return;
            }

            text = cachedComponent as Text;
            if (text != null)
            {
                targetType = TargetType.Text;
                return;
            }

            light = cachedComponent as Light;
            if (light != null)
            {
                targetType = TargetType.Light;
            }
        }

        public override void OnEnter()
        {
            if (target == Target.GameObject)
            {
                CheckCache();
            }

            base.OnEnter();

            InitOffsets();

            DoTween();
        }
     
        protected override void InitTargets()
        {
            switch (fromOption)
            {
                case TargetValueOptions.CurrentValue:
                    StartValue = GetTargetColor();
                    break;
                case TargetValueOptions.Value:
                    StartValue = fromValue.Value;
                    break;
                case TargetValueOptions.Offset:
                    // Derived classes need to implement GetOffsetValue:
                    StartValue = GetOffsetValue(variable.RawValue, fromValue.RawValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (toOption)
            {
                case TargetValueOptions.CurrentValue:
                    EndValue = GetTargetColor();
                    break;
                case TargetValueOptions.Value:
                    EndValue = toValue.Value;
                    break;
                case TargetValueOptions.Offset:
                    // Derived classes need to implement GetOffsetValue:
                    EndValue = GetOffsetValue(variable.RawValue, toValue.RawValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Color GetTargetColor()
        {
            if (target == Target.Variable)
            {
                return variable.Value;
            }

            switch (targetType)
            {
                case TargetType.None: return Color.white;
                case TargetType.Material: return material.color;
                case TargetType.Sprite: return spriteRenderer.color;
                case TargetType.Image: return image.color;
                case TargetType.Text: return text.color;
                case TargetType.Light: return light.color;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetTargetColor(Color color)
        {
            if (target == Target.Variable)
            {
                variable.Value = color;
                return;
            }

            switch (targetType)
            {
                case TargetType.None: 
                    break;
                case TargetType.Material:
                    material.color = color;
                    break;
                case TargetType.Sprite: 
                    spriteRenderer.color = color;
                    break;
                case TargetType.Image: 
                    image.color = color;
                    break;
                case TargetType.Text:
                    text.color = color;
                    break;
                case TargetType.Light: 
                    light.color = color;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InitOffsets()
        {
            if (fromOption == TargetValueOptions.Offset)
            {
                StartValue = ActionHelpers.BlendColor(fromOffsetBlendMode, GetTargetColor(), fromValue.Value);
            }
            if (toOption == TargetValueOptions.Offset)
            {
                EndValue = ActionHelpers.BlendColor(toOffsetBlendMode, GetTargetColor(), toValue.Value);
            }
        }

        protected override object GetOffsetValue(object value, object offset)
        {
            // implemented in InitOffsets because we need the ColorBlendMode
            return value;
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);
            SetTargetColor( Color.Lerp((Color) StartValue, (Color) EndValue, lerp));
        }


#if UNITY_EDITOR

        public override string ErrorCheck()
        {
            if (target == Target.Variable) return "";

            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) return "";

            CheckCache();

            if (targetType == TargetType.None)
            {
                return "@gameObject:GameObject needs a " + SupportedComponents;
            }

            return "";
        }

#endif
    }

}
