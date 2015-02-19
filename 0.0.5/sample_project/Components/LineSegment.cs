using System;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine
{
	/// <summary>
	/// Implements an OpenGL line
	/// </summary>
	public class LineSegment : GameObject
	{
		public Vec2 start;
		public Vec2 end;

		public uint color = 0xffffffff;
		public uint lineWidth = 1;

		public bool useGlobalCoords = false;

        private float movementL = 0;
        private float movementR = 0;

		public LineSegment (float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
			: this (new Vec2 (pStartX, pStartY), new Vec2 (pEndX, pEndY), pColor, pLineWidth)
		{
		}

		public LineSegment (Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
		{
			start = pStart;
			end = pEnd;
			color = pColor;
			lineWidth = pLineWidth;
			useGlobalCoords = pGlobalCoords;
		}
	
		//------------------------------------------------------------------------------------------------------------------------
		//														RenderSelf()
		//------------------------------------------------------------------------------------------------------------------------
		override protected void RenderSelf(GLContext glContext) {
			if (game != null && start != null && end != null) {
				RenderLine (start, end, color, lineWidth);
			}
		}

		static public void RenderLine (Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false) {
			RenderLine (pStart.x, pStart.y, pEnd.x, pEnd.y, pColor, pLineWidth, pGlobalCoords);
		}

		static public void RenderLine (float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false) {
			if (pGlobalCoords) GL.LoadIdentity ();
			GL.Disable (GL.TEXTURE_2D);
			GL.LineWidth (pLineWidth);
			GL.Color4ub ((byte)((pColor >> 16) & 0xff), (byte)((pColor >> 8) & 0xff), (byte)((pColor) & 0xff),(byte)((pColor >> 24) & 0xff));
			float[] vertices = new float[] { pStartX, pStartY, pEndX, pEndY };
			GL.EnableClientState( GL.VERTEX_ARRAY );
			GL.VertexPointer( 2, GL.FLOAT, 0, vertices);
			GL.DrawArrays(GL.LINES, 0, 2);
			GL.DisableClientState(GL.VERTEX_ARRAY);
			GL.Enable(GL.TEXTURE_2D);
		}
        public float Movement(float direction, bool paddle1)
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

            if (paddle1)
            {
                this.start.Sub(this.end);
                this.start.Add(new Vec2(0, 13 * direction));
                this.start.Add(this.end);

                this.end.Sub(this.start);
                this.end.Add(new Vec2(0, 13 * direction));
                this.end.Add(this.start);
            }
            else
            {
                this.start.Sub(this.end);
                this.start.Add(new Vec2(13 * direction, 0));
                this.start.Add(this.end);

                this.end.Sub(this.start);
                this.end.Add(new Vec2(13 * direction, 0));
                this.end.Add(this.start);
            }
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

