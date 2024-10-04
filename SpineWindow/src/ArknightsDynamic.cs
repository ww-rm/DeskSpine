using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public sealed class ArknightsDynamic : SpineWindow
    {
        private string animation_Idle = "Idle";
        private string animation_Interact = "Interact";
        private string animation_Special = "Special";

        private Random rnd = new();

        private float specialElapsedTime = 0f;
        private const float meanWaitingTime = 30f;
        private float nextSpecialTime = 10f;

        public ArknightsDynamic(uint slotCount) : base(slotCount) { }

        protected override void SpineLoaded(int index)
        {
            base.SpineLoaded(index);

            mutex.WaitOne();
            if (spineSlots[0] is not null) 
                spineSlots[0].CurrentAnimation = animation_Idle;
            mutex.ReleaseMutex();
        }

        protected override void Update(float delta)
        {
            base.Update(delta);

            specialElapsedTime += delta;
            if (specialElapsedTime >= nextSpecialTime)
            {
                specialElapsedTime = 0;
                nextSpecialTime = (float)(-meanWaitingTime * Math.Log(rnd.NextSingle()));
                Debug.WriteLine($"Next time to special: {nextSpecialTime}");
                mutex.WaitOne();
                if (spineSlots[0] is not null)
                {
                    if (spineSlots[0].CurrentAnimation == animation_Idle)
                    {
                        spineSlots[0].CurrentAnimation = animation_Special;
                        spineSlots[0].AddAnimation(animation_Idle);
                    }
                }
                mutex.ReleaseMutex();
            }
        }

        protected override void Click(SFML.Window.Mouse.Button button)
        {
            base.Click(button);
            mutex.WaitOne();
            if (spineSlots[0] is not null && spineSlots[0].CurrentAnimation != animation_Interact)
            {
                spineSlots[0].CurrentAnimation = animation_Interact;
                spineSlots[0].AddAnimation(animation_Idle);
            }
            mutex.ReleaseMutex();
        }
    }
}
