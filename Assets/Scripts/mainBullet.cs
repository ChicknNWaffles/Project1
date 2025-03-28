using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public int damage = 1;
    public float lifetime = 3f;
    
    void Start()
    {
        // Destroy the bullet after lifetime seconds
        // Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (gameObject.CompareTag("chargeBullet")) {
            // Rotate around the Z-axis for 2D rotation
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // if the bullet is a chargeBullet, then trigger an explosion.
            if (gameObject.CompareTag("chargeBullet"))
            {

            }

                // Destroy the bullet
                Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DestroyBullet"))
        {
            Destroy(gameObject);
        }
    }
}