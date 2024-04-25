using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    Animator plantAnimator;
    public float plantSize = 0.9f;
    float despawnDelay = 5f;

    private void Start()
    {
        plantAnimator = transform.GetChild(0).GetComponent<Animator>();
        transform.localScale *= Random.Range(plantSize, plantSize + 0.6f);

    }

    void Update()
    {
        if (plantAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !plantAnimator.IsInTransition(0))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyDelayed()
    {
        yield return new WaitForSecondsRealtime(despawnDelay);
        Destroy(gameObject);
    }
}
