using UnityEngine;

public abstract class Station : MonoBehaviour, IInteractable
{
    public GameEnums.WorkstationType workstationType;
    public virtual void Interact()
    {
        // Por defecto, no hace nada. Las clases hijas pueden sobrescribir.
    }
}