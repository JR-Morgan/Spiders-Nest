using UnityEngine;

/// <summary>
/// Enables and disables child animations or particle system on a duty cycle
/// </summary>
[SelectionBase]
[RequireComponent(typeof(AOEDamage))]
public class BasicTrap : MonoBehaviour
{
    private AOEDamage damage;
    private Animator anim;
    private ParticleSystem particles;

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
        anim = this.GetComponentInChildren<Animator>();
        particles = this.GetComponentInChildren<ParticleSystem>();
        Pause();
    }

    void Update()
    {
        AnimationUpdate();
    }

    private void AnimationUpdate()
    {
        if (state > 1 / frequency) //If we should start the next cycle
        {
            state = 0f;
            
        }

        if (state == 0f) //If this is the start of the duty cycle
        {
            Play();
        }
        else if (state > (1 / frequency) * dutyCycle) //if we have reached the end of the duty cycle
        {
            Pause();
        }



        state += Time.deltaTime;
    }

    private void Play()
    {
        if (anim != null) anim.speed = 1;
        if (particles != null) particles.Play();
        damage.enabled = true;
    }

    private void Pause()
    {
        if (anim != null) anim.speed = 0;
        if (particles != null) particles.Stop();
        damage.enabled = false;
    }

}
