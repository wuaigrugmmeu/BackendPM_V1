# BackendPM - 后台权限管理系统

## 项目简介

BackendPM是一个基于.NET 8开发的后台权限管理系统，采用领域驱动设计(DDD)架构。该系统提供了完善的用户、角色和权限管理功能，适用于需要精细化权限控制的各类管理系统。系统支持API资源的路径级权限控制，可以精确管理用户对不同接口的访问权限。

### 关键特性

- **基于角色的访问控制(RBAC)**：用户-角色-权限三层设计
- **精细化API权限管理**：支持HTTP方法和路径级别的权限控制
- **安全认证机制**：采用JWT + 刷新令牌双重安全机制
- **领域驱动设计**：严格的DDD分层架构，保证业务逻辑的一致性和可维护性
- **命令查询职责分离(CQRS)**：优化读写操作，提高系统性能
- **完善的接口文档**：集成Swagger/OpenAPI接口文档

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
- **日志系统**: 已集成结构化日志（可通过配置输出到不同目标）

## 系统架构

项目采用典型的DDD四层架构，各层职责清晰分离：

### 领域层 (BackendPM.Domain)

领域层是整个系统的核心，包含业务实体、领域事件、聚合根等核心业务逻辑。

- **实体(Entities)**:
  - 用户(User)：系统用户实体，包含用户基本信息和角色关联
  - 角色(Role)：角色实体，包含权限集合
  - 权限(Permission)：细粒度权限实体，包含资源类型、路径、HTTP方法等信息
  - 刷新令牌(RefreshToken)：用于JWT令牌刷新的安全机制
- **值对象(Value Objects)**: 不具有唯一标识的对象
- **领域事件(Domain Events)**: 描述系统中发生的重要业务事件，如用户创建、角色变更等
- **领域服务(Domain Services)**: 处理跨实体的业务规则
- **接口定义(Interfaces)**: 定义仓储和基础设施服务的抽象

### 应用层 (BackendPM.Application)

应用层负责协调领域对象完成用户请求，实现用例和业务流程。

- **命令与查询(Commands/Queries)**: 遵循CQRS模式，分离读写操作
  - 命令：修改系统状态的操作（如创建用户、分配角色）
  - 查询：不修改状态的数据检索操作（如获取用户列表）
- **数据传输对象(DTOs)**: 数据传输格式定义，用于API接口的请求和响应
- **验证器(Validators)**: 使用FluentValidation实现的输入验证
- **事件处理器(Event Handlers)**: 处理领域事件的组件
- **应用服务(Services)**: 实现应用层逻辑和用例，如认证服务

### 基础设施层 (BackendPM.Infrastructure)

基础设施层提供技术实现和对外部资源的访问。

- **持久化(Persistence)**: 
  - 数据库上下文(AppDbContext)
  - 仓储实现(Repositories)
  - 实体配置(Configurations)
- **数据迁移(Migrations)**: EF Core数据迁移文件
- **服务实现(Services)**: 实现领域层定义的接口
- **配置(Configuration)**: 系统配置和依赖注入设置
- **数据填充(Data Seeding)**: 初始数据生成，包括基础权限、角色和管理员用户

### 表示层 (BackendPM.Presentation)

表示层处理用户交互和API接口。

- **API控制器(Controllers)**: RESTful API接口实现
- **身份认证(Authentication)**: JWT认证机制实现
- **授权(Authorization)**: 基于权限的动态授权系统
- **中间件(Middleware)**: HTTP请求处理管道，如异常处理、权限验证
- **模型(Models)**: 视图模型和API请求/响应模型
- **过滤器(Filters)**: API控制器过滤器，用于横切关注点

## 核心功能模块

### 用户管理

- **用户注册与登录**：支持用户注册和JWT令牌认证登录
- **个人信息管理**：用户可查看和修改个人信息
- **密码管理**：安全的密码重置和修改功能（使用SHA-256加密）
- **用户状态管理**：管理员可激活或禁用用户账户
- **角色分配**：为用户分配不同角色，实现权限控制

### 角色管理

- **角色CRUD操作**：创建、读取、更新和删除角色
- **角色权限分配**：为角色分配细粒度权限
- **系统角色保护**：内置角色（如管理员）受到特殊保护，防止误删除
- **角色用户管理**：查看和管理属于特定角色的用户列表

### 权限管理

- **细粒度权限控制**：支持API路径和HTTP方法级别的权限定义
- **权限分组**：按功能模块组织权限，便于管理
- **资源类型区分**：区分API、菜单、按钮等不同类型的权限资源
- **权限编码唯一性**：确保权限编码的唯一性，避免冲突
- **系统权限保护**：内置权限不可删除，确保系统基础功能正常运行

### 安全认证

- **JWT令牌认证**：基于JSON Web Token的用户认证机制
- **刷新令牌机制**：安全的令牌刷新流程，延长用户会话
- **密码安全存储**：使用SHA-256算法哈希处理密码
- **令牌过期控制**：灵活配置令牌有效期
- **认证失败处理**：友好的错误提示和安全的失败处理机制

## 数据模型关系

系统包含以下核心实体及其关系：

- **用户(User)**: 
  - 主要属性：用户名、邮箱、密码哈希、是否激活等
  - 关系：一个用户可以拥有多个角色（多对多关系）

- **角色(Role)**: 
  - 主要属性：名称、编码、描述、是否系统角色等
  - 关系：一个角色可以包含多个权限（多对多关系），也可以分配给多个用户（多对多关系）

- **权限(Permission)**: 
  - 主要属性：名称、编码、分组、资源类型、资源路径、HTTP方法等
  - 关系：一个权限可以分配给多个角色（多对多关系）

- **用户角色(UserRole)**: 
  - 连接表：实现用户与角色的多对多关系
  - 包含创建时间，便于审计跟踪

- **角色权限(RolePermission)**: 
  - 连接表：实现角色与权限的多对多关系
  - 包含创建时间，便于审计跟踪

- **刷新令牌(RefreshToken)**: 
  - 主要属性：令牌值、过期时间、是否使用、是否撤销等
  - 关系：属于特定用户（多对一关系）

## 安装和启动指南

### 环境要求

- .NET 8 SDK 或更高版本
- 支持SQLite的开发环境（默认数据库）
- 可选：Visual Studio 2022或其他支持.NET 8的IDE

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

项目启动时会自动应用迁移，但也可以手动执行：

```bash
cd src/BackendPM.Presentation
dotnet ef database update
```

如果需要添加新的迁移（在修改实体模型后）：

```bash
dotnet ef migrations add MigrationName
```

### 启动项目

```bash
cd src/BackendPM.Presentation
dotnet run
```

项目启动后，访问 https://localhost:5001 即可在浏览器中看到Swagger API文档页面。

### 默认账户

系统初始化时会创建以下默认账户：

- 管理员账户
  - 用户名：admin
  - 密码：Admin123!
  - 角色：管理员（拥有所有权限）

- 普通用户账户
  - 用户名：user
  - 密码：User123!
  - 角色：普通用户（拥有基本查看权限）

## 接口说明

系统提供RESTful API接口，遵循REST原则。所有接口都需要在HTTP请求头中携带JWT令牌，除了登录和注册接口。

### API认证流程

1. 调用`/api/auth/login`接口获取访问令牌和刷新令牌
2. 在后续请求中，在Header中添加：`Authorization: Bearer {accessToken}`
3. 当访问令牌过期时，使用`/api/auth/refresh`接口和刷新令牌获取新的访问令牌

### 主要API分组

- **/api/auth**：身份认证相关API（登录、注册、刷新令牌等）
- **/api/users**：用户管理相关API
- **/api/roles**：角色管理相关API
- **/api/permissions**：权限管理相关API

完整API文档请参考启动后的Swagger页面（访问项目根路径）。

## 开发指南

### 添加新功能的步骤

1. **领域层**：
   - 在Domain层定义实体、值对象和领域事件
   - 确保实体包含适当的业务规则验证

2. **应用层**：
   - 添加相应的DTO模型
   - 创建命令/查询类及其处理器
   - 实现必要的验证器

3. **基础设施层**：
   - 在AppDbContext中添加DbSet（如果新增了实体）
   - 添加EntityTypeConfiguration配置类
   - 添加迁移以更新数据库模式

4. **表示层**：
   - 创建API控制器和端点
   - 实现必要的授权策略
   - 更新Swagger文档注释

### 添加新权限

在`AppDbInitializer.cs`的`SeedDataAsync`方法中添加新的权限定义：

```csharp
new Permission(
    name: "权限名称", 
    code: "权限编码", 
    group: "权限分组",
    description: "权限描述",
    resourceType: "API", // 可选：API, Menu, Button, Other
    resourcePath: "api/controller/action", // API路径，可使用通配符
    httpMethod: "GET" // HTTP方法：GET, POST, PUT, DELETE等
)
```

### 最佳实践

1. **命名约定**：
   - 实体使用单数名词（如User, Role）
   - 控制器使用复数名词（如UsersController）
   - 命令/查询使用动词+名词（如CreateUserCommand）

2. **安全性考虑**：
   - 总是验证用户输入
   - 使用参数化查询防止SQL注入
   - 避免在响应中包含敏感信息

3. **性能优化**：
   - 合理使用异步方法
   - 适当缓存频繁访问的数据
   - 优化数据库查询，避免N+1问题

## 测试策略

项目遵循全面的测试策略，包括：

- **单元测试**: 
  - 测试独立组件，如实体、命令处理器等
  - 使用模拟框架隔离外部依赖

- **集成测试**: 
  - 测试组件交互，如API控制器与数据库
  - 使用内存数据库或测试容器

- **端到端测试**: 
  - 测试完整业务流程
  - 验证系统在真实环境中的行为

## 持续集成/持续部署

项目支持CI/CD流程：

1. **持续集成**：
   - 运行单元测试和集成测试
   - 代码质量分析
   - 构建Docker镜像

2. **持续部署**：
   - 开发环境自动部署
   - 生产环境手动确认部署
   - 数据库迁移自动化

## 贡献指南

欢迎贡献代码和提出问题！请遵循以下步骤：

1. Fork项目仓库
2. 创建功能分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建Pull Request

### 代码规范

- 遵循C#编码规范
- 使用XML注释文档化公共API
- 编写单元测试覆盖新功能
- 确保所有测试通过

## 版本历史

- **1.0.0** - 初始版本
  - 基础用户、角色、权限管理功能
  - JWT认证系统
  - RESTful API接口

## 许可证

[MIT License](LICENSE)

## 联系与支持

如有问题或建议，请通过以下方式联系：

- 提交Issue
- 发送邮件至：your-email@example.com