using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public class AzurLaneSD : SpineWindow
    {
        private enum State
        {
            Idle = 0,
            Dragging = 1,
            Working = 2,
            Sleeping = 3,
        }

        private string animation_Idle;
        private string animation_Dragging = "";
        private string animation_MouseLeftClick = "";
        private string animation_MouseLeftDoubleClick = "";
        private string animation_MouseRightClick = "";
        private string animation_MouseRightDoubleClick = "";
        private string animation_MouseWheelScroll = "";
        private string animation_Working = "";
        private string animation_Sleep = "";

        private State state = State.Idle;
        private State dragBeforeState = State.Idle;

        public override bool FaceToRight 
        {
            get { mutex.WaitOne(); var v = !spine.FlipX; mutex.ReleaseMutex(); return v; } 
            set { mutex.WaitOne(); spine.FlipX = !value; mutex.ReleaseMutex(); }
        }

        protected override void Trigger_SpineLoaded()
        {
            base.Trigger_SpineLoaded();
            mutex.WaitOne();
            var animationNames = spine.AnimationNames;
            var defaultName = spine.DefaultAnimationName;
            mutex.ReleaseMutex();

            animation_Idle = animationNames.Contains("normal") ? "normal" : defaultName;
            animation_Dragging = animationNames.Contains("tuozhuai") ? "tuozhuai" : (animationNames.Contains("tuozhuai2") ? "tuozhuai2" : "");
            animation_MouseLeftClick = animationNames.Contains("touch") ? "touch" : "";
            animation_MouseLeftDoubleClick = animationNames.Contains("motou") ? "motou" : "";
            //animation_MouseRightClick = spine.AnimationNames.Contains("normal") ? null : null;
            //animation_MouseRightDbClick = spine.AnimationNames.Contains("normal") ? null : null;
            animation_MouseWheelScroll = animationNames.Contains("yun") ? "yun" : "";
            animation_Working = animationNames.Contains("walk") ? "walk" : "";
            animation_Sleep = animationNames.Contains("sleep") ? "sleep" : "";

            mutex.WaitOne();
            spine.CurrentAnimation = animation_Idle;
            mutex.ReleaseMutex();
            state = State.Idle;
        }

        protected override void Trigger_MouseButtonClick(MouseButtonEventArgs e)
        {
            base.Trigger_MouseButtonClick(e);
            switch (e.Button)
            {
                case Mouse.Button.Left:
                    if (!string.IsNullOrEmpty(animation_MouseLeftClick))
                    {
                        mutex.WaitOne();
                        spine.CurrentAnimation = animation_MouseLeftClick;
                        spine.AddAnimation(animation_Idle);
                        mutex.ReleaseMutex();
                    }
                    break;
                case Mouse.Button.Right:
                    if (!string.IsNullOrEmpty(animation_MouseRightClick))
                    {
                        mutex.WaitOne();
                        spine.CurrentAnimation = animation_MouseRightClick;
                        spine.AddAnimation(animation_Idle);
                        mutex.ReleaseMutex();
                    }
                    break;
            }

            state = state switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Dragging,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }

        protected override void Trigger_MouseButtonDoubleClick(MouseButtonEventArgs e)
        {
            base.Trigger_MouseButtonDoubleClick(e);
            switch (e.Button)
            {
                case Mouse.Button.Left:
                    if (!string.IsNullOrEmpty(animation_MouseLeftDoubleClick))
                    {
                        mutex.WaitOne();
                        spine.CurrentAnimation = animation_MouseLeftDoubleClick;
                        spine.AddAnimation(animation_Idle);
                        mutex.ReleaseMutex();
                    }
                    break;
                case Mouse.Button.Right:
                    if (!string.IsNullOrEmpty(animation_MouseRightDoubleClick))
                    {
                        mutex.WaitOne();
                        spine.CurrentAnimation = animation_MouseRightDoubleClick;
                        spine.AddAnimation(animation_Idle);
                        mutex.ReleaseMutex();
                    }
                    break;
            }

            state = state switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Dragging,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }

        protected override void Trigger_MouseWheelScroll(MouseWheelScrollEventArgs e)
        {
            base.Trigger_MouseWheelScroll(e);
            if (!string.IsNullOrEmpty(animation_MouseWheelScroll))
            {
                mutex.WaitOne();
                if (spine.CurrentAnimation != animation_MouseWheelScroll)
                {
                    spine.CurrentAnimation = animation_MouseWheelScroll;
                    int count = (int)Math.Abs(e.Delta);
                    for (int i = 0; i < count; i++)
                        spine.AddAnimation(animation_MouseWheelScroll);
                        spine.AddAnimation(animation_Idle);
                }
                mutex.ReleaseMutex();
            }

            state = state switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Dragging,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }

        protected override void Trigger_MouseDragBegin(MouseMoveEventArgs e)
        {
            base.Trigger_MouseDragBegin(e);

            if (!string.IsNullOrEmpty(animation_Dragging))
            {
                mutex.WaitOne();
                spine.CurrentAnimation = animation_Dragging;
                mutex.ReleaseMutex();
            }

            dragBeforeState = state;
            state = state switch
            {
                State.Idle => State.Dragging,
                State.Dragging => State.Dragging,
                State.Working => State.Dragging,
                State.Sleeping => State.Dragging,
                _ => state,
            };
        }

        protected override void Trigger_MouseDragEnd(MouseButtonEventArgs e)
        {
            base.Trigger_MouseDragEnd(e);

            mutex.WaitOne();
            spine.CurrentAnimation = dragBeforeState switch
            {
                State.Idle => animation_Idle,
                State.Dragging => animation_Idle,
                State.Working => animation_Working,
                State.Sleeping => animation_Idle,
                _ => animation_Idle,
            };
            mutex.ReleaseMutex();

            state = dragBeforeState switch
            {
                State.Idle => State.Idle,
                State.Dragging => State.Idle,
                State.Working => State.Working,
                State.Sleeping => State.Idle,
                _ => state,
            };
        }
    }
}
