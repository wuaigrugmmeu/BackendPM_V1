using BackendPM.Application.Commands.Users;
using FluentValidation;

namespace BackendPM.Application.Validators.Users;

/// <summary>
/// 创建用户命令验证器
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MinimumLength(3).WithMessage("用户名长度必须至少为3个字符")
            .MaximumLength(50).WithMessage("用户名长度不能超过50个字符");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("电子邮件不能为空")
            .EmailAddress().WithMessage("电子邮件格式不正确")
            .MaximumLength(100).WithMessage("电子邮件长度不能超过100个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度必须至少为6个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过100个字符")
            .Matches("[A-Z]").WithMessage("密码必须包含至少一个大写字母")
            .Matches("[a-z]").WithMessage("密码必须包含至少一个小写字母")
            .Matches("[0-9]").WithMessage("密码必须包含至少一个数字");

        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("全名长度不能超过100个字符");
    }
}