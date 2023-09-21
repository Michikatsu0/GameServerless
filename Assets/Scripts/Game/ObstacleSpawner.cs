using UnityEngine;

public class ObstacleSpawner : ObstacleSpawnerBase
{
    [SerializeField]
    private GameObject[] obstaclePrefabs;

    protected GameObject[] ObstaclePrefabs { get => obstaclePrefabs; }

    protected int ObjectIndex
    {
        get
        {
            int result = 0;

            if (obstaclePrefabs.Length > 1)
            {
                result = Random.Range(result, obstaclePrefabs.Length);
            }

            return result;
        }
    }

    protected override void SpawnObject()
    {
        Instantiate(
            ObstaclePrefabs[ObjectIndex],                // Retrieves the prefab to instantiate
            new Vector2(Random.Range(MinX, MaxX), YPos), // Sets the position to instantiate in 2D (Z is always 0)
            Quaternion.identity);
    }
}