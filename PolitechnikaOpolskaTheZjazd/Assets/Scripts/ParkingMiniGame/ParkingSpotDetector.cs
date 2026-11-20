
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ParkingSpotDetector : MonoBehaviour
{
    [Header("Parking")]
    [SerializeField] private ParkingManager parkingManager;

    [Header("Warunki zatrzymania")]
    [SerializeField] private float maxParkingSpeed = 0.2f;
    [SerializeField] private float requiredStopTime = 0.5f;

    public float Score { get; private set; }
    public bool IsParked { get; private set; }

    private Rigidbody2D rb;
    private BoxCollider2D carCollider;
    private float stoppedTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        carCollider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        if (IsParked || parkingManager == null)
            return;

        // Samochód musi się zatrzymać
        if (rb.linearVelocity.magnitude > maxParkingSpeed)
        {
            stoppedTime = 0f;
            return;
        }

        float bestScore = 0f;

        foreach (ParkingSpot spot in parkingManager.ParkingSpots)
        {
            if (spot == null || spot.IsOccupied || spot.grid == null)
                continue;

            // Wszystkie cztery rogi samochodu muszą być wewnątrz miejsca
            if (!IsFullyInside(spot))
                continue;

            float score = CalculateScore(spot);
            bestScore = Mathf.Max(bestScore, score);
        }

        if (bestScore <= 0f)
        {
            stoppedTime = 0f;
            return;
        }

        stoppedTime += Time.fixedDeltaTime;

        if (stoppedTime >= requiredStopTime)
        {
            Score = bestScore;
            IsParked = true;

            Debug.Log($"SUCCESS! Zaparkowano! Score: {Score:F2}/100");
        }
    }

    // Sprawdza, czy cały BoxCollider2D mieści się na miejscu
    bool IsFullyInside(ParkingSpot spot)
    {
        Vector2 cellSize = spot.grid.cellSize;

        float centerX = (spot.size.x - 1) * cellSize.x / 2f;
        float centerY = (spot.size.y - 1) * cellSize.y / 2f;

        float halfWidth = spot.size.x * cellSize.x / 2f;
        float halfHeight = spot.size.y * cellSize.y / 2f;

        Vector2 halfCarSize = carCollider.size / 2f;

        // Sprawdzamy wszystkie cztery rogi collidera
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                Vector3 corner = new Vector3(
                    carCollider.offset.x + x * halfCarSize.x,
                    carCollider.offset.y + y * halfCarSize.y,
                    0f
                );

                // Róg samochodu we współrzędnych świata
                Vector3 worldCorner =
                    carCollider.transform.TransformPoint(corner);

                // Ten sam punkt we współrzędnych miejsca parkingowego
                Vector3 localCorner =
                    spot.transform.InverseTransformPoint(worldCorner);

                if (Mathf.Abs(localCorner.x - centerX) > halfWidth ||
                    Mathf.Abs(localCorner.y - centerY) > halfHeight)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Ocena jakości parkowania od 0 do 100
    float CalculateScore(ParkingSpot spot)
    {
        Vector2 cellSize = spot.grid.cellSize;

        Vector2 spotCenter = new Vector2(
            (spot.size.x - 1) * cellSize.x / 2f,
            (spot.size.y - 1) * cellSize.y / 2f
        );

        Vector3 worldCarCenter =
            carCollider.transform.TransformPoint(carCollider.offset);

        Vector3 localCarCenter =
            spot.transform.InverseTransformPoint(worldCarCenter);

        // 1. Ocena wyśrodkowania
        float distance = Vector2.Distance(
            new Vector2(localCarCenter.x, localCarCenter.y),
            spotCenter
        );

        Vector2 halfSpotSize = new Vector2(
            spot.size.x * cellSize.x / 2f,
            spot.size.y * cellSize.y / 2f
        );

        float maxDistance = halfSpotSize.magnitude;

        float positionScore = 1f - Mathf.Clamp01(
            distance / maxDistance
        );

        // 2. Ocena kąta samochodu
        // Prefab samochodu jest dłuższy w osi Y,
        // a standardowe miejsce parkingowe w osi X.
        float targetAngle = spot.transform.eulerAngles.z + 90f;
        float carAngle = carCollider.transform.eulerAngles.z;

        float angleDifference = Mathf.Abs(
            Mathf.DeltaAngle(carAngle, targetAngle)
        );

        // Parkowanie przodem i tyłem jest równie poprawne
        angleDifference = Mathf.Min(
            angleDifference,
            180f - angleDifference
        );

        float rotationScore = 1f - angleDifference / 180f;

        // 3. Wynik końcowy
        return 100f * positionScore * rotationScore;
    }

    private void OnGUI()
    {
        if (!IsParked)
            return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 32;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.green;

        float width = 500f;
        float height = 80f;

        Rect rect = new Rect(
            (Screen.width - width) / 2f,
            30f,
            width,
            height
        );

        GUI.Box(rect, "SUKCES! ZAPARKOWANO!", style);
    }

}
