using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DoorLock : NetworkBehaviour
{
    private static int[] code = new int[4];
    private List<int> currentCode = new();
    private string defaultText = "Enter Code";

    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private DoorLockButton[] buttons;
    [SerializeField] private DoorLockButton resetButton, openButton;
    [SerializeField] private Animator doorAnim;

    [ServerRpc(RequireOwnership = false), ContextMenu("code")]
    private void GenerateCodeServerRpc()
    {
        for (int i = 0; i < 4; i++)
        {
            code[i] = Random.Range(0, 10);
            Debug.Log(code[i]);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnterNumberServerRpc(int num)
    {
        if(num == 10) //reset
        {
            ResetCodeClientRpc();
        }
        else if(num == 11) //open
        {
            bool check = true;
            for(int i = 0; i < 4; i++)
            {
                if (currentCode[i] != code[i])
                {
                    check = false;
                    break;
                }
            }

            if (check)
                doorAnim.SetTrigger("open");

            HandleEnterCodeClientRpc(check);
        }
        else
        {
            AddNumberClientRpc(num);
        }
    }

    [ClientRpc]
    private void AddNumberClientRpc(int newnum)
    {
        currentCode.Add(newnum);
        codeText.text = "";
        for(int i=0; i<currentCode.Count; i++)
        {
            codeText.text += "[" + currentCode[i] + "] "; 
        }

        if(currentCode.Count == 4)
        {
            foreach(var b in buttons)
            {
                b.gameObject.layer = 0;
            }
        }
    }

    [ClientRpc]
    private void ResetCodeClientRpc()
    {
        currentCode.Clear();
        codeText.text = defaultText;

        foreach (var b in buttons)
        {
            b.gameObject.layer = 10;
        }
    }

    [ClientRpc]
    private void HandleEnterCodeClientRpc(bool good)
    {
        StartCoroutine(IEEnterCode(good));
    }

    private IEnumerator IEEnterCode(bool good)
    {
        if(good)
        {
            codeText.color = Color.green;
            foreach (var b in buttons)
            {
                b.gameObject.layer = 0;
            }
            resetButton.gameObject.layer = 0;
            openButton.gameObject.layer = 0;
            yield return new WaitForSeconds(2f);

            codeText.text = "Door Open";
            if(IsHost)
            {
                doorAnim.gameObject.GetComponent<NetworkObject>().Despawn();
                Destroy(doorAnim.gameObject);
            }
        }
        else
        {
            codeText.color = Color.red;
            foreach (var b in buttons)
            {
                b.gameObject.layer = 0;
            }
            resetButton.gameObject.layer = 0;
            openButton.gameObject.layer = 0;
            yield return new WaitForSeconds(2f);

            codeText.color = Color.white;
            resetButton.gameObject.layer = 10;
            openButton.gameObject.layer = 10;
            ResetCodeClientRpc();
        }
    }
}
