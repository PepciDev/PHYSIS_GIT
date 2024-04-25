using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Rigidbody enemyRigidbody;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    PlayerController playerC;
    [SerializeField] Animator enemyAnimator;

    public GameObject triggerTarget;

    Vector3 moveVector;
    Vector3 turnVector;
    [SerializeField] Transform shoulderTransform;
    float gravity = -2f;

    bool playerSeen = false;
    bool seenLastFrame = false;
    bool patrol = false;
    Vector3 patrolDestination;

    bool pursue = false;
    bool follow = false;
    float pursueTreshold = 4f;
    float pursueDistance = 1f;
    int patrolBounds = 4;
    Vector3 pursueDestination;

    int slashDamage = -2;
    float slashRadius = 1f;
    [SerializeField] Transform slashPosition;
    float slashDamageDelay = 0.24f;

    bool shooting = true;
    float shootingCancelCooldown = 2.5f;
    bool cantShoot = false;
    float cantShootTime = 0;
    float shootDelay = 2f;
    [SerializeField] Transform shootPosition;

    [SerializeField] GameObject projectile;

    float walkingSpeed = 2f;
    float turningSpeed = 0.1f;

    [SerializeField] GameObject gun;
    [SerializeField] GameObject katana;

    int animationStateVariable = 10;
    public bool melee = false;
    bool exhaust = false;
    bool slash = false;
    bool meleePursue = true;
    float minSlashDistance = 2f;
    bool fullBodyAnimations = false;
    float recoilTime = 0.8f;
    bool recoiling = false;

    [SerializeField] SkinnedMeshRenderer shaderRenderer;
    float targetGlowStrength = 0.1f;

    bool dead = false;
    bool spawn = true;
    bool staggered = true;
    [SerializeField] GameObject deathParticles;

    [SerializeField] GameObject testCube;

    //Animation states

    void Start()
    {


        //melee or gun
        if (melee)
        {
            katana.SetActive(true);
            gun.SetActive(false);
            //health
            GetComponent<Health>().health = 3f;
        }
        else
        {
            katana.SetActive(false);
            gun.SetActive(true);
            //health
            GetComponent<Health>().health = 2f;
        }

        //shoot delay
        shootDelay = Random.Range(1.6f, 2.4f);

        //Spawn
        SendMessage("Invincible");
        StartCoroutine("DamageFx");

        //Shader
        shaderRenderer.material.SetFloat("_Strength", targetGlowStrength);

        //move vector set: grvity affects enemy
        moveVector = new Vector3(0f, gravity, 0f);

        //randomize walking speed
        walkingSpeed = Random.Range(2f, 3f);

        enemyRigidbody = GetComponent<Rigidbody>();
        playerC = GameObject.Find("Player").GetComponent<PlayerController>();

    }

    void Update()
    {
        //shader contols
        if ((shaderRenderer.material.GetFloat("_Strength") < (targetGlowStrength - 0.01f) || shaderRenderer.material.GetFloat("_Strength") > (targetGlowStrength + 0.01f)))
        {
            shaderRenderer.material.SetFloat("_Strength", Mathf.Lerp(shaderRenderer.material.GetFloat("_Strength"), targetGlowStrength, 0.3f));
            //shaderMaterial.SetFloat("_Strength", targetGlowStrength);
        }

        //cant shoot for a while after taking damage
        if (cantShoot)
        {
            if (cantShootTime <= 0f)
            {
                cantShoot = false;
            }
            else
            {
                cantShootTime -= (1f * Time.deltaTime);
            }
        }

        //ALIVE
        if (!dead)
        {

            //taking damage - staggered

            if (!staggered)
            {
                // BIG THREAD

                //slashing -> exhaust
                if (slash)
                {
                    pursue = false;
                    fullBodyAnimations = true;
                    enemyAnimator.SetInteger("AnimationInt", 9);

                    if ((enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainLayer.KatanaSlash"))
                        && (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !enemyAnimator.IsInTransition(0)))
                    {
                        exhaust = true;
                        staggered = true;
                        slash = false;
                        enemyAnimator.SetInteger("AnimationInt", 4);
                    }
                }

                //check player
                CheckPlayer();

                //patrol
                if (!playerSeen && patrol)
                {
                    Patrol();
                }

                //TURN
                if (!slash)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(turnVector), turningSpeed);
                }

                //motionless
                if (!patrol && !pursue && !slash)
                {
                    moveVector = new Vector3(0, gravity, 0);
                    if (!playerSeen && !recoiling)
                    {
                        if (melee)
                        {
                            enemyAnimator.SetInteger("AnimationInt", 1);
                        }
                        else
                        {
                            enemyAnimator.SetInteger("AnimationInt", animationStateVariable);
                        }
                    }
                    enemyAnimator.SetInteger("LowerBodyInt", 1);
                }
                else if ((enemyRigidbody.velocity.x > 0.1f || enemyRigidbody.velocity.x < -0.1f) || (enemyRigidbody.velocity.z > 0.1f || enemyRigidbody.velocity.z < -0.1f))
                {
                    //moving
                    if (!playerSeen && !recoiling)
                    {
                        if (melee && !slash)
                        {
                            enemyAnimator.SetInteger("AnimationInt", 2);
                        }
                        else
                        {
                            enemyAnimator.SetInteger("AnimationInt", 1 + animationStateVariable);
                        }
                    }
                    if (melee && !slash)
                    {
                        enemyAnimator.SetInteger("LowerBodyInt", 3);
                    }
                    else
                    {
                        enemyAnimator.SetInteger("LowerBodyInt", 2);
                    }
                }
            }
        }

        //control lowerbody animation weight
        if (fullBodyAnimations)
        {
            enemyAnimator.SetLayerWeight(1, 0);
        }
        else
        {
            enemyAnimator.SetLayerWeight(1, 1);
        }

        if (staggered)
        {
            //dead / damage - stagger stop
            if (spawn)
            {
                if ((enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainLayer.Spawn"))
                    && (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !enemyAnimator.IsInTransition(0)))
                {
                    staggered = false;
                    fullBodyAnimations = false;
                    spawn = false;
                    SendMessage("Mortal");

                    //start patrolling
                    StartCoroutine("PatrolPause");
                }
            }
            else
            {
                slash = false;

                if (exhaust)
                {
                    //after crouch animation-> getup animation and stop staggering
                    if ((enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainLayer.Crouch"))
                        && (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !enemyAnimator.IsInTransition(0)))
                    {
                        enemyAnimator.SetInteger("AnimationInt", 6);
                        //Debug.Log("Exhaust started");
                    }

                    if ((enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainLayer.GetUp"))
                        && (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !enemyAnimator.IsInTransition(0)))
                    {
                        staggered = false;
                        exhaust = false;
                        fullBodyAnimations = false;
                        meleePursue = true;
                        //Debug.Log("Exhaust successful");
                    }
                }
                else
                {
                    //stop staggering after animation
                    if ((enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainLayer.Stagger"))
                        && (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !enemyAnimator.IsInTransition(0)))
                    {
                        staggered = false;
                        fullBodyAnimations = false;

                        if (melee)
                        {
                            exhaust = false;
                            meleePursue = true;
                        }
                    }
                }
            }
        }

        //ground check - gravity - etc etc
        GroundCheck();

        //MOVE
        enemyRigidbody.velocity = moveVector;

        //death
        if (dead)
        {
            Death();
        }
    }

    public void Death()
    {
        //animation and fx
        fullBodyAnimations = false;
        enemyAnimator.SetInteger("AnimationInt", 4);
        moveVector = new Vector3(0, gravity, 0);

        targetGlowStrength = 3f;

        if ((enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("MainLayer.Crouch"))
            && (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enemyAnimator.IsInTransition(0)))
        {
            //after animation - die
            Instantiate(deathParticles, transform.position, deathParticles.transform.rotation);
            Destroy(gameObject);
        }
    }

    public void TakeDamage()
    {
        //take damage
        if (!dead)
        {
            StartCoroutine("DamageFx");
            //cant shoot for a while - adding more cooldown time to the timer according to remainig time
            if (cantShootTime < 2)
            {
                cantShootTime += 4f;
            }
            else
            {
                cantShootTime += 1.5f;
            }
            cantShoot = true;

        }
    }

    IEnumerator DamageFx()
    {
        //stop exhaust if true
        if (!exhaust)
        {
            // effects. etc - stagger animation
            fullBodyAnimations = true;
            staggered = true;
            moveVector = new Vector3(0, gravity, 0);
            //spawn / damaged
            if (spawn)
            {
                enemyAnimator.SetInteger("AnimationInt", 0);
            }
            else
            {
                enemyAnimator.SetInteger("AnimationInt", -1);
            }
        }

        targetGlowStrength = 3f;
        yield return new WaitForSecondsRealtime(0.18f);
        targetGlowStrength = 0.1f;
    }

    public void Die()
    {
        dead = true;

        if (triggerTarget != null)
        {
            triggerTarget.GetComponent<Trigger>().Triggered();
        }
    }

    void Shoot()
    {
        //shoot at player
        if (shooting)
        {
            //raycast from enemy to the gun
            Vector3 projectileRayPosition = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            float spearCheckDistance = Vector3.Distance(projectileRayPosition, shootPosition.position);

            RaycastHit spearRayHit;
            if (Physics.Raycast(projectileRayPosition, shootPosition.position - projectileRayPosition, out spearRayHit, spearCheckDistance, playerLayer))
            {
                //instantiate bullet to the hit point / prevents shooting through walls
                Vector3 projectileCheckPos = Vector3.Lerp(projectileRayPosition, spearRayHit.point, 0.5f);

                Vector3 forwardOnXZ = transform.forward;
                //forwardOnXZ.y = playerC.transform.position.y;
                GameObject instatiant = Instantiate(projectile, projectileCheckPos, transform.rotation);
                instatiant.GetComponent<Bullet>().targetPosition = playerC.transform.position;

            }
            else
            {
                //instantiate bullet to the orginal throw position

                Vector3 forwardOnXZ = transform.forward;
                //forwardOnXZ.y = playerC.transform.position.y;
                GameObject instatiant = Instantiate(projectile, shootPosition.position, transform.rotation);
                instatiant.GetComponent<Bullet>().targetPosition = playerC.transform.position;
            }

            //cooldown
            StartCoroutine("ShootCooldown");

            //animation
            StartCoroutine("ShootAnimation");
        }
    }

    IEnumerator ShootAnimation()
    {
        recoiling = true;
        enemyAnimator.SetInteger("AnimationInt", 3 + animationStateVariable);
        yield return new WaitForSecondsRealtime(recoilTime);
        recoiling = false;
    }

    IEnumerator ShootCooldown()
    {
        shooting = false;
        yield return new WaitForSecondsRealtime(shootDelay);
        shooting = true;
    }

    void CheckPlayer()
    {
        //check player
        Vector3 targetVector = playerC.transform.position - shoulderTransform.position;

        RaycastHit playerHit;

        if (Physics.Raycast(shoulderTransform.position, targetVector, out playerHit))
        {

            if (playerHit.transform.tag == "Player")
            {
                //see player
                playerSeen = true;
                patrol = false;

                //look at player:
                Vector3 correctAxisVector = targetVector;
                correctAxisVector.y = 0;
                turnVector = correctAxisVector;

                if (melee)
                {
                    if (meleePursue)
                    {
                        pursue = true;

                        //animation
                        enemyAnimator.SetInteger("AnimationInt", 8);

                        //move to player
                        Vector3 normalisedTargetVector = new Vector3(targetVector.x, 0, targetVector.z).normalized * walkingSpeed * 2f;
                        moveVector = new Vector3(normalisedTargetVector.x, gravity, normalisedTargetVector.z);

                        if (Vector3.Distance(transform.position, playerC.transform.position) < minSlashDistance)
                        {
                            //turn to player before slashing
                            transform.rotation = Quaternion.LookRotation(turnVector);

                            slash = true;
                            pursue = false;
                            meleePursue = false;
                            StartCoroutine("DamageSlash");
                        }
                    }
                }
                else
                {
                    //animation
                    if (!recoiling)
                    {
                        enemyAnimator.SetInteger("AnimationInt", 2 + animationStateVariable);
                    }

                    //pursue player
                    if (pursue)
                    {
                        pursueDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(pursueDestination.x, 0f, pursueDestination.z));

                        if (follow)
                        {
                            //go towards player
                            Vector3 pursueTargetVector = (pursueDestination - new Vector3(transform.position.x, 0f, transform.position.z));
                            Vector3 normalisedPursueTargetVector = new Vector3(pursueTargetVector.x, 0, pursueTargetVector.z).normalized * walkingSpeed;
                            moveVector = new Vector3(normalisedPursueTargetVector.x, gravity, normalisedPursueTargetVector.z);

                            if (pursueDistance < pursueTreshold)
                            {
                                //new point when distance reached
                                StartCoroutine("PursuePoint");
                            }
                        }
                        else
                        {
                            RaycastHit backCheck;

                            //raycast to backward - enemy doesnt try to walk through walls
                            if (Physics.Raycast(transform.position, -transform.forward, out backCheck, 1f, groundLayer))
                            {
                                //ray hits - check for new point
                                StartCoroutine("PursuePoint");
                            }
                            else
                            {
                                //ray doesnt hit - go away from player
                                Vector3 pursueTargetVector = (new Vector3(transform.position.x, 0f, transform.position.z) - pursueDestination);
                                Vector3 normalisedPursueTargetVector = new Vector3(pursueTargetVector.x, 0, pursueTargetVector.z).normalized * walkingSpeed;
                                moveVector = new Vector3(normalisedPursueTargetVector.x, gravity, normalisedPursueTargetVector.z);

                                if (pursueDistance > pursueTreshold)
                                {
                                    //new point when distance reached
                                    StartCoroutine("PursuePoint");
                                }
                            }
                        }
                    }

                    //shoot player
                    if (!cantShoot)
                    {
                        Shoot();
                    }

                   //seenlastframe used to be here - seems to be working now
                }

                if (!seenLastFrame)
                {
                    seenLastFrame = true;
                    StartCoroutine("PursuePoint");
                }

                seenLastFrame = true;
            }
            else
            {
                //cant see player
                pursue = false;
                playerSeen = false;
                if (seenLastFrame)
                {
                    seenLastFrame = false;
                    StartCoroutine("PatrolPause");
                }

                seenLastFrame = false;
            }
        }
    }

    IEnumerator PursuePoint()
    {
        pursue = false;
        yield return new WaitForSecondsRealtime(3f);
        pursueDestination = new Vector3(playerC.transform.position.x, 0f, playerC.transform.position.z);

        pursue = true;

        pursueDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(pursueDestination.x, 0f, pursueDestination.z));

        if (pursueDistance > pursueTreshold)
        {
            follow = true;
        }
        else
        {
            follow = false;
        }
    }

    IEnumerator PatrolPause()
    {
        //pause and create a random position to move to
        yield return new WaitForSecondsRealtime(2f);

        bool rayLoop = true;
        while (rayLoop)
        {
            //randomize the distance to destination for x axis
            int randomizedDistanceX = Random.Range(-patrolBounds, patrolBounds + 1);
            while (randomizedDistanceX < 2 && randomizedDistanceX > -2)
            {
                randomizedDistanceX = Random.Range(-patrolBounds, patrolBounds + 1);
            }
            float xPos = randomizedDistanceX + transform.position.x;

            //randomize the distance to destination for z axis
            int randomizedDistanceZ = Random.Range(-patrolBounds, patrolBounds + 1);
            while (randomizedDistanceZ < 2 && randomizedDistanceZ > -2)
            {
                randomizedDistanceZ = Random.Range(-patrolBounds, patrolBounds + 1);
            }
            float zPos = randomizedDistanceZ + transform.position.z;

            Vector3 patrolDirection = new Vector3(xPos, transform.position.y, zPos) - transform.position;
            float rayDistance = Vector3.Distance(transform.position, new Vector3(xPos, transform.position.y, zPos));

            RaycastHit patrolHit;
            //raycast to destination

            if (Physics.Raycast(transform.position, patrolDirection, out patrolHit, rayDistance, groundLayer))
            {
                //ray hits
                if (Vector3.Distance(transform.position, patrolDestination) > 1f)
                {
                    patrolDestination = patrolHit.point;
                    rayLoop = false;
                }
            }
            else
            {
                //ray doesnt hit
                patrolDestination = new Vector3(xPos, transform.position.y, zPos);
                rayLoop = false;
            }
        }

        if (!playerSeen)
        {
            patrol = true;
        }
    }

    void Patrol()
    {
        //move to target
        Vector3 targetVector = patrolDestination - transform.position;

        Vector3 normalisedTargetVector = new Vector3(targetVector.x, 0, targetVector.z).normalized * walkingSpeed;
        moveVector = new Vector3(normalisedTargetVector.x, gravity, normalisedTargetVector.z);

        //look at target
        Vector3 correctAxisVector = targetVector;
        correctAxisVector.y = 0;
        turnVector = correctAxisVector;

        float destinationDistance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), 
            new Vector3(patrolDestination.x, 0, patrolDestination.z));


        //pause patrol if destination is reached
        if (destinationDistance < 0.7f)
        {
            StartCoroutine("PatrolPause");
            patrol = false;
        }
    }

    void GroundCheck()
    {
        RaycastHit groundHit;

        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 0.9f, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, groundHit.point.y + (0.9f - 0.01f), transform.position.z);

            //grounded / gravity = normal value
            gravity = -2f;
        }
        else
        {
            //on air / maximum gravity pull = -15f
            if (gravity > -15f)
            {
                //increase velocity if its not reached maximum velocity
                gravity -= 0.1f;
            }
        }
    }

    IEnumerator DamageSlash()
    {
        yield return new WaitForSecondsRealtime(slashDamageDelay);

        Collider[] hitColliders = Physics.OverlapSphere(slashPosition.position, slashRadius);
        foreach (var hitcollider in hitColliders)
        {
            if (hitcollider.transform.GetComponent<Health>() != null)
            {
                if (hitcollider.transform.GetComponent<Health>().playerHealth == true)
                {
                    //Health script found - deal damage
                    hitcollider.transform.GetComponent<Health>().HPChange(slashDamage);
                }
            }
        }
    }
}

