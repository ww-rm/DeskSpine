using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public sealed class ArknightsBattle : SpineRenderWindow
    {
        private string animation_Start = "";
        private string animation_Idle = "";

        public ArknightsBattle(uint slotCount) : base(slotCount) { }

        private void SetSkillAnimation(int index)
        {
            mutex.WaitOne();
            if (spineSlots[0] is not null)
            {
                var animationNames = spineSlots[0].AnimationNames;
                var skillBegin = $"Skill_{index}_Begin";
                var skillLoop = $"Skill_{index}_Loop";
                var skillEnd = $"Skill_{index}_End";

                if (animationNames.Contains(skillBegin))
                {
                    spineSlots[0].CurrentAnimation = skillBegin;
                    spineSlots[0].AddAnimation(skillLoop);
                    spineSlots[0].AddAnimation(skillEnd);
                }
                else if (animationNames.Contains(skillLoop))
                {
                    spineSlots[0].CurrentAnimation = skillLoop;
                    spineSlots[0].AddAnimation(skillEnd);
                }

                spineSlots[0].AddAnimation(animation_Idle);
            }
            mutex.ReleaseMutex();
        }

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

                animation_Start = animationNames.Contains("Start") ? "Start" : "";
                animation_Idle = animationNames.Contains("Idle") ? "Idle" : defaultName;

                if (string.IsNullOrEmpty(animation_Start))
                {
                    spineSlots[0].CurrentAnimation = animation_Idle;
                }
                else
                {
                    spineSlots[0].CurrentAnimation = animation_Start;
                    spineSlots[0].AddAnimation(animation_Idle);
                }
            }
            mutex.ReleaseMutex();
        }

        protected override void Click(SFML.Window.Mouse.Button button)
        {
            base.Click(button);

            switch (button)
            {
                case SFML.Window.Mouse.Button.Left:
                    SetSkillAnimation(1);
                    break;
                case SFML.Window.Mouse.Button.Middle:
                    SetSkillAnimation(3);
                    break;
            }
        }

        protected override void DoubleClick(SFML.Window.Mouse.Button button)
        {
            base.DoubleClick(button);

            switch (button)
            {
                case SFML.Window.Mouse.Button.Left:
                    SetSkillAnimation(2);
                    break;
            }
        }
    }
}
