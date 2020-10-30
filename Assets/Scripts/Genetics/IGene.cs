using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGene
{
    string Serialize();
    object Deserialize(string sequence);

    // object Duplicate();
    object Mutate();
}

public abstract class IGene<T> : IGene
{
    public abstract string Serialize();
    public abstract T Deserialize(string sequence);

    // public abstract T Duplicate();
    public abstract T Mutate();


    object IGene.Deserialize(string sequence) => Deserialize(sequence);
    // object IGeneTranscriber.Duplicate() => Duplicate();
    object IGene.Mutate() => Mutate();
}
