<p align="center">
  <img src="./src/VideoDownloader.UI/Assets/Logo.png" width="200">
</p>
<h2 align="center">摘星辰</h2>
<p align="center">基于 yt-dlp 的跨平台视频下载工具</p>

## 简介

摘星辰是一个基于 [yt-dlp](https://github.com/yt-dlp/yt-dlp) 的视频下载工具，支持 yt-dlp 所支持的绝大多数视频网站。

## 功能特性

- **多平台支持**：桌面端（Windows）和 Android 双平台
- **批量下载**：支持同时解析和下载多个视频链接
- **画质选择**：自动获取视频可用画质，支持批量统一分辨率下载
- **格式分离**：视频和音频格式分开显示，灵活选择
- **下载进度**：实时显示下载进度
- **主题切换**：支持浅色/深色/跟随系统三种主题模式
- **代理设置**：支持配置代理服务器
- **自定义下载路径**：可设置默认下载文件夹

## 技术栈

- **UI 框架**：Avalonia UI 12.0
- **UI 主题**：Semi.Avalonia
- **MVVM 框架**：CommunityToolkit.Mvvm
- **目标框架**：.NET 10
- **核心依赖**：yt-dlp、ffmpeg

## 项目结构

```
src/
├── VideoDownloader.Core/          # 核心库
│   ├── Models/                    # 数据模型（VideoInfo、AppSettings、Enums）
│   ├── Services/                  # 服务接口（平台服务、设置服务、通知服务等）
│   └── Utils/                     # 工具类（YtDlp 封装）
├── VideoDownloader.UI/            # UI 层（Avalonia）
│   ├── ViewModels/                # 视图模型
│   ├── Views/                     # 视图页面
│   ├── Controls/                  # 自定义控件
│   └── Converters/                # 值转换器
├── VideoDownloader.Desktop/       # 桌面端（Windows）
│   └── Services/                  # 桌面端服务实现
└── VideoDownloader.Android/       # Android 端
    └── Services/                  # Android 端服务实现
```

## 使用说明

1. 将视频链接粘贴到输入框（支持多行输入多个链接）
2. 点击"解析"按钮获取视频信息
3. 选择要下载的视频和画质
4. 点击"下载选中项"开始下载

## 设置选项

- **下载路径**：设置视频默认保存位置
- **代理地址**：配置 HTTP/HTTPS 代理
- **主题模式**：切换浅色/深色主题

## 相关项目

- yt-dlp: https://github.com/yt-dlp/yt-dlp
- Avalonia UI: https://github.com/AvaloniaUI/Avalonia
- Semi.Avalonia: https://github.com/irihitech/Semi.Avalonia
