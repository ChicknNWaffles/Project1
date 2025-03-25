using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    //Audio Imports
    [SerializeField] private AudioClip shoot_sound;

    public int damage = 1;
    public float lifetime = 3f;

    void Start()
    {
        SoundFXManager.Instance.PlaySoundClip(shoot_sound, transform, 0.9f, 1f);

        // Destroy the bullet after lifetime seconds
        // Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bullet hit an enemy
        if (collision.CompareTag("Player"))
        {
            // Apply damage to the enemy
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
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