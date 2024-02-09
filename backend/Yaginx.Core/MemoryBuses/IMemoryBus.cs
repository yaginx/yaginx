using Yaginx.MemoryBuses.Events;

namespace Yaginx.MemoryBuses;

/// <summary>
/// 消息总线接口, 系统所有消息(内存消息, MQ消息)Api操作入口
/// </summary>
public interface IMemoryBus
{

    /// <summary>
    /// 发布事件
    /// </summary>
    /// <param name="event"></param>
    /// <param name="optionalHeaders"></param>
    /// <param name="isLongTimeMessage"></param>
    /// <param name="isShareWorkContext">是否共享WorkContext(事务,异常),<br/>
    /// true: 与主业务共享同样的上下文, 主业务可以收到事件处理的异常信息,<br/>
    /// false: 将使用线程池创建新的线程处理事件逻辑(主线程不等待子线程完成),使用全新的WorkContext, 由于使用了新的子线程, 主业务无法捕获异常</param>
    /// <returns></returns>
    Task SendAsync(Event @event, Dictionary<string, string> optionalHeaders = null, bool isLongTimeMessage = false, bool isShareWorkContext = true);

    /// <summary>
    /// 发布事件
    /// </summary>
    /// <param name="event"></param>
    /// <param name="optionalHeaders"></param>
    /// <param name="isLongTimeMessage"></param>
    /// <param name="isShareWorkContext"></param>
    void Send(Event @event, Dictionary<string, string> optionalHeaders = null, bool isLongTimeMessage = false, bool isShareWorkContext = true);
}
