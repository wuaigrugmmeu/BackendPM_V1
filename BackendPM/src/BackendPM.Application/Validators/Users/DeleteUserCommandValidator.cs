using BackendPM.Application.Commands.Users;
using FluentValidation;

namespace BackendPM.Application.Validators.Users;

/// <summary>
/// 删除用户命令验证器
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");
    }
}