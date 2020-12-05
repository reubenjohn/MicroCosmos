namespace Chemistry
{
    // public static class SubstanceMassUtilities
    // {
    //     public static SubstanceMass<T>[] Copy<T>(this SubstanceMass<T>[] substanceMasses) where T : Enum =>
    //         substanceMasses
    //             .Select(substanceMass => new SubstanceMass<T>()
    //                 {substance = substanceMass.substance, mass = substanceMass.mass}).ToArray();
    //
    //     public static float TotalMass<T>(this SubstanceMass<T>[] substanceMasses) where T : Enum =>
    //         substanceMasses.Sum(substanceMass => substanceMass.mass);
    //
    //     public static void ScaledTo<T>(this SubstanceMass<T>[] substanceMasses, float targetMass) where T : Enum =>
    //         ScaleBy(substanceMasses, targetMass / substanceMasses.TotalMass());
    //
    //     public static void ScaleBy<T>(this SubstanceMass<T>[] substanceMasses, float scaleFactor) where T : Enum
    //     {
    //         for (var i = 0; i < substanceMasses.Length; i++)
    //             substanceMasses[i] = new SubstanceMass<T>
    //                 {substance = substanceMasses[i].substance, mass = substanceMasses[i].mass * scaleFactor};
    //     }
    //
    //     public static ImmutableSubstanceMass<T>[] ToImmutable<T>(this SubstanceMass<T>[] substanceMasses)
    //         where T : Enum => substanceMasses.Select(substanceMass => substanceMass.ToImmutable()).ToArray();
    //
    //     public static SubstanceMass<T>[] ToMutable<T>(this ImmutableSubstanceMass<T>[] substanceMasses)
    //         where T : Enum => substanceMasses.Select(substanceMass => substanceMass.ToMutable()).ToArray();
    // }
}