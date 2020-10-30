using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGeneTranscriber
{
    string Serialize(object gene);
    object Deserialize(string sequence);

    object Mutate(object gene);
}

public abstract class IGeneTranscriber<T> : IGeneTranscriber
{
    public abstract string Serialize(T gene);
    public abstract T Deserialize(string sequence);

    public abstract T Mutate(T gene);

    string IGeneTranscriber.Serialize(object gene) => Serialize((T)gene);
    object IGeneTranscriber.Deserialize(string sequence) => Deserialize(sequence);

    object IGeneTranscriber.Mutate(object gene) => Mutate((T)gene);
}