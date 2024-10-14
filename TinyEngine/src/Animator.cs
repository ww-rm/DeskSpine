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
    public enum AnimatorType
    {
        AzurLaneSD = 0,                 // 碧蓝航线_后宅小人
        AzurLaneDynamic = 1,            // 碧蓝航线_动态立绘
        ArknightsDynamic = 2,           // 明日方舟_动态立绘
        ArknightsBuild = 3,             // 明日方舟_基建小人
        ArknightsBattle = 4,            // 明日方舟_战斗小人
    }

    internal enum State
    {
        Idle = 0,
        Dragging = 1,
        Working = 2,
        Sleeping = 3,
    }

    public abstract class Animator : Renderable
    {
        /// <summary>
        /// 创建指定类型动画控制器
        /// </summary>
        public static Animator New(AnimatorType type, Spine.Spine[] slots)
        {
            return type switch
            {
                AnimatorType.AzurLaneSD => new AzurLaneSD(slots),
                AnimatorType.AzurLaneDynamic => new AzurLaneDynamic(slots),
                AnimatorType.ArknightsDynamic => throw new NotImplementedException(),
                AnimatorType.ArknightsBuild => throw new NotImplementedException(),
                AnimatorType.ArknightsBattle => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };
        }

        protected readonly Spine.Spine[] slots;

        public Animator(Spine.Spine[] slots) { this.slots = slots; }

        /// <summary>
        /// 重置状态
        /// </summary>
        public abstract void Reset();
    }

    /// <summary>
    /// 碧蓝航线后宅小人
    /// </summary>
    internal class AzurLaneSD : Animator
    {
        private State state = State.Idle;
        private State previousState = State.Idle;

        private Random rnd = new();
        private float randomElapsedTime = 0f;
        private const float meanWaitingTime = 30f;
        private float nextRandomTime = 10f;

        private const string Animation_Idle = "normal";
        private const string Animation_Dragging = "tuozhuai";
        private const string Animation_MouseLeftClick = "touch";
        private const string Animation_MouseLeftDoubleClick = "motou";
        private const string Animation_MouseWheelScroll = "yun";
        private const string Animation_Working = "walk";
        private const string Animation_Sleep = "sleep";
        private const string Animation_Random = "stand";

        public AzurLaneSD(Spine.Spine[] slots) : base(slots) { }

        public override void Reset() { state = State.Idle; foreach (var sp in slots) if (sp is not null) sp.CurrentAnimation = Animation_Idle; }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (slots[0] is null) return;

            randomElapsedTime += delta;
            if (randomElapsedTime >= nextRandomTime)
            {
                randomElapsedTime = 0;
                nextRandomTime = (float)(-meanWaitingTime * Math.Log(rnd.NextSingle()));
                Debug.WriteLine($"Next time to stand: {nextRandomTime}");
                if (state == State.Idle)
                {
                    if (slots[0].CurrentAnimation == Animation_Idle)
                    {
                        slots[0].CurrentAnimation = Animation_Random;
                        slots[0].AddAnimation(Animation_Idle);
                    }
                }
            }
        }

        public override void Click(SFML.Window.Mouse.Button button)
        {
            base.Click(button);

            if (slots[0] is null) return;

            string nextAnimation = state switch
            {
                State.Idle => Animation_Idle,
                State.Dragging => Animation_Dragging,
                State.Working => Animation_Working,
                State.Sleeping => Animation_Idle,
                _ => slots[0].CurrentAnimation,
            };

            switch (button)
            {
                case SFML.Window.Mouse.Button.Left:
                    slots[0].CurrentAnimation = Animation_MouseLeftClick;
                    break;
            }

            slots[0].AddAnimation(nextAnimation);
            state = state switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Dragging,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }

        public override void DoubleClick(SFML.Window.Mouse.Button button)
        {
            base.DoubleClick(button);

            if (slots[0] is null) return;

            string nextAnimation = state switch
            {
                State.Idle => Animation_Idle,
                State.Dragging => Animation_Dragging,
                State.Working => Animation_Working,
                State.Sleeping => Animation_Idle,
                _ => slots[0].CurrentAnimation,
            };

            switch (button)
            {
                case SFML.Window.Mouse.Button.Left:
                    slots[0].CurrentAnimation = Animation_MouseLeftDoubleClick;
                    break;
            }

            slots[0].AddAnimation(nextAnimation);
            state = state switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Dragging,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }

        public override void Scroll(SFML.Window.Mouse.Wheel wheel, float delta)
        {
            base.Scroll(wheel, delta);

            if (slots[0] is null) return;

            string nextAnimation = state switch
            {
                State.Idle => Animation_Idle,
                State.Dragging => Animation_Dragging,
                State.Working => Animation_Working,
                State.Sleeping => Animation_Idle,
                _ => slots[0].CurrentAnimation,
            };

            if (slots[0].CurrentAnimation != Animation_MouseWheelScroll)
            {
                slots[0].CurrentAnimation = Animation_MouseWheelScroll;
                for (int i = 0; i < (int)Math.Abs(delta); i++)
                    slots[0].AddAnimation(Animation_MouseWheelScroll);
            }

            slots[0].AddAnimation(nextAnimation);
            state = state switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Dragging,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }

        public override void DragBegin(SFML.Window.Mouse.Button button)
        {
            base.DragBegin(button);

            if (slots[0] is null) return;

            slots[0].CurrentAnimation = Animation_Dragging;

            previousState = state;
            state = State.Dragging;
        }

        public override void DragEnd(SFML.Window.Mouse.Button button)
        {
            base.DragEnd(button);

            if (slots[0] is null) return;

            slots[0].CurrentAnimation = previousState switch
            {
                State.Idle => Animation_Idle,
                State.Dragging => Animation_Idle,
                State.Working => Animation_Working,
                State.Sleeping => Animation_Idle,
                _ => Animation_Idle,
            };

            state = previousState switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Idle,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }

        public override void SleepStateChange(bool sleep)
        {
            base.SleepStateChange(sleep);

            if (slots[0] is null) return;

            if (sleep)
            {
                if (state == State.Idle)
                {
                    slots[0].CurrentAnimation = Animation_Sleep;
                    state = State.Sleeping;
                }
            }
            else
            {
                if (state == State.Sleeping)
                {
                    slots[0].CurrentAnimation = Animation_Idle;
                    state = State.Idle;
                }
            }
        }
    }

    /// <summary>
    /// 碧蓝航线动态立绘
    /// </summary>
    internal class AzurLaneDynamic : Animator
    {
        public AzurLaneDynamic(Spine.Spine[] slots) : base(slots) { }

        public override void Reset() { foreach (var sp in slots) if (sp is not null) sp.CurrentAnimation = "normal"; }

        public override void Click(SFML.Window.Mouse.Button button)
        {
            base.Click(button);
            if (button == SFML.Window.Mouse.Button.Left)
            {
                foreach (var sp in slots) { if (sp is not null) { sp.CurrentAnimation = "click"; sp.AddAnimation("normal"); } }
            }
        }
    }

    /// <summary>
    /// 明日方舟动态立绘
    /// </summary>
    internal class ArknightsDynamic : Animator
    {
        private Random rnd = new();
        private float randomElapsedTime = 0f;
        private const float meanWaitingTime = 30f;
        private float nextRandomTime = 10f;

        private const string Animation_Idle = "Idle";
        private const string Animation_MouseLeftClick = "Interact";
        private const string Animation_Random = "Special";

        public ArknightsDynamic(Spine.Spine[] slots) : base(slots) { }

        public override void Reset() { foreach (var sp in slots) if (sp is not null) sp.CurrentAnimation = Animation_Idle; }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (slots[0] is null) return;

            randomElapsedTime += delta;
            if (randomElapsedTime >= nextRandomTime)
            {
                randomElapsedTime = 0;
                nextRandomTime = (float)(-meanWaitingTime * Math.Log(rnd.NextSingle()));
                Debug.WriteLine($"Next time to stand: {nextRandomTime}");
                if (slots[0].CurrentAnimation == Animation_Idle)
                {
                    slots[0].CurrentAnimation = Animation_Random;
                    slots[0].AddAnimation(Animation_Idle);
                }
            }
        }

        public override void Click(SFML.Window.Mouse.Button button)
        {
            base.Click(button);
            if (slots[0] is null) return;

            if (button == SFML.Window.Mouse.Button.Left && slots[0].CurrentAnimation != Animation_MouseLeftClick)
            {
                slots[0].CurrentAnimation = Animation_MouseLeftClick;
                slots[0].AddAnimation(Animation_Idle);
            }
        }
    }
}
