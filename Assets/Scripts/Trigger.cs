using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] GameObject eventTarget;
    [SerializeField] GameObject eventHider;
    [SerializeField] bool disableOnDie;
    [SerializeField] bool shrink;
    [SerializeField] bool growOnStart;
    [SerializeField] float growSpeed;
    Vector3 ogSize;

    [SerializeField] bool nullTrigger;
    [SerializeField] bool sceneTransition = false;
    [SerializeField] int sceneToTransition;
    //target can be used with both nullTrigger and to disable the text object in the event + used for scene transition
    [SerializeField] bool enableObject = false;
    [SerializeField] GameObject enableTarget;
    [SerializeField] GameObject target;
    [SerializeField] float shrinkSpeed = 0.5f;
    [SerializeField] bool animated;
    [SerializeField] GameObject animationTarget;
    [SerializeField] AnimationClip animClip;
    [SerializeField] KeyCode key1;
    [SerializeField] KeyCode key2;

    Vector3 playerFreezePos;
    public bool triggerEvent = false;
    bool eventing = false;
    public bool animate = false;
    public bool die = false;
    float originalYSize;

    void Start()
    {
        originalYSize = transform.localScale.y;

        if (growOnStart)
        {
            ogSize = transform.localScale;
            transform.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        if (growOnStart)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, ogSize, growSpeed);

            if (transform.localScale.y >= originalYSize - (originalYSize/15f))
            {
                transform.localScale = ogSize;
                growOnStart = false;
            }
        }

        if (nullTrigger && target == null)
        {
            Triggered();
        }

        if (die)
        {
            //transition to another scene
            if (sceneTransition)
            {
                target.GetComponent<MenuAndScene>().sceneToTransition = sceneToTransition;
                target.GetComponent<MenuAndScene>().SceneTransition();
            }

            if (shrink)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed);

                if (transform.localScale.y < originalYSize / 25f)
                {
                    if (disableOnDie)
                    {
                        GetComponent<Trigger>().enabled = false;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                if (disableOnDie)
                {
                    GetComponent<Trigger>().enabled = false;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if (animate)
        {
            animationTarget.GetComponent<Animator>().Play(animClip.name);
        }

        if (eventing)
        {
            //freeze player
            GameObject playerC = GameObject.Find("Player");
            playerC.transform.position = playerFreezePos;
            playerC.GetComponent<Rigidbody>().velocity = Vector3.zero;

            //disable player controller - looks like works for now
            playerC.GetComponent<PlayerController>().enabled = false;

            if ((eventTarget.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MainLayer.SurpriseAttack"))
                    && eventTarget.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.634f && !eventTarget.GetComponent<Animator>().IsInTransition(0)) 
            {
                //enable the text object
                if (target != null)
                {
                    target.SetActive(true);
                }
                //stop time at halftime
                Time.timeScale = 0;

                if (Input.GetKey(key1) && Input.GetKey(key2))
                {
                    playerC.GetComponent<PlayerController>().enabled = true;
                    Time.timeScale = 1;
                    //trigger the text object to dissapear
                    target.GetComponent<Trigger>().Triggered();
                    eventing = false;
                }
            }
        }
        else if (triggerEvent && eventTarget.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !eventTarget.GetComponent<Animator>().IsInTransition(0))
        {
            Destroy(gameObject);
        }
    }

    public void Triggered()
    {
        if (triggerEvent)
        {
            eventing = true;
            eventHider.SetActive(true);

            eventTarget.GetComponent<Animator>().SetLayerWeight(1, 0);
            eventTarget.GetComponent<Animator>().SetInteger("AnimationInt", -2);

            playerFreezePos = GameObject.Find("Player").transform.position;
        }
        else if (animated)
        {
            animate = true;
        }
        else
        {
            die = true;
        }

        if (enableObject)
        {
            if (enableTarget != null)
            {
                enableTarget.SetActive(true);
            }
        }
    }
}
