using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] GameObject breakParticles;
    bool dead = false;
    bool shrinking = false;
    float shrinkSpeed = 0.1f;
    float deathSpeed = 0.2f;
    [SerializeField] float hitSizeMultiplier;
    Vector3 orginalSize;
    Vector3 targetSize;

    private void Start()
    {
        orginalSize = transform.localScale;
    }

    void Update()
    {
        if (!dead)
        {
            if (shrinking)
            {
                if (transform.localScale.y > orginalSize.y * hitSizeMultiplier)
                {
                    //shrink
                    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed);
                }
                else
                {
                    shrinking = false;
                }
            }
            else
            {
                //grow
                transform.localScale = Vector3.Lerp(transform.localScale, orginalSize, shrinkSpeed);
            }
        }
        else
        {
            //shrink to 0
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, deathSpeed);

            if (transform.localScale.y < orginalSize.y / 8f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage()
    {
        //take damage
        if (!dead)
        {
            Instantiate(breakParticles, transform.position, breakParticles.transform.rotation);
            shrinking = true;
        }
    }

    public void Die()
    {
        dead = true;
    }
}
