using System.Collections;
using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>
    /// Ruler swing shown when the player strikes. Lives on a child of the player (its origin is the hand);
    /// the "Swish" arc is a separate sprite. Has a cooldown: <see cref="TrySwing"/> returns false while it runs.
    /// </summary>
    public class RulerSwing : MonoBehaviour
    {
        [SerializeField] SpriteRenderer ruler;
        [SerializeField] SpriteRenderer swish;
        [SerializeField] Sprite[] swishFrames;
        [SerializeField] float cooldown = 0.5f;
        [SerializeField] float duration = 0.2f;
        [SerializeField] float handDistance = 0.2f;
        [SerializeField] float swishDistance = 0.7f;
        // Size of the ruler and of the arc relative to their sprites (1 = full size).
        [SerializeField] float rulerScale = 0.7f;
        [SerializeField] float swishScale = 0.8f;

        float _lastSwingTime = float.NegativeInfinity;
        YSorter _sorter;

        void Awake()
        {
            _sorter = GetComponentInParent<YSorter>();
            transform.localScale = Vector3.one * rulerScale;
            swish.transform.localScale = Vector3.one * swishScale;
            ruler.enabled = false;
            swish.enabled = false;
        }

        /// <summary>Starts a swing towards <paramref name="facing"/> unless the cooldown is still running.</summary>
        public bool TrySwing(Vector2 facing)
        {
            if (Time.time - _lastSwingTime < cooldown) return false;
            _lastSwingTime = Time.time;
            StopAllCoroutines();
            StartCoroutine(Play(facing.normalized));
            return true;
        }

        IEnumerator Play(Vector2 facing)
        {
            // Facing up: the ruler is drawn behind the player, otherwise in front of him.
            bool behind = facing.y > 0.5f;
            if (_sorter != null)
            {
                _sorter.SetRelativeOrder(ruler, behind ? -1 : 2);
                _sorter.SetRelativeOrder(swish, behind ? -2 : 1);
            }

            transform.localPosition = facing * handDistance;
            swish.transform.localPosition = facing * swishDistance;
            swish.transform.rotation = Quaternion.Euler(0f, 0f, SwingArc.FacingAngle(facing));
            ruler.enabled = true;
            swish.enabled = swishFrames != null && swishFrames.Length > 0;

            for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                float t = elapsed / duration;
                transform.rotation = Quaternion.Euler(0f, 0f, SwingArc.Angle(facing, t));
                if (swish.enabled)
                    swish.sprite = swishFrames[Mathf.Min(swishFrames.Length - 1, (int)(t * swishFrames.Length))];
                yield return null;
            }

            ruler.enabled = false;
            swish.enabled = false;
        }
    }
}
