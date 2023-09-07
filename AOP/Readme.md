# AOP增强

**一、技术选型**

名称：Aspect Injector	

Version：2.8.1



**二、选型依据**

+ 开源轻量级 AOP 框架
+ 使用静态织入，而非动态代理模式，确保执行效率基础下，不破坏原有的程序设计
+ 无运行时依赖，可独立使用，不局限NetCore平台
+ 所有必需的工作都是在编译时完成，无性能问题
+ 可结合增强的IOC功能进行使用
+ 不依赖Web框架，可在任意位置，任意层次进行切面处理
+ 切面支持异步执行



**三、使用场景**

+ 日志处理、缓存处理、异常处理、权限处理...



**四、简易示例**

形式一：单注解单切面

```c#
//定义切面&触发器
[Aspect(Scope.Global)]
[Injection(typeof(LogAttribute))]
public class LogAttribute : Attribute
{
    // Advice Definition ...
}

// 织入
public class TestService(){
    [Log]
    public void run(){};
}
```

形式二：单注解多切面

```c#
[Aspect(Scope.Global)]
public class LogAspect
{
    // Advice Definition ...
}

[Aspect(Scope.Global)]
public class CacheAspect
{
    // Advice Definition ...
}

[Injection(typeof(LogAspect))]
[Injection(typeof(CacheAspect),Priority = 2)]
public class LogCacheAttribute : Attribute
{
    // Attribute Definition ...
}

public class TestService(){
    [LogCache]
    public void run(){};
}
```



**五、切面定义详解**

```C#
// 声明为切面且对切面生命周期定义（单例 OR 瞬态）
[Aspect(Scope.Global)]
public class LogAspect
{
    // 声明为Advice（横切行为），以及对Pointcut定义（切点）
    [Advice(Kind.Before, Targets = Target.Method)]
    public void Before([Argument(Source.Name)]string name)
    {
        Console.WriteLine("Before");
    }

    [Advice(Kind.After, Targets = Target.Method)]
    public void After([Argument(Source.Name)]string name)
    {
        Console.WriteLine("After");
    }
    
    [Advice(Kind.Around, Targets = Target.Method)]
    public virtual object Around(
        [Argument(Source.Name)]string name,
        [Argument(Source.Arguments)] object[] arguments,
        [Argument(Source.Target)] Func<object[], object> target)
    {
        Console.WriteLine("On Around Before");
        var result = target(arguments);
        Console.WriteLine("On Around After");
        return result;
    }
}
```

切点说明：

> ```C#
> [Advice(Kind.After, Targets = Target.Method)]
> 
> Kind.Before | Kind.After | Kind.Around		// 切点触发器：执行前，执行后，执行前后
> Target.Constructor | Target.Static | Target.Method   // 切点作用域：更多详见Target定义
> ```

参数说明：

> ```C#
> [Argument(Source.Instance)] object Instance,				// 获取实例（仅限静态对象）
> [Argument(Source.Metadata)] MethodBase Metadata,			// 获取方法
> [Argument(Source.Type)] Type Type,							// 织入对象类
> [Argument(Source.Name)] string name,						// 方法名称
> [Argument(Source.Arguments)] object[] arguments,			// 方法参数
> [Argument(Source.ReturnType)] Type ReturnType,				// 返回类型
> [Argument(Source.ReturnValue)] object ReturnValue, 			// 返回值（只有Kind.After可获取）
> [Argument(Source.Triggers)] Attribute[] Triggers,			// 获取当前对象所持有的Aop特性
> [Argument(Source.Target)] Func<object[], object> target)	// 不懂，好像是用于返回值用的
> ```



**六、触发器详解**

```C#
[Injection(typeof(LogAspect),Priority = 1,Propagation = PropagateTo.Members)]
[Injection(typeof(CacheAspect),Priority = 2)]
public class LogCacheAttribute : Attribute
{
    // Attribute Definition ...
}
```

补充说明：

> ```c#
> Aspect = typeof(LogAspect)  // 切面
> Priority = 1				// 触发优先级
> Propagation = PropagateTo.Members // 通知策略
> ```

执行顺序：

- Before：依序將 Advice 插入在目标方法的`最上面`（头插）。
- Around：依序將 Advice `往內`包裝目標方法（内包）。
- After：依序將 Advice 插入在目标方法的`最下面`（尾插）。

+ 多切面的情况下，也按照上序规则进行排序，可通过`Priority`对优先级进行排序



