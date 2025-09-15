using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    public static StationManager Instance;
    private List<Station> allStations = new List<Station>();

    private void Awake()
    {
        Instance = this;
        allStations.AddRange(Object.FindObjectsByType<Station>(FindObjectsSortMode.None));
    }

    public List<Station> GetAllStations() => allStations;

    public List<Station> GetStationsByType(GameEnums.WorkstationType type)
    {
        return allStations.FindAll(s => s.workstationType == type);
    }
}