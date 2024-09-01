using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : NetworkBehaviour
{
    private float maxHealth = 100;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Camera cam;
    [SerializeField] SC_FPSController fpsController;
    private float endSlowTime;
    private float normalSpeed;
    [SerializeField] private CharacterController controller;
    [SerializeField] private TextMeshProUGUI goldTxt;

    public int gold { get; private set; } = 100;
    [HideInInspector] public NetworkVariable<bool> isAlive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> health = new NetworkVariable<float>(99, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector]
    public float Health {
        get { return health.Value; } 
        private set
        {
            if (health.Value != value && IsOwner)
            {
                health.Value = value;
                OnHealthChanged();
            }
        }
    }

    public int maxUpgrades { get; private set; } = 10;
    public List<GunUpgrade> mUpgrades { get; private set; } = new List<GunUpgrade>();
    [SerializeField] private GameObject imgPref;
    [SerializeField] private Transform imgParent;
    private List<GameObject> imgs = new List<GameObject>();

    private void Start()
    {
        if (!IsOwner) return;

        normalSpeed = fpsController.walkingSpeed;
        hpSlider.maxValue = maxHealth;
        Health = maxHealth;
        goldTxt.text = "Gold: " + gold;
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
            gameObject.transform.position = new Vector3(-30, -6, -30);
        if (IsOwner && controller != null)
            controller.enabled = true;

        base.OnNetworkSpawn();      
    }

    private void OnHealthChanged()
    {
        hpSlider.value = Health;
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

    public void DamagePlayer(float dmg)
    {
        if (Health - dmg >= 0)
            Health -= dmg;
        else
            Health = 0;

        if (Health <= 0)
        {
            isAlive.Value = false;
        }

        Shake(0.2f, 0.25f);
        Slow(2f, 1);
    }


    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    public void Slow(float duration, float tempSpeed)
    {
        fpsController.canSprint = false;
        fpsController.walkingSpeed = tempSpeed;
        endSlowTime = Time.time + duration;
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

        if(Time.time > endSlowTime)
        {
            fpsController.canSprint = true;
            fpsController.walkingSpeed = normalSpeed;
        }
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
