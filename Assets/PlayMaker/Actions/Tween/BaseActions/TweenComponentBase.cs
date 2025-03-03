// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base class for tweening Component properties
    /// Deals with finding and caching the Component
    /// NOTE: Tweening actions generally don't handle the target GameObject changing
    /// NOTE: If the component is known at edit time we could cache it in Preprocess...
    /// </summary>
    public abstract class TweenComponentBase<T>  : TweenActionBase where T : Component
    {
        [DisplayOrder(0)]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Game Object to tween.")]
        public FsmOwnerDefault gameObject;

        /// <summary>
        /// The cached GameObject. Call UpdateCache() first
        /// </summary>
        protected GameObject cachedGameObject;
             
        /// <summary>
        /// The cached component. Call UpdateCache() first
        /// </summary>
        protected T cachedComponent;

        public override void Reset()
        {
            base.Reset();

            gameObject = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null)
            {
                Finish();
            }

            if (!UpdateCache(go))
            {
                Finish();
            }
        }

        /// <summary>
        /// Check that the GameObject is the same as we cached
        /// and that we have a component reference cached
        /// </summary>
        protected bool UpdateCache(GameObject go)
        {
            if (go == null) return false;

            if (cachedComponent == null || cachedGameObject != go)
            {
                cachedComponent = go.GetComponent<T>();
                cachedGameObject = go;

                if (cachedComponent == null)
                {
                    LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
                }
            }

            return cachedComponent != null;
        }

        protected override void DoTween()
        {
            throw new NotImplementedException();
        }

        public override string ErrorCheck()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go != null && go.GetComponent<T>() == null)
            {
                if (typeof(T) == typeof(RectTransform))
                    return "This Tween only works with UI GameObjects";
                return "GameObject missing component:\n" + typeof(T);
            }

            return "";
        }
    }
}