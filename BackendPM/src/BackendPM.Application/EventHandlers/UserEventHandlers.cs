using BackendPM.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace BackendPM.Application.EventHandlers;

/// <summary>
/// 用户创建事件处理器
/// </summary>
public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger) : INotificationHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger = logger;

    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("用户创建事件处理: 用户 {Username} (ID: {Id}) 已创建",
            notification.User.Username, notification.User.Id);

        // 这里可以添加其他业务逻辑，如发送欢迎邮件等

        return Task.CompletedTask;
    }
}

/// <summary>
/// 用户信息更新事件处理器
/// </summary>
public class UserProfileUpdatedEventHandler(ILogger<UserProfileUpdatedEventHandler> logger) : INotificationHandler<UserProfileUpdatedEvent>
{
    private readonly ILogger<UserProfileUpdatedEventHandler> _logger = logger;

    public Task Handle(UserProfileUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("用户信息更新事件处理: 用户 {Username} (ID: {Id}) 的信息已更新，邮箱从 {OldEmail} 变更为 {NewEmail}",
            notification.User.Username, notification.User.Id, notification.OldEmail, notification.User.Email);

        // 这里可以添加其他业务逻辑，如发送确认邮件等

        return Task.CompletedTask;
    }
}

/// <summary>
/// 用户状态变更事件处理器
/// </summary>
public class UserStatusChangedEventHandler(ILogger<UserStatusChangedEventHandler> logger) : INotificationHandler<UserStatusChangedEvent>
{
    private readonly ILogger<UserStatusChangedEventHandler> _logger = logger;

    public Task Handle(UserStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        string statusChange = notification.NewStatus ? "激活" : "停用";
        _logger.LogInformation("用户状态变更事件处理: 用户 {Username} (ID: {Id}) 状态已{Status}",
            notification.User.Username, notification.User.Id, statusChange);

        // 这里可以添加其他业务逻辑，如发送通知等

        return Task.CompletedTask;
    }
}

/// <summary>
/// 用户密码修改事件处理器
/// </summary>
public class UserPasswordChangedEventHandler(ILogger<UserPasswordChangedEventHandler> logger) : INotificationHandler<UserPasswordChangedEvent>
{
    private readonly ILogger<UserPasswordChangedEventHandler> _logger = logger;

    public Task Handle(UserPasswordChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("用户密码修改事件处理: 用户 {Username} (ID: {Id}) 的密码已修改",
            notification.User.Username, notification.User.Id);

        // 这里可以添加其他业务逻辑，如发送安全提醒邮件等

        return Task.CompletedTask;
    }
}