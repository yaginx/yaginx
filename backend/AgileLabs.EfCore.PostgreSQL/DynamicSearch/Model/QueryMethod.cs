namespace AgileLabs.EfCore.PostgreSQL.DynamicSearch.Model;

public enum QueryMethod
{
    /// <summary>
    /// 未设置, 使用自动探测规则
    /// </summary>
    AutoDetect = -1,
    /// <summary>
    ///     等于
    /// </summary>
    //[GlobalCode("=", OnlyAttribute = true)]
    Equal = 0,

    /// <summary>
    ///     小于
    /// </summary>
    //// [GlobalCode("<", OnlyAttribute = true)]
    LessThan = 1,

    /// <summary>
    ///     大于
    /// </summary>
    // [GlobalCode(">", OnlyAttribute = true)]
    GreaterThan = 2,

    /// <summary>
    ///     小于等于
    /// </summary>
    // [GlobalCode("<=", OnlyAttribute = true)]
    LessThanOrEqual = 3,

    /// <summary>
    ///     大于等于
    /// </summary>
    // [GlobalCode(">=", OnlyAttribute = true)]
    GreaterThanOrEqual = 4,

    /// <summary>
    ///     输入一个时间获取当前天的时间块操作, ToSql未实现，仅实现了IQueryable
    /// </summary>
    // [GlobalCode("between", OnlyAttribute = true)]
    [Obsolete("没有实现此属性功能")]
    DateBlock = 8,

    /// <summary>
    ///     不等于
    /// </summary>
    NotEqual = 9,

    /// <summary>
    ///     开始于
    /// </summary>
    StartsWith = 10,

    /// <summary>
    ///     结束于
    /// </summary>
    EndsWith = 11,

    /// <summary>
    ///     处理Like的问题
    /// </summary>
    Contains = 12,

    /// <summary>
    ///     处理In的问题
    /// </summary>
    StdIn = 13,

    /// <summary>
    ///     处理Not In的问题
    /// </summary>
    StdNotIn = 14,

    /// <summary>
    /// 不包涵
    /// </summary>
    NotLike = 15,
}