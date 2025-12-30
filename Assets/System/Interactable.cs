using UnityEngine;

public static class Interactable
{
    public static IInteractable Current;

    public static void TryInteract()
    {
        Current?.Interact();
    }
}

public interface IInteractable
{
    void Interact();
}