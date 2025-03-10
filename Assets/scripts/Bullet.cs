using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private GameController shooter;

    //initialsing the bullet
    public void Initialize(float moveSpeed, GameController shooterReference)
    {
        speed = moveSpeed;
        shooter = shooterReference;
    }

    //its movement and Object Pooling
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        if (transform.position.y > 10f) // if it goes far away from the screen
        {
            shooter.ReturnBulletToPool(gameObject);
        }
    }

    // trigger for checking different hits 
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit something"+ collision.gameObject.name);

        if (collision.CompareTag("Helicopter"))
        {
            Debug.Log("Hit Helicopter");
            GameController.coin += 5;
            shooter.BurstEffect(transform.position);

            collision.gameObject.SetActive(false);
            HelicopterSpawner.instance.ReturnToPool(collision.gameObject);

            shooter.ReturnBulletToPool(gameObject);
        }
        if (collision.CompareTag("paratrooper"))
        {
            GameController.coin += 10;
            Destroy(collision.gameObject);
            shooter.BloodEffect(transform.position);
            shooter.ReturnBulletToPool(gameObject);

        }
        if (collision.CompareTag("parachute"))
        {
            GameController.coin += 10;
            collision.GetComponentInParent<Paratrooper>().closeParachute();
            shooter.ParachuteDestroyEffect(transform.position);
            shooter.ReturnBulletToPool(gameObject);

        }
        if (collision.CompareTag("jet"))
        {
            GameController.diamonds += 10;
            shooter.BurstEffect(transform.position);
            collision.GetComponent<Jet>().destroy_itself();
            shooter.ReturnBulletToPool(gameObject);

        }
        if (collision.CompareTag("bomb"))
        {
            GameController.diamonds += 10;
            Destroy(collision.gameObject);
            shooter.BurstEffect(transform.position);
            shooter.ReturnBulletToPool(gameObject);

        }
    }
}
