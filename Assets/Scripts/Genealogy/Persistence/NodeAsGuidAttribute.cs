using System;

namespace Genealogy
{
    [AttributeUsage(AttributeTargets.Field |
                    AttributeTargets.Property |
                    AttributeTargets.Parameter)
    ]
    public class NodeAsGuidAttribute : Attribute
    {
    }
}