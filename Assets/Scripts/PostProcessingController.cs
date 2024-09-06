using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PostProcessingController : MonoBehaviour
{
    private Volume globalVolume => NetworkGameManager.instance.globalVolume;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;
    float startVignette;
    float startLensDistortion;
    float startChromaticAberration;
    float startMotionBlur;

    private float timeToEndEffect = 0;
    private bool effectStarted = false;

    private void Start()
    {
        if (globalVolume.profile.TryGet(out vignette) &&
            globalVolume.profile.TryGet(out lensDistortion) &&
            globalVolume.profile.TryGet(out chromaticAberration) &&
            globalVolume.profile.TryGet(out motionBlur))
        {
            Debug.Log("Efekty za³adowane.");
        }
        else
        {
            Debug.LogError("Nie uda³o siê za³adowaæ efektów z profilu Global Volume.");
        }
    }

    public void StartVodkaEffect(float time)
    {
        timeToEndEffect = Time.time + time;
        effectStarted = true;
        StopAllCoroutines();
        ChangeEffectsOverTime(0.4f, 0.4f, 1, 1, 3f);
    }

    private void ChangeEffectsOverTime(float targetVig,float targetLens,float targetChrom,float targetMot,float duration)
    {
        StartCoroutine(ChangeEffectsRoutine(targetVig,targetLens,targetChrom,targetMot,duration));
    }

    private IEnumerator ChangeEffectsRoutine(float targetVig,float targetLens,float targetChrom,float targetMot,float duration)
    {
        float timeElapsed = 0f;

        startVignette = vignette.intensity.value;
        startLensDistortion = lensDistortion.intensity.value;
        startChromaticAberration = chromaticAberration.intensity.value;
        startMotionBlur = motionBlur.intensity.value;

        while (timeElapsed < duration)
        {
            // Interpolacja wartoœci efektów
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVig, timeElapsed / duration);
            lensDistortion.intensity.value = Mathf.Lerp(startLensDistortion, targetLens, timeElapsed / duration);
            chromaticAberration.intensity.value = Mathf.Lerp(startChromaticAberration, targetChrom, timeElapsed / duration);
            motionBlur.intensity.value = Mathf.Lerp(startMotionBlur, targetMot, timeElapsed / duration);

            // Odmierzanie czasu
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        // Ustawienie docelowych wartoœci na koñcu interpolacji
        vignette.intensity.value = targetVig;
        lensDistortion.intensity.value = targetLens;
        chromaticAberration.intensity.value = targetChrom;
        motionBlur.intensity.value = targetMot;
    }

    private void Update()
    {
        if(effectStarted && Time.time > timeToEndEffect)
        {
            effectStarted = false;
            StopAllCoroutines();
            ChangeEffectsOverTime(0.25f, 0f, 0f, 0f, 5f);
        }
    }
}
