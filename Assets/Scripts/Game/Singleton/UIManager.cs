using UnityEngine;

public sealed class UIManager : UIManagerBase
{
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private GameControllerBase gameController;

    protected override PlayerControllerBase PlayerController => playerController;

    protected override GameControllerBase GameController => gameController;
}