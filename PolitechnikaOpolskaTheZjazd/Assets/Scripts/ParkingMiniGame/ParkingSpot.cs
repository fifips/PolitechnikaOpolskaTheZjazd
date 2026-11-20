using UnityEngine;

[ExecuteAlways]
public class ParkingSpot : MonoBehaviour
{
    [Header("Wymiary miejsca")]
    [Tooltip("Rozmiar miejsca w kratkach przed obrotem, np. 3 x 2.")]
    [Min(1)]
    public Vector2Int size = new Vector2Int(3, 2);

    [Header("Obrót")]
    [Tooltip("0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°.")]
    [Range(0, 3)]
    public int rotationStep = 0;

    [Header("Siatka")]
    [Tooltip("Przeciągnij tutaj komponent Grid używany przez mapę.")]
    public Grid grid;

    [Header("Wygląd w edytorze")]
    public Color outlineColor = Color.green;
    public Color occupiedColor = Color.red;


    // Stan miejsca zmieniany przez ParkingManager.
    [SerializeField, HideInInspector]
    private bool isOccupied;

    public bool IsOccupied => isOccupied;


    // Zwraca wymiary miejsca po uwzględnieniu obrotu.
    public Vector2Int RotatedSize
    {
        get
        {
            return rotationStep % 2 == 0
                ? size
                : new Vector2Int(size.y, size.x);
        }
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneView.RepaintAll();
#endif
    }

    private void OnValidate()
    {
        size.x = Mathf.Max(1, size.x);
        size.y = Mathf.Max(1, size.y);

        rotationStep = ((rotationStep % 4) + 4) % 4;

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            rotationStep * 90f
        );
    }

    private void OnDrawGizmos()
    {
        if (grid == null)
            return;

        Vector2Int occupiedSize = RotatedSize;
        Vector3 cellSize = grid.cellSize;

        Vector3 localCenterOffset = new Vector3(
            (size.x - 1) * 0.5f * cellSize.x,
            (size.y - 1) * 0.5f * cellSize.y,
            0f
        );

        Vector3 worldCenter = transform.TransformPoint(localCenterOffset);

        Vector3 worldSize = new Vector3(
            occupiedSize.x * cellSize.x,
            occupiedSize.y * cellSize.y,
            0.05f
        );

        Gizmos.color = isOccupied ? occupiedColor : outlineColor;
        Gizmos.DrawWireCube(worldCenter, worldSize);
    }
}
