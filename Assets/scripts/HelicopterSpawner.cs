using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterSpawner : MonoBehaviour
{
    public static HelicopterSpawner instance;
    [Header("Prefabs")]
    public GameObject helicopterPrefab; 
    public GameObject JetPrefab;

    [Header("Helicopter Settings")]
    public int poolSize = 5;
    public float spawnInterval = 2f;
    public float minY = 3f, maxY = 6f; // Random spawn height range
    public float speed = 3f;
    public int spawnPrecentage = 70;
    private Queue<GameObject> helicopterPool = new Queue<GameObject>();
    private bool canSpawn = false;

    [Header("Jet Settings")]
    public float JetSpeed =10f;
    private bool canSpawnJet = false;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        // Initialize object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject helicopter = Instantiate(helicopterPrefab);
            helicopter.SetActive(false);
            helicopterPool.Enqueue(helicopter);
        }
    }

    public void StartSpawningHelicopters()
    {
        if (!canSpawn)
        {
            canSpawn = true;
            StartCoroutine(SpawnHelicoptersCoroutine()); // start spawning the helis;
        }
    }
    public void StartSpawningJet()
    {
        if (!canSpawnJet)
        {
            canSpawnJet = true;
            StartCoroutine(SpawnJetCoroutine());//start spawning the jets;
        }
    }
    IEnumerator SpawnHelicoptersCoroutine()
    {
        while (canSpawn && !GameController.gameover)
        {
            SpawnHelicopter();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    IEnumerator SpawnJetCoroutine()
    {
        while (canSpawnJet && !GameController.gameover)
        {
            SpawnJet();
            yield return new WaitForSeconds(6f);
        }
    }
    public void SpawnJet()
    {
        GameObject jet = Instantiate(JetPrefab);
        jet.transform.position = new Vector2(-10f, Random.Range(minY, maxY)); 
        jet.GetComponent<Jet>().Initialize(JetSpeed); 
    }
    void SpawnHelicopter()
    {
        if (helicopterPool.Count > 0)
        {
            GameObject helicopter = helicopterPool.Dequeue();
            helicopter.transform.position = new Vector2(-10f, Random.Range(minY, maxY));
            helicopter.SetActive(true);
            bool spawn_para = (Random.Range(0, 100) >= spawnPrecentage);
            helicopter.GetComponent<Helicopter>().Initialize(speed, this,spawn_para);
        }
    }

    public void ReturnToPool(GameObject helicopter)
    {
        helicopter.SetActive(false);
        helicopterPool.Enqueue(helicopter);
    }
}
