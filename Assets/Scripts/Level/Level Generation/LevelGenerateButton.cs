using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewLevelGenerator))]
public class LevelGenerateButton : MonoBehaviour
{
    private NewLevelGenerator _gen;
    private NewLevelGenerator Gen { get
        {
            if (_gen == null)
            {
                _gen = GetComponent<NewLevelGenerator>();
            }
            return _gen;
        }
    }

    public void GenerateRooms()
    {
        Gen.InstantiateAll();
    }

}
