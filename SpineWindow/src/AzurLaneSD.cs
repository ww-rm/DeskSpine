using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public sealed class AzurLaneSD : SpineRenderWindow
    {
        private enum State
        {
            Idle = 0,
            Dragging = 1,
            Working = 2,
            Sleeping = 3,
        }

        private string animation_Idle = "";
        private string animation_Dragging = "";
        private string animation_MouseLeftClick = "";
        private string animation_MouseLeftDoubleClick = "";
        private string animation_MouseRightClick = "";
        private string animation_MouseRightDoubleClick = "";
        private string animation_MouseWheelScroll = "";
        private string animation_Working = "";
        private string animation_Sleep = "";
        private string animation_Stand = "";

        private State state = State.Idle;
        private State previousState = State.Idle;
        private Random rnd = new();

        private float standElapsedTime = 0f;
        private const float meanWaitingTime = 30f;
        private float nextStandTime = 10f;

        public AzurLaneSD(uint slotCount) : base(slotCount) { }

        protected override void SpineLoaded(int index)
        {
            base.SpineLoaded(index);

            if (index != 0)
                return;

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                var animationNames = spineSlots[0].AnimationNames;
                var defaultName = spineSlots[0].DefaultAnimationName;

                animation_Idle = animationNames.Contains("normal") ? "normal" : defaultName;
                animation_Dragging = animationNames.Contains("tuozhuai") ? "tuozhuai" : (animationNames.Contains("tuozhuai2") ? "tuozhuai2" : "");
                animation_MouseLeftClick = animationNames.Contains("touch") ? "touch" : "";
                animation_MouseLeftDoubleClick = animationNames.Contains("motou") ? "motou" : "";
                animation_MouseWheelScroll = animationNames.Contains("yun") ? "yun" : "";
                animation_Working = animationNames.Contains("walk") ? "walk" : "";
                animation_Sleep = animationNames.Contains("sleep") ? "sleep" : "";
                animation_Stand = animationNames.Contains("stand") ? "stand" : (animationNames.Contains("stand2") ? "stand2" : "");

                spineSlots[0].CurrentAnimation = animation_Idle;
                state = State.Idle;
            }
            mutex.ReleaseMutex();
        }

        protected override void Update(float delta)
        {
            base.Update(delta);

            standElapsedTime += delta;
            if (standElapsedTime >= nextStandTime)
            {
                standElapsedTime = 0;
                nextStandTime = (float)(-meanWaitingTime * Math.Log(rnd.NextSingle()));
                Debug.WriteLine($"Next time to stand: {nextStandTime}");
                if (state == State.Idle)
                {
                    mutex.WaitOne();
                    if (spineSlots[0] is not null)
                    {
                        if (spineSlots[0].CurrentAnimation == animation_Idle)
                        {
                            spineSlots[0].CurrentAnimation = animation_Stand;
                            spineSlots[0].AddAnimation(animation_Idle);
                        }
                    }
                    mutex.ReleaseMutex();
                }
            }
        }

        protected override void Click(SFML.Window.Mouse.Button button)
        {
            base.Click(button);

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                string nextAnimation = state switch
                {
                    State.Idle => animation_Idle,
                    State.Dragging => animation_Dragging,
                    State.Working => animation_Working,
                    State.Sleeping => animation_Idle,
                    _ => spineSlots[0].CurrentAnimation,
                };

                switch (button)
                {
                    case SFML.Window.Mouse.Button.Left:
                        spineSlots[0].CurrentAnimation = animation_MouseLeftClick;
                        break;
                    case SFML.Window.Mouse.Button.Right:
                        spineSlots[0].CurrentAnimation = animation_MouseRightClick;
                        break;
                }

                spineSlots[0].AddAnimation(nextAnimation);
                state = state switch
                {
                    State.Idle => State.Idle,
                    State.Dragging => State.Dragging,
                    State.Working => State.Working,
                    State.Sleeping => State.Idle,
                    _ => state,
                };
            }
            mutex.ReleaseMutex();
        }

        protected override void DoubleClick(SFML.Window.Mouse.Button button)
        {
            base.DoubleClick(button);

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                string nextAnimation = state switch
                {
                    State.Idle => animation_Idle,
                    State.Dragging => animation_Dragging,
                    State.Working => animation_Working,
                    State.Sleeping => animation_Idle,
                    _ => spineSlots[0].CurrentAnimation,
                };

                switch (button)
                {
                    case SFML.Window.Mouse.Button.Left:
                        spineSlots[0].CurrentAnimation = animation_MouseLeftDoubleClick;
                        break;
                    case SFML.Window.Mouse.Button.Right:
                        spineSlots[0].CurrentAnimation = animation_MouseRightDoubleClick;
                        break;
                }

                spineSlots[0].AddAnimation(nextAnimation);
                state = state switch
                {
                    State.Idle => State.Idle,
                    State.Dragging => State.Dragging,
                    State.Working => State.Working,
                    State.Sleeping => State.Idle,
                    _ => state,
                };
            }
            mutex.ReleaseMutex();
        }

        protected override void Scroll(SFML.Window.Mouse.Wheel wheel, float delta)
        {
            base.Scroll(wheel, delta);

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                string nextAnimation = state switch
                {
                    State.Idle => animation_Idle,
                    State.Dragging => animation_Dragging,
                    State.Working => animation_Working,
                    State.Sleeping => animation_Idle,
                    _ => spineSlots[0].CurrentAnimation,
                };

                if (spineSlots[0].CurrentAnimation != animation_MouseWheelScroll)
                {
                    spineSlots[0].CurrentAnimation = animation_MouseWheelScroll;
                    for (int i = 0; i < (int)Math.Abs(delta); i++)
                        spineSlots[0].AddAnimation(animation_MouseWheelScroll);
                }

                spineSlots[0].AddAnimation(nextAnimation);
                state = state switch
                {
                    State.Idle => State.Idle,
                    State.Dragging => State.Dragging,
                    State.Working => State.Working,
                    State.Sleeping => State.Idle,
                    _ => state,
                };
            }
            mutex.ReleaseMutex();
        }

        protected override void DragBegin(SFML.Window.Mouse.Button button)
        {
            base.DragBegin(button);

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                spineSlots[0].CurrentAnimation = animation_Dragging;

                previousState = state;
                state = state switch
                {
                    State.Idle => State.Dragging,
                    State.Dragging => State.Dragging,
                    State.Working => State.Dragging,
                    State.Sleeping => State.Dragging,
                    _ => state,
                };
            }
            mutex.ReleaseMutex();
        }

        protected override void DragEnd(SFML.Window.Mouse.Button button)
        {
            base.DragEnd(button);

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                spineSlots[0].CurrentAnimation = previousState switch
                {
                    State.Idle => animation_Idle,
                    State.Dragging => animation_Idle,
                    State.Working => animation_Working,
                    State.Sleeping => animation_Idle,
                    _ => animation_Idle,
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
            mutex.ReleaseMutex();
        }

        protected override void SleepStateChange(bool sleep)
        {
            base.SleepStateChange(sleep);

            mutex.WaitOne();
            if (sleep)
            {
                if (spineSlots[0] is not null && state == State.Idle)
                {
                    spineSlots[0].CurrentAnimation = animation_Sleep;
                    state = State.Sleeping;
                }
            }
            else
            {
                if (spineSlots[0] is not null && state == State.Sleeping)
                {
                    spineSlots[0].CurrentAnimation = animation_Idle;
                    state = State.Idle;
                }
            }
            mutex.ReleaseMutex();
        }
    }
}
