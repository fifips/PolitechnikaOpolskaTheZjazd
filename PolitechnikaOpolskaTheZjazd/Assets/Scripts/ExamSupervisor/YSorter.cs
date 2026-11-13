using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>
    /// Draws objects "in layers" in a top-down view: the lower an object's feet are on screen,
    /// the closer to the camera it is drawn. Without it the player is always drawn under (or over)
    /// the desks, no matter which side of a desk they stand on.
    /// Child sprites (e.g. the "!" marker) keep their authored order relative to the parent sprite.
    /// </summary>
    public class YSorter : MonoBehaviour
    {
        // Distance from the object's position to its feet, in world units (negative = feet below the centre).
        [SerializeField] float footOffset = 0f;

        SpriteRenderer[] _renderers;
        int[] _relativeOrders;

        void Awake()
        {
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            _relativeOrders = new int[_renderers.Length];
            int rootOrder = GetComponent<SpriteRenderer>() != null ? GetComponent<SpriteRenderer>().sortingOrder : 0;
            for (int i = 0; i < _renderers.Length; i++)
                _relativeOrders[i] = _renderers[i].sortingOrder - rootOrder;
        }

        /// <summary>Changes a child sprite's order relative to the parent (e.g. ruler behind or in front of the player).</summary>
        public void SetRelativeOrder(SpriteRenderer sprite, int relativeOrder)
        {
            for (int i = 0; i < _renderers.Length; i++)
                if (_renderers[i] == sprite) _relativeOrders[i] = relativeOrder;
        }

        void LateUpdate()
        {
            int baseOrder = -Mathf.RoundToInt((transform.position.y + footOffset) * 100f);
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].sortingOrder = baseOrder + _relativeOrders[i];
        }
    }
}
