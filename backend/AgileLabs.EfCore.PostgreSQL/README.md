# PostgreSQL EFCore使用

## 注册

服务注册
```csharp
services.RegisterDbContext<CenterDbContext>();
```

自动提交注册
```csharp
public class AutoCommiterPiplineRegister : IRequestPiplineRegister
{
    public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
    {
        piplineActions.Register("AutoCommiter", RequestPiplineStage.BeginOfPipline, app => app.UseMiddleware<AutoCommiterMiddleware>());
        return piplineActions;
    }
}

or

piplineActions.Register("AutoCommiterMiddleware", RequestPiplineStage.BeginOfPipline, app => app.UseMiddleware<AutoCommiterMiddleware>());
```

## 设计时DbContextFactory
新建一个 DesignDbContextFactory:DesignContextFactoryAbstract, 在构造函数中传入ConnectionString即可

## 如何在多租户的场景中使用?

## 使用Repository
1. 在使用上可以让Repository继承自: CrudRepository<TEntity>
1. 可以直接注入CrudRepository或者CrudRepository<TEntity>


## 其他附带处理的内容
1. 根据Entity类名转换出表名
1. DateTime=>DateTimeOffset的自动转换, 解决PostgreSQL不接受非UTC Time的问题
1. WoDbContextFactory用来创建DbContext
1. DbDataSourceManager用来管理连接串