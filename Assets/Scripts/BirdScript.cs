using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{

    public GameObject secondBird;
    Animator bird;
    int lastDecision = 1;
    int lastestDecision = 1;

    void Start()
    {
        bird = gameObject.transform.GetChild(0).GetComponent<Animator>();
        StartCoroutine("BirdFly");
    }

    IEnumerator BirdFly()
    {
        //random decision 1 OR 2
        int randomAttack = Random.Range(1, 3);

        while ((lastestDecision + lastDecision) / 2 == randomAttack)
        {
            randomAttack = Random.Range(1, 3);
        }

        lastestDecision = lastDecision;
        lastDecision = randomAttack;

        if (randomAttack == 1)
        {
            bird.Play("BirdFly", 0, 0);
        }
        else
        {
            secondBird.transform.GetChild(0).GetComponent<Animator>().Play("BirdFly", 0, 0);
        }

        Quaternion newRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        secondBird.transform.rotation = newRotation;

        float birdCooldown = Random.Range(3f, 9f);
        yield return new WaitForSecondsRealtime(birdCooldown);
        StartCoroutine("BirdFly");
    }

    public void StopBirds()
    {
        //Destroy Birds
        Destroy(secondBird);
        Destroy(gameObject);
    }
}
