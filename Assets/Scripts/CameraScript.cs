using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    public Vector3 targetPos;
    float yOffset = 9.5f;
    float distanceOffset = 2f;
    public Transform casterPoint;
    Vector3 lookatPos;
    public LayerMask camMask;

    private void Start()
    {
        //set start position
        float yPos = player.position.y + yOffset / 10f;

        targetPos = new Vector3(
            player.position.x,
            yPos,
            player.position.z - distanceOffset);

        //set start rotation
        lookatPos = player.position;
        transform.position = Vector3.Lerp(player.position, targetPos, 0.5f);
    }

    void FixedUpdate()
    {
        float yPos = player.position.y + yOffset;

        targetPos = new Vector3(
            player.position.x,
            yPos,
            player.position.z - distanceOffset);

        lookatPos = Vector3.Lerp(lookatPos, player.position, 0.1f);
        transform.LookAt(lookatPos);

        casterPoint.position = new Vector3(player.position.x, player.position.y + 1f, player.position.z);
        casterPoint.LookAt(targetPos);

        float camDistance = Vector3.Distance(targetPos, player.position);

        RaycastHit hit;

        if (Physics.Raycast(casterPoint.position, casterPoint.forward, out hit, camDistance, camMask))
        {
            Vector3 distancedHitPoint = Vector3.Lerp(casterPoint.position, hit.point, 0.6f);
            transform.position = distancedHitPoint;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
        }
    }
}