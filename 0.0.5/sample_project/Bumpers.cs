using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class Bumpers : Canvas
    {

        private Canvas _canvas;
        private Vec2 _position;
        private float _bounciness;
        public readonly int radius;
        private Color _ballColor;
        float frame = 0.0f;
        int firstFrame = 0;
        int lastFrame = 1;
        AnimSprite sprite;
        AnimSprite Gtower;
        AnimSprite Btower;
        bool _hit = false;
        
        List<AnimSprite> bumpers = new List<AnimSprite>();
        int score = 0;
       

        public Bumpers(int pRadius, Vec2 pPosition, Color? pColor = null, float pBounciness = 1.0f, int tower1 = 0) : base(pRadius*2,pRadius*2) 
        {
            if (pRadius == 13)
            {
                sprite = new AnimSprite("bumper.png", 2, 1);
                AddChild(sprite);

                sprite.SetOrigin(width / 2, height / 2);
                sprite.SetXY(-27, -40);
                sprite.SetScaleXY(0.5f, 0.5f);
            }
            

            if (pRadius == 15)
            {
                if (tower1 == 1)
                {
                    Gtower = new AnimSprite("Green Tower.png", 2, 1);
                    Gtower.SetOrigin(width / 2, height / 2);
                    Gtower.SetXY(-55, -100);
                    Gtower.SetScaleXY(0.5f, 0.5f);
                    AddChild(Gtower);
                }

                else 
                {
                    Btower = new AnimSprite("Blue Tower.png", 2, 1);

                    Btower.SetOrigin(width / 2, height / 2);
                    Btower.SetXY(-55, -100);
                    Btower.SetScaleXY(0.5f, 0.5f);
                    AddChild(Btower);
                
                }
      
                
            }

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
        void Update()
        {
            if (sprite != null)
            {
                UpdateAnimation();
                Hit(_hit);
            }

            if (Gtower != null)
            { 
                UpdateAnimation2();
            }

            if (Btower != null)
            {
                UpdateAnimation3();
            }

         

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

        

        void SetAnimationRange(int first, int last)
        {
            firstFrame = first;
            lastFrame = last;
        }

        void UpdateAnimation()
        {
            frame = frame + 0.01f;
            if (frame >= lastFrame + 1)
                frame = firstFrame;
            if (frame < firstFrame)
                frame = lastFrame;
            sprite.SetFrame((int)frame);

           
        }
        void UpdateAnimation2()
        {
            frame = frame + 0.1f;
            if (frame >= lastFrame + 1)
                frame = firstFrame;
            if (frame < firstFrame)
                frame = lastFrame;
            Gtower.SetFrame((int)frame);


        }

        void UpdateAnimation3()
        {
            frame = frame + 0.1f;
            if (frame >= lastFrame + 1)
                frame = firstFrame;
            if (frame < firstFrame)
                frame = lastFrame;
            Btower.SetFrame((int)frame);

        }
        public void Hit(bool hit)
        {
            _hit = hit;
           
            if (hit == true)
            {
                SetAnimationRange(0, 1);
                SoundManager.PlaySound(SoundManager.SoundEffect.bumper, 0.1f);
               
            }
            if (hit == false)
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
