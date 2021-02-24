using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewLevelGenerator))]
public class LevelGenerateButton : MonoBehaviour
{
    NewLevelGenerator gen;

    public void Generate()
    {
        if(gen == null)
        {
            gen = GetComponent<NewLevelGenerator>();
        }
        gen.InstantiateWalls();
    }
}
