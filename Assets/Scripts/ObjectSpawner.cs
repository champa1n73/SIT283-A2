using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private static List<SteelBarController> spawnedSteelBars = new List<SteelBarController>();


    // SETTERS
    public static void AddSpawnedSteelBar(SteelBarController steelBar) => spawnedSteelBars.Add(steelBar);
    public static void RemoveSpawnedSteelBar(SteelBarController steelBar) => spawnedSteelBars.Remove(steelBar);


    // GETTERS
    public static List<SteelBarController> GetSpawnedSteelBars() => spawnedSteelBars;

    public void SpawnObject()
    {
        if (prefab != null)
        {
            GameObject spawnObject = Instantiate(prefab, transform.position, Quaternion.identity);
            spawnObject.transform.up = transform.up;

            SteelBarController steelBarController = spawnObject.GetComponent<SteelBarController>();
            if (steelBarController != null)
            {
                spawnedSteelBars.Add(steelBarController);
            }
        }
    }
}
