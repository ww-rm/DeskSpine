using SFML.Window;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public class AzurLaneDynamic : SpineWindow
    {
        public AzurLaneDynamic(uint slotCount) : base(slotCount) { }

        protected override void Trigger_SpineLoaded(int index)
        {
            base.Trigger_SpineLoaded(index);

            mutex.WaitOne();
            foreach (var sp in spineSlots) { if (sp is not null) sp.CurrentAnimation = "normal"; }
            mutex.ReleaseMutex();
        }

        protected override void Trigger_MouseButtonClick(MouseButtonEventArgs e)
        {
            base.Trigger_MouseButtonClick(e);
            mutex.WaitOne();
            foreach (var sp in spineSlots) { if (sp is not null) { sp.CurrentAnimation = "click"; sp.AddAnimation("normal"); } }
            mutex.ReleaseMutex();
        }
    }
}
