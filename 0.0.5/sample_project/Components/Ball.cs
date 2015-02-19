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
        private float movementL;
        private float movementR;
        private float _bounciness;

		public Ball (int pRadius, Vec2 pPosition = null, Vec2 pVelocity = null, Color? pColor = null, float pBounciness = 0.5f):base (pRadius*2, pRadius*2)
		{
			radius = pRadius;
			SetOrigin (radius, radius);
            pBounciness = _bounciness;

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

        public float Movement(float direction)
        {

            if (direction < 0 || direction > 0)
            {
                movementL = movementL + direction;

                if (movementL < -10)
                {
                    movementL = -10;
                    return -10;
                }
            }
            else
            {
                movementR = movementR + direction;

                if (movementR > 10)
                {
                    movementR = 10;
                    return 10;
                }
            }

            //this.x.Add(new Vec2(13 * direction, 0));
            this.x = this.x + (13 * direction);
            return direction;
        }

        public void MoveBack(float direction)
        {

            //if (rotateL < 0)
            //{
            //    rotateL = rotateL + direction;
            //    if (rotateL > 0)
            //    {
            //        rotateL = 0;
            //        return;
            //    }

            //    this.start.Sub(this.end);
            //    this.start.RotateDegrees(direction * 90);
            //    this.start.Add(this.end);
            //}
            //else if (rotateR > 0)
            //{
            //    rotateR = rotateR + direction;
            //    if (rotateR < 0)
            //    {
            //        rotateR = 0;
            //        return;
            //    }

            //    this.start.Sub(this.end);
            //    this.start.RotateDegrees(direction * 90);
            //    this.start.Add(this.end);
            //}
        }
        public Vec2 Reflect(Ball ball, Vec2 normal, float bounciness)
        {
            return ball.velocity.Reflect(normal, bounciness);
        }
	}
}

