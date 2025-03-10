using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Vector2 targetPosition;
    public float fallSpeed = 5f;
    public GameObject explosionEffect;
    public int bombDamage=5;
    
    //initialising the bomb 
    public void Initialize(Vector2 target)
    {
        targetPosition = target;
    }

    
    //Movement of the bomb towards target
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);
    }

    //trigger for checking if it hit the shooter or not
    void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.CompareTag("shooter"))
        {
            GameController.TowerHealth -= bombDamage;
            GameController.instance.checkGameover();
        }
        if (collision.CompareTag("Ground"))
        {
            Explode();
        }
       
    }

    //VFX for Explosion
    void Explode()
    {
        if (explosionEffect != null)
        {
            AudioManager.PlaySound(GameController.instance.blastSound);
            GameObject g = Instantiate(explosionEffect, transform.position, Quaternion.identity).gameObject;
            Destroy(g, 0.33f);
        }
        Destroy(gameObject);
    }
}
