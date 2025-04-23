using BackendPM.Application.Abstractions;
using BackendPM.Infrastructure.Configuration;
using BackendPM.Infrastructure.DataSeeding;
using BackendPM.Infrastructure.Persistence.DbContexts;
using BackendPM.Presentation.Authorization;
using BackendPM.Presentation.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 添加控制器支持，并配置JSON序列化选项
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 循环引用处理
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // 枚举为字符串
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 添加Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "后台权限管理系统 API",
        Version = "v1",
        Description = "基于DDD架构的.NET Core 8后台权限管理系统API"
    });
    
    // 添加JWT认证支持
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 注册应用程序层服务
builder.Services.AddApplicationServices();

// 注册基础设施层服务
builder.Services.AddInfrastructureServices(builder.Configuration);

// 添加基于权限的授权 - 使用完全限定的命名空间和类型名称来解决方法歧义
BackendPM.Presentation.Authorization.AuthorizationExtensions.AddPermissionBasedAuthorization(builder.Services);

// CORS配置
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    // 初始化数据库和种子数据
    await AppDbInitializer.InitializeDatabaseAsync(app.Services, isProduction: false);

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "后台权限管理系统 API V1");
        
        // 设置Swagger UI为根路径，使应用启动时直接打开Swagger页面
        options.RoutePrefix = string.Empty;
        
        // 自动展开操作和标签
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        
        // 启用筛选
        options.EnableFilter();
        
        // 启用深度链接
        options.EnableDeepLinking();
        
        // 显示请求持续时间
        options.DisplayRequestDuration();
    });
}
else
{
    // 生产环境也需要确保数据库已创建并迁移
    await AppDbInitializer.InitializeDatabaseAsync(app.Services, isProduction: true);
    
    // 生产环境默认使用ASP.NET Core的内置异常处理页面
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 注册全局异常处理中间件（放在管道前面以捕获所有异常）
app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();

// 启用CORS
app.UseCors();

// 启用身份验证
app.UseAuthentication();

// 启用授权
app.UseAuthorization();

// 将根路径重定向到Swagger页面（适用于所有环境）
app.MapGet("/", () => Results.Redirect("/index.html"));

// 映射控制器路由
app.MapControllers();

app.Run();
