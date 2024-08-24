using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpineWindow
{
    public class AzurLaneSD : SpineWindow
    {
        public override string SpineVersion { get => "3.6.53"; }

        public AzurLaneSD(string skelPath, string? atlasPath = null) : base(skelPath, atlasPath)
        {
            //spine.X = 400;
            //spine.Y = 400;
            //spine.FlipX = true;
            
        }
    }
}
