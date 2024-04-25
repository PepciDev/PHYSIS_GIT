using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingSpear : MonoBehaviour
{
    float rayReach = 0.58f;
    float speed = 0.56f;
    bool move = true;
    float lifetime = 3f;
    bool shrink = false;
    float shrinkDelay = 0.26f;
    float shrinkMultiplier = 0.9f;
    int damage = -2;
    public ParticleSystem trailParticles;

    public LayerMask hitMask;

    public Vector3 targetPosition;
    public bool targetNull = true;

    void Start()
    {
        if (!targetNull)
        {
            transform.LookAt(targetPosition);
        }

        StartCoroutine("Lifetime");
    }

    void FixedUpdate()
    {
        RaycastHit checkHit;
        if (Physics.Raycast(transform.position, transform.forward, out checkHit, rayReach, hitMask) && move)
        {
            if (checkHit.transform.GetComponent<Health>() != null)
            {
                //Health script found - deal damage
                checkHit.transform.GetComponent<Health>().HPChange(damage);
            }
            else
            {
                //Hit default
            }

            StartCoroutine("ShrinkDelay");
            move = false;
            trailParticles.Stop();
        }

        //move forward
        if (move)
        {
            transform.Translate(Vector3.forward * speed, Space.Self);
        }
    }

    void Update()
    {
        if (shrink)
        {
            transform.localScale *= shrinkMultiplier;

            if (transform.localScale.y <= 0.1)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSecondsRealtime(lifetime);
        shrink = true;
    }

    IEnumerator ShrinkDelay()
    {
        yield return new WaitForSecondsRealtime(shrinkDelay);
        shrink = true;
    }
}
