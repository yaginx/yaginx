using System.Reflection;
using Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;

namespace Yaginx.SelfManagement.Features;

public class ManagedApiFeature : IManagedApiFeature
{
    public ManagedApiMetadataModel Model { get; set; }
    public Type ServiceType { get; set; }
    public MethodInfo MethodInfo { get; set; }
}

