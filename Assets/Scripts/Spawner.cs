using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject spawnParticles;
    [SerializeField] Animator beamAnimator;
    [SerializeField] bool triggererEnemy = false;
    [SerializeField] GameObject objectToTrigger;
    [SerializeField] bool triggerInstantly = false;
    public bool melee = false;
    public float triggerRadius;

    bool spawned = false;
    bool enemyInstantiated = false;
    bool particlesInstantiated = false;

    void Start()
    {
        beamAnimator.transform.gameObject.SetActive(false);
    }

    void Update()
    {
        if (triggerInstantly && !spawned)
        {
            beamAnimator.transform.gameObject.SetActive(true);
            spawned = true;
        }
        else if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < triggerRadius && !spawned)
        {
            beamAnimator.transform.gameObject.SetActive(true);
            spawned = true;
        }

        if (spawned)
        {
            if ((beamAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainLayer.Beam")))
            {
                //particles
                if (beamAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7 && !beamAnimator.IsInTransition(0) && !particlesInstantiated)
                {
                    Instantiate(spawnParticles, transform.position, spawnParticles.transform.rotation);
                    particlesInstantiated = true;
                }
                //enemy
                if (beamAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6 && !beamAnimator.IsInTransition(0) && !enemyInstantiated)
                {
                    GameObject instantion = Instantiate(enemyPrefab, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), enemyPrefab.transform.rotation);
                    if (melee)
                    {
                        instantion.transform.GetComponent<EnemyScript>().melee = true;
                    }
                    if (triggererEnemy)
                    {
                        instantion.transform.GetComponent<EnemyScript>().triggerTarget = objectToTrigger;
                    }
                    enemyInstantiated = true;
                }
                //over
                if (beamAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !beamAnimator.IsInTransition(0))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
