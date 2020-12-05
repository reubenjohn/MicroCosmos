using System;
using System.Collections.Generic;

namespace Chemistry
{
    public class MixtureDictionary<T> : Dictionary<T, float> where T : Enum
    {
        // public static Reaction<T> operator >(MixtureDictionary<T> a, MixtureDictionary<T> b) => new Reaction<T>(a, b);
        //
        // public static Reaction<T> operator <(MixtureDictionary<T> a, MixtureDictionary<T> b) =>
        //     throw new NotImplementedException();

        public override bool Equals(object obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();
    }
}