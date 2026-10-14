# CC-Launcher

Claude Code 启动器 —— 一个轻量的 Windows 桌面启动器（WPF / .NET Framework 4.8），用来管理工作区并在 Windows Terminal 中启动 Claude Code。

## 功能

- **工作区历史**：记录最近打开过的 10 个工作区，双击即可再次启动
- **选择新工作区**：通过文件夹选择框打开任意目录
- **删除选中**：从历史列表中移除条目
- **背景图**：可在设置中指定背景图片并支持清除
- **Windows Terminal 启动**：在 `wt.exe` 中运行 `claude`（老版 conhost 无法正确渲染 Claude Code 的界面）

数据保存在 `%LocalAppData%\ClaudeCode\`：

| 文件 | 内容 |
| --- | --- |
| `workspace_history.txt` | 工作区历史 |
| `settings.txt` | 背景图路径 |

## 编译

需要 .NET Framework 4.8 目标包（`Microsoft.NETFramework.ReferenceAssemblies`，已通过 NuGet 自动还原）：

```sh
msbuild src/ClaudeCode启动器.csproj -c Release
```

## 运行

双击 `ClaudeCode启动器.exe`。启动 Claude Code 需要系统已安装 `claude` 命令，并建议安装 Windows Terminal。
