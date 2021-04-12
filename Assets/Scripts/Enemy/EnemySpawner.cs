using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float startTime = 60;
    [SerializeField]
    private float spawnTime = 60;
    [SerializeField]
    private int spawnSize = 5;
    [SerializeField]
    private float radius = 6;

    private float timeToSpawn;

    private TextMeshPro text;
    private new Camera camera;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        timeToSpawn = startTime;
    }

    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Update()
    {
        timeToSpawn -= Time.deltaTime;
        if(timeToSpawn <= 0)
        {
            SpawnEnemys(spawnSize);
            timeToSpawn = spawnTime;
        }
        text.text = ((int)timeToSpawn).ToString();
        text.transform.eulerAngles = new Vector3(0f, camera.transform.eulerAngles.y, 0f);
    }

    private void SpawnEnemys(int size)
    {
        for(int i = 0; i < size; i++)
        {
            EnemyManager.Instance.GetInitialisedEnemy(transform.position, Quaternion.identity);
        }
    }

}
