using AgileLabs;
using AgileLabs.WorkContexts.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Yaginx.DataStores;

namespace Yaginx.ApiControllers;

public abstract class YaginxControllerBase : ControllerBase
{
    //为了便于编写代码, 下面的部分属性,以下划线小写开头, 保持与构造函数注入的本地依赖方法命名一致
    protected IWorkContextCore _workContext => AgileLabContexts.Context.CurrentWorkContext;
    protected IMapper _mapper => AgileLabContexts.Context.Mapper;
    protected ActivitySource _activitySource => _workContext.Resolve<ActivitySource>();

    protected DatabaseRepository _databaseRepository => _workContext.Resolve<DatabaseRepository>();
}
