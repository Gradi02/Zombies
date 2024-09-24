using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class DoorLock : NetworkBehaviour
{
    private static int[] code = new int[4];
    private List<int> currentCode = new();
    private string defaultText = "Enter Code";

    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private DoorLockButton[] buttons;
    [SerializeField] private DoorLockButton resetButton, openButton;
    [SerializeField] private Animator doorAnim;
    [SerializeField] private List<LockDoorHint> hints = new();
    [SerializeField] private List<Transform> anchors = new();
    [SerializeField] private NetworkObject objToDespawn;

    [ServerRpc(RequireOwnership = false)]
    public void GenerateCodeServerRpc()
    {
        for (int i = 0; i < 4; i++)
        {
            code[i] = Random.Range(0, 10);
        }

        List<FixedString512Bytes> hs = new();

        //Hints 1-4
        for (int i = 0; i < code.Length; i++)
        {
            if (Random.Range(0, i+1) == 0)
            {
                hs.Add($"You: The {Ordinal(i + 1)} Number Of The Code Is Equal To " + code[i]);
            }
            else
            {
                int rand = Random.Range(0, 10);
                string parity = (rand > code[i] ? "Smaller Than " : "Bigger Than ") + rand;
                if (rand == code[i]) parity = "Equal To " + rand;
                hs.Add($"You: The {Ordinal(i + 1)} Number Of The Code Is {parity}");
            }
        }       
        //Hint 5
        string sum = (code[0] + code[1] + code[2] + code[3]).ToString();
        hs.Add("You: The Sum Of All Digits Of The Code Is Equal To " + sum);
        //Hints 6-8
        for (int i = 0; i < code.Length-1; i++)
        {
            int sum2 = code[i] + code[i + 1];
            hs.Add($"You: The Sum Of The {Ordinal(i+1)} And {Ordinal(i + 2)} Numbers Is Equal To {sum2}");
        }

        for (int i = 0; i < 6; i++)
        {
            int rand = Random.Range(0, hs.Count);
            hints[i].hint.Value = hs[rand];
            hs.RemoveAt(rand);

            int pos = Random.Range(0, anchors.Count);
            SetTransformsClientRpc(i, pos);
        }
    }

    [ClientRpc]
    private void SetTransformsClientRpc(int i, int pos)
    {
        hints[i].transform.position = anchors[pos].transform.position;
        hints[i].transform.rotation = anchors[pos].transform.rotation;
        anchors.RemoveAt(pos);
    }

    string Ordinal(int number)
    {
        switch (number)
        {
            case 1: return "First";
            case 2: return "Second";
            case 3: return "Third";
            case 4: return "Fourth";
            default: return number + "th";
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
            bool check = false;

            if (currentCode.Count == 4)
            {
                check = true;
                for (int i = 0; i < 4; i++)
                {
                    if (currentCode[i] != code[i])
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                    doorAnim.SetTrigger("open");
            }

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
            DespawnTriggerObjectServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    private void DespawnTriggerObjectServerRpc()
    {
        objToDespawn.Despawn();
        Destroy(objToDespawn);
    }
}
