using UnityEngine;

public abstract class SpecialStation : Station
{
    public override void Interact()
    {
        // Lógica especial, por ejemplo dormir, comer, etc.
        Debug.Log("Interactuando con una estación especial.");
    }
}
