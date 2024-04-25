using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool dead = false;
    [SerializeField] GameObject deathScreen;

    public Rigidbody playerRb;
    float speed = 4.2f;
    float rotationSpeed = 6f;
    public LayerMask layerMask;
    public LayerMask groundMask;
    float growCooldown = 0.2f;

    float xPos;
    float zPos;

    float height = 1.25f;

    float gravity = -2f;

    public GameObject[] plant;
    int plantIndex = 0;

    bool canMove;
    bool grow = false;
    float growBounds = 2f;

    public Animator playerAnimator;

    public Camera mainCam;
    public Vector3 targetWorldPosition = new Vector3(1, 1, 1);
    Quaternion lookRotation = new Quaternion(1, 1, 1, 1);

    public GameObject throwingSpear;
    [SerializeField] GameObject spearIndicator;
    GameObject spearInstantion;
    Vector3 spearTarget;

    public GameObject arm;

    bool idling = true;

    bool slashing = false;
    float slashRadius = 0.6f;
    [SerializeField] Transform slashPosition;
    int slashDamage = -1;
    //1 or 2
    int slashSide = 2;
    float slashDelay = 0.14f;

    bool canParticle = true;

    bool spearLoad = false;
    public Transform spearPosition;
    public GameObject breakParticle;
    public ParticleSystem loadParticle;

    bool spearThrow = false;
    bool targetNull = true;
    public GameObject spear;
    public Transform throwPosition;

    Vector3 moveVector;

    bool canDodge = true;
    float dodgeSpeed = 10f;
    float dodgeCooldown = 1.5f;
    bool immortal = false;
    public GameObject leafPoof;

    public Renderer[] playerRenderer;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        StartCoroutine("GrowCooldown");
    }

    private void FixedUpdate()
    {
        //spear load & throw
        if (Input.GetKey(KeyCode.Mouse1) && idling)
        {
            playerAnimator.SetInteger("AnimationInt", 3);

            spearLoad = true;
            idling = false;
        }

        if (spearLoad && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpearLoad"))
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f)
            {
                //if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.93f) an old if for when you want to cancel spearload after a duration

                //ray to spear target
                Vector3 rayStartPos = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), throwPosition.transform.position, 0.4f);
                float distanceToCursor = Vector3.Distance(rayStartPos, targetWorldPosition) + 0.01f;
                //RaycastHit spearRayHit;
                RaycastHit[] spearRayHits = Physics.RaycastAll(rayStartPos, targetWorldPosition - rayStartPos, distanceToCursor);
                if (spearRayHits != null)
                {
                    spearTarget = ReturnRightRayPoint(spearRayHits);
                }

                //ready + particle + spear indicator
                if (canParticle)
                {
                    loadParticle.Play();
                    canParticle = false;
                    //instantiate spear indicatpr and store it
                    spearInstantion = Instantiate(spearIndicator, spearTarget, spearIndicator.transform.rotation);
                }

                //move spear indicator to target
                spearInstantion.transform.position = spearTarget;

                //throw spear when letting go of button
                if (!Input.GetKey(KeyCode.Mouse1))
                {
                    spearLoad = false;
                    spearThrow = true;
                    playerAnimator.SetInteger("AnimationInt", 4);
                    spearInstantion.GetComponent<Trigger>().Triggered();
                }
            }
            else if (!Input.GetKey(KeyCode.Mouse1))
            {
                //cancel loading when letting go
                spearLoad = false;
                idling = true;
                playerAnimator.SetInteger("AnimationInt", 0);
            }
            /*
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAnimator.IsInTransition(0))
            {
                //cancel loading after holding for too long
                spearLoad = false;
                playerAnimator.SetInteger("AnimationInt", 0);
                idling = true;
            }*/
        }

        if (spearThrow && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SpearThrow"))
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAnimator.IsInTransition(0))
            {
                idling = true;
                playerAnimator.SetInteger("AnimationInt", 0);
            }
        }
    }

    private void Update()
    {

        //idle
        if (idling)
        {
            canParticle = true;
        }

        //dodge
        if (idling && canDodge && Input.GetKey(KeyCode.Space) && !playerAnimator.IsInTransition(0))
        {
            idling = false;
            StartCoroutine("Dodge");
        }

        //slash
        if (Input.GetKey(KeyCode.Mouse0) && idling)
        {
            playerAnimator.SetInteger("AnimationInt", slashSide);
            StartCoroutine("SlashDelay");

            slashing = true;
            idling = false;
        }

        if ((playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Slash") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Slash2")) 
            && (slashing && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAnimator.IsInTransition(0)))
        {
            if (slashSide == 1) {
                slashSide = 2;
            }
            else {
                slashSide = 1;
            }

            slashing = false;
            idling = true;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                playerAnimator.SetInteger("AnimationInt", slashSide);
            }
            else
            {
                playerAnimator.SetInteger("AnimationInt", 0);
            }
        }

        //raycast / ground check
        RaycastHit groundHit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out groundHit, height, groundMask))
        {
            //grounded
            transform.position = new Vector3(transform.position.x, groundHit.point.y + (height-0.01f), transform.position.z);
            //gravitys starting value = -2f
            gravity = -2f;
        }
        else
        {
            //on air

            //maximum gravity pull = -15f
            if (gravity > -15f)
            {
                //increase velocity if its not reached maximum velocity
                gravity -= 0.1f;
            }
        }

        //mouse pointing
        RaycastHit hit;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            targetWorldPosition = hit.point;
            targetNull = false;
        }
        else
        {
            targetWorldPosition = transform.forward;
            targetNull = true;
        }

        //look at
        //transform.LookAt(new Vector3(targetWorldPosition.x, targetWorldPosition.y, targetWorldPosition.z));
        var lookPos = targetWorldPosition - transform.position;
        lookPos.y = 0;
        lookRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);


        //move
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //transform.position += new Vector3(x, 0, z).normalized * speed * Time.deltaTime;
        Vector3 normalisedDirection = new Vector3(x, 0, z).normalized * speed;
        moveVector = new Vector3(normalisedDirection.x, gravity, normalisedDirection.z);
        playerRb.velocity = moveVector;

        //grow plants around player
        if (grow)
        {
            RaycastHit growHit;
            xPos = Random.Range(-growBounds, growBounds) + transform.position.x;
            zPos = Random.Range(-growBounds, growBounds) + transform.position.z;

            if (Physics.Raycast(new Vector3(xPos, transform.position.y + transform.localScale.y, zPos), Vector3.down, out growHit, 5f, groundMask))
            {
                //ray hits, grow a plant on hitpoint
                int newIndex = Random.Range(0, 5);
                while (newIndex == plantIndex)
                {
                    newIndex = Random.Range(0, 5);
                }
                plantIndex = newIndex;
                Quaternion plantRotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);
                Instantiate(plant[plantIndex], growHit.point, plantRotation);
                StartCoroutine("GrowCooldown");
            }
            else
            {
                xPos = Random.Range(-growBounds, growBounds) + transform.position.x;
                zPos = Random.Range(-growBounds, growBounds) + transform.position.z;
            }
        }
    }

    /*TEMPORARY TESTING
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(slashPosition.position, slashRadius);
    }
    */

    public void SpearThrow()
    {
        //raycast from player to the hand
        Vector3 spearRayPosition = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        float spearCheckDistance = Vector3.Distance(spearRayPosition, throwPosition.position);
        
        RaycastHit spearRayHit;
        if (Physics.Raycast(spearRayPosition, throwPosition.position - spearRayPosition, out spearRayHit, spearCheckDistance, layerMask))
        {
            //instantiate spear to the hit point / prevents throwing through walls
            Vector3 spearCheckInstantiatePos = Vector3.Lerp(spearRayPosition, spearRayHit.point, 0.5f);
            GameObject instatiant = Instantiate(spear, spearCheckInstantiatePos, transform.rotation);
            if (!targetNull)
            {
                instatiant.GetComponent<ThrowingSpear>().targetNull = false;
                instatiant.GetComponent<ThrowingSpear>().targetPosition = targetWorldPosition;
            }
        }
        else
        {
            //instantiate spear to the orginal throw position
            GameObject instatiant = Instantiate(spear, throwPosition.position, transform.rotation);
            if (!targetNull)
            {
                instatiant.GetComponent<ThrowingSpear>().targetNull = false;
                instatiant.GetComponent<ThrowingSpear>().targetPosition = targetWorldPosition;
            }
        }

    }

    IEnumerator SlashDelay()
    {
        yield return new WaitForSecondsRealtime(slashDelay);

        Collider[] hitColliders = Physics.OverlapSphere(slashPosition.position, slashRadius);
        foreach (var hitcollider in hitColliders)
        {
            if (hitcollider.transform.GetComponent<Health>() != null)
            {
                if (hitcollider.transform.GetComponent<Health>().playerHealth == false)
                {
                    //Health script found - deal damage
                    hitcollider.transform.GetComponent<Health>().HPChange(slashDamage);
                }
            }
        }
    }

    IEnumerator GrowCooldown()
    {
        grow = false;
        yield return new WaitForSecondsRealtime(growCooldown);
        grow = true;
    }

    IEnumerator Dodge()
    {
        GameObject instantiaton = Instantiate(leafPoof, transform.position, leafPoof.transform.rotation);
        instantiaton.transform.GetComponent<FollowTarget>().target = gameObject;

        playerAnimator.SetInteger("AnimationInt", 5);
        immortal = true;
        canDodge = false;
        speed += dodgeSpeed;
        yield return new WaitForSecondsRealtime(0.2f);
        speed -= dodgeSpeed;
        playerAnimator.SetInteger("AnimationInt", 0);
        immortal = false;
        idling = true;
        StartCoroutine("DodgeCooldown");
    }

    IEnumerator DodgeCooldown()
    {
        yield return new WaitForSecondsRealtime(dodgeCooldown);
        canDodge = true;
    }

    public void TakeDamage()
    {
        //take damage
    }

    public void Die()
    {
        //lose all health = die
        dead = true;
        //deathscreen activation
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }

        GetComponent<PlayerController>().enabled = false;

    }

    Vector3 ReturnRightRayPoint(RaycastHit[] hits)
    {
        Vector3 correctHitPoint = transform.forward;
        bool looping = true;
        if (looping)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject.layer != 8)
                {
                    correctHitPoint = hits[i].point;
                    looping = false;
                }
            }
        }
        return correctHitPoint;
    }
}