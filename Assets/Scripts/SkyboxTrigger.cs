using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxTrigger : MonoBehaviour
{
    [SerializeField]
    private Color color;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(other.TryGetComponentInChildren(out Camera camera))
            {
                camera.backgroundColor = color;
            }
        }
    }
}
