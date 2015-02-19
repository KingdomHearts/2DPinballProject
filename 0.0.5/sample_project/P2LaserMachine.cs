using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class P2LaserMachine : AnimSprite 
    {
        float frame = 0.0f;
        int firstFrame = 0;
        int lastFrame = 10;

        public P2LaserMachine () : base ("laserP2.png", 10 ,1)
        {
            SetOrigin(width / 2, height / 2);
            SetScaleXY(0.5f, 0.5f);
         
        }
        void Update()
        {
            UpdateAnimation();
        }
        void SetAnimationRange(int first, int last)
        {
            firstFrame = first;
            lastFrame = last;
        }

        void UpdateAnimation()
        {
            frame = frame + 0.05f;
            if (frame >= lastFrame + 1)
                frame = firstFrame;
            if (frame < firstFrame)
                frame = lastFrame;
            SetFrame((int)frame);
        }



    }

}
