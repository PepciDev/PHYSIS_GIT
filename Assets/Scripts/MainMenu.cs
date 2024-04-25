using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button optionsButton;
    public GameObject optionsTab;
    public PlayableDirector timeline;

    public Animator player;
    public Animator mask;
    public GameObject birds;

    bool optionsOpen = false;
    bool started = false;

    bool skipCutscene = false;

    void Start()
    {
        player.enabled = false;
        mask.enabled = false;
    }

    public void Options()
    {
        //open / close options
        bool canChange = true;

        if (canChange)
        {
            if (!optionsOpen)
            {
                optionsTab.SetActive(true);
                optionsOpen = true;
                canChange = false;
            }
            else
            {
                optionsTab.SetActive(false);
                optionsOpen = false;
                canChange = false;
            }
        }
    }

    public void StartGame()
    {
        player.enabled = true;
        mask.enabled = true;
        birds.SetActive(false);

        optionsTab.SetActive(false);
        optionsOpen = false;
        timeline.Play();

    }

    public void CutsceneEnd()
    {
        //load new scene
        SceneManager.LoadScene(1);
    }

    public void ShortCutscene()
    {
        //load new scene
        if (skipCutscene)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void SkipCutsceneButton()
    {
        bool canChange = true;
        //enable / disable ctscene skipping
        if (canChange)
        {
            if (!skipCutscene)
            {
                skipCutscene = true;
                canChange = false;
            }
            else
            {
                skipCutscene = false;
                canChange = false;
            }
        }
    }

    public void VolumeSlider()
    {
        //change volume slider
    }

    public void VfxSlider()
    {
        //change vfx slider
    }
}
