using System;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine
{
	/// <summary>
	/// Implements a line with normal representation
	/// </summary>
	public class NLineSegment : LineSegment
    {
        private float rotateL = 0;
        private float rotateR = 0;
		private Arrow _normal;
	
		public NLineSegment (float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
			: this (new Vec2 (pStartX, pStartY), new Vec2 (pEndX, pEndY), pColor, pLineWidth)
		{
		}

		public NLineSegment (Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
			: base (pStart, pEnd, pColor, pLineWidth, pGlobalCoords)
		{
			_normal = new Arrow (null, null, 40, 0xffff0000, 1);
			AddChild (_normal);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														RenderSelf()
		//------------------------------------------------------------------------------------------------------------------------
		override protected void RenderSelf(GLContext glContext) {
			if (game != null && start != null && end != null) {
				recalc ();
				RenderLine (start, end, color, lineWidth);
			}
		}

		private void recalc() {
			_normal.startPoint = start.Clone ().Add (end).Scale (0.5f);
			_normal.vector = end.Clone ().Sub (start).Normal ();
		}

        public float Rotate(float direction)
        {

            if (direction < 0)
            {
                rotateL = rotateL + direction;

                if (rotateL < -1)
                {
                    rotateL = -1;
                    return -1;
                }
            }
            else
            {
                rotateR = rotateR + direction;

                if (rotateR > 1)
                {
                    rotateR = 1;
                    return 1;
                }
            }
            

            this.start.Sub(this.end);
            this.start.RotateDegrees(direction * 90);
            this.start.Add(this.end);

            return direction * 90;
        }

        public void RotateBack(float direction)
        {

            if (rotateL < 0)
            {
                rotateL = rotateL + direction;
                if (rotateL > 0)
                {
                    rotateL = 0;
                    return;
                }

                this.start.Sub(this.end);
                this.start.RotateDegrees(direction * 90);
                this.start.Add(this.end);
            }
            else if (rotateR > 0)
            {
                rotateR = rotateR + direction;
                if (rotateR < 0)
                {
                    rotateR = 0;
                    return;
                }

                this.start.Sub(this.end);
                this.start.RotateDegrees(direction * 90);
                this.start.Add(this.end);
            }
        }
	}
}

