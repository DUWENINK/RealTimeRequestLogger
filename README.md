# 🕵️‍♂️ RealTimeRequestLogger 🕵️‍♀️

> **别怀疑，它可能是你调试世界的救世主。**

### 什么是 RealTimeRequestLogger？

你是否曾经有过这种痛苦：你正在调试一个接口，服务器告诉你它收到了请求，但却神秘地没有响应，仿佛数据被困在了数字黑洞里？或者你尝试回调时，回调信息就像是一个莫名其妙的谜团？别怕，**RealTimeRequestLogger** 来拯救你！

这是一个简单的调试工具，你可以将它部署在服务器上。然后，当你在调试接口或回调时，它可以实时显示所有传入请求的相关信息，比如 `Host`、`QueryString` 和 `Body`。有了它，你将再也不需要对着屏幕发呆，猜测对方传来了什么。

### 特性 ✨

- **实时显示请求信息**：包括 `Host`、`QueryString`、`Body` 等等。再也不用猜测 "对面到底传了啥"。
- **轻松部署**：只需几分钟，你就能把它部署到任何你喜欢的服务器上，开始接收请求吧！
- **简便易用**：直接打开浏览器，即刻展示所有请求。你调试的秘密武器已上线！
- **实时更新** 🔄：基于 SignalR 技术，请求信息实时推送，无需刷新页面
- **优雅界面** 💅：采用现代化的界面设计，清晰展示每个请求的细节
- **跨平台支持** 💻：支持 Windows、Linux、macOS，到处都能用！
- **自定义响应** 🎯：支持通过特殊参数自定义响应内容，模拟各种场景
- **请求历史记录** 📜：保存最近的请求记录，方便回溯查看
- **请求详情展示** 🔍：支持展开/折叠请求详情，更清晰的信息展示
- **自动格式化** ✨：自动格式化 JSON 数据，提升可读性
- **响应时间统计** ⏱️：显示请求处理时间，帮助性能分析

### 使用场景 🎯

- 当你觉得调试接口像是在玩"猜猜我是谁"的游戏时。
- 当你需要查看回调参数时，而文档没有明确告诉你它传了什么。
- 当你在开发第三方平台集成时需要验证 Webhook 请求。
- 当你需要快速验证客户端发送的请求格式是否正确。
- 当你需要模拟特定的响应场景进行测试。
- 或者……你就是单纯喜欢看别人发来的数据。🤷‍♂️

### 一键运行，Docker Style 🐳

谁说部署调试工具需要复杂的操作？用 Docker 来简化你的生活吧！你只需要运行一个命令，就能轻松启动这个调试小助手。

```bash
docker run -d -p 8080:8080 duwenink/realtimerequestlogger
```

就这么简单！打完这个命令，RealTimeRequestLogger 就已经在你的服务器上跑起来了，你可以通过访问 `http://<your-server-ip>:8080` 实时查看所有请求。

### 自定义响应功能 🎭

想要模拟特定的响应场景？我们提供了强大的自定义响应功能！

#### 使用方法

在请求中添加特殊参数来控制响应内容：

```
# 示例 1：返回自定义 JSON
GET http://your-server:8080/api/test?__response={"code":200,"message":"success"}

# 示例 2：返回自定义状态码
GET http://your-server:8080/api/test?__status=404

# 示例 3：同时自定义状态码和响应
GET http://your-server:8080/api/test?__status=201&__response={"created":true}
```

#### 特殊参数说明

- `__response`: 自定义响应体内容
- `__status`: 自定义 HTTP 状态码
- `__delay`: 自定义响应延迟（毫秒）
- `__contentType`: 自定义响应内容类型
- `__headers`: 自定义响应头

这些参数让你能够：
- 模拟各种 API 响应场景
- 测试错误处理逻辑
- 验证超时处理机制
- 测试不同内容类型的处理
- 验证自定义头部的处理

### 本地开发运行 🏃‍♂️

如果你想在本地运行或开发，也超级简单：

```bash
# 克隆仓库
git clone https://github.com/DUWENINK/RealTimeRequestLogger.git

# 进入目录
cd RealTimeRequestLogger

# 运行项目
dotnet run
```

### 配置和自定义 ⚙️

不需要什么复杂的配置。你只需要：
1. **启动 Docker 容器**：如上所示。
2. **访问界面**：使用浏览器访问你的 IP 地址加上端口号（例如 `http://localhost:8080`）。
3. **开始调试**：现在你可以实时查看所有请求信息。
4. **自定义响应**：需要特定响应时，使用特殊参数进行配置。

### 技术栈 🛠️

- **后端**：ASP.NET Core
- **实时通信**：SignalR
- **前端样式**：Tailwind CSS
- **容器化**：Docker

### 安全提示 🔒

请注意：
- 建议仅在开发环境中使用
- 不要在生产环境暴露此服务
- 如果必须在公网使用，请做好访问控制
- 注意自定义响应功能可能带来的安全风险
- 定期清理请求历史记录以防止内存占用过大

### 贡献指南 🤝

嘿，如果你对这个项目有想法、有改进，别害羞！欢迎 PR，也欢迎提出 Issue，让这个项目变得更有趣、更实用。😄

### 更新日志 📝

#### v1.2.0
- 🎨 优化界面设计和用户体验
- 📊 新增请求历史记录功能
- 🔍 支持请求详情展开/折叠
- ⚡ 优化性能，提升响应速度
- 🛠️ 新增更多自定义响应选项

#### v1.1.0
- ✨ 新增自定义响应功能
- 🎯 支持状态码自定义
- ⏱️ 支持响应延迟设置
- 📝 完善文档说明

#### v1.0.0
- 🎉 首次发布
- ✨ 实时请求监控
- 🔄 SignalR 实时推送
- 🐳 Docker 支持

### 最后的话

还在等什么？不再需要对着接口的返回值发呆了，RealTimeRequestLogger 帮你看穿一切！去吧，调试大师，世界将会感谢你的选择！👨‍💻👩‍💻

### 许可证 📄

MIT License - 随便用，开心就好！
