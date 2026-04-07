using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Campfire : MonoBehaviour
{
    private ParticleSystem fireParticles;
    private Light2D fireLight;

    private bool isActivated = false;

    private void Awake()
    {
        fireParticles = GetComponentInChildren<ParticleSystem>();
        fireLight = GetComponentInChildren<Light2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            //  Activate visuals only once
            if (!isActivated)
            {
                isActivated = true;

                fireParticles?.Play();
                if (fireLight != null)
                    fireLight.enabled = true;

                playerHealth.SetCheckpoint(transform.position);
            }

            //  Heal EVERY time
            playerHealth.RestoreFullHealth();

            PlayerMana playerMana = collision.gameObject.GetComponent<PlayerMana>();
            if (playerMana != null)
            {
                playerMana.RestoreFullMana();
            }
        }
    }
}