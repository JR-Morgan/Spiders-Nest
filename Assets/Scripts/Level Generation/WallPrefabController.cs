using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class WallPrefabController : MonoBehaviour
{
    [SerializeField]
    private WallType wallType = WallType.Solid;
    [SerializeField]
    private GameObject[] types;


    private void OnValidate()
    {
        foreach (GameObject t in types) t.SetActive(false);
        types[(int)wallType].SetActive(true);
    }


}
