using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OutlineLookAt : MonoBehaviour
{
    [SerializeField]
    private float maxSelectDist;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask interactableLayers;

    private static Outline currentOutliner;
    private static Outline prevOutliner;

    private static Interactable currentInteractable;

    private static Color validInteraction = new Color(0f, 1f, 0.2f);
    private static Color lockedInteraction = new Color(1f, 0f, 0f);

    void Update()
    {
        Vector3 target = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 direction = (target - cam.transform.position).normalized;
        Ray ray = new Ray(cam.transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxSelectDist, interactableLayers))
		{
            currentOutliner = hit.collider.GetComponent<Outline>();
            currentInteractable = hit.collider.GetComponent<Interactable>();

            if (prevOutliner != currentOutliner)
            {
                HideOutline();
                ShowOutline();
            }

            prevOutliner = currentOutliner;
		}
		else
		{
            HideOutline();
            currentInteractable = null;
        }

        //Interact
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) && currentInteractable != null)
		{
            currentInteractable.Interact();
		}
    }

    void ShowOutline()
	{
        if (currentOutliner != null)
        {
            if (currentInteractable != null && GameDiagramManager.IsUnlocked(currentInteractable.GetRoomCode()))
            {
                currentOutliner.OutlineColor = validInteraction;
                currentOutliner.enabled = true;
            }
            else
            {
                currentOutliner.OutlineColor = lockedInteraction;
                currentOutliner.enabled = true;
            }
        }
    }
    
    void HideOutline()
	{
        if (prevOutliner != null)
        {
            prevOutliner.enabled = false;
            prevOutliner = null;
        }
    }
}

