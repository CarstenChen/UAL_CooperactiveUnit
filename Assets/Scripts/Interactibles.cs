using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactibes : MonoBehaviour
{
    [Header("Interactibe Settings")]
    public bool destroyAfterCollected;
    public ParticleSystem particle = default;
    public PlayerController player;
    public GameObject interactionUI;

    protected Animator animator;

    protected bool canInteract = true;


    private void Awake()
    {
        animator = GetComponent<Animator>();

    }
    private void Start()
    {
        interactionUI.SetActive(false);
    }
    public virtual void Interact()
    {

    }

    public virtual void OnTriggerForcedEnd()
    {

    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player"&& canInteract)
        {
            if ( !LinesManager.isPlayingLines)
                interactionUI.SetActive(true);
            else
                interactionUI.SetActive(false);

            if (player.playerInput.InteractInput)
            {
                Interact();
                interactionUI.SetActive(false);
                canInteract = false;
            }
        }

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            interactionUI.SetActive(false);
    }
}
