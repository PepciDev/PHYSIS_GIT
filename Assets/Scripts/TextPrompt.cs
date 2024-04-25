using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPrompt : MonoBehaviour
{
    [SerializeField] TextMeshPro textComponent;
    [SerializeField] float changeSpeed = 5f;
    [SerializeField] float visibleRadius = 2f;
    [SerializeField] GameObject target;
    [SerializeField] bool playerTarget = false;
    [SerializeField] bool disableOnInvisible = false;
    [SerializeField] bool followScriptIncluded = true;
    [SerializeField] FollowTarget followScript;

    float targetOpacity = 0;
    bool visible = false;

    void Start()
    {
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, targetOpacity);

        if (followScriptIncluded && GetComponent<FollowTarget>() != null)
        {
            followScript = GetComponent<FollowTarget>();
        }

        if (playerTarget)
        {
            target = GameObject.Find("Player");
        }
    }

    void Update()
    {
        if (!followScriptIncluded && followScript == null)
        {
            GetComponent<TextPrompt>().enabled = false;
        }

        if (Vector3.Distance(transform.position, target.transform.position) < visibleRadius)
        {
            //visible
            targetOpacity = 1;
            visible = true;
            if (disableOnInvisible)
            {
                ChangeOnInvisible();
            }
        }
        else
        {
            //invisible
            targetOpacity = 0;
            visible = false;
            if (disableOnInvisible)
            {
                ChangeOnInvisible();
            }
        }

        //prevents infinite lerping
        if (visible)
        {
            if (textComponent.color.a <= 0.95f)
            {
                ChangeOpacity();
            }
            else
            {
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, targetOpacity);
            }
        }
        else
        {
            if (textComponent.color.a >= 0.05f)
            {
                ChangeOpacity();
            }
            else
            {
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, targetOpacity);
            }
        }
    }

    void ChangeOpacity()
    {
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, Mathf.Lerp(textComponent.color.a, targetOpacity, changeSpeed * Time.deltaTime));
    }

    void ChangeOnInvisible()
    {
        if (followScript != null)
        {
            followScript.enabled = visible;
        }
    }
}
