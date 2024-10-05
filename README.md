# DeskSpine

[中文](README.md) | [English](README.en.md)

---

## 简介

这是一个适用于 Windows 系统下, 基于 [Spine](https://esotericsoftware.com/spine-in-depth) 动画的桌宠软件.

- 支持自定义 Spine 模型并放置在桌面进行交互, 包括动态立绘
- 支持同时加载同一角色拆分的多个骨骼模型文件~~说的就是某B, 1 个船 7 个模型~~
- 支持多个 Spine 运行时版本
- 支持预乘 Alpha 纹理格式
- 支持自定义角色交互语音~~终于能说话了~~
- 支持嵌入桌面的动态壁纸效果
- 支持多个二游的预设动画交互方案

除此之外, 还额外提供一些小功能.

- Spine 动画导出
    - 支持导出最高分辨率 8k, 最大帧率 120 帧, PNG 格式的完整动画帧序列
    - 支持自由调节模型大小与空间位置
- 性能监视窗
    - 通知栏常驻图标左键单击可以查看处理器等系统占用情况~~炫酷的 RGB 显示~~
    - 记录实时及一段时间内的占用变化~~再也不用点任务管理器了~~
- 报时小工具
    - 支持自定义气泡图标的整点报时小工具~~准时准点提醒你该摸鱼了~~

## 安装

请前往 Release 页面下载最新的 `msi` 安装包.

安装完成后桌面以及开始菜单可以找到程序启动方式, 启动后过一会可以看到通知栏区域有常驻图标, 不出意外桌面也会显示默认的动画小人.

## 窗口大小及位置调节方法

- 使用鼠标左键**拖动窗口**
- 使用鼠标右键**拖动精灵自身**, 窗口位置不发生变化
- 双击右键显示窗口缩放框, 可以快捷调整窗口显示区域大小
- 精灵自身缩放比例需要在设置面板进行调节

## 动画导出工具使用方法

SpineTool 内包含动画导出工具, 可以在启动了精灵的情况下直接通知栏区域图标右键菜单里启动, 也可以直接在安装目录启动 `SpineTool.exe` (或者快捷方式).

加载模型的方式与桌宠相同. 右侧的画面是预览画面, 可以使用鼠标左键进行位置拖动, 或者滚轮进行画面缩放来调节需要的画面位置.

点击导出按钮之后, 会按照设定的分辨率和帧率, 以及右侧预览画面的位置与缩放对动画进行逐帧导出.

## 桌宠画面背景问题

受限于渲染机制, 异形窗口的实现原理是设定一个透明颜色键 `crKey`, 然后用 `SetLayeredWindowAttributes` 抠掉画面中所有是这个颜色的像素点. 因此部分半透明区域是无法做到和桌面背景完美混合的, 只能与背景色混合.

常见的问题就是画面中的半透明发光体会呈现背景色, 例如灰色, 比较影响观感, 因此折中的办法是去除画面中的半透明区域, 这可以通过 SpineTool 中的边缘处理工具来完成.

当然, 动画导出工具中不存在这个问题, 会导出准确的 PNG 格式半透明帧序列.

---

*如果你觉得这个项目不错请给个 :star:, 并分享给更多人知道! :)*
