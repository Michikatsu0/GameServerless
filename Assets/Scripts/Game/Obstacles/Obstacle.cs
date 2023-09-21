public abstract class Obstacle : ObstacleBase
{
    private static GameController gameController;

    protected override GameControllerBase GameController
    {
        get
        {
            if (gameController == null)
            {
                gameController = FindObjectOfType<GameController>();
            }

            return gameController;
        }
    }

    protected override void DestroyObstacle(bool notify = false)
    {
        if (notify)
        {
            GameController?.SendMessage("OnObstacleDestroyed", HP);
        }

        Destroy(gameObject);
    }
}