using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class BatteryCharge : Canvas
    {
        private Vec2 _position;
        public BatteryCharge(Vec2 pPosition): base(20, 20)
        {
            _position = pPosition;
            x = _position.x;
            y = _position.y;
            draw();
        }

        public int Score()
        {
            return 0;
        }

        private void draw()
        {
            graphics.Clear(Color.Empty);
            graphics.FillEllipse(
                new SolidBrush(Color.Pink),
                0, 0, 2 * 10, 2 * 10
            );
        }
    }
}
