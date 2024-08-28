using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public class AzurLaneDynamic : SpineWindow
    {
        private string animation_T = "normal";
        private string animation_M = "normal";
        private string animation_B = "normal";

        protected override void Trigger_SpineLoaded(uint index)
        {
            base.Trigger_SpineLoaded(index);

            mutex.WaitOne();
            if (spineSlots[0] is not null) spineSlots[0].CurrentAnimation = animation_T;
            if (spineSlots[1] is not null) spineSlots[1].CurrentAnimation = animation_M;
            if (spineSlots[2] is not null) spineSlots[2].CurrentAnimation = animation_B;
            mutex.ReleaseMutex();
        }
    }
}
