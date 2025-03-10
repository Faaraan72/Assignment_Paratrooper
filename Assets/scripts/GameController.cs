using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    public static GameController instance; // static instance of class 
    [Header("Shooter,Bullet and ts CHaracterstics")]
    public Transform Shooter;
    public float rotationSpeed = 90f;
    public float minRotation = -45f;
    public float maxRotation = 45f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;
    public int bulletPoolSize = 10;

    [Header("VFX")]
    public GameObject burstEffectPrefab;
    public GameObject bloodEffectPrefab;
    public GameObject blastEffectPrefab;
    public GameObject fireEffectPrefab;
    public GameObject parachuteDestroyEffect;

    [Header("SFX")]
    public AudioClip shootSound;
    public AudioClip burstSound;
    public AudioClip blastSound;
    public AudioClip paraT_ejectSound;
    public AudioClip gameoverSound;
    public AudioClip levelUPsound;
    public AudioClip hurtSound;

    [Header("Game Manager Variables")]
    public static int coin = 0;
    public static int diamonds = 0;
    public static int score = 0;
    public static int TowerHealth = 100;
    public static bool gameover;


    private Queue<GameObject> bulletPool = new Queue<GameObject>(); // Object Pooling of Bullets

    [Header("landed Paratroopers")]
    public List<GameObject> rightPara;
    public List<GameObject> leftPara;

    [Header("Demolition positions")]
    public Transform[] Left_Dpositions;
    public Transform[] Right_Dpositions;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //resetting some values
        coin = 0;
        diamonds = 0;
        score = 0;
        gameover = false;

        // Initialize object pool
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    // movement handling
    void Update()
    {
        if (!gameover)
        {
            HandleRotation();
            HandleShooting();
        }
    }
    #region Controls and shooting
    void HandleRotation()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        float newRotation = Shooter.transform.eulerAngles.z - rotationInput * rotationSpeed * Time.deltaTime;

        if (newRotation > 180) newRotation -= 360; // Normalize angle
        newRotation = Mathf.Clamp(newRotation, minRotation, maxRotation);
        Shooter.transform.rotation = Quaternion.Euler(0f, 0f, newRotation);
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shooter.GetComponent<Animator>().SetBool("shoot", true);
            Shoot();
        }
        else
        {
            Shooter.GetComponent<Animator>().SetBool("shoot", false);

        }
    }

    void Shoot()
    {
        if (bulletPool.Count > 0)
        {
            AudioManager.PlaySound(shootSound);
            GameObject bullet = bulletPool.Dequeue();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = bulletSpawnPoint.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<Bullet>().Initialize(bulletSpeed, this);
        }
    }
    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
    #endregion

    #region VFX & Score
    public void BurstEffect(Vector2 position)
    {
        GameObject effect = Instantiate(burstEffectPrefab, position, Quaternion.identity);
        AudioManager.PlaySound(burstSound);
        Destroy(effect, 0.33f);
        UI_Manager.instance.setScoreUI();
    }
    public void BloodEffect(Vector2 position)
    {
        GameObject effect = Instantiate(bloodEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 0.33f);
        UI_Manager.instance.setScoreUI();
        AudioManager.PlaySound(hurtSound);

    }

    // Tower Blast
    public void BlastEffect(Vector2 position)
    {
        GameObject effect = Instantiate(blastEffectPrefab, position, Quaternion.identity);
        AudioManager.PlaySound(blastSound);
        Destroy(effect, 0.33f);
        Shooter.GetComponent<SpriteRenderer>().color = Color.black;
        FireEffect(Shooter.transform.position);
        UI_Manager.instance.setScoreUI();

    }
    public void FireEffect(Vector2 position)
    {
        GameObject effect = Instantiate(fireEffectPrefab, position, Quaternion.identity);
        AudioManager.PlaySound(gameoverSound);
    }
    public void ParachuteDestroyEffect(Vector2 position)
    {
        GameObject effect = Instantiate(parachuteDestroyEffect, position, Quaternion.identity);
        Destroy(effect, 0.33f);
        UI_Manager.instance.setScoreUI();

    }
    
    #endregion

    #region Level Up
    public void StartSpawningJets()
    {
        HelicopterSpawner.instance.StartSpawningJet();  
    }
    #endregion

    #region GameOver and After
    public void checkGameover()
    {
        if(TowerHealth <= 0)
        {
            UI_Manager.instance.towerHealth.value = 0;
            gameover = true;
            UI_Manager.instance.setScoreUI();
        }
        if(rightPara.Count == 4 && !gameover) 
        {
            gameover = true;

            StartCoroutine(MoveToPositions(rightPara, Right_Dpositions, 2f));

        }
        else if (leftPara.Count == 4 && !gameover)
        {
            gameover = true;
            StartCoroutine(MoveToPositions(leftPara, Left_Dpositions, 2f));
        }
    }
    IEnumerator MoveToPositions(List<GameObject> objects, Transform[] positions, float speed)
    {
        for (int i = 0; i < 4; i++)
        {
            objects[i].GetComponent<Animator>().SetBool("run", true);
          Destroy(objects[i].GetComponent<Rigidbody2D>());
            if (i < positions.Length)
            {
                StartCoroutine(MoveObject(objects[i], positions[i].position, speed));

                if (i == 3 && positions.Length > 4) 
                {
                    yield return new WaitForSeconds(1f); 
                    StartCoroutine(MoveObject(objects[i], positions[4].position, speed));
                }
                yield return new WaitForSeconds(1f);
            }
        }
        ShowBlast();
    }

    IEnumerator MoveObject(GameObject obj, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.1f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        obj.GetComponent<Animator>().SetBool("run", false);
    }
    void ShowBlast()
    {
        BlastEffect(Shooter.transform.position);
    }
    #endregion
}
