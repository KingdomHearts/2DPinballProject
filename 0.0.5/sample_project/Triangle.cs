using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Triangle : GameObject
    {
        private Vec2 _point1;
        private Vec2 _point2;
        private Vec2 _point3;
        private float _bounciness;

        private List<string> _bouncelinesList;

        public NLineSegment line12;
        public NLineSegment line13;
        public NLineSegment line23;

        public Triangle(Vec2 pPosition1, Vec2 pPosition2, Vec2 pPosition3, float pBounciness = 1.0f, List<string> pBouncelinesList = null)
        {
            _point1 = pPosition1;
            _point2 = pPosition2;
            _point3 = pPosition3;
            _bounciness = pBounciness;

            line12 = new NLineSegment(_point1, _point2);
            line13 = new NLineSegment(_point1, _point3);
            line23 = new NLineSegment(_point2, _point3);

            _bouncelinesList = pBouncelinesList;

            AddChild(line12);
            AddChild(line13);
            AddChild(line23);
            
        }

        public Vec2 Reflect(Vec2 normal, Ball ball, string pointLine)
        {
            int count = 0;
            foreach (string line in _bouncelinesList)
            {
                count++;
                if (pointLine == line)
                {
                    return ball.velocity.Reflect(normal, _bounciness);
                }
                else
                {
                    if (count == _bouncelinesList.Count)
                    {
                        return ball.velocity.Reflect(normal, 1);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return null;
        }

        public int Score()
        {
            return 0;
        }
    }
}
