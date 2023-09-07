# IOC增强

## 更新说明
### 20230608：
+ 语义上区分 瞬态 域态 单例 三种类型
+ 注解更改为 `[ScopedService] [SingleService] [TrabsientService]`
+ 自动加载构造器 由 `IocService `更改为 `IocScoped` 和 `IocSingle`
+ 解决了Service层 ， BasePage等公共基础层需要使用单例的场景

使用方式规范如下：

| 自动加载构造器 | Controller层 | Service层 | 加载 Scoped Bean | 加载 Single Bean | 加载 Trabsient Bean |
| :------------: | :----------: | :-------: | :--------------: | :--------------: | :-----------------: |
| `**IocScoped** |      √       |     ×     |        √         |        √         |          √          |
| **IocSingle**  |      ×       |     √     |        ×         |        √         |          √          |

Warning：如果需要在单例服务中使用Scoped域中Bean，请使用构造器注入

备注： 本次升级所带来的IOC报错，可通过以下步骤处理
+ 可将程序中原有的 `IocService` 修改为 `IocScoped`
+ 将原有单例注解 `[Service]` 修改为 `[SingleService]`




## 一、服务注册

**启用方式：**

```c#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServicesByAttribute();
```

**单例注册：**

```C#
[SingleService]
public class MyService
{
    //functions
}
```

**请求域注册：**

```C#
[ScopedService]
public class MyService
{
    //functions
}
```

> Warning: 未指定注册类型，默认将对当前类进行注册

**指定注册类型 (支持多个)：**

```c#
[SingleService/ScopedService(typeof(IService))]
[SingleService/ScopedService(typeof(IService1),typeof(IService2))]
public class MyService
{
    //functions
}
```



## 二、全局获取

**说明：**

+ 可突破框架注入限制，在Web应用任意位置获取容器池中的 **瞬态或单例** 对象

**启用方式：**

```C#
var app = builder.Build();
// IOC 全局辅助
app.UseServiceLocator();
```

**调用示例：**

```C#
// 存在多个则默认取最后一个Bean
ServiceLocator.GetService<IService>();
// 获取所有注册为IService的Bean
ServiceLocator.GetServices<IService>();
```

> Warning：通过`ServiceLocator`方式，不能找到注册为`Scoped`的Bean，需要获取请求域Bean详见`Autowird`



## 三、依赖注入

**说明：**

+ `IocScoped` 可通过调用链向下传递使用
+ `IocScoped` 获取的Bean是 当前请求域范围的容器，包含所有生命周期形式的Bean
+ 不通过依赖注入进行创建的`IocService`，只能拿到 瞬态或单例 对象

**使用方式：**

```C#
[ApiController]
[Route("[controller]/[Action]")]
public class MyController
{
    private IService a;
    private IService b;
    private IService c;
    
    public MyController(IocScoped iocService)
    {
        a = iocService.GetService<ServiceTest1>();
        b = iocService.GetService<ServiceTest2>();
        c = iocService.GetService<ServiceTest3>();
    }
    
    //functions
}
```



## 四、属性注入

**使用方式：**

```C#
[ApiController]
[Route("[controller]/[Action]")]
public class MyController
{  
    // 自动找到 注册为 当前变量什么类型 的实例
    // 存在多个实例则默认拿最后一个
    [Autowired]
    public IService s1;
    [Autowired]
    public SingletonServiceTest s11;
    
    // 自动找到 注册为 指定类型 的实例
    [Autowired(typeof(Service1))]
    public IService s2;
    [Autowired(typeof(Service2))]
    public IService s3;
    
    public MyController(IocScoped iocService)
    {
        IocService.Autowired(this);
    }
    
    //functions
}
```