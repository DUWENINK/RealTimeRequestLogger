# 🕵️‍♂️ RealTimeRequestLogger 🕵️‍♀️

> **别怀疑，它可能是你调试世界的救世主。**

### 什么是 RealTimeRequestLogger？

你是否曾经有过这种痛苦：你正在调试一个接口，服务器告诉你它收到了请求，但却神秘地没有响应，仿佛数据被困在了数字黑洞里？或者你尝试回调时，回调信息就像是一个莫名其妙的谜团？别怕，**RealTimeRequestLogger** 来拯救你！

这是一个简单的调试工具，你可以将它部署在服务器上。然后，当你在调试接口或回调时，它可以实时显示所有传入请求的相关信息，比如 `Host`、`QueryString` 和 `Body`。有了它，你将再也不需要对着屏幕发呆，猜测对方传来了什么。

### 特性 ✨

- **实时显示请求信息**：包括 `Host`、`QueryString`、`Body` 等等。再也不用猜测 "对面到底传了啥"。
- **轻松部署**：只需几分钟，你就能把它部署到任何你喜欢的服务器上，开始接收请求吧！
- **简便易用**：直接打开浏览器，即刻展示所有请求。你调试的秘密武器已上线！

### 使用场景

- 当你觉得调试接口像是在玩“猜猜我是谁”的游戏时。
- 当你需要查看回调参数时，而文档没有明确告诉你它传了什么。
- 或者……你就是单纯喜欢看别人发来的数据。🤷‍♂️

### 一键运行，Docker Style 🐳

谁说部署调试工具需要复杂的操作？用 Docker 来简化你的生活吧！你只需要运行一个命令，就能轻松启动这个调试小助手。

```bash
docker run -d -p 8080:8080 duwenink/realtimerequestlogger
```

就这么简单！打完这个命令，RealTimeRequestLogger 就已经在你的服务器上跑起来了，你可以通过访问 `http://<your-server-ip>:8080` 实时查看所有请求。

### 配置和自定义

不需要什么复杂的配置。你只需要：
1. **启动 Docker 容器**：如上所示。
2. **访问界面**：使用浏览器访问你的 IP 地址加上端口号（例如 `http://localhost:8080`）。
3. **开始调试**：现在你可以实时查看所有请求信息。

### 贡献指南

嘿，如果你对这个项目有想法、有改进，别害羞！欢迎 PR，也欢迎提出 Issue，让这个项目变得更有趣、更实用。😄

### 最后的话

还在等什么？不再需要对着接口的返回值发呆了，RealTimeRequestLogger 帮你看穿一切！去吧，调试大师，世界将会感谢你的选择！👨‍💻👩‍💻
