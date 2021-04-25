using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class EnemySpawner : MonoBehaviourPun
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

    private void Awake()
    {
        this.RequireComponentInChildren(out text);
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
        if(timeToSpawn <= 0 && PlayerManager.IsMasterOrOffline)
        {
            SpawnEnemys(spawnSize);
        }
        text.text = ((int)Mathf.Max(timeToSpawn, 0)).ToString();

        if(PlayerManager.IsInitialised && PlayerManager.Instance.Local != null && PlayerManager.Instance.Local.Camera != null)
            text.transform.eulerAngles = new Vector3(0f, PlayerManager.Instance.Local.Camera.transform.eulerAngles.y, 0f);
    }

    private void SpawnEnemys(int size)
    {
        for (int i = 0; i < size; i++)
        {
            EnemyManager.Instance.CreateEnemy(out _, transform.position);
        }
        timeToSpawn = spawnTime;
    }

}
