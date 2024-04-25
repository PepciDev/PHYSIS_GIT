using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventController : MonoBehaviour
{
    [SerializeField] GameObject spawnParticles;
    [SerializeField] GameObject dieParticles;

    public TrailRenderer slashParticleR;
    public TrailRenderer slashParticleL;

    [SerializeField] bool player = false;

    PlayerController playerc;

    float particleOffset = 1.2f;

    private void Start()
    {
        //Start
        if (player)
        {
            playerc = GameObject.Find("Player").GetComponent<PlayerController>();
        }
    }

    public void StartSlashR()
    {
        //start slash
        slashParticleR.gameObject.SetActive(true);
    }

    public void StartSlashL()
    {
        //start slash
        slashParticleL.gameObject.SetActive(true);
    }

    public void EndSlash()
    {
        //end slash
        if (slashParticleL != null)
        {
            slashParticleL.gameObject.SetActive(false);
        }

        if (slashParticleR != null)
        {
            slashParticleR.gameObject.SetActive(false);
        }
    }

    public void SpearThrowEvent()
    {
        playerc.SpearThrow();
    }

    public void SpawnParticles()
    {
        Instantiate(spawnParticles, transform.position, spawnParticles.transform.rotation);
    }

    public void DieParticles()
    {
        Instantiate(dieParticles, transform.position, spawnParticles.transform.rotation);
    }
}
