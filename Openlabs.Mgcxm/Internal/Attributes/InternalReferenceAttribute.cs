// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Internal;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class InternalReferenceAttribute : Attribute
{
    public InternalReferenceAttribute(string assemblyName)
    {
        AssemblyName = assemblyName;
    }
    
    public string AssemblyName { get; }
}