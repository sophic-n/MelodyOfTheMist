using UnityEngine;

public class PlayerFluteController : MonoBehaviour
{
    private Animator anim;
    private float idleTimer;
    private float idleSwitchInterval;

    void Awake()
    {
        anim = GetComponent<Animator>();
        ResetIdleTimer();
    }

    void Update()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            // Random chance to switch to Idle2
            if (Random.value < 0.3f) // 30% chance
            {
                anim.SetTrigger("idle2");
            }

            ResetIdleTimer();
        }
    }

    void ResetIdleTimer()
    {
        idleTimer = Random.Range(4f, 8f); // Check every 4-8 seconds
    }

    public void PlayAttackAnimation()
    {
        anim.SetTrigger("attack");
    }

    public void PlayDefeatAnimation()
    {
        anim.SetTrigger("defeat");
    }

    public void PlayLayAnimation()
    {
        anim.SetTrigger("lay");
    }
}
