// using System.ComponentModel.DataAnnotations;
// using Grpc.Core;
// using Microsoft.AspNetCore.Mvc;
// using Reformat.Framework.Core.Aspects;
// using Reformat.Framework.Core.GRPC.Clients;
// using Reformat.Framework.Core.IOC.Attributes;
// using Reformat.Framework.Core.IOC.Services;
// using Reformat.Framework.Core.MVC;
//
// namespace Reformat.Framework.Core.GRPC.Controller;
//
// /// <summary>
// /// GRPC调试
// /// </summary>
// [ApiSwitch(Enable = false,OnlyDevEnv = true)]
// [ExceptionHandle]
// public class GrpcDebug: BaseController
// {
//     [Autowired]
//     private GrpcDemoClient grpcDemoClient;
//     public GrpcDebug(IocScoped iocScoped) : base(iocScoped)
//     {
//     }
//     
//     [HttpGet]
//     public APIResponse<string> SyncTest([Required][FromQuery] string name)
//     {
//         return Api.RestSuccess(grpcDemoClient.Test(name));
//     }
//     
//     [HttpGet]
//     public async Task<APIResponse<string>> AsyncTest([Required][FromQuery] string name)
//     {
//         string testAsync = await grpcDemoClient.TestAsync(name);
//         return Api.RestSuccess(testAsync);
//     }
//     
//     [HttpGet]
//     public async Task<APIResponse<string>> ExceptionTest([Required][FromQuery] string name)
//     {
//         try
//         {
//             string testAsync = await grpcDemoClient.ExceptionTestAsync(name);
//             return Api.RestSuccess(testAsync);
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//     
//     [HttpGet]
//     public APIResponse<string> OutTimeTest([Required][FromQuery] string name)
//     {
//         try
//         {
//             string testAsync = grpcDemoClient.OutTimeTest(name);
//             return Api.RestSuccess(testAsync);
//         }
//         catch (RpcException e)
//         {
//             Console.WriteLine("超时：" + e.Message);
//             throw;
//         }
//     }
// }