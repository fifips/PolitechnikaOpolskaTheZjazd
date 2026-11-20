using System.Collections.Generic;
using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    [Header("Losowanie parkingu")]
    [Range(0, 100)]
    [Tooltip("Procent miejsc, które mają być zajęte.")]
    public int occupancyPercentage = 40;

    [Tooltip("Czy losować zajętość automatycznie po uruchomieniu gry.")]
    public bool randomizeOnStart = true;

    [Header("Wykryte miejsca")]
    [SerializeField]
    private List<ParkingSpot> parkingSpots = new List<ParkingSpot>();

    public IReadOnlyList<ParkingSpot> ParkingSpots => parkingSpots;

    [Header("Samochody")]
    public GameObject parkedCarPrefab;

    private List<GameObject> spawnedCars = new List<GameObject>();

    [Header("Niedokładne parkowanie")]
    [Range(0f, 0.5f)]
    public float maxPositionOffset = 0.2f;

    [Range(0f, 15f)]
    public float maxRotationOffset = 5f;

    private void Start()
    {
        FindParkingSpots();

        if (randomizeOnStart)
            RandomizeParking();
    }

    [ContextMenu("Znajdź miejsca parkingowe")]
    public void FindParkingSpots()
    {
        parkingSpots.Clear();

        // Wyszukujemy wszystkie ParkingSpot w załadowanej scenie,
        // również te znajdujące się na nieaktywnych obiektach.
        ParkingSpot[] foundSpots = FindObjectsOfType<ParkingSpot>(true);

        parkingSpots.AddRange(foundSpots);

        Debug.Log($"ParkingManager: znaleziono {parkingSpots.Count} miejsc parkingowych.", this);
    }

    [ContextMenu("Losuj zajętość parkingu")]
    public void RandomizeParking()
    {
        // Jeśli lista jest pusta, spróbujmy ponownie znaleźć miejsca.
        if (parkingSpots.Count == 0)
            FindParkingSpots();

        if (parkingSpots.Count == 0)
        {
            Debug.LogWarning("ParkingManager: nie znaleziono żadnych ParkingSpot.", this);
            return;
        }

        // Liczba zajętych miejsc jest dokładnie wyliczana z procentu.
        int occupiedCount = Mathf.RoundToInt(
            parkingSpots.Count * occupancyPercentage / 100f
        );

        // Tworzymy kopię listy, aby nie zmieniać kolejności głównej listy.
        List<ParkingSpot> shuffledSpots = new List<ParkingSpot>(parkingSpots);

        // Tasowanie Fishera-Yatesa.
        for (int i = shuffledSpots.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            ParkingSpot temp = shuffledSpots[i];
            shuffledSpots[i] = shuffledSpots[randomIndex];
            shuffledSpots[randomIndex] = temp;
        }

        // Najpierw zwalniamy wszystkie miejsca.
        foreach (ParkingSpot spot in parkingSpots)
        {
            if (spot != null)
                spot.SetOccupied(false);
        }

        // Zajmujemy dokładnie wylosowaną liczbę miejsc.
        for (int i = 0; i < occupiedCount; i++)
        {
            if (shuffledSpots[i] != null)
                shuffledSpots[i].SetOccupied(true);
        }

        // Generujemy zaparkowane samochody
        if (Application.isPlaying && parkedCarPrefab != null)
            SpawnParkedCars();

        Debug.Log(
            $"ParkingManager: zajęte {occupiedCount} z {parkingSpots.Count} miejsc " +
            $"({occupancyPercentage}%).",
            this
        );
    }

    void SpawnParkedCars()
    {
        // Usuwamy poprzednio wygenerowane samochody
        foreach (GameObject car in spawnedCars)
        {
            if (car != null)
                Destroy(car);
        }

        spawnedCars.Clear();

        foreach (ParkingSpot spot in parkingSpots)
        {
            if (!spot.IsOccupied)
                continue;

            // Losowe przesunięcie względem środka miejsca
            float randomX = Random.Range(-maxPositionOffset, maxPositionOffset);
            float randomY = Random.Range(-maxPositionOffset, maxPositionOffset);

            Vector3 offset = new Vector3(
                (spot.size.x - 1) * spot.grid.cellSize.x / 2f + randomX,
                (spot.size.y - 1) * spot.grid.cellSize.y / 2f + randomY,
                0f
            );

            Vector3 position = spot.transform.TransformPoint(offset);

            // Podstawowy obrót zgodny z miejscem parkingowym
            Quaternion rotation = spot.transform.rotation *
                                Quaternion.Euler(0f, 0f, 90f);

            // Lekko krzywe parkowanie
            float randomAngle = Random.Range(-maxRotationOffset, maxRotationOffset);

            rotation *= Quaternion.Euler(0f, 0f, randomAngle);

            GameObject car = Instantiate(parkedCarPrefab, position, rotation);

            spawnedCars.Add(car);
        }
    }
}

