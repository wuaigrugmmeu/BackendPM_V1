# BackendPM - 后台权限管理系统

## 项目简介

BackendPM是一个基于.NET 8开发的后台权限管理系统，采用领域驱动设计(DDD)架构。该系统提供了完善的用户、角色和权限管理功能，适用于需要精细化权限控制的各类管理系统。

## 技术栈

- **开发平台**: .NET 8
- **架构模式**: 领域驱动设计 (DDD)
- **数据持久化**: Entity Framework Core 9.0 + SQLite
- **认证方式**: JWT (JSON Web Token)
- **API文档**: Swagger / OpenAPI
- **依赖注入**: 内置 .NET 依赖注入容器
- **消息中介**: MediatR (CQRS模式)
- **数据验证**: FluentValidation
- **对象映射**: AutoMapper
- **跨域支持**: CORS

## 系统架构

项目采用典型的DDD四层架构，各层职责清晰分离：

### 领域层 (BackendPM.Domain)

领域层是整个系统的核心，包含业务实体、领域事件、聚合根等核心业务逻辑。

- **实体(Entities)**: 包含业务对象如用户(User)、角色(Role)、权限(Permission)等
- **值对象(Value Objects)**: 不具有唯一标识的对象
- **领域事件(Domain Events)**: 描述系统中发生的重要业务事件
- **领域服务(Domain Services)**: 处理跨实体的业务规则
- **接口定义(Interfaces)**: 定义仓储和基础设施服务的抽象

### 应用层 (BackendPM.Application)

应用层负责协调领域对象完成用户请求，实现用例和业务流程。

- **命令与查询(Commands/Queries)**: 遵循CQRS模式
- **数据传输对象(DTOs)**: 数据传输格式定义
- **验证器(Validators)**: 输入验证
- **事件处理器(Event Handlers)**: 处理领域事件
- **应用服务(Services)**: 实现应用层逻辑和用例

### 基础设施层 (BackendPM.Infrastructure)

基础设施层提供技术实现和对外部资源的访问。

- **持久化(Persistence)**: 数据库访问、仓储实现
- **数据迁移(Migrations)**: EF Core数据迁移
- **服务实现(Services)**: 实现领域层定义的接口
- **配置(Configuration)**: 系统配置
- **数据填充(Data Seeding)**: 初始数据生成

### 表示层 (BackendPM.Presentation)

表示层处理用户交互和API接口。

- **API控制器(Controllers)**: REST API接口
- **身份认证(Authentication)**: JWT认证实现
- **授权(Authorization)**: 基于权限的授权
- **中间件(Middleware)**: HTTP请求处理管道
- **模型(Models)**: 视图模型和API请求/响应模型

## 核心功能模块

### 用户管理

- 用户注册、登录、注销
- 个人信息管理
- 密码修改
- 用户状态管理（激活/禁用）

### 角色管理

- 角色创建、编辑、删除
- 角色权限分配
- 系统角色保护

### 权限管理

- 权限创建、编辑、删除
- 权限分组管理
- 权限编码唯一性保证

### 安全认证

- JWT令牌认证
- 刷新令牌机制
- 密码加密存储

## 数据模型关系

系统包含以下核心实体及其关系：

- **用户(User)**: 系统的操作人员，可以拥有多个角色
- **角色(Role)**: 权限的集合，定义用户在系统中可执行的操作
- **权限(Permission)**: 系统中最小的操作单元，如"查看用户"、"编辑角色"等
- **用户角色(UserRole)**: 用户与角色的多对多关系
- **角色权限(RolePermission)**: 角色与权限的多对多关系
- **刷新令牌(RefreshToken)**: 用于更新JWT访问令牌

## 安装和启动指南

### 环境要求

- .NET 8 SDK 或更高版本
- 支持SQLite的开发环境

### 克隆项目

```bash
git clone https://github.com/your-username/BackendPM.git
cd BackendPM
```

### 构建项目

```bash
dotnet restore
dotnet build
```

### 运行数据迁移

```bash
cd src/BackendPM.Presentation
dotnet ef database update
```

### 启动项目

```bash
dotnet run
```

项目启动后，访问 https://localhost:5001 即可在浏览器中看到Swagger API文档页面。

## 接口说明

系统提供RESTful API接口，遵循REST原则。所有接口都需要在HTTP请求头中携带JWT令牌，除了登录和注册接口。

完整API文档请参考启动后的Swagger页面。

## 开发指南

### 添加新功能

1. 在Domain层定义实体和业务规则
2. 在Application层添加命令/查询和处理器
3. 在Infrastructure层添加仓储实现和技术服务
4. 在Presentation层添加API控制器端点

### 添加新权限

在`AppDbInitializer.cs`的`SeedDataAsync`方法中添加新的权限定义：

```csharp
new Permission("权限名称", "权限编码", "权限分组", "权限描述")
```

## 测试策略

项目遵循全面的测试策略，包括：

- **单元测试**: 测试独立组件
- **集成测试**: 测试组件交互
- **端到端测试**: 测试完整业务流程

## 贡献指南

欢迎贡献代码和提出问题！请遵循以下步骤：

1. Fork项目仓库
2. 创建功能分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建Pull Request

## 许可证

[MIT License](LICENSE)