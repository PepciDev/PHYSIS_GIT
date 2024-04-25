using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine;

public class DeathController : MonoBehaviour
{
    [SerializeField] GameObject videoScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] VideoPlayer videoPlayer;
    GameObject fader;
    bool death = false;
    bool stop = false;

    void Start()
    {
        videoScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        fader = GameObject.Find("FaderController");
    }

    void Update()
    {
        if (!stop)
        {
            //start death and fade screen
            if (!death)
            {
                death = true;
                fader.GetComponent<Fader>().ChangeFadeSpeed(4);
                fader.GetComponent<Fader>().ChangeTransparency(1);
            }

            //start cutscene when screen is dark
            if (death && fader.GetComponent<Fader>().dark)
            {
                videoScreen.SetActive(true);
                videoPlayer.Play();
            }
        }

        //video stopped
        videoPlayer.loopPointReached += CheckOver;
    }


    IEnumerator EnableUI()
    {
        //enable ui
        yield return new WaitForSecondsRealtime(1.2f);
        gameOverScreen.SetActive(true);
    }

    void CheckOver(VideoPlayer vp)
    {
        StartCoroutine("EnableUI");
        stop = true;
    }

    //UINAPIT
    public void MainMenuButton()
    {
        StartCoroutine("LoadScene", 0);
    }

    public void ContinueButton()
    {
        StartCoroutine("LoadScene", 1);
    }

    IEnumerator LoadScene(int scene)
    {
        yield return new WaitForSecondsRealtime(1.2f);
        SceneManager.LoadScene(scene);
    }
}
