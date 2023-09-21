using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour
{
    [Header("Movement params")]
    [SerializeField]
    [Range(1F, 30F)]
    private float speed = 10F;

    [Header("Shoot params")]
    [SerializeField]
    protected Rigidbody[] bulletPrefabs;

    [SerializeField]
    protected Transform spawnPos;

    [SerializeField]
    [Range(0F, 500F)]
    protected float shootForce = 250F;

    [SerializeField]
    protected FixedJoystick joystick;
    private float
        hVal = 0F,
        minXPos,
        maxXPos,
        defaultYPos,
        validXPos;

    private bool buttonShot = false, changeButton = false;
    private int currentBulllet = 0;

    private Vector2 moveDirection;

    public uint Score { get; protected set; }

    protected abstract bool NoSelectedBullet { get; }

    protected abstract void Shoot();

    protected abstract void SelectBullet(int index);

    protected void UpdateScore(int scoreAdd) =>
        Score += (uint)System.Math.Abs(scoreAdd);

    protected void OnGameOver()
    {
        enabled = false;
    }

    protected virtual void Start()
    {
        float playerWidth = GetComponent<Collider>().bounds.size.x;

        maxXPos = GameUtils.GetScreenDimensions()
            .GetUseableScreenWidth(GameUtils.SCREEN_WIDTH_PERCENT) - playerWidth;
        minXPos = -maxXPos + playerWidth;

        defaultYPos = transform.position.y;
    }

    // Update is called once per frame
    private void Update()
    {
        hVal = joystick.Horizontal;
        moveDirection = (new Vector2(hVal, 0).normalized) * speed * Time.deltaTime;
        validXPos = Mathf.Clamp(transform.position.x + moveDirection.x, minXPos, maxXPos);

        transform.position = new Vector3(validXPos, defaultYPos, 0F);

        ProcessShooting();
    }

    public void ShotButton()
    {
        buttonShot = true;
    }
    public void ChangeButton()
    {
        changeButton = true;
        currentBulllet++;
        if (currentBulllet >= 3)
            currentBulllet = 0;
    }

    private void ProcessShooting()
    {
        if (buttonShot)
        {
            buttonShot = false;
            //Fire
            if (NoSelectedBullet)
            {
                SelectBullet(0);
            }

            if (spawnPos != null)
            {
                Shoot();
            }
        }

        if (changeButton)
        {
            SelectBullet(currentBulllet);
        }
    }
}