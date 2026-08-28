using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractor : MonoBehaviour
{
    private List<Interactable> nearbyInteractables = new List<Interactable>();
    private PlayerInput playerInput;
    private PlayerAnimationController playerAnimationController;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
    }

    void Update()
    {
        if (GameplayManager.Instance != null && GameplayManager.Instance.IsGameOver) return;
        if (Time.timeScale == 0f) return;

        if (playerInput.InteractPressedThisFrame)
        {
            nearbyInteractables.RemoveAll(i => i == null);

            if (nearbyInteractables.Count > 0)
            {
                if (playerAnimationController != null) playerAnimationController.SetInteracting(true);
                
                bool hasItem = false;
                List<Interactable> toInteract = new List<Interactable>();
                
                foreach (var interactable in nearbyInteractables)
                {
                    if (interactable is InteractableWorldItem)
                    {
                        toInteract.Add(interactable);
                        hasItem = true;
                    }
                }

                if (hasItem)
                {
                    foreach (var item in toInteract)
                    {
                        item.Interact();
                    }
                    nearbyInteractables.RemoveAll(i => i is InteractableWorldItem);
                }
                else
                {
                    nearbyInteractables[0].Interact();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable != null)
        {
            if (!nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Add(interactable);
                interactable.TogglePrompt(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable != null && nearbyInteractables.Contains(interactable))
        {
            interactable.TogglePrompt(false);
            nearbyInteractables.Remove(interactable);
        }
    }
}
