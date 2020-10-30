using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MembraneOrganelle
{
    public Vector2 position { get; private set; }

    public MembraneOrganelle(Vector2 position)
    {
        this.position = position;
    }

    public abstract void Attach(CellMembrane membrane);
}
