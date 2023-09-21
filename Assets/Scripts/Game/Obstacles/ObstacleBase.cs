using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class ObstacleBase : MonoBehaviour
{
    [SerializeField, Layer]
    private int layerToCollideWith;

    [SerializeField]
    [Range(1, 3)]
    private int hp = 1;

    private int remainingHP;

    protected int HP { get => hp; }

    protected virtual void Start()
    {
        remainingHP = hp;
    }

    protected abstract GameControllerBase GameController { get; }

    protected abstract void DestroyObstacle(bool notify = false);

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer(GameUtils.KILLVOLUME_LAYER_NAME)))
        {
            DestroyObstacle(false);
        }
        else
        {
            int collisionLayer = collision.gameObject.layer;
            Destroy(collision.gameObject);

            if (collisionLayer.Equals(LayerMask.NameToLayer(GameUtils.BULLET_HARD_LAYER_NAME)))
            {
                remainingHP -= GameUtils.BULLET_HARD_PWR;
            }
            else if (collisionLayer.Equals(LayerMask.NameToLayer(GameUtils.BULLET_MID_LAYER_NAME)))
            {
                remainingHP -= GameUtils.BULLET_MID_PWR;
            }
            else
            {
                remainingHP -= GameUtils.BULLET_LOW_PWR;
            }

            if (remainingHP < 1)
            {
                DestroyObstacle(true);
            }
        }
    }
}