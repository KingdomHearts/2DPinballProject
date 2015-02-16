using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniLua;
using System.Drawing;

namespace GXPEngine
{
    class LevelController : Canvas
    {
        #region lua global variable
        ILuaState _lua;
        private List<NLineSegment> _levelLines;
        private List<NLineSegment> _lines;
        private List<Bumbers> _circles;
        private List<Square> _squaresL;
        private List<Triangle> _triangleL;
        private int _timer = 0;
        #endregion

        #region Ball handle and ball
        private Ball _ballP1;
        private Ball _ballP2;

        private MouseHandler _ballHandlerP1 = null;
        private MouseHandler _ballHandlerP2 = null;
        private float _ballDownTime = 0;
        private Vec2 _ballDownPositionP1 = null;
        private Vec2 _ballDownPositionP2 = null;
        //linecaps
        private List<Ball> _lineCaps;
        #endregion

        #region gravity
        private bool _toggleGravity = true;
        private bool _ballMoves = false;
        private Vec2 _gravity = new Vec2(0, 0.2f);
        #endregion gravity

        #region
        private List<NLineSegment> _paddlesP1;
        private List<NLineSegment> _paddlesP2;
        private const float dir = 0.2f;
        #endregion
        Prototype p;

        public LevelController(int gWidth, int gHeigh) : base(gWidth,gHeigh)
        {
            //Lists
            _lineCaps = new List<Ball>();
            _paddlesP1 = new List<NLineSegment>();
            _paddlesP2 = new List<NLineSegment>();

            LuaUpdate();
            _ballP1 = new Ball(10, new Vec2(50, gHeigh - 100), null, Color.Green);
            _ballHandlerP1 = new MouseHandler(_ballP1);
            _ballHandlerP1.OnMouseDownOnTarget += onBallMouseDownP1;

            //ChildAdds
            AddChild(_ballP1);
            PaddlesP1(new Vec2(200, 415), new Vec2(275, 415), 0xff00ffff);
            //PaddlesP2(new Vec2(350, 500), new Vec2(450, 500), 0xff00ffff);
        }

        #region Paddles
        private void PaddlesP1(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            NLineSegment paddle = new NLineSegment(start, end, color, 4);
            AddChild(paddle);
            _paddlesP1.Add(paddle);
        }

        private void PaddleL1(float dir)
        {
            foreach (NLineSegment line in _paddlesP1)
            {
                line.Movement(dir);
            }
        }
        private void PaddleCapsP1()
        {
        }
        private void PaddlesP2(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            NLineSegment paddle = new NLineSegment(start, end, color, 4);
            AddChild(paddle);
            _paddlesP1.Add(paddle);
        }

        private void PaddleL2(float dir)
        {
            foreach (NLineSegment line in _paddlesP2)
            {
                line.Movement(dir);
            }
        }
        private void PaddleCapsP2()
        {
        }
        #endregion
        #region Ball mouse Down/Move/Up
        //player 1
        private void onBallMouseDownP1(GameObject target, MouseEventType type)
        {
            _ballHandlerP1.OnMouseMove += onBallMouseMoveP1;
            _ballHandlerP1.OnMouseUp += onBallMouseUpP1;

            _ballDownTime = Time.now;
            _ballDownPositionP1 = _ballP1.position.Clone();
        }

        private void onBallMouseMoveP1(GameObject target, MouseEventType type)
        {
            Vec2 MousePosition1 = new Vec2(Input.mouseX, Input.mouseY);
            Vec2 delta = MousePosition1.Clone().Sub(_ballDownPositionP1);
            Vec2 deltaClone = delta.Clone();
            deltaClone.Normalize().Scale((float)Math.Sqrt((double)delta.Length())).Scale(8);

        }
        private void onBallMouseUpP1(GameObject target, MouseEventType type)
        {
            Console.WriteLine(target + "mouse up, time taken:" + (Time.now - _ballDownTime));
            _ballHandlerP1.OnMouseUp -= onBallMouseUpP1;
            _ballHandlerP1.OnMouseMove -= onBallMouseMoveP1;
            _ballP1.velocity = _ballDownPositionP1.Sub(_ballP1.position).Scale(1 / 10.0f);
            //gravityP1 = true;
            _ballMoves = true;
        }
        #endregion

        #region gravity and moving
        void ApplyGravity()
        {
            if (Input.GetKeyDown(Key.G))
            {
                if (_toggleGravity == true)
                {
                    _toggleGravity = false;
                }
                else
                {
                    _toggleGravity = true;
                }
            }
        }
        void Moving()
        {
            if (_ballMoves == true)
            {
                if (_toggleGravity == true)
                {
                    _ballP1.acceleration.Add(_gravity.Clone());
                }
                else
                {
                    _ballP1.acceleration = Vec2.zero;
                }
            }

            if (_ballP1.velocity.x > 8)
            {
                _ballP1.velocity.x = 8;
            }
            if (_ballP1.velocity.x < -8)
            {
                _ballP1.velocity.x = -8;
            }
            if (_ballP1.velocity.y > 8)
            {
                _ballP1.velocity.y = 8;
            }
            if (_ballP1.velocity.y < -8)
            {
                _ballP1.velocity.y = -8;
            }
        }
        #endregion

        #region Lua Add Object
        private void AddLineLevel(Vec2 start, Vec2 end, uint color = 0xff00ff00)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _levelLines.Add(line);
        }

        void AddLineCaps(Vec2 start, Vec2 end)
        {
            Ball startCap = new Ball(1, start);
            Ball endCap = new Ball(1, end);
            _lineCaps.Add(startCap);
            _lineCaps.Add(endCap);
            AddChild(startCap);
            AddChild(endCap);
        }

        int AddTriangle(ILuaState lua)
        {
            float point1X = lua.ToInteger(1);
            float point1Y = lua.ToInteger(2);
            float point2X = lua.ToInteger(3);
            float point2Y = lua.ToInteger(4);
            float point3X = lua.ToInteger(5);
            float point3Y = lua.ToInteger(6);
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
            AddLineCaps(new Vec2(point1X, point1Y), new Vec2(point2X, point2Y));
            AddLineCaps(new Vec2(point2X, point2Y), new Vec2(point3X, point3Y));
            AddLineCaps(new Vec2(point1X, point1Y), new Vec2(point3X, point3Y));

            AddChild(triangle);
            _triangleL.Add(triangle);
            return 0;
        }

        int AddSquare(ILuaState lua)
        {
            int radius = lua.ToInteger(1);
            float positionX = lua.ToInteger(2);
            float positionY = lua.ToInteger(3);
            int rotation = lua.ToInteger(4);
            string sbounciness = lua.ToString(5);
            float bounciness = (float)Convert.ToDecimal(sbounciness);

            Square sqaure = new Square(radius, positionX * 2, positionY * 2, rotation, bounciness);

            AddLineCaps(sqaure.rightBottom, sqaure.leftBottom);
            AddLineCaps(sqaure.rightBottom, sqaure.rightTop);
            AddLineCaps(sqaure.rightTop, sqaure.leftTop);
            AddLineCaps(sqaure.leftTop, sqaure.leftBottom);

            AddChild(sqaure);
            _squaresL.Add(sqaure);

            #region old junk
            /*
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
             */
            #endregion
            return 0;
        }

        int AddCircle(ILuaState lua)
        {
            int radius = lua.ToInteger(1);
            float positionX = lua.ToInteger(2);
            float positionY = lua.ToInteger(3);
            string sbounciness = lua.ToString(4);
            float bounciness = (float)Convert.ToDecimal(sbounciness);
            Bumbers circle = new Bumbers(radius, new Vec2(positionX * 2, positionY * 2));
            AddChild(circle);
            _circles.Add(circle);
            return 0;
        }

        int AddLine(ILuaState lua)
        {
            float startX = lua.ToInteger(1);
            float startY = lua.ToInteger(2);
            float endX = lua.ToInteger(3);
            float endY = lua.ToInteger(4);
            string sbounciness = lua.ToString(5);
            float bounciness = (float)Convert.ToDecimal(sbounciness);
            NLineSegment line = new NLineSegment(new Vec2((startX * 2) - 10, (startY * 2) - 10), new Vec2((endX * 2) - 10, (endY * 2) - 10), 0xff00ff00, 4);
            AddLineCaps(new Vec2((startX * 2) - 10, (startY * 2) - 10), new Vec2((endX * 2) - 10, (endY * 2) - 10));
            AddChild(line);
            _lines.Add(line);
            return 0;
        }
        #endregion

        #region Collisions
        void BumberCollision(Bumbers bumber)
        {
            Vec2 difference = bumber.position.Clone().Sub(_ballP1.position.Clone());
            float distance = difference.Length();

            if (distance < (_ballP1.radius + bumber.radius))
            {
                float overlap = (_ballP1.radius + bumber.radius) - distance;
                Vec2 normal = difference.Normalize();
                Vec2 bounce = normal.Clone().Scale(overlap);

                _ballP1.position.Sub(bounce);
                _ballP1.velocity = bumber.Reflect(_ballP1, normal);
            }
            _ballP1.y = _ballP1.position.y;
            _ballP1.x = _ballP1.position.x;
        }

        void SquareCollision(Square square, bool flipNormal)
        {
            List<NLineSegment> lines = new List<NLineSegment>() { square.lineLeftBottomRightBottom, square.lineRightBottomRightTop, square.lineRightTopLeftTop, square.lineLeftTopLeftBottom };

            foreach (NLineSegment line in lines)
            {
                Vec2 differentVector = _ballP1.position.Clone().Sub(line.start);
                Vec2 lineNormal = line.end.Clone().Sub(line.start).Normal();
                float ballDistance = differentVector.Dot(lineNormal);

                float lineLength = line.start.Clone().Sub(line.end).Length();
                Vec2 normalizedLine = line.end.Clone().Sub(line.start).Normalize();
                float AlongLine = differentVector.Dot(normalizedLine);

                Vec2 project = line.start.Clone().Add(normalizedLine.Scale(AlongLine));
                Vec2 difference = _ballP1.position.Clone().Sub(project);

                lineNormal.Scale(flipNormal ? -1 : 1);

                if (AlongLine >= 0 && AlongLine <= lineLength)
                {
                    if (difference.Length() < _ballP1.radius)
                    {
                        difference = difference.SetLength(_ballP1.radius);
                        _ballP1.position = project.Add(difference);
                        //Vec2 checkLineStart = line.start;
                        //Vec2 checkLineEnd = line.end;
                        _ballP1.velocity = square.Reflect(lineNormal, _ballP1);

                        //reflect on normal from line.
                    }
                }
                _ballP1.y = _ballP1.position.y;
                _ballP1.x = _ballP1.position.x;
            }

        }
        void TriangleCollision(Triangle triangle, bool flipNormal)
        {
            List<NLineSegment> lines = new List<NLineSegment>() { triangle.line12, triangle.line13, triangle.line23 };

            foreach (NLineSegment line in lines)
            {
                Vec2 differentVector = _ballP1.position.Clone().Sub(line.start);
                Vec2 lineNormal = line.end.Clone().Sub(line.start).Normal();
                float ballDistance = differentVector.Dot(lineNormal);

                float lineLength = line.start.Clone().Sub(line.end).Length();
                Vec2 normalizedLine = line.end.Clone().Sub(line.start).Normalize();
                float AlongLine = differentVector.Dot(normalizedLine);

                Vec2 project = line.start.Clone().Add(normalizedLine.Scale(AlongLine));
                Vec2 difference = _ballP1.position.Clone().Sub(project);

                lineNormal.Scale(flipNormal ? -1 : 1);

                if (AlongLine >= 0 && AlongLine <= lineLength)
                {
                    if (difference.Length() < _ballP1.radius)
                    {
                        _ballP1.position = project.Add(difference);
                        //_ballP1.velocity.Reflect(lineNormal);
                        Vec2 checkLineStart = line.start;
                        Vec2 checkLineEnd = line.end;
                        if (checkLineStart == triangle.line12.start && checkLineEnd == triangle.line12.end)
                        {
                            _ballP1.velocity = triangle.Reflect(lineNormal, _ballP1, "point12");
                        }
                        if (checkLineStart == triangle.line13.start && checkLineEnd == triangle.line13.end)
                        {
                            _ballP1.velocity = triangle.Reflect(lineNormal, _ballP1, "point13");
                        }
                        if (checkLineStart == triangle.line23.start && checkLineEnd == triangle.line23.end)
                        {
                            _ballP1.velocity = triangle.Reflect(lineNormal, _ballP1, "point23");
                        }
                        //reflect on normal from line.

                    }
                }
                _ballP1.y = _ballP1.position.y;
                _ballP1.x = _ballP1.position.x;
            }

        }
        void lineCollision(NLineSegment line, bool flipNormal)
        {

            Vec2 differentVector = _ballP1.position.Clone().Sub(line.start);
            Vec2 lineNormal = line.end.Clone().Sub(line.start).Normal();
            float ballDistance = differentVector.Dot(lineNormal);

            float lineLength = line.start.Clone().Sub(line.end).Length();
            Vec2 normalizedLine = line.end.Clone().Sub(line.start).Normalize();
            float AlongLine = differentVector.Dot(normalizedLine);

            Vec2 project = line.start.Clone().Add(normalizedLine.Scale(AlongLine));
            Vec2 difference = _ballP1.position.Clone().Sub(project);

            lineNormal.Scale(flipNormal ? -1 : 1);

            if (AlongLine >= 0 && AlongLine <= lineLength)
            {
                if (difference.Length() < _ballP1.radius)
                {
                    //_ballP1.position.x += (-ballDistance + _ballP1.radius) * lineNormal.x;
                    //_ballP1.position.y += (-ballDistance + _ballP1.radius) * lineNormal.y;
                    difference = difference.SetLength(_ballP1.radius);
                    _ballP1.position = project.Add(difference);
                    _ballP1.velocity.Reflect(lineNormal);
                    //reflect on normal from line.

                }
            }
            else
            {
                foreach (Ball cap in _lineCaps)
                {
                    CapCollision(cap);
                }
            }

            _ballP1.y = _ballP1.position.y;
            _ballP1.x = _ballP1.position.x;

        }
        void CapCollision(Ball lineCap)
        {
            Vec2 Difference = lineCap.position.Clone().Sub(_ballP1.position.Clone());
            float distance = Difference.Length();

            if (distance < (_ballP1.radius + lineCap.radius))
            {
                float separation = _ballP1.radius + lineCap.radius - distance;
                Vec2 normal = Difference.Normalize();
                Vec2 impulse = normal.Clone().Scale(separation);

                _ballP1.position.Sub(impulse);
                _ballP1.velocity.Reflect(normal);

            }
        }

        void paddleCollision(NLineSegment paddle)
        {
            Vec2 differentVector = _ballP1.position.Clone().Sub(paddle.start);
            Vec2 paddleNormal = paddle.end.Clone().Sub(paddle.start).Normal();
            float BallDistance = differentVector.Dot(paddleNormal);

            float paddleLength = paddle.start.Clone().Sub(paddle.end).Length();
            Vec2 normalizedPaddle = paddle.end.Clone().Sub(paddle.start).Normalize();
            float alongPaddle = differentVector.Dot(normalizedPaddle);

            Vec2 project = paddle.start.Clone().Add(normalizedPaddle.Scale(alongPaddle));
            Vec2 difference = _ballP1.position.Clone().Sub(project);

            if (alongPaddle >= 0 && alongPaddle <= paddleLength)
            {
                if (difference.Length() < _ballP1.radius)
                {
                    _ballP1.position = project.Add(difference);
                    _ballP1.velocity.Reflect(paddleNormal, 4.0f);

                }
            }
            _ballP1.x = _ballP1.position.x;
            _ballP1.y = _ballP1.position.y;
        }
        #endregion

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
            _ballP1.acceleration = Vec2.zero;
            Moving();
            ApplyGravity();
            if (Input.GetMouseButton(0))
            {
                _ballP1.position.Set(Input.mouseX, Input.mouseY);
            }
            _ballP1.Step();

            #region collisions update
            for (int i = 0; i < _lines.Count; i++)
            {
                lineCollision(_lines[i], true);
                lineCollision(_lines[i], false);
            }
            for (int i = 0; i < _squaresL.Count; i++)
            {
                SquareCollision(_squaresL[i], true);
                SquareCollision(_squaresL[i], false);
            }
            for (int i = 0; i < _triangleL.Count; i++)
            {
                TriangleCollision(_triangleL[i], true);
                TriangleCollision(_triangleL[i], false);
            }
            for (int i = 0; i < _circles.Count; i++)
            {
                BumberCollision(_circles[i]);
            } 
            for (int i = 0; i < _paddlesP1.Count; i++)
            {
                paddleCollision(_paddlesP1[i]);
            }
            #endregion

            #region GetKeys
            if (Input.GetKey(Key.A))
            {
                PaddleL1(-dir);
            }

            if (Input.GetKey(Key.D))
            {
                PaddleL1(dir);
            }
            #endregion

            #region Lua refresh in Update
            //if (Time.now > (_timer + 50))
            //{
            //    _lines = null; _circles = null; _squares = null; /*squareList = null*/; _triangleL = null; //triangleList = null;

            //    LevelController lc = new LevelController(game.width, game.height);
            //    lc.AddChild(lc);

            //    this.Destroy();
            //}
            //if (Time.now > (_timer + 10))
            //{
            //    game.Add(this);
            //    _timer = Time.now;
            //    LuaUpdate();
            //}
            #endregion

        }
        #region LuaUpdate
        void LuaUpdate()
        {

            _lines = new List<NLineSegment>();
            _circles = new List<Bumbers>();
            _squaresL = new List<Square>();
            //squareList = new List<List<NLineSegment>>();
            _triangleL = new List<Triangle>();
            //triangleList = new List<List<NLineSegment>>();

            _levelLines = new List<NLineSegment>();
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
        #endregion
    }
}
