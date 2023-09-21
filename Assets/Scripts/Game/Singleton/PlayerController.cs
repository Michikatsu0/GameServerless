using UnityEngine;

public class PlayerController : PlayerControllerBase
{
    [SerializeField]
    private UIManager uiManager;

    protected Rigidbody selectedBullet;

    protected override bool NoSelectedBullet => selectedBullet == null;

    protected override void Shoot()
    {
        Instantiate(selectedBullet, spawnPos.position, spawnPos.rotation)
                            .AddForce(transform.forward * shootForce, ForceMode.Force);
    }

    protected override void SelectBullet(int index)
    {
        selectedBullet = bulletPrefabs[GameUtils.GetClampedValue(index, bulletPrefabs.Length)];
        uiManager.SendMessage("EnableIcon", index);
    }
}