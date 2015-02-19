using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class Laser : Canvas
    {
        private Ball _laserball;
        private LineSegment _laserline;
        private Vec2 _position;
        private Vec2 _velocity; 

        public Laser(Vec2 pPosition, Vec2 pVelocity) : base(10,10)
        {
            _velocity = pVelocity;
            _position = pPosition;
            draw();
            Step();
        }

        public void FireLaser(bool isPlayer1)
        {
            if (isPlayer1)
            {
                //sprite player 1 laser.
                _laserball = new Ball(1, _position, _velocity);
                _laserline = new LineSegment(_position, new Vec2(_position.x, _position.y - 10),0xffffff,4);
                AddChild(_laserball);
                AddChild(_laserline);
            }
            else
            {
                //sprite player 2 laser
            }
        }

        private void draw()
        {
            graphics.Clear(Color.Empty);
            graphics.FillEllipse(
                new SolidBrush(Color.HotPink),
                0, 0, 2 * 1, 2 * 1
            );
        }

        public void Step()
        {
            _position.Add(_velocity);

            x = _position.x;
            y = _position.y;
        }
    }
}
