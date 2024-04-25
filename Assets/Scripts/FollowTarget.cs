using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public bool follow = true;
    public bool mainCam = false;
    public bool player = false;
    public bool limitCamRotation = false;
    public float limitDistance = -0.4f;
    public bool smoothFollow = false;
    public float followSpeed = 5f;
    public GameObject target;
    public GameObject lookableTarget;
    public bool lookTarget = false;
    public bool smoothLook = false;
    public float lookSpeed = 5f;
    bool checkedLastFrame = false;
    Vector3 savedTargetPos;

    void Start()
    {
        if (mainCam)
        {
            lookableTarget = GameObject.Find("Main Camera");
        }
        else if (player)
        {
            lookableTarget = GameObject.Find("Player");
        }

        if (lookTarget)
        {
            transform.LookAt(lookableTarget.transform.position);
        }
    }

    void Update()
    {
        if (target != null)
        {
            if (follow)
            {
                if (!smoothFollow)
                {
                    transform.position = target.transform.position;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, target.transform.position, followSpeed * Time.deltaTime);
                }
            }
        }

        if (lookableTarget != null)
        {
            if (lookTarget)
            {
                Look();
            }
        }
    }

    void Look()
    {
        if (smoothLook)
        {

            //if limit pos
            if (limitCamRotation && mainCam)
            {
                var lookPos = lookableTarget.transform.position - transform.position;

                if (lookableTarget.transform.position.z < transform.position.z + limitDistance)
                {
                    //camera on "inbounds"
                    if (!checkedLastFrame)
                    {
                        checkedLastFrame = true;
                    }
                }
                else
                {
                    //camera on "liian pitkällä"

                    if (checkedLastFrame)
                    {
                        checkedLastFrame = false;
                        savedTargetPos = lookableTarget.transform.position;
                    }

                    lookPos = new Vector3(lookableTarget.transform.position.x, lookableTarget.transform.position.y, savedTargetPos.z) - transform.position;
                }

                //lookPos.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
            }
            else
            {
                //look smoothly
                var lookingPos = lookableTarget.transform.position - transform.position;

                //lookPos.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(lookingPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
            }

        }
        else
        {
            //look straight
            transform.LookAt(lookableTarget.transform.position);
        }
    }
}
