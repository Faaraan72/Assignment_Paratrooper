using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jet : MonoBehaviour
{
    private float speed;
    public GameObject bombPrefab;
    private Transform shooterTransform;

    public void Initialize(float moveSpeed)
    {
        speed = moveSpeed;
        shooterTransform = GameController.instance.Shooter.transform;
    }
   
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (transform.position.x > 10f) // goes far out of screen
        {
            destroy_itself();
        }


    }
    
    void DropBomb()
    {
        if (bombPrefab != null && shooterTransform != null)
        {
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.GetComponent<Bomb>().Initialize(shooterTransform.position);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.name == "left")
        {
            Invoke(nameof(DropBomb), Random.Range(0.5f, 1.5f)); // to randomize its drop positon a lttlebit
        }
    }
    public void destroy_itself()
    {
        Destroy(gameObject);
    }
}
