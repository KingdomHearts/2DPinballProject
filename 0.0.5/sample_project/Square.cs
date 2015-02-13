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
        private int _rotation;
        private float _bounciness;

        private List<string> _bouncelinesList;

        public NLineSegment lineLeftBottomRightBottom;
        public NLineSegment lineRightBottomRightTop;
        public NLineSegment lineRightTopLeftTop;
        public NLineSegment lineLeftTopLeftBottom;

        public Square(int pRadius, int pPositionX, int pPositionY, int pRotation, float pBounciness)
        {
            _radius = pRadius;
            _position = new Vec2(pPositionX, pPositionY);
            _rotation = pRotation;
            _bounciness = pBounciness;

            int left = pPositionX + _radius;
            int right = pPositionX - _radius;
            int top = pPositionY + _radius;
            int bottom = pPositionY - _radius;

            Vec2 leftBottom;
            Vec2 rightBottom;
            Vec2 rightTop;
            Vec2 leftTop;
            if (_rotation == 45 || _rotation == 135 || _rotation == 225 || _rotation == 315)
            {
                leftBottom = new Vec2(left, pPositionY);
                rightBottom = new Vec2(pPositionX, bottom);
                rightTop = new Vec2(right, pPositionY);
                leftTop = new Vec2(pPositionX, top);
            }
            else
            {
                leftBottom = new Vec2(left, bottom);
                rightBottom = new Vec2(right, bottom);
                rightTop = new Vec2(right, top);
                leftTop = new Vec2(left, top);
            }
            #region rotation
            if (_rotation == 45 || _rotation == 90)
            {
                lineLeftBottomRightBottom = new NLineSegment(leftBottom, rightBottom, 0xff00ff00);
            }
            else
            {
                lineLeftBottomRightBottom = new NLineSegment(leftBottom, rightBottom);
            }

            if (_rotation == 135 || _rotation == 180)
            {
                lineRightBottomRightTop = new NLineSegment(rightBottom, rightTop, 0xff00ff00);
            }
            else
            {
                lineRightBottomRightTop = new NLineSegment(rightBottom, rightTop);
            }

            if (_rotation == 225 || _rotation == 270)
            {
                lineRightTopLeftTop = new NLineSegment(rightTop, leftTop, 0xff00ff00);
            }
            else
            {
                lineRightTopLeftTop = new NLineSegment(rightTop, leftTop);
            }

            if (_rotation == 315 || _rotation == 360)
            {
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
    }
}
