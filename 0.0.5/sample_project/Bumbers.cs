using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class Bumbers : Canvas
    {
        private Canvas _canvas;
        private Vec2 _position;
        private float _bounciness;
        public readonly int radius;
        private Color _ballColor;

        public Bumbers(int pRadius, Vec2 pPosition, Color? pColor = null, float pBounciness = 1.0f) : base(pRadius*2,pRadius*2)
        {
            _canvas = new Canvas(game.width, game.height);
            _bounciness = pBounciness;
            radius = pRadius;
            SetOrigin(radius, radius);

            _ballColor = pColor ?? Color.Blue;
            position = pPosition ?? Vec2.zero;

            x = position.x;
            y = position.y;
            draw();
        }

        private void draw()
        {
            graphics.Clear(Color.Empty);
            graphics.FillEllipse(
                new SolidBrush(_ballColor),
                0, 0, 2 * radius, 2 * radius
            );
        }

        public Vec2 position
        {
            set
            {
                _position = value ?? Vec2.zero;
            }
            get
            {
                return _position;
            }
        }

        public Color ballColor
        {
            get
            {
                return _ballColor;
            }

            set
            {
                _ballColor = value;
                draw();
            }
        }

        public Vec2 Reflect(Ball ball, Vec2 normal)
        {
            return ball.velocity.Reflect(normal, _bounciness);
        }
    }
}
