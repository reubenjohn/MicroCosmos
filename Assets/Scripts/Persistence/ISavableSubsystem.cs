using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Persistence
{
    public interface ISavableSubsystem
    {
        string GetID();
        int GetPersistenceVersion();
        Type GetSavableType();
        IEnumerable Save();
        void Load(IEnumerable save);
        JsonSerializer GetSerializer();
    }

    public interface ISavableSubsystem<T> : ISavableSubsystem
    {
        new IEnumerable<T> Save();
        void Load(IEnumerable<T> save);
    }
}