using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    public GameObject sun;
    private bool tweening = false;

    private float minLight = 0.01f;
    private float maxFog = 0.8f;
    void Start()
    {
        RenderSettings.fogColor = Color.black;
        StartCoroutine(DayNightCycle());
    }

    void Update()
    {
        float rotationX = sun.transform.eulerAngles.x;

        //nigght
        if (rotationX > 0 && rotationX < 20 && !tweening)
        {
            tweening = true;
            LeanTween.value(sun, 2f, minLight, 4f).setOnUpdate((float val) =>
            {
                sun.GetComponent<Light>().intensity = val;
            }).setOnComplete(() =>
            {
                tweening = false;
                sun.GetComponent<Light>().intensity = minLight;
            });
            LeanTween.value(sun, 0, maxFog, 4f).setOnUpdate((float val) =>
            {
                RenderSettings.fogDensity = val;
            });
        }
        //DAY
        if (rotationX > 330 && rotationX < 350 && !tweening)
        {
            tweening = true;
            LeanTween.value(sun, minLight, 2f, 4f).setOnUpdate((float val) =>
            {
               sun.GetComponent<Light>().intensity = val;
            }).setOnComplete(() =>
            {
                tweening = false;
                sun.GetComponent<Light>().intensity = 2f;
            });
            LeanTween.value(sun, maxFog, 0f, 4f).setOnUpdate((float val) =>
            {
                RenderSettings.fogDensity = val;
            });
        }
    }
    IEnumerator DayNightCycle()
    {
        while (true)
        {
            LeanTween.rotateAround(sun, new Vector3(1, 0, 0), 360, 24).setLoopCount(-1);
            yield return new WaitForSeconds(24);


        }
    }
}
