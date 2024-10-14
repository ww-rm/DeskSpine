using Spine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyEngine
{
    /// <summary>
    /// 可放置多个 Spine 骨骼的对象
    /// </summary>
    public class SpineSlot : Renderable, IDisposable
    {
        private readonly Spine.Spine[] slots;
        private Animator animator;

        public SpineSlot(int slotCount)
        {
            slots = new Spine.Spine[slotCount];
            animator = Animator.New(animatorType, slots);
            components["animator"] = animator;
        }

        ~SpineSlot() { Dispose(false); }
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i]?.Dispose();
                slots[i] = null;
            }
        }

        /// <summary>
        /// 加载 Spine 到指定槽位
        /// </summary>
        public void SetSpine(string? skelPath, int index)
        {
            if (index >= slots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {slots.Length}, got index {index}");

            if (skelPath == slots[index]?.SkelPath)
                return;

            if (string.IsNullOrEmpty(skelPath))
            {
                slots[index]?.Dispose();
                slots[index] = null;
                Debug.WriteLine($"Unload spine[{index}]");
                return;
            }

            Debug.WriteLine($"Loading spine[{index}]({version}) from {skelPath}");
            Spine.Spine spineNew = Spine.Spine.New(version, skelPath, defaultMix: 0.15f);

            // 设置位置大小信息
            var originalPosition = Position;
            spineNew.X = originalPosition.X;
            spineNew.Y = originalPosition.Y;
            spineNew.Scale = Scale;
            spineNew.FlipX = Flip;
            spineNew.UsePremultipliedAlpha = UsePremultipliedAlpha;

            slots[index] = spineNew;
            animator.Reset();

            Debug.Write("spine animiation: ");
            foreach (var a in slots[index].AnimationNames) Debug.Write($"{a}; "); Debug.WriteLine("");
        }

        /// <summary>
        /// 获取指定槽位的 Spine 资源路径
        /// </summary>
        public string? GetSpineSkelPath(int index)
        {
            if (index >= slots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {slots.Length}, got index {index}");
            return slots[index]?.SkelPath;
        }

        /// <summary>
        /// 运行时版本
        /// </summary>
        public SpineVersion Version
        {
            get => version;
            set
            {
                if (version == value) return;
                var originalPosition = Position;
                var originalScale = Scale;
                var originalFlip = Flip;
                var originalUsePMA = UsePremultipliedAlpha;

                for (int i = 0; i < slots.Length; i++)
                {
                    var sp = slots[i];
                    if (sp is null) continue;
                    var newSpine = Spine.Spine.New(value, sp.SkelPath, defaultMix: 0.15f);
                    newSpine.X = originalPosition.X;
                    newSpine.Y = originalPosition.Y;
                    newSpine.Scale = originalScale;
                    newSpine.FlipX = originalFlip;
                    newSpine.UsePremultipliedAlpha = originalUsePMA;
                    slots[i] = newSpine;
                    animator.Reset();
                }
                version = value;
            }
        }
        private SpineVersion version = SpineVersion.V38;

        /// <summary>
        /// 左右翻转
        /// </summary>
        public bool Flip
        {
            get => flip;
            set { flip = value; foreach(var sp in slots)  { if (sp is not null) { sp.FlipX = value; } } }
        }
        private bool flip = false;

        /// <summary>
        /// 缩放比例
        /// </summary>
        public float Scale
        {
            get => scale;
            set
            {
                if (Math.Abs(value - scale) < 1e-3) return;
                scale = value;
                foreach (var sp in slots) { if (sp is not null) sp.Scale = value; }
            }
        }
        private float scale = 1f;

        /// <summary>
        /// 是否使用预乘 Alpha
        /// </summary>
        public bool UsePremultipliedAlpha
        {
            get => usePremultipliedAlpha;
            set { usePremultipliedAlpha = value; foreach (var sp in slots) { if (sp is not null) sp.UsePremultipliedAlpha = value; } }
        }
        private bool usePremultipliedAlpha = false;

        /// <summary>
        /// Spine 位置
        /// </summary>
        public SFML.System.Vector2f Position
        {
            get => position;
            set
            {
                position = value;
                foreach (var sp in slots) { if (sp is not null) { sp.X = value.X; sp.Y = value.Y; } }
            }
        }
        private SFML.System.Vector2f position = new(0, 0);

        /// <summary>
        /// 动画交互类型
        /// </summary>
        public AnimatorType AnimatorType
        {
            get => animatorType;
            set
            {
                if (value == animatorType) return;
                animator = Animator.New(value, slots);
                components["animator"] = animator;
                animator.Reset();
                animatorType = value;
            }
        }
        private AnimatorType animatorType = AnimatorType.AzurLaneSD;

        public override void Render(SFML.Graphics.RenderTarget target)
        {
            base.Render(target);
            for (int i = slots.Length - 1; i >= 0; i--)
            {
                var sp = slots[i];
                if (sp is not null) target.Draw(sp);
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            foreach (var sp in slots)
            {
                sp?.Update(delta);
            }
        }

        public override void Drag(SFML.Window.Mouse.Button button, SFML.System.Vector2f delta, SFML.System.Vector2f deltaFromSrc)
        {
            base.Drag(button, delta, deltaFromSrc);
            if (button == SFML.Window.Mouse.Button.Right)
            {
                Position += delta;
            }
        }
    }
}