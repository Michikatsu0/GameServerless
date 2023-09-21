using UnityEngine;

public abstract class ObstacleSpawnerBase : MonoBehaviour
{
    [SerializeField]
    private bool debug;

    [SerializeField]
    [Range(0.5F, 2F)]
    private float instanceRate = 1.25F;

    protected float MinX { get; private set; }
    protected float MaxX { get; private set; }
    protected float YPos { get; private set; }

    protected abstract void SpawnObject();

    protected void OnGameOver()
    {
        CancelInvoke("SpawnObject");
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetMovementBounds();
        InvokeRepeating("SpawnObject", 1F, instanceRate);
    }

    private void SetMovementBounds()
    {
        if (debug)
        {
            MaxX = 0;
            MinX = 0;
            YPos = GameUtils.GetScreenDimensions().y;

            return;
        }

        // Retrieves the screen dimensions from camera to calculate a point in world coordinates matching the top right screen corner
        Vector2 bounds = GameUtils.GetScreenDimensions();

        // Only uses a fraction of the screen width to spawn objects
        MaxX = bounds.GetUseableScreenWidth(GameUtils.SCREEN_WIDTH_PERCENT);
        MinX = -MaxX;

        // Always spawn objects from top
        YPos = bounds.y;
    }
}