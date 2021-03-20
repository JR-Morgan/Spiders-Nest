using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleController : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve scale;
    [Tooltip("Time in seconds")]
    [SerializeField]
    private float lifeSpan = 5;

    private float time;

    void Update()
    {
        time += Time.deltaTime;
        if(time > lifeSpan)
        {
            Destroy(gameObject);
        }
        else
        {
            float s = scale.Evaluate(time/lifeSpan);
            this.transform.localScale = new Vector3(s, 1f, s);
        }
        
    }



}
