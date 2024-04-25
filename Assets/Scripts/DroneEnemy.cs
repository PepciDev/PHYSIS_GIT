using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemy : MonoBehaviour
{
    AudioSource droneAudio;
    public AudioClip gunshot;

    public LayerMask rayLayer;
    public float enemyHealth = 2f;
    public GameObject deathParticle;
    public GameObject turret;
    GameObject player;
    bool canShoot = true;

    float speed = 0.005f;

    int maxPickups = 4;

    int chanceToLoot = 3;
    public GameObject healthPickup;

    public GameObject spawnParticle;

    public GameObject projectile;

    float shootDelay = 1f;

    private void Start()
    {
        droneAudio = GetComponent<AudioSource>();

        //spawnParticles
        Instantiate(spawnParticle, transform.position, transform.rotation);

        //random speed
        speed = Random.Range(0.004f, 0.012f);

        //randomize shooting frequency
        shootDelay = Random.Range(0.6f, 2.2f);

        player = GameObject.Find("Boss");
    }

    private void LateUpdate()
    {
        //aim rotation
        turret.transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z));
    }

    void Update()
    {

        //kuolema health
        if (enemyHealth <= 0f)
        {
            Instantiate(deathParticle, turret.transform.position, turret.transform.rotation);
            Destroy(this.gameObject);
            

            int randomnumber = Random.Range(1, chanceToLoot + 1);
            if (randomnumber == chanceToLoot && GameObject.FindGameObjectsWithTag("Pickup").Length <= maxPickups)
            {
                Instantiate(healthPickup, transform.position, healthPickup.transform.rotation);
            }
        }

        //raycat pelaajaan
        RaycastHit objectHit;
        Vector3 fwd = turret.transform.TransformDirection(Vector3.forward);
        /*if (Physics.Raycast(this.transform.position, fwd, out objectHit, rayLayer))
        {
            //do something if ray hits object
            if (objectHit.transform.tag == "Player" && player.GetComponent<PlayerController>().gameOver == false)
            {
                //moveTowardplayer
                transform.position = Vector3.MoveTowards(transform.position, objectHit.point, speed);

                if (canShoot)
                {
                    //startShoot
                    StartCoroutine("ShootCooldown");
                }
            }
        }*/
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSecondsRealtime(shootDelay);
        //shoot
        Instantiate(projectile, turret.transform.position, turret.transform.rotation);
        droneAudio.pitch = Random.Range(1.4f, 2.4f);
        droneAudio.PlayOneShot(gunshot);
        canShoot = true;
    }
}
