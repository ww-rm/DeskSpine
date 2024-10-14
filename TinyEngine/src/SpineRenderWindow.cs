using Microsoft.Win32;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace TinyEngine
{
    /// <summary>
    /// 显示窗口
    /// </summary>
    public class SpineRenderWindow : LayeredRenderWindow
    {
        private SpineSlot spineSlots;
        private Speaker speaker = new();

        /// <summary>
        /// SpineRenderWindow 基类, 提供 Spine 更新和渲染
        /// </summary>
        public SpineRenderWindow(int slotCount)
        {
            spineSlots = new SpineSlot(slotCount);
            scene.Renderables["spineSlots"] = spineSlots;
            scene.Renderables["speaker"] = speaker;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            scene.Renderables.Clear();
            spineSlots.Dispose();
            //speaker.Dispose();
        }

        /// <summary>
        /// 操作 Spine 对象属性
        /// </summary>
        public SpineSlot Spine { get => spineSlots; }

        /// <summary>
        /// 操作语音属性
        /// </summary>
        public Speaker Speaker { get => speaker; }

        /// <summary>
        /// 重置窗口和 Spine 的位置和大小
        /// </summary>
        public void ResetPositionAndSize()
        {
            spineSlots.Position = new(0, 0);
            Position = new(0, 0);
            Size = new(1000, 1000);
        }
    }
}

