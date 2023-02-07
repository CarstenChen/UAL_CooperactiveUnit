using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactibes : MonoBehaviour
{
    [Header("Interactibe Settings")]
    public bool destroyAfterCollected;
    public ParticleSystem particle = default;
    protected GameObject interactionUI;
    protected PlayerController player;
    protected Animator animator;

    protected bool canInteract = true;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        interactionUI = player.transform.Find("PlayerCanvas").Find("InteractionUI").gameObject;
    }
    private void Start()
    {
        interactionUI.SetActive(false);
        if (particle != default)
            particle.Play();
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
                if (particle != default)
                    particle.Stop();
            }
        }

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            interactionUI.SetActive(false);
    }
}
