using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public class AzurLaneDynamic : SpineWindow
    {
        protected override void Trigger_SpineLoaded(uint index)
        {
            base.Trigger_SpineLoaded(index);

            mutex.WaitOne();
            foreach (var sp in spineSlots) { if (sp is not null) sp.CurrentAnimation = "normal"; }
            mutex.ReleaseMutex();
        }
    }
}
