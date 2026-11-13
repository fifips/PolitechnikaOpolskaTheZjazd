using System.Collections.Generic;
using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>Spawns the grid of students (4 columns by default).</summary>
    public class ClassroomLayout : MonoBehaviour
    {
        [SerializeField] Student studentPrefab;
        [SerializeField] int columns = 4;
        // Free space between the colliders of neighbouring students (desk + seated student), not between their centres.
        // Both the columns and the rows leave aisles wide enough for the player (feet collider 0.46 x 0.7) to walk through.
        [SerializeField] Vector2 gap = new Vector2(1.7f, 1f);

        // The student prefab's pivot is at its feet (for Y sorting), 1.35 below the centre of the desk grid slot.
        [SerializeField] Vector2 pivotToSlotOffset = new Vector2(0f, -1.35f);

        readonly List<Student> _students = new List<Student>();
        public IReadOnlyList<Student> Students => _students;

        // Size of one student-at-desk in world units, taken from the prefab's collider and scale.
        Vector2 DeskSize
        {
            get
            {
                var box = studentPrefab.GetComponent<BoxCollider2D>();
                var scale = studentPrefab.transform.localScale;
                return new Vector2(box.size.x * scale.x, box.size.y * scale.y);
            }
        }

        public void Build(int count)
        {
            Clear();
            var spacing = gap + DeskSize;
            int rows = Mathf.CeilToInt(count / (float)columns);
            var origin = new Vector2(-(columns - 1) * spacing.x / 2f, (rows - 1) * spacing.y / 2f);
            for (int i = 0; i < count; i++)
            {
                var pos = origin + new Vector2((i % columns) * spacing.x, -(i / columns) * spacing.y);
                var s = Instantiate(studentPrefab, transform);
                s.transform.localPosition = pos + pivotToSlotOffset;
                s.Init(i);
                _students.Add(s);
            }
        }

        public void Clear()
        {
            foreach (var s in _students) if (s != null) Destroy(s.gameObject);
            _students.Clear();
        }

        public Student Get(int id) => id >= 0 && id < _students.Count ? _students[id] : null;
    }
}
