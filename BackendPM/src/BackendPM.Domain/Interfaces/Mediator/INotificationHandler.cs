// 此文件已不再需要，因为我们直接使用MediatR的接口
// 保留此文件仅作为历史记录，在确认项目稳定运行后可以删除

/*
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BackendPM.Domain.Interfaces.Mediator;

/// <summary>
/// 通知处理器接口
/// </summary>
/// <typeparam name="TNotification">通知类型</typeparam>
public interface INotificationHandler<in TNotification> : MediatR.INotificationHandler<TNotification>
    where TNotification : MediatR.INotification
{
    // 继承自MediatR.INotificationHandler<TNotification>的Handle方法
    // 不需要在这里重新声明
}
*/