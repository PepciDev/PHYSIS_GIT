using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float rayReach = 0.32f;
    float speed = 0.3f;
    bool move = true;
    float lifetime = 3f;
    bool shrink = false;
    float shrinkMultiplier = 0.85f;
    public ParticleSystem trailParticles;
    float damage = -1f;

    public LayerMask hitMask;

    public Vector3 targetPosition;

    void Start()
    {
        //look at target
        transform.LookAt(targetPosition);

        StartCoroutine("Lifetime");
    }

    void FixedUpdate()
    {
        RaycastHit checkHit;
        if (Physics.Raycast(transform.position, transform.forward, out checkHit, rayReach, hitMask))
        {
            if (checkHit.transform.GetComponent<Health>() != null)
            {
                if (checkHit.transform.GetComponent<Health>().playerHealth == true)
                {
                    checkHit.transform.GetComponent<Health>().HPChange(damage);
                    Destroy(gameObject);
                }
            }
            else
            {
                //hitting "default"
            }

            shrink = true;
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
}

