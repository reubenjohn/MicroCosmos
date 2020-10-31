using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IGeneTranscriber
{
    object Deserialize(JToken gene);

    object Mutate(object gene);
}

public abstract class IGeneTranscriber<T> : IGeneTranscriber
{
    public abstract T Deserialize(JToken gene);
    object IGeneTranscriber.Deserialize(JToken gene) => Deserialize(gene);

    public abstract T Mutate(T gene);
    object IGeneTranscriber.Mutate(object gene) => Mutate((T)gene);
}