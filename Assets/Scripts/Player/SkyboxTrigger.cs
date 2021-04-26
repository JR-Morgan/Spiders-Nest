using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This <see cref="MonoBehaviour"/> will change a cameras background colour <see cref="OnTriggerEnter"/>
/// </summary>
[RequireComponent(typeof(Collider))]
public class SkyboxTrigger : MonoBehaviour
{
    [Tooltip("The desired background colour")]
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
