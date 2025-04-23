using BackendPM.Application.Commands.Users;
using FluentValidation;

namespace BackendPM.Application.Validators.Users;

/// <summary>
/// 更新用户命令验证器
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("电子邮件格式不正确")
            .MaximumLength(100).WithMessage("电子邮件长度不能超过100个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("全名长度不能超过100个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));
    }
}