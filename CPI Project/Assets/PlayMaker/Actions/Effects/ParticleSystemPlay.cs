// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Effects)]
	[Tooltip("Plays a ParticleSystem. Optionally destroy the GameObject when the ParticleSystem is finished.")]
	public class ParticleSystemPlay : ComponentAction<ParticleSystem>
	{
        [RequiredField]
        [Tooltip("The GameObject with the ParticleSystem.")]
        [CheckForComponent(typeof(ParticleSystem))]
        public FsmOwnerDefault gameObject;

        [Tooltip("Play ParticleSystems on all child GameObjects too.")]
        public FsmBool withChildren;

        [Tooltip("''With Children'' can be quite expensive since it has to find Particle Systems in all children. " +
                 "If the hierarchy doesn't change, use Cache Children to speed this up.")]
        public FsmBool cacheChildren;

        [Tooltip("Stop playing when state exits")]
        public FsmBool stopOnExit;

        [Tooltip("Destroy the GameObject when the ParticleSystem has finished playing. " + 
                 "'With Children' means all child particle systems also need to finish.")]
        public FsmBool destroyOnFinish;

        private GameObject go;
        private ParticleSystem[] childParticleSystems;

        public override void Reset()
        {
            gameObject = null;
            withChildren = null;
            cacheChildren = null;
            destroyOnFinish = null;
        }

        public override void Awake()
        {
            if (!withChildren.Value || !cacheChildren.Value) return;

            go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                childParticleSystems = go.GetComponentsInChildren<ParticleSystem>();
            }
        }

        public override void OnEnter()
		{
			DoParticleSystemPlay();

            if (!destroyOnFinish.Value)
            {
                Finish();
            }
		}

        public override void OnExit()
        {
            if (!stopOnExit.Value) return;

            if (withChildren.Value && cacheChildren.Value)
            {
                cachedComponent.Stop(false);
                for (var i = 0; i < childParticleSystems.Length; i++)
                {
                    var system = childParticleSystems[i];
                    if (system != null)
                        system.Stop(false);
                }
            }
            else
            {
                cachedComponent.Stop(withChildren.Value);
            }
        }

        public override void OnUpdate()
        {
            if (withChildren.Value && cacheChildren.Value)
            {
                if (cachedComponent.IsAlive(false)) return;
                for (var i = 0; i < childParticleSystems.Length; i++)
                {
                    var system = childParticleSystems[i];
                    if (system != null)
                        if (system.IsAlive(false))
                            return;
                }
            }
            else
            {
                if (cachedComponent.IsAlive(withChildren.Value))
                {
                    return;
                }
            }

            Object.Destroy(go);
            Finish();
        }

        private void DoParticleSystemPlay()
        {
            go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (!UpdateCache(go)) return;

            if (withChildren.Value && cacheChildren.Value)
            {
                cachedComponent.Play(false);
                for (var i = 0; i < childParticleSystems.Length; i++)
                {
                    var system = childParticleSystems[i];
                    if (system != null)
                        system.Play(false);
                }
            }
            else
            {
                cachedComponent.Play(withChildren.Value);
            }
        }

	}

}
