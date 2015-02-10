using System;
using System.Drawing;

namespace GXPEngine
{
	public class Ball : Canvas
	{
        private Vec2 _position;
        private Vec2 _velocity;
        private Vec2 _acceleration = Vec2.zero;
		public readonly int radius;
		private Color _ballColor;

		public Ball (int pRadius, Vec2 pPosition = null, Vec2 pVelocity = null, Color? pColor = null):base (pRadius*2, pRadius*2)
		{
			radius = pRadius;
			SetOrigin (radius, radius);

			position = pPosition ?? Vec2.zero;
			velocity = pVelocity ?? Vec2.zero;
			_ballColor = pColor ?? Color.Blue;

			draw ();
			Step ();
		}

		private void draw() {
			graphics.Clear (Color.Empty);
			graphics.FillEllipse (
				new SolidBrush (_ballColor),
				0, 0, 2 * radius, 2 * radius
			);
		}

        public Vec2 acceleration
        {
            set
            {
                _acceleration = value ?? Vec2.zero;
            }
            get
            {
                return _acceleration;
            }
        }

        public Vec2 velocity
        {
            set
            {
                _velocity = value ?? Vec2.zero;
            }
            get
            {
                return _velocity;
            }
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

		public void Step() 
        {
            //if (position == null || velocity == null)
            //    return;

            _velocity.Add(_acceleration);
            _position.Add(_velocity);
            //if (!skipVelocity) position.Add (velocity);

			x = position.x;
			y = position.y;
		}

		public Color ballColor {
			get {
				return _ballColor;
			}

			set {
				_ballColor = value;
				draw ();
			}
		}

	}
}

