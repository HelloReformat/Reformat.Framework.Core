namespace Reformat.Framework.Core.GRPC.Attributes;

public class ProtoSourceAttribute : Attribute
{
    public string PackageName { get; set; }
    public string ServiceName { get; set; }
    public string NameSpace { get; set; }
    public ProtoSourceAttribute(string packageName, string serviceName = "",string nameSpace="")
    {
        PackageName = packageName;
        ServiceName = serviceName;
        NameSpace= nameSpace;
    }
}