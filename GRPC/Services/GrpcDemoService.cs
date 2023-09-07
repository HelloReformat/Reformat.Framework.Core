using Grpc.Core;
using Microsoft.Extensions.Logging;
using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Services;
using WebApi;
using Request = WebApi.Request;

namespace Reformat.Framework.Core.GRPC.Services;

[SingleService]
public class GrpcDemoService : DemoProto.DemoProtoBase
{
    private readonly ILogger<GrpcDemoService> _logger;

    
    public GrpcDemoService(IocSingle iocSingle,ILogger<GrpcDemoService> _logger)
    {
        iocSingle.Autowired(this);
        this._logger = _logger;
    }

    public override Task<Reply> Test(Request request, ServerCallContext context)
    {
        _logger.LogInformation("服务端接收到消息： " + request.Name);
        
        Reply reply = new Reply();
        reply.Message = "响应返回消息： " + request.Name;
        return Task.FromResult(reply);
    }

    public override Task<Reply> ExceptionTest(Request request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Internal, "GRPC服务端异常测试"));
    }

    public override Task<Reply> OutTimeTest(Request request, ServerCallContext context)
    {
        Console.WriteLine("服务端接收到消息： " + request.Name + "，开始休眠10秒");
        Thread.Sleep(10000);
        
        Reply reply = new Reply();
        reply.Message = "响应返回消息： " + request.Name;
        return Task.FromResult(reply);
    }
}