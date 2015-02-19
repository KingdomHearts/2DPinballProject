using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class BatteryCharge : Sprite
    {
        
        public BatteryCharge(): base("battery pack.png")
        {
            SetOrigin(width / 2, height / 2);
            SetScaleXY(0.1f, 0.1f);
            
        }

        public int Score()
        {
            return 0;
        }

        }
    }

