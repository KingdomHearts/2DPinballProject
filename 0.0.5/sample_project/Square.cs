using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Square : GameObject
    {
        private int _radius;
        private Vec2 _position;
        public int rotation;
        private float _bounciness;

        private List<string> _bouncelinesList;

        public NLineSegment lineLeftBottomRightBottom;
        public NLineSegment lineRightBottomRightTop;
        public NLineSegment lineRightTopLeftTop;
        public NLineSegment lineLeftTopLeftBottom;

        public Vec2 leftBottom;
        public Vec2 rightBottom;
        public Vec2 rightTop;
        public Vec2 leftTop;

        public Square(int pRadius, float pPositionX, float pPositionY, int pRotation, float pBounciness)
        {
            _radius = pRadius;
            _position = new Vec2(pPositionX, pPositionY);
            rotation = pRotation;
            _bounciness = pBounciness;

            Rotate(rotation);
        }

        public void Rotate(int rotation)
        {
            float left = _position.x + _radius;
            float right = _position.x - _radius;
            float top = _position.y + _radius;
            float bottom = _position.y - _radius;

            if (rotation == 45 || rotation == 135 || rotation == 225 || rotation == 315)
            {
                leftBottom = new Vec2(left, _position.y);
                rightBottom = new Vec2(_position.x, bottom);
                rightTop = new Vec2(right, _position.y);
                leftTop = new Vec2(_position.x, top);
            }
            else
            {
                leftBottom = new Vec2(left, bottom);
                rightBottom = new Vec2(right, bottom);
                rightTop = new Vec2(right, top);
                leftTop = new Vec2(left, top);
            }
            #region rotation
            if (rotation == 45 || rotation == 90)
            {
                lineLeftBottomRightBottom = new NLineSegment(leftBottom, rightBottom, 0xff00ff00);
            }
            else
            {
                lineLeftBottomRightBottom = new NLineSegment(leftBottom, rightBottom);
            }

            if (rotation == 135 || rotation == 180)
            {
                lineRightBottomRightTop = new NLineSegment(rightBottom, rightTop, 0xff00ff00);
            }
            else
            {
                lineRightBottomRightTop = new NLineSegment(rightBottom, rightTop);
            }

            if (rotation == 225 || rotation == 270)
            {
                if (rotation == 225)
                {
                    LineSegment line = new LineSegment(new Vec2(left, top), new Vec2(right, bottom));
                    //LineSegment line1 = new LineSegment(new Vec2(left, bottom), new Vec2(right, top));
                    AddChild(line);
                    //AddChild(line1);
                }
                lineRightTopLeftTop = new NLineSegment(rightTop, leftTop, 0xff00ff00);
            }
            else
            {
                lineRightTopLeftTop = new NLineSegment(rightTop, leftTop);
            }

            if (rotation == 315 || rotation == 360)
            {
                if (rotation == 315)
                {
                    LineSegment line = new LineSegment(new Vec2(left, bottom), new Vec2(right, top));
                    AddChild(line);
                }
                lineLeftTopLeftBottom = new NLineSegment(leftTop, leftBottom, 0xff00ff00);
            }
            else
            {
                lineLeftTopLeftBottom = new NLineSegment(leftTop, leftBottom);
            }
            #endregion

            AddChild(lineLeftBottomRightBottom);
            AddChild(lineRightBottomRightTop);
            AddChild(lineRightTopLeftTop);
            AddChild(lineLeftTopLeftBottom);

        }

        public Vec2 Reflect(Vec2 normal, Ball ball)
        {
            return ball.velocity.Reflect(normal, _bounciness);
        }
        public Vec2 ReflectMirror(Vec2 normal, string mirrorLine, LineSegment line = null)
        {
            return null; 
        }

        //public void MirrorCollision(Ball ball)
        //{
        //    if (ball.HitTest()
        //    {
                
        //    }
        //}
    }
}
