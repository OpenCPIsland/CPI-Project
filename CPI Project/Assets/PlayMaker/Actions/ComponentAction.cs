// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

// Unity 5.1 introduced a new networking library. 
// Unless we define PLAYMAKER_LEGACY_NETWORK old network actions are disabled
#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || PLAYMAKER_LEGACY_NETWORK)
#define UNITY_NEW_NETWORK
#endif

// Some platforms do not support networking (at least the old network library)
#if (UNITY_SWITCH || UNITY_FLASH || UNITY_NACL || UNITY_METRO || UNITY_WP8 || UNITY_WIIU || UNITY_PSM || UNITY_WEBGL || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE)
#define PLATFORM_NOT_SUPPORTED
#endif

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    // Base class for actions that access a Component on a GameObject.
    // Caches the component for performance
    public abstract class ComponentAction<T> : FsmStateAction where T : Component
    {
		/// <summary>
		/// The cached GameObject. Call UpdateCache() to update.
		/// </summary>
        protected GameObject cachedGameObject;

        /// <summary>
        /// Transform for the cached GameObject.
        /// </summary>
        public Transform cachedTransform { get; private set; }

		/// <summary>
		/// The cached component. Call UpdateCache() to update.
		/// </summary>
        protected T cachedComponent;

        // Properties to access common components

        protected Rigidbody rigidbody
        {
            get { return cachedComponent as Rigidbody; }
        }

        protected Rigidbody2D rigidbody2d
        {
            get { return cachedComponent as Rigidbody2D; }
        }

        protected Renderer renderer
        {
            get { return cachedComponent as Renderer; }
        }

        protected Animation animation
        {
            get { return cachedComponent as Animation; }
        }

        protected AudioSource audio
        {
            get { return cachedComponent as AudioSource; }
        }

        protected Camera camera
        {
            get { return cachedComponent as Camera; }
        }

#if !UNITY_2019_3_OR_NEWER

		#if UNITY_2017_2_OR_NEWER
		#pragma warning disable 618 
        #endif
        protected GUIText guiText
        {
            get { return cachedComponent as GUIText; }
        }

        protected GUITexture guiTexture
        {
            get { return cachedComponent as GUITexture; }
        }
        #if UNITY_2017_2_OR_NEWER
        #pragma warning restore CS0618 
		#endif

#endif

        protected Light light
        {
            get { return cachedComponent as Light; }
        }

#if !(PLATFORM_NOT_SUPPORTED || UNITY_NEW_NETWORK || PLAYMAKER_NO_NETWORK)
        protected NetworkView networkView
        {
            get { return cachedComponent as NetworkView; }
        }
#endif

        // Check that the GameObject is the same
        // and that we have a component reference cached
        // Returns true if component is cached.
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

        // Use this instead of UpdateCache if T is Transform
        // since it avoids the cost of GetComponent
        protected bool UpdateCachedTransform(GameObject go)
        {
            if (go == null) return false;

            if (cachedTransform == null || cachedGameObject != go)
            {
                cachedTransform = go.transform;
                cachedComponent = cachedTransform as T;
                cachedGameObject = go;
            }

            return cachedTransform != null;
        }

        // Same as UpdateCache, but caches transform as well
        // Use this if you need to access the transform as well as a component on the GameObject
        protected bool UpdateCacheAndTransform(GameObject go)
        {
            if (!UpdateCache(go))
                return false;

            cachedTransform = go.transform;
            return true;
        }

        // Same as UpdateCache, but adds a new component if missing
        protected bool UpdateCacheAddComponent(GameObject go)
        {
            if (go == null) return false;

            if (cachedComponent == null || cachedGameObject != go)
            {
                cachedComponent = go.GetComponent<T>();
                cachedGameObject = go;

                if (cachedComponent == null)
                {
                    cachedComponent = go.AddComponent<T>();
                    cachedComponent.hideFlags = HideFlags.DontSaveInEditor;                       
                }
            }

            return cachedComponent != null;
        }

        protected void SendEvent(FsmEventTarget eventTarget, FsmEvent fsmEvent)
        {
            Fsm.Event(cachedGameObject, eventTarget, fsmEvent);
        }
    }

    /// <summary>
    /// Version of ComponentAction for 2 components each on their own GameObject
    /// </summary>
    public abstract class ComponentAction<T1, T2> : FsmStateAction where T1 : Component where T2 : Component
    {
        /// <summary>
        /// The first cached GameObject. Call UpdateCache() first
        /// </summary>
        protected GameObject cachedGameObject1;

        /// <summary>
        /// The second cached GameObject. Call UpdateCache() first
        /// </summary>
        protected GameObject cachedGameObject2;

        /// <summary>
        /// The first cached component. Call UpdateCache() first
        /// </summary>
        protected T1 cachedComponent1;

        /// <summary>
        /// The second cached component. Call UpdateCache() first
        /// </summary>
        protected T2 cachedComponent2;

        protected Transform cachedTransform2;

        // Check that the GameObjects are the same
        // and that we have component references cached.
        // Returns true if components are cached.
        protected bool UpdateCache(GameObject go1, GameObject go2)
        {
            if (go1 == null || go2 == null) return false;

            if (cachedComponent1 == null || cachedGameObject1 != go1)
            {
                cachedComponent1 = go1.GetComponent<T1>();
                cachedGameObject1 = go1;

                if (cachedComponent1 == null)
                {
                    LogWarning("Missing component: " + typeof(T1).FullName + " on: " + go1.name);
                    return false;
                }
            }

            if (cachedComponent2 == null || cachedGameObject2 != go2)
            {
                cachedComponent2 = go2.GetComponent<T2>();
                cachedGameObject2 = go2;

                if (cachedComponent2 == null)
                {
                    LogWarning("Missing component: " + typeof(T2).FullName + " on: " + go2.name);
                    return false;
                }
            }

            cachedTransform2 = cachedGameObject2.transform;

            return true;
        }
    }
}