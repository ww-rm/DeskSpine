using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public sealed class AzurLaneDynamic : SpineWindow
    {
        public AzurLaneDynamic(uint slotCount) : base(slotCount) { }

        protected override void SpineLoaded(int index)
        {
            base.SpineLoaded(index);

            mutex.WaitOne();
            foreach (var sp in spineSlots) { if (sp is not null) sp.CurrentAnimation = "normal"; }
            mutex.ReleaseMutex();
        }

        protected override void Click(SFML.Window.Mouse.Button button)
        {
            base.Click(button);
            mutex.WaitOne();
            foreach (var sp in spineSlots) { if (sp is not null) { sp.CurrentAnimation = "click"; sp.AddAnimation("normal"); } }
            mutex.ReleaseMutex();
        }
    }
}
