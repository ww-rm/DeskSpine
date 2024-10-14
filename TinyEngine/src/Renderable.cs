using Microsoft.Win32;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyEngine
{
    /// <summary>
    /// 可渲染对象
    /// </summary>
    public abstract class Renderable
    {
        /// <summary>
        /// 可渲染对象集合
        /// </summary>
        protected readonly Dictionary<string, Renderable> components = [];

        /// <summary>
        /// 渲染对象
        /// </summary>
        public virtual void Render(SFML.Graphics.RenderTarget target) 
        { foreach (var r in components.Values) r?.Render(target); }

        /// <summary>
        /// 状态更新
        /// </summary>
        public virtual void Update(float delta) 
        { foreach (var r in components.Values) r?.Update(delta); }

        /// <summary>
        /// 画面显示/隐藏
        /// </summary>
        public virtual void VisibleChange(bool visible)  
        { foreach (var r in components.Values) r?.VisibleChange(visible); }

        /// <summary>
        /// 睡眠/唤醒
        /// </summary>
        public virtual void SleepStateChange(bool sleep) 
        { foreach (var r in components.Values) r?.SleepStateChange(sleep); }

        /// <summary>
        /// 鼠标单击
        /// </summary>
        public virtual void Click(SFML.Window.Mouse.Button button) 
        { foreach (var r in components.Values) r?.Click(button); }

        /// <summary>
        /// 鼠标双击
        /// </summary>
        public virtual void DoubleClick(SFML.Window.Mouse.Button button) 
        { foreach (var r in components.Values) r?.DoubleClick(button); }

        /// <summary>
        /// 开始拖动
        /// </summary>
        public virtual void DragBegin(SFML.Window.Mouse.Button button) 
        { foreach (var r in components.Values) r?.DragBegin(button); }

        /// <summary>
        /// 拖动
        /// </summary>
        public virtual void Drag(SFML.Window.Mouse.Button button, SFML.System.Vector2f delta, SFML.System.Vector2f deltaFromSrc) 
        { foreach (var r in components.Values) r?.Drag(button, delta, deltaFromSrc); }

        /// <summary>
        /// 结束拖动
        /// </summary>
        public virtual void DragEnd(SFML.Window.Mouse.Button button) 
        { foreach (var r in components.Values) r?.DragEnd(button); }

        /// <summary>
        /// 滚轮滚动
        /// </summary>
        public virtual void Scroll(SFML.Window.Mouse.Wheel wheel, float delta) 
        { foreach (var r in components.Values) r?.Scroll(wheel, delta); }
    }
}
