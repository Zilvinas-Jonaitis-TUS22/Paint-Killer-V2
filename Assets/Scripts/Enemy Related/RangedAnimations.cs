using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAnimations : MonoBehaviour
{
    public RangedEnemyAI rangedEnemyAI;

    public void ShootProjectile()
    {
        rangedEnemyAI.ShootProjectile();
    }

    public void StopShootingAnimation()
    {
        rangedEnemyAI.StopShooting();
    }
}
