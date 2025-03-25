using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLimbComponent : MonoBehaviour
{
    public BossEnemy parentBoss;
    public int limbIndex;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            parentBoss.DamageLimb(limbIndex, bullet.damage);

            Destroy(collision.gameObject);
        }
    }
}