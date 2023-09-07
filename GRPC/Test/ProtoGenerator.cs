using System.Reflection;
using NUnit.Framework;
using Reformat.Framework.Core.GRPC.Test.demo;
using Reformat.Framework.Core.GRPC.Utils;

namespace Reformat.Framework.Core.GRPC.Test;

public class ProtoGenerator
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Run()
    {
        var convertInfo = ProtoUtils.Convert(Assembly.GetAssembly(typeof(IBuilderDemo)));
        convertInfo.ForEach(b =>
        {
            b.CreateCode();
        });
    }
}