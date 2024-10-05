using Microsoft.Win32;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpineWindow
{
    /// <summary>
    /// 窗体类型枚举量
    /// </summary>
    public enum SpineWindowType
    {
        AzurLaneSD = 0,                 // 碧蓝航线_后宅小人
        AzurLaneDynamic = 1,            // 碧蓝航线_动态立绘
        ArknightsDynamic = 2,           // 明日方舟_动态立绘
        ArknightsBuild = 3,             // 明日方舟_基建小人
        ArknightsBattle = 4,            // 明日方舟_战斗小人
    }

    /// <summary>
    /// 自动背景颜色类型
    /// </summary>
    public enum AutoBackgroudColorType
    {
        None = 0,
        Black = 1,
        White = 2,
        Gray = 3,
    }

    /// <summary>
    /// SpineWindow 抽象基类, 提供对 Spine 对象的基本操作, 完成逻辑更新和渲染
    /// </summary>
    public abstract class SpineRenderWindow : LayeredRenderWindow
    {
        /// <summary>
        /// 创建指定类型 Spine 窗口
        /// </summary>
        public static SpineRenderWindow New(SpineWindowType type, uint slotCount)
        {
            return type switch
            {
                SpineWindowType.AzurLaneSD => new AzurLaneSD(slotCount),
                SpineWindowType.AzurLaneDynamic => new AzurLaneDynamic(slotCount),
                SpineWindowType.ArknightsDynamic => new ArknightsDynamic(slotCount),
                SpineWindowType.ArknightsBuild => new ArknightsBuild(slotCount),
                SpineWindowType.ArknightsBattle => new ArknightsBattle(slotCount),
                _ => throw new NotImplementedException($"Unknown SpineWindow type: {type}"),
            };
        }

        /// <summary>
        /// 窗口类型
        /// </summary>
        public SpineWindowType Type
        {
            get
            {
                var t = GetType();
                if (t == typeof(AzurLaneSD)) return SpineWindowType.AzurLaneSD;
                if (t == typeof(AzurLaneDynamic)) return SpineWindowType.AzurLaneDynamic;
                if (t == typeof(ArknightsDynamic)) return SpineWindowType.ArknightsDynamic;
                if (t == typeof(ArknightsBuild)) return SpineWindowType.ArknightsBuild;
                if (t == typeof(ArknightsBattle)) return SpineWindowType.ArknightsBattle;
                throw new InvalidOperationException($"Unknown SpineWindow type {this}");
            }
        }

        protected Mutex mutex = new();
        protected Spine.Spine?[] spineSlots;                        // Spine 对象装载数组
        public int SlotCount { get => spineSlots.Length; }          // 窗口可用最大 Spine 装载数
        private Dictionary<uint, uint>?[] colorTables;              // 背景颜色表, 供自动生成背景颜色时使用

        /// <summary>
        /// SpineRenderWindow 基类, 提供 Spine 更新和渲染
        /// </summary>
        public SpineRenderWindow(uint slotCount)
        {
            spineSlots = new Spine.Spine[slotCount];
            colorTables = new Dictionary<uint, uint>[slotCount];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                mutex.Dispose();
            }
        }

        protected sealed override void UpdateFrame(float delta)
        {
            mutex.WaitOne();
            foreach (var sp in spineSlots) sp?.Update(delta);
            mutex.ReleaseMutex();
        }

        protected sealed override void RenderFrame(SFML.Graphics.RenderTarget target)
        {
            mutex.WaitOne();
            for (int i = spineSlots.Length - 1; i >= 0; i--) { var sp = spineSlots[i]; if (sp is not null) target.Draw(sp); }
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Spine 在注册表中存储的位置
        /// </summary>
        private static SFML.System.Vector2f SpinePositionReg
        {
            get
            {
                SFML.System.Vector2f ret = new(0, 0);
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{RegKeyName}"))
                {
                    if (spkey is not null)
                    {
                        float.TryParse(spkey.GetValue("SpinePositionX", "0").ToString(), out ret.X);
                        float.TryParse(spkey.GetValue("SpinePositionY", "0").ToString(), out ret.Y);
                    }
                }
                return ret;
            }
            set
            {
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{RegKeyName}"))
                {
                    if (spkey is not null)
                    {
                        spkey.SetValue("SpinePositionX", value.X.ToString());
                        spkey.SetValue("SpinePositionY", value.Y.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前正在运行的 Spine 版本
        /// </summary>
        public string SpineVersion
        {
            get
            {
                var v = "3.8.x";
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.Version; break; } }
                mutex.ReleaseMutex();
                return v;
            }
        }

        /// <summary>
        /// 控制 Spine 是否水平翻转
        /// </summary>
        public bool SpineFlip
        {
            get
            {
                var v = false;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.FlipX; break; } }
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { sp.FlipX = value; } }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 控制 Spine 缩放比例
        /// </summary>
        public float SpineScale
        {
            get
            {
                var v = 1f;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.Scale; break; } }
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null && Math.Abs(value - sp.Scale) > 1e-3) { sp.Scale = value; } }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Spine 是否使用预乘 Alpha
        /// </summary>
        public bool SpineUsePMA
        {
            get
            {
                var v = false;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.UsePremultipliedAlpha; break; } }
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { sp.UsePremultipliedAlpha = value; } }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Spine 位置
        /// </summary>
        public SFML.System.Vector2f SpinePosition
        {
            get
            {
                var v = SpinePositionReg;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = new SFML.System.Vector2f(sp.X, sp.Y); break; } }
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { sp.X = value.X; sp.Y = value.Y; } }
                mutex.ReleaseMutex();
                SpinePositionReg = value;
            }
        }

        /// <summary>
        /// 加载 Spine 到指定槽位
        /// </summary>
        public void LoadSpine(string version, string skelPath, int index)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");

            if (version == spineSlots[index]?.Version && skelPath == spineSlots[index]?.SkelPath)
                return;

            Debug.WriteLine($"Loading spine[{index}]({version}) from {skelPath}");
            Spine.Spine spineNew;
            try { spineNew = Spine.Spine.New(version, skelPath, defaultMix: 0.15f); }
            catch { throw; }

            // 尝试用已有的 Spine 对象恢复位置大小信息
            var originalPosition = SpinePosition;
            spineNew.X = originalPosition.X;
            spineNew.Y = originalPosition.Y;
            spineNew.Scale = SpineScale;
            spineNew.FlipX = SpineFlip;

            mutex.WaitOne();
            spineSlots[index] = spineNew;
            colorTables[index] = null;
            mutex.ReleaseMutex();

            // 尝试修正自动背景颜色
            SetProperAutoBackgroudColor();

            SpineLoaded(index);
            Debug.Write("spine animiation: ");
            foreach (var a in spineSlots[index].AnimationNames) Debug.Write($"{a}; "); Debug.WriteLine("");
        }

        /// <summary>
        /// 卸载指定槽位 Spine
        /// </summary>
        public void UnloadSpine(int index)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");

            if (spineSlots[index]?.SkelPath is null) return;

            mutex.WaitOne();
            spineSlots[index] = null;
            colorTables[index] = null;
            mutex.ReleaseMutex();

            Debug.WriteLine($"Unload spine[{index}]");
        }

        /// <summary>
        /// 获取指定槽位的 Spine 资源路径
        /// </summary>
        public string? GetSpineSkelPath(int index)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");
            mutex.WaitOne();
            var v = spineSlots[index]?.SkelPath;
            mutex.ReleaseMutex();
            return v;
        }

        /// <summary>
        /// 要使用的自动背景颜色, null 不进行自动颜色, 可以通过 BackgroundColor 属性进行设置
        /// </summary>
        public AutoBackgroudColorType AutoBackgroudColor
        {
            get => autoBackgroudColor;
            set { if (value == autoBackgroudColor) return; autoBackgroudColor = value; SetProperAutoBackgroudColor(); }
        }
        private AutoBackgroudColorType autoBackgroudColor = AutoBackgroudColorType.Gray;

        /// <summary>
        /// 重置窗口和 Spine 的位置和大小
        /// </summary>
        public void ResetPositionAndSize()
        {
            SpinePosition = new(0, 0);
            Position = new(0, 0);
            Size = new(1000, 1000);
        }

        /// <summary>
        /// 自动背景色模式下设置一个正确的背景色, 如果未使用自动颜色则不做任何操作
        /// </summary>
        private void SetProperAutoBackgroudColor()
        {
            if (autoBackgroudColor == AutoBackgroudColorType.None)
                return;

            // 确保每个加载了 Spine 的槽位有颜色表
            for (int i = 0; i < spineSlots.Length; i++)
            {
                if (spineSlots[i] is not null && colorTables[i] is null)
                    colorTables[i] = spineSlots[i].ColorTable;
            }

            var rnd = new Random();
            var bestColor = SFML.Graphics.Color.Transparent;
            uint bestColorSameCount = uint.MaxValue;
            var currentColor = SFML.Graphics.Color.Transparent;
            for (int i = 0; i < 10; i++)
            {
                // BUG: SetLayeredWindowAttributes 的 R 和 B 分量必须相等才能让背景部分的透明和穿透同时生效
                switch (autoBackgroudColor)
                {
                    case AutoBackgroudColorType.Black:
                        currentColor.R = currentColor.B = (byte)rnd.Next(0, 20);
                        currentColor.G = (byte)rnd.Next(0, 20);
                        break;
                    case AutoBackgroudColorType.White:
                        currentColor.R = currentColor.B = (byte)rnd.Next(235, 255);
                        currentColor.G = (byte)rnd.Next(235, 255);
                        break;
                    case AutoBackgroudColorType.Gray:
                        currentColor.R = currentColor.B = (byte)rnd.Next(118, 138);
                        currentColor.G = (byte)rnd.Next(118, 138);
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid type: {autoBackgroudColor}");
                }

                var k = currentColor.ToInteger();
                uint count = 0;
                uint tmp = 0;
                // 统计在所有表中该颜色出现的次数
                // 选择重复次数最少的颜色, 如果发现了 0 次, 则立即结束查找
                foreach (var table in colorTables)
                {
                    if (table is not null && table.TryGetValue(k, out tmp))
                        count += tmp;
                }
                if (count < bestColorSameCount)
                {
                    bestColor = currentColor;
                    bestColorSameCount = count;
                }
                if (bestColorSameCount <= 0)
                    break;
            }

            Debug.WriteLine($"AutoBackground Color: {bestColor}, Count: {bestColorSameCount}");
            BackgroudColor = bestColor;
        }

        // TODO 语音功能实现

        /********************************* 子类可重写事件 *********************************/

        protected override void Drag(SFML.Window.Mouse.Button button, SFML.System.Vector2f delta,SFML.System.Vector2f deltaFromSrc)
        {
            base.Drag(button, delta, deltaFromSrc);

            if (button == SFML.Window.Mouse.Button.Right)
            {
                SpinePosition += delta;
            }
        }

        /// <summary>
        /// Spine 被加载
        /// </summary>
        protected virtual void SpineLoaded(int index) { }
    }
}

