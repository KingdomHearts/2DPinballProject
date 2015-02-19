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

        public Vec2 leftBottom;
        public Vec2 rightBottom;
        public Vec2 rightTop;
        public Vec2 leftTop;

        AnimSprite sprite;
        AnimSprite _mirror;
        float frame = 0.0f;
        int firstFrame = 0;
        int lastFrame = 10;
        bool _open = false;
        public Square(int pRadius, float pPositionX, float pPositionY, int pRotation, float pBounciness, int closet = 0)
        {           
            _radius = pRadius;
            _position = new Vec2(pPositionX, pPositionY);
            _rotation = pRotation;
            _bounciness = pBounciness;

            float left = pPositionX + _radius;
            float right = pPositionX - _radius;
            float top = pPositionY + _radius;
            float bottom = pPositionY - _radius;

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

            if (closet == 1)
            {
                sprite = new AnimSprite("closet.png", 4, 4);
                sprite.SetOrigin(sprite.width / 2, sprite.height / 2);
                sprite.SetScaleXY(0.6f, 0.6f);
                sprite.SetXY(pPositionX + 1.5, pPositionY - 30);
                AddChild(sprite);
            }
            else
            {
                _mirror = new AnimSprite("closet.png", 4, 4);
                _mirror.SetOrigin(_mirror.width / 2, _mirror.height / 2);
                _mirror.SetScaleXY(0.6f, 0.6f);
                _mirror.SetXY(pPositionX + 1.5, pPositionY - 30);
                AddChild(_mirror);

            }
        }

        void Update()
        {
            if (sprite != null)
            {
                UpdateAnimation();
                OpenDoor(_open);
            }
            if (_mirror != null)
            { 
            
            }
        }
        public Vec2 Reflect(Vec2 normal, Ball ball)
        {
            return ball.velocity.Reflect(normal, _bounciness);
        }
        public Vec2 ReflectMirror(Vec2 normal, string mirrorLine, LineSegment line = null)
        {
            return null; 
        }
        void UpdateAnimation()
        {
            frame = frame + 0.1f;
            if (frame >= lastFrame + 1)
                frame = firstFrame;
            if (frame < firstFrame)
                frame = lastFrame;
            sprite.SetFrame((int)frame);

        }

        public void SetAnimationRange(int first, int last)
        {
            firstFrame = first;
            lastFrame = last;
        }

        public void OpenDoor(bool open)
        {
            _open = open;
            if (open == true)
            {
                SetAnimationRange(0, 10);
                SoundManager.PlaySound(SoundManager.SoundEffect.bumper);
            }
            if (open == false)
            {
                if (frame > 0)
                {
                    
                }
                else if (frame != lastFrame)
                {
                    SetAnimationRange(0, 0);
                }
            }
        }
    }
}
