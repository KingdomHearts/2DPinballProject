using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniLua;

namespace GXPEngine
{
    class LevelController : Canvas
    {
        ILuaState _lua;

        List<NLineSegment> levelLines;

        List<NLineSegment> lines;
        List<Ball> circles;

        List<NLineSegment> squares;
        List<List<NLineSegment>> squareList;

        List<Triangle> triangleL;
        //List<List<NLineSegment>> triangleList;

        int timer = 0;

        public LevelController(int gWidth, int gHeigh) : base(gWidth,gHeigh)
        {

            Prototype p = new Prototype();
            AddChild(p);
        }

        private void AddLineLevel(Vec2 start, Vec2 end, uint color = 0xff00ff00)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            levelLines.Add(line);
        }

        int AddTriangle(ILuaState lua)
        {
            int point1X = lua.ToInteger(1);
            int point1Y = lua.ToInteger(2);
            int point2X = lua.ToInteger(3);
            int point2Y = lua.ToInteger(4);
            int point3X = lua.ToInteger(5);
            int point3Y = lua.ToInteger(6);
            string sbounciness = lua.ToString(7);
            float bounciness = (float)Convert.ToDecimal(sbounciness);
            string bounceline1 = lua.ToString(8);
            string bounceline2 = lua.ToString(9);
            string bounceline3 = lua.ToString(10);
            List<string> bouncelines = new List<string>();
            if (bounceline1 != null) { bouncelines.Add(bounceline1); };
            if (bounceline2 != null) { bouncelines.Add(bounceline2); };
            if (bounceline3 != null) { bouncelines.Add(bounceline3); };

            Triangle triangle = new Triangle(new Vec2(point1X, point1Y), new Vec2(point2X, point2Y), new Vec2(point3X,point3Y),bounciness, bouncelines);
            AddChild(triangle);
            triangleL.Add(triangle);
            return 0;
        }

        int AddSquare(ILuaState lua)
        {
            int radius = lua.ToInteger(1);
            int positionX = lua.ToInteger(2);
            int positionY = lua.ToInteger(3);
            int rotation = lua.ToInteger(4);
            string sbounciness = lua.ToString(5);
            float bounciness = (float)Convert.ToDecimal(sbounciness);

            int left = positionX + radius;
            int right = positionX - radius;
            int top = positionY + radius;
            int bottom = positionY - radius;

            Vec2 leftBottom;
            Vec2 rightBottom;
            Vec2 rightTop;
            Vec2 leftTop;
            if (rotation == 45 || rotation == 135 || rotation == 225 || rotation == 315)
            {
                leftBottom = new Vec2(left, positionY);
                rightBottom = new Vec2(positionX,bottom);
                rightTop = new Vec2(right,positionY);
                leftTop = new Vec2(positionX,top);
            }
            else
            {
                leftBottom = new Vec2(left,bottom);
                rightBottom = new Vec2(right,bottom);
                rightTop = new Vec2(right,top);
                leftTop= new Vec2(left,top);
            }
            NLineSegment lineLeftBottomRightBottom;
            NLineSegment lineRightBottomRightTop;
            NLineSegment lineRightTopLeftTop;
            NLineSegment lineLeftTopLeftBottom;
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
                lineRightTopLeftTop = new NLineSegment(rightTop, leftTop, 0xff00ff00);
            }
            else
            {
                lineRightTopLeftTop = new NLineSegment(rightTop, leftTop);
            }

            if (rotation == 315 || rotation == 360)
            {
                lineLeftTopLeftBottom = new NLineSegment(leftTop, leftBottom, 0xff00ff00);
            }
            else
            {
                lineLeftTopLeftBottom = new NLineSegment(leftTop, leftBottom);
            }

            AddChild(lineLeftBottomRightBottom);
            AddChild(lineRightBottomRightTop);
            AddChild(lineRightTopLeftTop);
            AddChild(lineLeftTopLeftBottom);
            squares.Add(lineLeftBottomRightBottom);
            squares.Add(lineRightBottomRightTop);
            squares.Add(lineRightTopLeftTop);
            squares.Add(lineLeftTopLeftBottom);
            squareList.Add(squares);
            return 0;
        }

        int AddCircle(ILuaState lua)
        {
            int radius = lua.ToInteger(1);
            int positionX = lua.ToInteger(2);
            int positionY = lua.ToInteger(3);
            string sbounciness = lua.ToString(4);
            float bounciness = (float)Convert.ToDecimal(sbounciness);
            Ball circle = new Ball(radius, new Vec2(positionX, positionY));
            AddChild(circle);
            circles.Add(circle);
            return 0;
        }

        int AddLine(ILuaState lua)
        {
            int startX = lua.ToInteger(1);
            int startY = lua.ToInteger(2);
            int endX = lua.ToInteger(3);
            int endY = lua.ToInteger(4);
            string sbounciness = lua.ToString(5);
            float bounciness = (float)Convert.ToDecimal(sbounciness);
            NLineSegment line = new NLineSegment(new Vec2(startX,startY), new Vec2(endX,endY), 0xff00ff00, 4);
            AddChild(line);
            lines.Add(line);
            return 0;
        }

        public void LevelState(/*enum of andere mogelijk om level te bepalen*/)
        {
        }

        private void LevelReader(/*enum of andere mogelijk om level te bepalen*/)
        {
            //Vanuit LevelState het bestand meesturen dat hierin wordt opgehaald.
        }

        private void LevelBuilder(/* array van XML */)
        {
            //De array wordt hierin uitgelezen en gebouwd. Vanuit deze wordt de bijpassende CS aangeroepen waar de rest van de events wordt uitgevoerd.
        }

        void Update()
        {

            //if (Time.now > (timer + 50))
            //{
            //    lines = null; circles = null; squares = null; squareList = null; triangleL = null; //triangleList = null;
            //    this.Destroy();
            //}
            //if (Time.now > (timer + 10))
            //{
            //    game.Add(this);
            //    timer = Time.now;
            //    LuaUpdate();
            //}
            
        }
        void LuaUpdate()
        {

            lines = new List<NLineSegment>();
            circles = new List<Ball>();
            squares = new List<NLineSegment>();
            squareList = new List<List<NLineSegment>>();
            triangleL = new List<Triangle>();
            //triangleList = new List<List<NLineSegment>>();

            levelLines = new List<NLineSegment>();
            //Level lines
            AddLineLevel(new Vec2(0, 0), new Vec2(width, 0));
            AddLineLevel(new Vec2(width, 0), new Vec2(width, height));
            AddLineLevel(new Vec2(width, height), new Vec2(0, height));
            AddLineLevel(new Vec2(0, height), new Vec2(0, 0));


            _lua = LuaAPI.NewState();
            _lua.L_OpenLibs();


            _lua.PushCSharpFunction(AddLine);
            _lua.SetGlobal("addline");
            _lua.PushCSharpFunction(AddCircle);
            _lua.SetGlobal("addcircle");
            _lua.PushCSharpFunction(AddSquare);
            _lua.SetGlobal("addsquare");
            _lua.PushCSharpFunction(AddTriangle);
            _lua.SetGlobal("addtriangle");

            var status = _lua.L_DoFile("level.lua");
            if (status != ThreadStatus.LUA_OK) { throw new Exception(_lua.ToString(-1)); }
        }
    }
}
