using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam;
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 5;
    public LayerMask interactionLayer;

    public PlayerItemHolder playerItemHolder;
    public Image crosshair;

    private Ray ray;
    private RaycastHit hit;

    void Update()
    {
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, interactDistance, interactionLayer))
        {
            crosshair.color = Color.red;

            if (Input.GetKeyDown(interactKey))
            {
                try
                {
                    IInteractable inter = hit.collider.GetComponent<IInteractable>();
                    inter.MakeInteraction();

                    /*if(inter.IsItem())
                    {
                        playerItemHolder.CollectItem(hit.collider.gameObject);
                    }*/
                }
                catch
                {
                    Debug.LogWarning("You try to interact with object that dont have IInteractable interface! Make sure that this object should be in interaction Layer!");
                }
            }
        }
        else
        {
            crosshair.color = Color.white;
        }
    }
}
