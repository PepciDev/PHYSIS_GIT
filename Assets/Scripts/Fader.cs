using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] GameObject fader;
    float fadeMultiplier = 2.6f;
    int targetAlpha = 0;
    public bool transparent = false;
    public bool dark = true;

    void Start()
    {
        //fader = GameObject.Find("Fader");
        fader.SetActive(true);
    }

    void Update()
    {
        //lerp to target
        fader.GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Lerp(fader.GetComponent<Image>().color.a, targetAlpha, Time.deltaTime * fadeMultiplier));

        //change transparent bool and active*
        if (fader.GetComponent<Image>().color.a < 0.01f)
        {
            transparent = true;
        }
        else
        {
            transparent = false;
        }
        //change dark bool
        if (fader.GetComponent<Image>().color.a > 0.99f)
        {
            dark = true;
        }
        else
        {
            dark = false;
        }

        //TESTING
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeTransparency(0);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeTransparency(1);
        }
    }

    public void ChangeTransparency(int transparency)
    {
        targetAlpha = transparency;
    }

    public void ChangeFadeSpeed(float speed)
    {
        fadeMultiplier = speed;
    }
}
