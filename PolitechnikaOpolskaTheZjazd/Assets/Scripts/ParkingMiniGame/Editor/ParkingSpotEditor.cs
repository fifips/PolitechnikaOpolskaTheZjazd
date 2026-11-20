using UnityEngine;
using UnityEditor;

/// <summary>
/// Własny edytor komponentu ParkingSpot.
/// Przesuwa obiekt uchwytem w Scene View i przyciąga go
/// do środka najbliższej komórki przypisanego Grid.
/// </summary>
[CustomEditor(typeof(ParkingSpot))]
public class ParkingSpotEditor : Editor
{
    private void OnSceneGUI()
    {
        // W OnSceneGUI korzystamy z pojedynczego target,
        // a nie z tablicy targets.
        ParkingSpot spot = target as ParkingSpot;

        if (spot == null || spot.grid == null)
            return;

        Transform spotTransform = spot.transform;
        Vector3 currentPosition = spotTransform.position;

        // Wyświetlamy własny uchwyt przesuwania.
        EditorGUI.BeginChangeCheck();

        Vector3 requestedPosition = Handles.PositionHandle(
            currentPosition,
            Quaternion.identity
        );

        // Reagujemy tylko wtedy, gdy użytkownik przesunął uchwyt.
        if (!EditorGUI.EndChangeCheck())
            return;

        // Zamieniamy pozycję świata na komórkę Grid,
        // a następnie pobieramy środek tej komórki.
        Vector3Int cell = spot.grid.WorldToCell(requestedPosition);
        Vector3 snappedPosition = spot.grid.GetCellCenterWorld(cell);

        // Zachowujemy dotychczasową wysokość obiektu.
        snappedPosition.z = currentPosition.z;

        Undo.RecordObject(spotTransform, "Przesuń miejsce parkingowe");

        spotTransform.position = snappedPosition;

        EditorUtility.SetDirty(spotTransform);
        PrefabUtility.RecordPrefabInstancePropertyModifications(spotTransform);

        SceneView.RepaintAll();
    }
}
