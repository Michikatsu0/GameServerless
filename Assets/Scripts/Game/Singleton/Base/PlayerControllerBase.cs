using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour
{
    [Header("Movement params")]
    [SerializeField]
    [Range(20F, 50F)]
    private float speed = 20F;

    [Header("Shoot params")]
    [SerializeField]
    protected Rigidbody[] bulletPrefabs;

    [SerializeField]
    protected Transform spawnPos;

    [SerializeField]
    [Range(0F, 500F)]
    protected float shootForce = 250F;

    private float
        hVal = 0F,
        minXPos,
        maxXPos,
        defaultYPos,
        validXPos;

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
        hVal = Input.GetAxis("Horizontal");
        moveDirection = (new Vector2(hVal, 0).normalized) * speed * Time.deltaTime;
        validXPos = Mathf.Clamp(transform.position.x + moveDirection.x, minXPos, maxXPos);

        transform.position = new Vector3(validXPos, defaultYPos, 0F);

        ProcessShooting();
    }

    private void ProcessShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectBullet(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectBullet(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectBullet(2);
        }
    }
}