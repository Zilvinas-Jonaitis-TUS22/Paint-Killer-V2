using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUI : MonoBehaviour
{
    public Animator animator;
    public bool bossDead;

    void OnTriggerEnter(Collider other)
    {
        if (animator != null && !bossDead)
        {
            animator.SetBool("Dropped", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (animator != null)
        {
            animator.SetBool("Dropped", false);
        }
    }

    private void Update()
    {
        if(bossDead)
        {
            animator.SetBool("Dropped", false);
        }
    }
}
