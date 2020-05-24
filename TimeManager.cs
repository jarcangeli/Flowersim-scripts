using UnityEngine;
using TMPro;
using System;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public enum TimeState { pause, play, ff }
public class TimeManager : MonoBehaviour
{
    public float timeScale = 60f*60*7*24 / 12; // 1 second is how much time in game (1/12th of a week for 12s weeks)
    DateTime time = new DateTime();
    public float elapsedTime = 0f;
    public TimeState state = TimeState.pause;
    public Renderer window;

    [SerializeField]
    Button pauseButton = null;
    [SerializeField]
    Button playButton = null;
    [SerializeField]
    Button ffButton = null;
    Color buttonNormalColor;
    Button[] buttons;


    [SerializeField]
    TextMeshProUGUI timeDisplay = null;

    private void Start()
    {
        buttonNormalColor = pauseButton.colors.normalColor;
        buttons = new Button[] { pauseButton, playButton, ffButton };
        PauseTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != TimeState.pause)
        {
            float timeSkip = Time.deltaTime * timeScale;

            time = time.AddSeconds(timeSkip);
            elapsedTime += Time.deltaTime;

            timeDisplay.text = time.ToString("HH:00 - dd MMMM");

            if (state == TimeState.play)
            {
                window.material.SetFloat("_TimeH", time.Hour);
            }
            else
            {
                window.material.SetFloat("_TimeH", 7f);
            }

        }
    }

    void UpdateButtonColors()
    {
        for (int i = 0; i < buttons.Length; ++i)
        {
            var colors = buttons[i].colors;
            if ((int)state == i)
            {
                colors.normalColor = colors.pressedColor;
            }
            else
            {
                colors.normalColor = buttonNormalColor;
            }
            buttons[i].colors = colors;
        }
    }

    public void PauseTime()
    {
        state = TimeState.pause;
        UpdateButtonColors();

    }
    public void PlayTime()
    {
        state = TimeState.play;
        UpdateButtonColors();
    }

    public void FastForward()
    {
        state = TimeState.ff;
        UpdateButtonColors();
    }
}
