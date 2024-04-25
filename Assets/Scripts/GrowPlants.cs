using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowPlants : MonoBehaviour
{
    bool grow = true;
    float xPos;
    float zPos;
    float growBounds = 4f;
    int plantIndex = 0;
    public GameObject[] plant;
    public LayerMask groundMask;
    float growCooldown = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("GrowCooldown");
    }

    // Update is called once per frame
    void Update()
    {
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
                PlantScript instantPlant = Instantiate(plant[plantIndex], growHit.point, plantRotation).GetComponent<PlantScript>();
                instantPlant.plantSize = 0.6f;
                StartCoroutine("GrowCooldown");
            }
            else
            {
                xPos = Random.Range(-growBounds, growBounds) + transform.position.x;
                zPos = Random.Range(-growBounds, growBounds) + transform.position.z;
            }
        }
    }

    IEnumerator GrowCooldown()
    {
        grow = false;
        yield return new WaitForSecondsRealtime(growCooldown);
        grow = true;
    }
}