# DeskSpine

[中文](README.md) | [English](README.en.md)

---

## Introduction

This is a desktop pet software for Windows based on [Spine](https://esotericsoftware.com/spine-in-depth) animations.

- Supports custom Spine models and placing them on the desktop for interaction, including dynamic illustrations.
- Supports loading multiple skeletal model files split from the same character.
- Supports multiple Spine runtime versions.
- Supports pre-multiplied alpha texture format.
- Supports custom character interaction voice lines.
- Supports dynamic wallpaper effects embedded on the desktop.
- Supports various pre-set animation interaction schemes for multiple gacha games.

In addition, some extra features are provided.

- Spine Animation Export
    - Supports exporting full animation frame sequences in PNG format with a maximum resolution of 8k and a frame rate of up to 120 FPS.
    - Allows free adjustment of model size and position.
- Performance Monitor Window
    - Left-click the tray icon to view system resource usage, such as the CPU.
    - Records real-time and historical resource usage changes.
- Hourly Chime Tool
    - Supports a custom bubble icon for hourly chimes.

## Installation

Please go to the Release page to download the latest `msi` installer.

After installation, you can find the program in the desktop or Start menu. Once launched, you should see a tray icon appear shortly, and, unless something goes wrong, a default animated character will appear on your desktop.

## Adjusting Window Size and Position

- Use the left mouse button to **drag the window**.
- Use the right mouse button to **drag the sprite itself**, without changing the window's position.
- Double-click the right mouse button to show the window resize frame, allowing quick adjustment of the display area size.
- The sprite's own scaling ratio needs to be adjusted in the settings panel.

## How to Use the Animation Export Tool

The SpineTool includes an animation export tool. You can launch it from the tray icon's right-click menu after the sprite has been started, or directly run `SpineTool.exe` (or use the shortcut) from the installation directory.

The model loading method is the same as for the desktop pet. The screen on the right is a preview, where you can use the left mouse button to drag the position, or the mouse wheel to zoom in/out and adjust the desired frame location.

After clicking the export button, the tool will export the animation frame by frame according to the set resolution, frame rate, and the position/zoom of the preview on the right.

## Background Issues with the Desktop Pet

Due to rendering limitations, the implementation of non-rectangular windows uses a transparent color key (`crKey`), and `SetLayeredWindowAttributes` is used to cut out all pixels of this color. Therefore, some semi-transparent areas cannot blend perfectly with the desktop background, and can only blend with the background color.

A common issue is that semi-transparent glowing objects in the image may appear with the background color, such as gray, which can affect the visual quality. As a compromise, you can remove the semi-transparent areas from the image, which can be done using the edge processing tool in the SpineTool.

Of course, this issue does not exist in the animation export tool, as it exports accurate PNG sequences with semi-transparent frames.

---

*If you think this project is cool, please give it a :star: and share it with more people! :)*
