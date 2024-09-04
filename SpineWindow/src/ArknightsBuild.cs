using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public class ArknightsBuild: SpineWindow
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

        private SFML.System.Clock clockStand = new();
        private const float meanWaitingTime = 30f;
        private float nextStandTime = 10f;

        public ArknightsBuild(uint slotCount) : base(slotCount) { }

        protected override void Trigger_SpineLoaded(int index)
        {
            base.Trigger_SpineLoaded(index);

            if (index != 0)
                return;

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                var animationNames = spineSlots[0].AnimationNames;
                var defaultName = spineSlots[0].DefaultAnimationName;

                //"Default; Interact; Move; Relax; Sit; Sleep; Special;"
                animation_Idle = animationNames.Contains("Idle") ? "Idle" : defaultName;
                animation_Dragging = animationNames.Contains("tuozhuai") ? "tuozhuai" : (animationNames.Contains("tuozhuai2") ? "tuozhuai2" : "");
                animation_MouseLeftClick = animationNames.Contains("Attack") ? "Skill_2_Loop" : "";
                animation_MouseLeftDoubleClick = animationNames.Contains("motou") ? "motou" : "";
                animation_MouseWheelScroll = animationNames.Contains("yun") ? "yun" : "";
                animation_Working = animationNames.Contains("walk") ? "walk" : "";
                animation_Sleep = animationNames.Contains("Sleep") ? "Sleep" : "";
                animation_Stand = animationNames.Contains("stand") ? "stand" : (animationNames.Contains("stand2") ? "stand2" : "");

                spineSlots[0].CurrentAnimation = animation_Idle;
                state = State.Idle;
            }
            mutex.ReleaseMutex();

            clockStand.Restart();
        }

        protected override void Trigger_StateUpdated()
        {
            base.Trigger_StateUpdated();

            if (clockStand.ElapsedTime.AsSeconds() >= nextStandTime)
            {
                clockStand.Restart();
                nextStandTime = (float)(-meanWaitingTime * Math.Log(rnd.NextSingle()));
                Debug.WriteLine($"Next time to stand: {nextStandTime}");
                if (!string.IsNullOrEmpty(animation_Stand) && state == State.Idle)
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

        protected override void Trigger_MouseButtonClick(MouseButtonEventArgs e)
        {
            base.Trigger_MouseButtonClick(e);

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

                switch (e.Button)
                {
                    case Mouse.Button.Left:
                        if (!string.IsNullOrEmpty(animation_MouseLeftClick))
                            spineSlots[0].CurrentAnimation = animation_MouseLeftClick;
                        break;
                    case Mouse.Button.Right:
                        if (!string.IsNullOrEmpty(animation_MouseRightClick))
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

        protected override void Trigger_MouseButtonDoubleClick(MouseButtonEventArgs e)
        {
            base.Trigger_MouseButtonDoubleClick(e);

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

                switch (e.Button)
                {
                    case Mouse.Button.Left:
                        if (!string.IsNullOrEmpty(animation_MouseLeftDoubleClick))
                            spineSlots[0].CurrentAnimation = animation_MouseLeftDoubleClick;
                        break;
                    case Mouse.Button.Right:
                        if (!string.IsNullOrEmpty(animation_MouseRightDoubleClick))
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

        protected override void Trigger_MouseWheelScroll(MouseWheelScrollEventArgs e)
        {
            base.Trigger_MouseWheelScroll(e);

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

                if (!string.IsNullOrEmpty(animation_MouseWheelScroll))
                {
                    if (spineSlots[0].CurrentAnimation != animation_MouseWheelScroll)
                    {
                        spineSlots[0].CurrentAnimation = animation_MouseWheelScroll;
                        for (int i = 0; i < (int)Math.Abs(e.Delta); i++)
                            spineSlots[0].AddAnimation(animation_MouseWheelScroll);
                    }
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

        protected override void Trigger_MouseDragBegin(MouseMoveEventArgs e)
        {
            base.Trigger_MouseDragBegin(e);

            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                if (!string.IsNullOrEmpty(animation_Dragging))
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

        protected override void Trigger_MouseDragEnd(MouseButtonEventArgs e)
        {
            base.Trigger_MouseDragEnd(e);

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

        protected override void Trigger_FallAsleep()
        {
            base.Trigger_FallAsleep();

            mutex.WaitOne();
            if (spineSlots[0] is not null && state == State.Idle)
            {
                if (!string.IsNullOrEmpty(animation_Sleep))
                    spineSlots[0].CurrentAnimation = animation_Sleep;
                state = State.Sleeping;
            }
            mutex.ReleaseMutex();
        }

        protected override void Trigger_WakeUp()
        {
            base.Trigger_WakeUp();

            mutex.WaitOne();
            if (spineSlots[0] is not null && state == State.Sleeping)
            {
                spineSlots[0].CurrentAnimation = animation_Idle;
                state = State.Idle;
            }
            mutex.ReleaseMutex();
        }
    }
}
