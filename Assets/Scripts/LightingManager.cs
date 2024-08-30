using UnityEngine;
using Unity.Netcode;

[ExecuteAlways]
public class LightingManager : NetworkBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField, Range(0.001f, 1.0f)] private float Duration;
    [SerializeField] private int currentDay = 0;

    private void Start()
    {
        if (Preset == null || !IsServer) return;

        TimeOfDay += Time.deltaTime * Duration;
        TimeOfDay %= 24;
    }
    private void Update()
    {
        if (Preset == null || !IsServer || !NetworkGameManager.instance.gameStarted)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime * Duration;

            if (TimeOfDay >= 24)
            {
                TimeOfDay %= 24;
                currentDay++;
                NetworkGameManager.instance.UpdateDayValue(currentDay);
                Debug.Log($"New Day: {currentDay}");
            }
        }

        UpdateDayValuesClientRpc(TimeOfDay);
        UpdateLightingClientRpc(TimeOfDay / 24f);
    }

    [ClientRpc]
    private void UpdateDayValuesClientRpc(float tm)
    {
        TimeOfDay = tm;
    }

    [ClientRpc]
    private void UpdateLightingClientRpc(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        RenderSettings.skybox.SetColor("_TintColor", Preset.DirectionalColor.Evaluate(timePercent));

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }

    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetupSunServerRpc()
    {
        UpdateDayValuesClientRpc(TimeOfDay);
        UpdateLightingClientRpc(TimeOfDay / 24f);
    }
}