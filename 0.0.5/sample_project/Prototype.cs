using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Prototype : GameObject
    {
        private List<NLineSegment> _lines;
        private Ball _ball;

        private List<Ball> _lineBalls;

        private Vec2 _previousPosition;
        public Prototype()
        {
        }
    }
}
