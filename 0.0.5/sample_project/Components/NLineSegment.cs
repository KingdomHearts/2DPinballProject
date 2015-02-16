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
        private float movementL = 0;
        private float movementR = 0;
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


            this.start.Sub(this.end);
            this.start.Add(new Vec2(13 * direction, 0));
            this.start.Add(this.end);

            this.end.Sub(this.start);
            this.end.Add(new Vec2(13 * direction, 0));
            this.end.Add(this.start);

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
	}
}

