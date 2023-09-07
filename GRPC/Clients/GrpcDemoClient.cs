using Grpc.Core;
using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Services;
using WebApi;

namespace Reformat.Framework.Core.GRPC.Clients;

[SingleService]
public class GrpcDemoClient
{

    [Autowired]
    private DemoProto.DemoProtoClient Client;
    
    public GrpcDemoClient(IocSingle iocSingle)
    {
        iocSingle.Autowired(this);
    }

    public async Task<string> TestAsync(string name)
    {
        var reply = await Client.TestAsync(new Request { Name = name });
        return reply.Message;
    }
    
    public string Test(string name)
    {
        var timeout = TimeSpan.FromSeconds(5);
        var callOptions = new CallOptions().WithDeadline(DateTime.UtcNow.Add(timeout));
        
        var reply = Client.Test(new Request { Name = name });
        return reply.Message;
    }
    
    public async Task<string> ExceptionTestAsync(string name)
    {
        var reply = await Client.ExceptionTestAsync(new Request { Name = name });
        return reply.Message;
    }
    
    public string OutTimeTest(string name)
    {
        // 超时设置
        var timeout = TimeSpan.FromSeconds(5);
        var callOptions = new CallOptions().WithDeadline(DateTime.UtcNow.Add(timeout));
        
        var reply = Client.OutTimeTest(new Request { Name = name },callOptions);
        return reply.Message;
    }
}