using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(AOEDamage))]
public class BasicTrap : MonoBehaviour
{
    private AOEDamage damage;
    private Animator anim;

    #region Serialised Fields
    [SerializeField]
    [Range(0,1)]
    private float dutyCycle;
    [SerializeField]
    private float state;

    [SerializeField]
    private float frequency;

    #endregion

    void Awake()
    {
        damage = GetComponent<AOEDamage>();
        this.RequireComponentInChildren(out anim);
    }

    void Update()
    {
        AnimationUpdate();
    }

    void AnimationUpdate()
    {
        if (state > 1 / frequency) //If we should start the next cycle
        {
            state = 0f;
            
        }

        if (state == 0f) //If this is the start of the duty cycle
        {
            anim.speed = 1;
            damage.enabled = true;
        }
        else if (state > (1 / frequency) * dutyCycle) //if we have reached the end of the duty cycle
        {
            anim.speed = 0;
            damage.enabled = false;
        }



        state += Time.deltaTime;
    }

}
