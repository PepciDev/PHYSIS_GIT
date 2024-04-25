using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //compatible with hp
    [SerializeField] bool collide;
    [SerializeField] bool trigger;
    [SerializeField] bool enable;
    [SerializeField] bool dieOnTrigger;
    [SerializeField] bool keyTrigger;
    [SerializeField] KeyCode triggerKey;
    [SerializeField] float keyRadius;

    [SerializeField] GameObject[] targets;
    [SerializeField] bool materialChange;
    [SerializeField] Material newMaterial;
    [SerializeField] GameObject particles;

    bool triggered = false;
    Renderer rend;

    void Start()
    {
        //non sos nopodemos
        rend = GetComponent<Renderer>();
    }

    public void TakeDamage()
    {
        //single interaction
    }

    private void Update()
    {
        if (keyTrigger)
        {
            if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < keyRadius && Input.GetKeyDown(triggerKey))
            {
                Die();
            }
        }
    }

    public void Die()
    {
        if (!triggered)
        {
            //trigger triggers
            if (trigger)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i].GetComponent<Trigger>().Triggered();
                }
            }

            if (materialChange)
            {
                MaterialChange();
            }

            if (enable)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i].SetActive(true);
                }
            }

            if (particles != null)
            {
                Instantiate(particles, transform.position, particles.transform.rotation);
            }

            //trigger only once
            triggered = true;

            //die when triggered if ...
            if (dieOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }


    void MaterialChange()
    {
        Material[] mats = rend.materials;

        //change material/s to target material
        int matsLength = rend.materials.Length;
        for (int i = 0; i < matsLength; i++)
        {
            mats[i] = newMaterial;
        }

        rend.materials = mats;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collide && other.transform.tag == "Player")
        {
            Die();
        }
    }
}
