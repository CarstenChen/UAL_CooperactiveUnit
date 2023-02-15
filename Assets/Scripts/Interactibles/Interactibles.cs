using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactibes : MonoBehaviour
{
    [Header("Interactibe Settings")]
    public GameObject destroyAfterCollected;
    public GameObject particle;
    protected GameObject interactionUI;
    protected PlayerController player;
    protected Animator animator;

    protected bool canInteract = true;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        interactionUI = player.transform.Find("PlayerCanvas").Find("InteractionUI").gameObject;
    }
    protected virtual void Start()
    {
        interactionUI.SetActive(false);
        if (particle != null)
            particle.SetActive(true);
    }
    public virtual void Interact()
    {
        if (destroyAfterCollected!=null)
        {
            destroyAfterCollected.GetComponent<MeshRenderer>().enabled = false;
        }

        if (particle != default)
            particle.SetActive(false);
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
