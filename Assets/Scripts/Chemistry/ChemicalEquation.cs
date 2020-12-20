using System;
using UnityEngine;

namespace Chemistry
{
    public static class ChemicalEquation
    {
        public static Term<T> M<T>(this T substance, float mass) where T : Enum => new Term<T>(substance, mass);
        public static Term<T> M<T>(this T substance, double mass) where T : Enum => M(substance, (float) mass);

        public class Term<T> : ImmutableSubstanceMass<T> where T : Enum
        {
            public Term(T substance, float mass) : base(substance, mass) { }

            public static Expression<T> operator +(Term<T> a, Term<T> b) =>
                new Expression<T> {[a.substance] = a.mass, [b.substance] = b.mass};

            public static Reaction<T> operator >(Term<T> a, Term<T> b) =>
                new Expression<T> {[a.substance] = a.mass} >
                new Expression<T> {[b.substance] = b.mass};

            public static Reaction<T> operator <(Term<T> a, Term<T> b) => throw new NotImplementedException();
        }

        public class Expression<T> : MixtureDictionary<T> where T : Enum
        {
            public Expression() { }

            public Expression(Mixture<T> mix)
            {
                for (var i = 0; i < mix.contents.Length; i++)
                    if (!Mathf.Approximately(mix.contents[i], 0))
                        Add((T) Enum.ToObject(typeof(T), i), mix.contents[i]);
            }

            public static Expression<T> operator +(Expression<T> a, Term<T> b) =>
                new Expression<T>(a.ToMixture() + new Expression<T> {[b.substance] = b.mass}.ToMixture());

            public static Reaction<T> operator >(Expression<T> a, Expression<T> b) =>
                new Reaction<T>(a.ToMixture(), b.ToMixture());

            public static Reaction<T> operator >(Expression<T> a, Term<T> b) =>
                a > new Expression<T> {[b.substance] = b.mass};

            public static Reaction<T> operator >(Term<T> a, Expression<T> b) =>
                new Expression<T> {[a.substance] = a.mass} > b;

            public static Reaction<T> operator <(Expression<T> a, Expression<T> b) =>
                throw new NotImplementedException();

            public static Reaction<T> operator <(Expression<T> a, Term<T> b) => throw new NotImplementedException();
            public static Reaction<T> operator <(Term<T> a, Expression<T> b) => throw new NotImplementedException();
        }
    }
}