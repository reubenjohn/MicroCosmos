using System;

namespace Genealogy.Persistence
{
    [AttributeUsage(AttributeTargets.Field |
                    AttributeTargets.Property |
                    AttributeTargets.Parameter)
    ]
    public class NodeAsGuidAttribute : Attribute
    {
    }
}