using UnityEngine;

public class Helicopter : MonoBehaviour
{
    private float speed;
    private HelicopterSpawner spawner;
    public GameObject paratrooperPrefab;
    public bool spawn;
    int chances; // for randomizing chances of spawning a paratrooper on left or right
    public void Initialize(float moveSpeed, HelicopterSpawner spawnerReference,bool spawn_para)
    {
        speed = moveSpeed;
        spawner = spawnerReference;
        spawn = spawn_para;
        chances = Random.Range(0, 2);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (transform.position.x > 10f) 
        {
            spawner.ReturnToPool(gameObject);
        }
        //testing purpose
        //if ( Input.GetKeyDown(KeyCode.W)){
        //    spawnParatrooper();
        //}
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
       if(collision.gameObject.name == "left")
        {
            if (spawn && chances == 0)
            {
                Invoke(nameof(spawnParatrooper), Random.Range(0.5f, 1.75f)); //to randomize its droping positon a lttlebit
                Debug.Log("Left side");
                spawn = false;
            }
        }else if(collision.gameObject.name == "right")
        {
            if (spawn && chances == 1)
            {
                Invoke(nameof(spawnParatrooper), Random.Range(0.5f, 1.5f));
                Debug.Log("right side");
                spawn = false;
            }
        }
    }
    public void spawnParatrooper()
    {
        if (!GameController.gameover && gameObject.activeInHierarchy)
        {
            AudioManager.PlaySound(GameController.instance.paraT_ejectSound);
        GameObject g = Instantiate(paratrooperPrefab, transform.position, Quaternion.identity);
        g.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f, 1f) * 5f, ForceMode2D.Impulse); // give look of a jump from heli
        g.GetComponent<Paratrooper>().Initialize(chances==0);
        }
    }

}
