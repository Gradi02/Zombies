using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    public GameObject sun;
    private bool tweening = false;

    private float minLight = 0.1f;
    private float maxFog = 0.1f;

    [SerializeField] private Color[] nightGradient, dayGradient;
    void Start()
    {
        LeanTween.rotateAround(sun, new Vector3(1, 0, 0), 360, 24).setLoopCount(-1);
        StartCoroutine(DayNightCycle());
    }

    void Update()
    {
        float rotationX = sun.transform.rotation.x;

        //x - 0 wschód & x - 180 zachód

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

/*            RenderSettings.ambientSkyColor = nightGradient[0];
            RenderSettings.ambientEquatorColor = nightGradient[1];
            RenderSettings.ambientGroundColor = nightGradient[2];*/
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

/*            RenderSettings.ambientSkyColor = dayGradient[0];
            RenderSettings.ambientEquatorColor = dayGradient[1];
            RenderSettings.ambientGroundColor = dayGradient[2];*/
        }
    }
    IEnumerator DayNightCycle()
    {
        while (true)
        {
            //LeanTween.rotateAround(sun, new Vector3(1, 0, 0), 360, 24).setLoopCount(-1);
            yield return new WaitForSeconds(24);
        }
    }
}
