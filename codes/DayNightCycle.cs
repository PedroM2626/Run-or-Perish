using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight;
    public Color dayColor = Color.white;
    public Color nightColor = Color.blue;
    public float dayDuration = 20f;
    public float nightDuration = 300f;
    public float transitionDelay = 5f;

    private float timer;
    private bool isDay = true;

    void Start()
    {
        timer = dayDuration;
        SetDay(true);
        //StartCoroutine(DayNightLoop());
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (isDay)
            {
                StartCoroutine(TransitionToNight());
            }
            else
            {
                StartCoroutine(TransitionToDay());
            }
        }
    }

    IEnumerator DayNightLoop()
    {
        while (true)
        {
            if (isDay)
            {
                yield return StartCoroutine(TransitionToNight());
            }
            else
            {
                yield return StartCoroutine(TransitionToDay());
            }
        }
    }

    IEnumerator TransitionToNight()
    {
        SetDay(false);
        yield return new WaitForSeconds(transitionDelay); //transição
        timer = nightDuration;
    }

    IEnumerator TransitionToDay()
    {
        SetDay(true);
        yield return new WaitForSeconds(transitionDelay); //transição
        timer = dayDuration;
    }

    void SetDay(bool day)
    {
        isDay = day;
        directionalLight.color = day ? dayColor : nightColor;
        RenderSettings.ambientLight = day ? dayColor : nightColor;
    }
}
