using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Pizzatron
{
    public class mg_pt_SauceHolderObject : mg_pt_ToppingHolderObject
    {
        private static readonly string ANIM_TRIGGER_GRABBED = "OnGrabbed";

        private Animator m_animator;
        private Transform spriteTransform;
        private Vector3 originalSpriteLocalPos;
        private Vector3 originalHolderLocalPos;

        private Coroutine resetCoroutine;
        private Coroutine holdPositionCoroutine;

        private FieldInfo originalPosField;

        public override bool IsSauce => true;

        public override void Initialize(GameObject p_resource, mg_pt_EToppingType p_toppingType, string p_grabbedTagSFX, string p_heldedTagSFX)
        {
            base.Initialize(p_resource, p_toppingType, p_grabbedTagSFX, p_heldedTagSFX);

            m_animator = GetComponentInChildren<Animator>();

            if (m_animator != null)
            {
                spriteTransform = m_animator.transform;
                originalSpriteLocalPos = spriteTransform.localPosition;
            }

            originalHolderLocalPos = transform.localPosition;

            originalPosField = typeof(mg_pt_ToppingHolderObject).GetField(
                "m_originalPos",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
        }

        public override void OnGrabbed()
        {
            base.OnGrabbed();

            if (m_animator != null)
            {
                m_animator.ResetTrigger(ANIM_TRIGGER_GRABBED);
                m_animator.SetTrigger(ANIM_TRIGGER_GRABBED);
            }

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
                resetCoroutine = null;
            }

            if (holdPositionCoroutine != null)
            {
                StopCoroutine(holdPositionCoroutine);
                holdPositionCoroutine = null;
            }

            ForceResetPositions();

            resetCoroutine = StartCoroutine(ResetPositionAfterDelay(0.21f));
            holdPositionCoroutine = StartCoroutine(HoldSpritePositionCoroutine(0.5f));
        }

        private IEnumerator ResetPositionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            ForceResetPositions();

            resetCoroutine = null;
        }

        private IEnumerator HoldSpritePositionCoroutine(float duration)
        {
            float timer = 0f;

            while (timer < duration)
            {
                ForceResetPositions();

                timer += Time.deltaTime;

                yield return null;
            }

            ForceResetPositions();

            holdPositionCoroutine = null;
        }

        private void LateUpdate()
        {
            ForceResetPositions();
        }

        private void ForceResetPositions()
        {
            if (spriteTransform != null)
            {
                spriteTransform.localPosition = originalSpriteLocalPos;
                spriteTransform.localRotation = Quaternion.identity;
                spriteTransform.localScale = Vector3.one;
            }

            transform.localPosition = originalHolderLocalPos;

            if (originalPosField != null)
            {
                originalPosField.SetValue(this, (Vector2)originalHolderLocalPos);
            }
        }
    }
}