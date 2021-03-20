using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class WallFactoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPrefab, doorPrefab;


    private Dictionary<WallType, GameObject> map;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        map = new Dictionary<WallType, GameObject>
        {
            { WallType.Solid, wallPrefab },
            { WallType.Door, doorPrefab }
        };
    }

    public GameObject GetWall(WallType type) => map[type];


    public static WallFactoryManager Instance { get; private set; }
}
