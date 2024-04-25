using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuAndScene : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    GameObject fader;
    bool menuOpen = false;
    bool transitioning = false;
    public int sceneToTransition = 0;
    float transitionDelay = 1;
    bool canMenu = true;

    void Start()
    {
        pauseMenu.SetActive(false);
        fader = GameObject.Find("FaderController");
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            canMenu = true;
        }
        else
        {
            canMenu = false;
        }

        if (canMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuOpen)
            {
                //open menu && time stop
                menuOpen = true;
                pauseMenu.SetActive(true);
            }
            else
            {
                //close menu && time continue
                menuOpen = false;
                pauseMenu.SetActive(false);
            }
        }

        if (transitioning)
        {
            if (fader.GetComponent<Fader>().dark)
            {
                //change scene when dark
                StartCoroutine("SceneSwitchDelay");
            }
        }
    }

    public void SceneTransition()
    {
        //quit to mainmenu and fade to dark
        fader.GetComponent<Fader>().ChangeTransparency(1);
        transitioning = true;
    }

    IEnumerator SceneSwitchDelay()
    {
        yield return new WaitForSecondsRealtime(transitionDelay);
        SceneManager.LoadScene(sceneToTransition);
    }
}
