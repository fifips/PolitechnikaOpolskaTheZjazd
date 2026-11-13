using System.Collections;
using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>Student sitting at a desk. The sprite is tinted while cheating / when hit, "!" shows while cheating.</summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Student : MonoBehaviour
    {
        [SerializeField] GameObject cheatingMarker;
        // Student-at-desk sprites; each student gets one by id so the classroom is not full of clones.
        [SerializeField] Sprite[] looks;
        // White = sprite drawn with its own colours; the others tint it.
        [SerializeField] Color idleColor = Color.white;
        [SerializeField] Color cheatingColor = new Color(1f, 0.9f, 0.5f);
        [SerializeField] Color hitColor = new Color(1f, 0.5f, 0.5f);

        SpriteRenderer _sr;
        public int Id { get; private set; }

        void Awake() => _sr = GetComponent<SpriteRenderer>();

        public void Init(int id)
        {
            Id = id;
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (looks != null && looks.Length > 0) _sr.sprite = looks[id % looks.Length];
            SetCheating(false);
        }

        public void SetCheating(bool cheating)
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            _sr.color = cheating ? cheatingColor : idleColor;
            if (cheatingMarker != null) cheatingMarker.SetActive(cheating);
        }

        /// <summary>Ruler hit feedback: short red flash.</summary>
        public void PlayHit()
        {
            StopAllCoroutines();
            StartCoroutine(Flash());
        }

        IEnumerator Flash()
        {
            _sr.color = hitColor;
            if (cheatingMarker != null) cheatingMarker.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            _sr.color = idleColor;
        }
    }
}
