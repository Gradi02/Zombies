using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : NetworkBehaviour
{
    [HideInInspector] public float maxHealth = 100, maxSprint = 100;
    [SerializeField] private Slider hpSlider, sprintSlider;
    [SerializeField] private Image sprintColor;
    [SerializeField] private Color superSpeedColor, sprintNormalColor;
    [SerializeField] private Camera cam;
    [SerializeField] SC_FPSController fpsController;
    private float endSlowTime;
    private float normalSpeed;
    [SerializeField] private CharacterController controller;
    [SerializeField] private TextMeshProUGUI goldTxt;

    public int gold { get; private set; } = 100;
    [HideInInspector] public NetworkVariable<bool> isAlive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [HideInInspector] public NetworkVariable<float> health = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private float sprintValue = 100;
    public float sprintUsePower, sprintRegenPower;

    public int maxUpgrades { get; private set; } = 10;
    public List<GunUpgrade> mUpgrades { get; private set; } = new List<GunUpgrade>();
    [SerializeField] private GameObject imgPref;
    [SerializeField] private Transform imgParent;
    private List<GameObject> imgs = new List<GameObject>();
    private bool slowed = false;
    [SerializeField] private Image mapicon;
    [SerializeField] private TextMeshProUGUI emergencyText;

    private void Start()
    {
        if (!IsOwner) return;

        mapicon.color = Color.green;
        normalSpeed = fpsController.walkingSpeed;
        hpSlider.maxValue = maxHealth;
        goldTxt.text = "Gold: " + gold;
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
            gameObject.transform.position = new Vector3(-60, -6, -50);
        if (IsOwner && controller != null)
            controller.enabled = true;

        base.OnNetworkSpawn();      
    }

    public void AddRemoveGold(int ng, bool substract = false)
    {
        if(substract)
        {
            gold -= ng;

            if(gold < 0) gold = 0;
        }
        else
        {
            gold += ng;
        }

        goldTxt.text = "Gold: " + gold;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamagePlayerServerRpc(float dmg)
    {
        if (health.Value - dmg >= 0)
            health.Value -= dmg;
        else
            health.Value = 0;

        if (health.Value <= 0 && isAlive.Value)
        {
            isAlive.Value = false;
            NetworkGameManager.instance.HandleDeadPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            DamagePlayerClientRpc();
        }
        else
        {
            DamageEffectsClientRpc();
        }
    }

    [ClientRpc]
    private void DamagePlayerClientRpc()
    {
        mapicon.enabled = false;
        GetComponent<PlayerInteraction>().enabled = false;
        GetComponent<PlayerShooting>().enabled = false;
    }

    [ClientRpc]
    private void DamageEffectsClientRpc()
    {
        Shake(0.2f, 0.25f);
        Slow(1f, 1);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealPlayerServerRpc(float hp)
    {
        health.Value += hp;
        if(health.Value > maxHealth)
            health.Value = maxHealth;
    }

    public void RevivePlayer()
    {
        RevivePlayerServerRpc();
        HealPlayerServerRpc(1000);
        GetComponent<PlayerInteraction>().enabled = true;
        GetComponent<PlayerShooting>().enabled = true;
        mapicon.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RevivePlayerServerRpc()
    {
        isAlive.Value = true;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    public void Slow(float duration, float tempSpeed, bool color = false)
    {
        slowed = true;
        fpsController.canSprint = false;
        fpsController.walkingSpeed = tempSpeed;
        endSlowTime = Time.time + duration;

        if(color)
        {
            sprintColor.color = superSpeedColor;
        }
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        Vector3 originalPos = cam.transform.localPosition;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Zastosowanie wyg³adzania i zmniejszania magnitude w czasie
            float smoothMagnitude = Mathf.SmoothStep(magnitude, 0, elapsed / duration);
            cam.transform.localPosition = new Vector3(x * smoothMagnitude + originalPos.x, y * smoothMagnitude + originalPos.y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Powrót do oryginalnej pozycji po zakoñczeniu trzêsienia
        cam.transform.localPosition = originalPos;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if(slowed && Time.time > endSlowTime)
        {
            slowed = false;
            fpsController.canSprint = true;
            fpsController.walkingSpeed = normalSpeed;
            sprintColor.color = sprintNormalColor;
        }

        if (!slowed)
        {
            if (fpsController.isRunning)
            {
                sprintValue -= Time.deltaTime * sprintUsePower;

                if (sprintValue <= 0)
                {
                    fpsController.canSprint = false;
                }
            }
            else if (sprintValue < maxSprint)
            {
                sprintValue += Time.deltaTime * sprintRegenPower;

                if (sprintValue > 0.05 * maxSprint)
                {
                    fpsController.canSprint = true;
                }
            }
            sprintSlider.value = sprintValue;
        }
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        hpSlider.value = health.Value;

        if(IsHost)
        {
            if (NetworkGameManager.instance.finalEvent)
            {
                int m = NetworkGameManager.instance.timeToWin1;
                int s = (int)NetworkGameManager.instance.timeToWin2;
                UpdateGameTime2ClientRpc(m,s);
            }
            else
            {
                int d = NetworkGameManager.instance.daysToEmergency;
                int h = NetworkGameManager.instance.hoursToEmergency;
                UpdateGameTimeClientRpc(d, h);
            }
        }
    }

    [ClientRpc]
    private void UpdateGameTimeClientRpc(int d, int h)
    {
        emergencyText.text = $"Time To Emergency: {d}d {h}h";
    }

    [ClientRpc]
    private void UpdateGameTime2ClientRpc(int m, int s)
    {
        emergencyText.text = $"Survive Zombies Attack: {m}m {s}s";
    }

    public void BuyUpgrade(GunUpgrade upgr, int goldToRemove)
    {
        AddRemoveGold(goldToRemove, true);
        mUpgrades.Add(upgr);
        AddUpgradesImage(upgr);
    }

    private void AddUpgradesImage(GunUpgrade upgr)
    {
        GameObject g = Instantiate(imgPref, imgParent);
        g.GetComponent<Image>().sprite = upgr.image;
        imgs.Add(g);
    }
}
