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
        private List<LineSegment> _levelLines;
        private List<LineSegment> _lines;
        private List<Bumbers> _circles;
        private List<Square> _squaresL;
        private List<Triangle> _triangleL;
        private int _timer = 0;
        #endregion

        #region Ball handle and ball
        private Ball _ballP1;
        private Ball _ballP2;

        private int _timerMirror = 0;

        private MouseHandler _ballHandlerP1 = null;
        private MouseHandler _ballHandlerP2 = null;
        private float _ballDownTimeP1 = 0;
        private float _ballDownTimeP2 = 0;
        private Vec2 _ballDownPositionP1 = null;
        private Vec2 _ballDownPositionP2 = null;
        //linecaps
        private List<Ball> _lineCaps;
        #endregion

        #region gravity
        private bool _toggleGravity = true;
        private bool _ballMovesP1 = false;
        private bool _ballMovesP2 = false;
        private Vec2 _gravity = new Vec2(0, 0.1f);
        #endregion gravity

        #region paddles
        private List<LineSegment> _paddlesP1;
        private List<LineSegment> _paddlesP2;
        private List<Ball> _paddlesP1Caps;
        private List<Ball> _paddlesP2Caps;
        private const float dir = 0.2f;
        #endregion

        #region laser
        private Laser _laser;
        private bool _isP1Fired = false;
        private bool _isP2Fired = false;
        #endregion

        #region launcher
        private Vec2 _launcherP1;
        private int _launchTimeP1 = 0;
        private float _launchPowerP1 = 0;
        private bool _isFiredP1 = true;
        private bool _isLaunchedP1 = false;
        private Sprite _blue;
        private Sprite _green;
        private Vec2 _launcherP2;
        private int _launchTimeP2 = 0;
        private float _launchPowerP2 = 0;
        private bool _isFiredP2 = true;
        private bool _isLaunchedP2 = false;

        private int _tubeEnd = 0;
        private List<LineSegment> _tubeLines;
        #endregion

        #region items
        private int _generatorTime =0;
        #endregion
        Prototype p;

        public LevelController(int gWidth, int gHeigh) : base(gWidth,gHeigh)
        {
            //Lists
            _lineCaps = new List<Ball>();
            _paddlesP1 = new List<LineSegment>();
            _paddlesP2 = new List<LineSegment>();
            _paddlesP1Caps = new List<Ball>();
            _paddlesP2Caps = new List<Ball>();
            _tubeLines = new List<LineSegment>();

            LuaUpdate();

            //ball
            _ballP1 = new Ball(9, new Vec2(0, gHeigh - 60), null, Color.Green);
            _ballHandlerP1 = new MouseHandler(_ballP1);
            _ballHandlerP1.OnMouseDownOnTarget += onBallMouseDownP1;

            _ballP2 = new Ball(9, new Vec2(gWidth - 75, gHeigh - 60), null, Color.Yellow);
            _ballHandlerP1 = new MouseHandler(_ballP2);
            _ballHandlerP1.OnMouseDownOnTarget += onBallMouseDownP1;

            //Launcher
            _green = new Sprite("pressureDisplayGreen.png");
            _green.x = 30;
            _green.y = game.height - 125;
            _green.height = _green.height/ 2 +20;
            _blue = new Sprite("pressureDisplayBlue.png");
            _blue.x = 825;
            _blue.y = game.height - 125;
            _blue.height = _blue.height / 2 + 20;

            //ChildAdds
            AddChild(_ballP1);
            AddChild(_ballP2);
            PaddlesP1(new Vec2(75, 200), new Vec2(75, 250), 0xff00ffff);
            PaddlesP1(new Vec2(150, 440), new Vec2(200, 440), 0xff00ffff);
            PaddlesP2(new Vec2(700, 440), new Vec2(750, 440), 0xff00ffff);
            PaddlesP2(new Vec2(825, 225), new Vec2(825, 275), 0xff00ffff);
            AddChild(_green);
            AddChild(_blue);

        }

        #region Paddles
        private void PaddlesP1(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            LineSegment paddle = new LineSegment(start, end, color, 4);
            //Ball paddleCapStart = new Ball(3, start);
            //Ball paddleCapEnd = new Ball(3, end);
            //Ball paddleCapStartE = new Ball(3, new Vec2(start.x + 10, start.y));
            //Ball paddleCapEndE = new Ball(3, new Vec2(end.x - 10, start.y));
            //_paddlesP1Caps.Add(paddleCapStart);
            //_paddlesP1Caps.Add(paddleCapEnd);
            //_paddlesP1Caps.Add(paddleCapStartE);
            //_paddlesP1Caps.Add(paddleCapEndE);
            //AddChild(paddleCapStart);
            //AddChild(paddleCapEnd);
            //AddChild(paddleCapStartE);
            //AddChild(paddleCapEndE);
            AddChild(paddle);
            _paddlesP1.Add(paddle);
        }

        private void PaddleL1(float dir)
        {
            int i =0;
            foreach (LineSegment line in _paddlesP1)
            {
                if (i == 0)
                {
                    line.Movement(-dir, true);
                    i++;
                }
                else
                {
                    line.Movement(dir, false);
                }
            }
            foreach (Ball cap in _paddlesP1Caps)
            {
                cap.Movement(dir);
            }
        }

        private void PaddleCapsP1(Vec2 start, Vec2 end)
        {
            Ball paddleCapStart = new Ball(1, start);
            Ball paddleCapEnd = new Ball(1, end);
            _paddlesP1Caps.Add(paddleCapStart);
            _paddlesP1Caps.Add(paddleCapEnd);
            AddChild(paddleCapStart);
            AddChild(paddleCapEnd);
        }

        private void PaddlesP2(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            LineSegment paddle = new LineSegment(start, end, color, 4);
            //Ball paddleCapStart = new Ball(3, start);
            //Ball paddleCapEnd = new Ball(3, end);
            //Ball paddleCapStartE = new Ball(3, new Vec2(start.x + 10, start.y));
            //Ball paddleCapEndE = new Ball(3, new Vec2(end.x - 10, start.y));
            //_paddlesP2Caps.Add(paddleCapStart);
            //_paddlesP2Caps.Add(paddleCapEnd);
            //_paddlesP2Caps.Add(paddleCapStartE);
            //_paddlesP2Caps.Add(paddleCapEndE);
            //AddChild(paddleCapStart);
            //AddChild(paddleCapEnd);
            //AddChild(paddleCapStartE);
            //AddChild(paddleCapEndE);
            AddChild(paddle);
            _paddlesP2.Add(paddle);
        }

        private void PaddleL2(float dir)
        {
            int i = 0;
            foreach (LineSegment line in _paddlesP2)
            {
                if (i == 0)
                {
                    line.Movement(dir, false);
                    i++;
                }
                else
                {
                    line.Movement(dir, true);
                }
            }
            foreach (Ball cap in _paddlesP1Caps)
            {
                cap.Movement(dir);
            }
        }
        private void PaddleCapsP2(Vec2 start, Vec2 end)
        {
            Ball paddleCapStart = new Ball(1, start);
            Ball paddleCapEnd = new Ball(1, end);
            _paddlesP2Caps.Add(paddleCapStart);
            _paddlesP2Caps.Add(paddleCapEnd);
            AddChild(paddleCapStart);
            AddChild(paddleCapEnd);
        }
        #endregion
        #region Ball mouse Down/Move/Up
        //player 1
        private void onBallMouseDownP1(GameObject target, MouseEventType type)
        {
            _ballHandlerP1.OnMouseMove += onBallMouseMoveP1;
            _ballHandlerP1.OnMouseUp += onBallMouseUpP1;

            _ballDownTimeP1 = Time.now;
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
            Console.WriteLine(target + "mouse up, time taken:" + (Time.now - _ballDownTimeP1));
            _ballHandlerP1.OnMouseUp -= onBallMouseUpP1;
            _ballHandlerP1.OnMouseMove -= onBallMouseMoveP1;
            _ballP1.velocity = _ballDownPositionP1.Sub(_ballP1.position).Scale(1 / 10.0f);
            //gravityP1 = true;
            //_ballMoves = true;
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
            if (_ballP1 != null)
            {
                if (_ballMovesP1 == true)
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
            }
            if (_ballP2 != null)
	        {

                if (_ballMovesP2 == true)
                {
                    if (_toggleGravity == true)
                    {
                        _ballP2.acceleration.Add(_gravity.Clone());
                    }
                    else
                    {
                        _ballP2.acceleration = Vec2.zero;
                    }
                }
            }
                if (_isLaunchedP1)
                {

                    if (_ballP1.velocity.x > 20)
                    {
                        _ballP1.velocity.x = 20;
                    }
                    if (_ballP1.velocity.x < -20)
                    {
                        _ballP1.velocity.x = -20;
                    }
                    if (_ballP1.velocity.y > 20)
                    {
                        _ballP1.velocity.y = 20;
                    }
                    if (_ballP1.velocity.y < -20)
                    {
                        _ballP1.velocity.y = -20;
                    }
                }
                else if (_isLaunchedP2)
                {

                    if (_ballP2.velocity.x > 20)
                    {
                        _ballP2.velocity.x = 20;
                    }
                    if (_ballP2.velocity.x < -20)
                    {
                        _ballP2.velocity.x = -20;
                    }
                    if (_ballP2.velocity.y > 20)
                    {
                        _ballP2.velocity.y = 20;
                    }
                    if (_ballP2.velocity.y < -20)
                    {
                        _ballP2.velocity.y = -20;
                    }
                }
                else
                {
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

                    if (_ballP2.velocity.x > 8)
                    {
                        _ballP2.velocity.x = 8;
                    }
                    if (_ballP2.velocity.x < -8)
                    {
                        _ballP2.velocity.x = -8;
                    }
                    if (_ballP2.velocity.y > 8)
                    {
                        _ballP2.velocity.y = 8;
                    }
                    if (_ballP2.velocity.y < -8)
                    {
                        _ballP2.velocity.y = -8;
                    }
                }
            }
        #endregion

        #region items
        void CreateRepairKit(int x, int y)
        {
            RepairKit kit = new RepairKit();
            AddChild(kit);
            kit.x = x;
            kit.y = y;

        }

        void CreateBatteryPack(int x, int y)
        {
            BatteryCharge battery = new BatteryCharge();
            AddChild(battery);
            battery.x = x;
            battery.y = y;
        }

        void Generator()
        {
            //CreateRepairKit(295, 485);
            //CreateBatteryPack(295, 485);
            int valueRandom = Utils.Random(0, 3);
            if (Time.now > (_generatorTime + 3000))
            {
                int value = valueRandom;
                _generatorTime = Time.now;
                switch (value)
                {
                    case 0:
                        CreateBatteryPack(295, 485);
                        break;

                    case 1:
                        CreateRepairKit(295, 485);
                        break;
                    case 2:
                        break;
                }
            }
        }
        #endregion
        #region Lua Add Object
        private void AddLineLevel(Vec2 start, Vec2 end, uint color = 0xff00ff00)
        {
            LineSegment line = new LineSegment(start, end, color, 4);
            AddChild(line);
            _levelLines.Add(line);
        }

        void AddLineCaps(Vec2 start, Vec2 end, float bounciness)
        {
            Ball startCap = new Ball(1, start,null,null,bounciness);
            Ball endCap = new Ball(1, end, null, null, bounciness);
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
            AddLineCaps(new Vec2(point1X, point1Y), new Vec2(point2X, point2Y),bounciness);
            AddLineCaps(new Vec2(point2X, point2Y), new Vec2(point3X, point3Y), bounciness);
            AddLineCaps(new Vec2(point1X, point1Y), new Vec2(point3X, point3Y), bounciness);

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

            //AddLineCaps(sqaure.rightBottom, sqaure.leftBottom, bounciness);
            //AddLineCaps(sqaure.rightBottom, sqaure.rightTop, bounciness);
            //AddLineCaps(sqaure.rightTop, sqaure.leftTop, bounciness);
            //AddLineCaps(sqaure.leftTop, sqaure.leftBottom, bounciness);

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
            LineSegment lineLeftBottomRightBottom;
            LineSegment lineRightBottomRightTop;
            LineSegment lineRightTopLeftTop;
            LineSegment lineLeftTopLeftBottom;
            if (rotation == 45 || rotation == 90)
            {
                lineLeftBottomRightBottom = new LineSegment(leftBottom, rightBottom, 0xff00ff00);
            }
            else
            {
                lineLeftBottomRightBottom = new LineSegment(leftBottom, rightBottom);
            }

            if (rotation == 135 || rotation == 180)
            {
                lineRightBottomRightTop = new LineSegment(rightBottom, rightTop, 0xff00ff00);
            }
            else
            {
                lineRightBottomRightTop = new LineSegment(rightBottom, rightTop);
            }

            if (rotation == 225 || rotation == 270)
            {
                lineRightTopLeftTop = new LineSegment(rightTop, leftTop, 0xff00ff00);
            }
            else
            {
                lineRightTopLeftTop = new LineSegment(rightTop, leftTop);
            }

            if (rotation == 315 || rotation == 360)
            {
                lineLeftTopLeftBottom = new LineSegment(leftTop, leftBottom, 0xff00ff00);
            }
            else
            {
                lineLeftTopLeftBottom = new LineSegment(leftTop, leftBottom);
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
            _tubeEnd = lua.ToInteger(6);
            LineSegment line = new LineSegment(new Vec2((startX * 2) - 10, (startY * 2) - 10), new Vec2((endX * 2) - 10, (endY * 2) - 10), 0xff00ff00, 4);
            AddLineCaps(new Vec2((startX * 2) - 10, (startY * 2) - 10), new Vec2((endX * 2) - 10, (endY * 2) - 10), bounciness);
            if (_tubeEnd == 1)
            {
                _tubeLines.Add(line);
            }
            else
            {
                AddChild(line);
                _lines.Add(line);
            }
            return 0;
        }
        #endregion

        #region Collisions
        void BumberCollision(Bumbers bumber, Ball ball)
        {
            Vec2 difference = bumber.position.Clone().Sub(ball.position.Clone());
            float distance = difference.Length();

            if (distance < (ball.radius + bumber.radius))
            {
                float overlap = (ball.radius + bumber.radius) - distance;
                Vec2 normal = difference.Normalize();
                Vec2 bounce = normal.Clone().Scale(overlap);

                ball.position.Sub(bounce);
                ball.velocity = bumber.Reflect(ball, normal);
            }
            ball.y = _ballP1.position.y;
            ball.x = _ballP1.position.x;
        }

        bool SquareCollision(Square square, bool flipNormal, Ball ball)
        {
            List<LineSegment> lines = new List<LineSegment>() { square.lineLeftBottomRightBottom, square.lineRightBottomRightTop, square.lineRightTopLeftTop, square.lineLeftTopLeftBottom };

            foreach (LineSegment line in lines)
            {
                Vec2 differentVector = ball.position.Clone().Sub(line.start);
                Vec2 lineNormal = line.end.Clone().Sub(line.start).Normal();
                float ballDistance = differentVector.Dot(lineNormal);

                float lineLength = line.start.Clone().Sub(line.end).Length();
                Vec2 normalizedLine = line.end.Clone().Sub(line.start).Normalize();
                float AlongLine = differentVector.Dot(normalizedLine);

                Vec2 project = line.start.Clone().Add(normalizedLine.Scale(AlongLine));
                Vec2 difference = ball.position.Clone().Sub(project);

                lineNormal.Scale(flipNormal ? -1 : 1);

                if (AlongLine >= 0 && AlongLine <= lineLength)
                {
                    if (difference.Length() < ball.radius)
                    {
                        difference = difference.SetLength(_ballP1.radius);
                        ball.position = project.Add(difference);
                        //Vec2 checkLineStart = line.start;
                        //Vec2 checkLineEnd = line.end;
                        ball.velocity = square.Reflect(lineNormal, ball);
                        return true;
                        //reflect on normal from line.
                    }
                    return false;
                }
                return false;
                ball.y = ball.position.y;
                ball.x = ball.position.x;
            }
            return false;
        }
        void TriangleCollision(Triangle triangle, bool flipNormal, Ball ball)
        {
            List<LineSegment> lines = new List<LineSegment>() { triangle.line12, triangle.line13, triangle.line23 };

            foreach (LineSegment line in lines)
            {
                Vec2 differentVector = ball.position.Clone().Sub(line.start);
                Vec2 lineNormal = line.end.Clone().Sub(line.start).Normal();
                float ballDistance = differentVector.Dot(lineNormal);

                float lineLength = line.start.Clone().Sub(line.end).Length();
                Vec2 normalizedLine = line.end.Clone().Sub(line.start).Normalize();
                float AlongLine = differentVector.Dot(normalizedLine);

                Vec2 project = line.start.Clone().Add(normalizedLine.Scale(AlongLine));
                Vec2 difference = ball.position.Clone().Sub(project);

                lineNormal.Scale(flipNormal ? -1 : 1);

                if (AlongLine >= 0 && AlongLine <= lineLength)
                {
                    if (difference.Length() < ball.radius)
                    {
                        ball.position = project.Add(difference);
                        //_ballP1.velocity.Reflect(lineNormal);
                        Vec2 checkLineStart = line.start;
                        Vec2 checkLineEnd = line.end;
                        if (checkLineStart == triangle.line12.start && checkLineEnd == triangle.line12.end)
                        {
                            _ballP1.velocity = triangle.Reflect(lineNormal, ball, "point12");
                        }
                        if (checkLineStart == triangle.line13.start && checkLineEnd == triangle.line13.end)
                        {
                            _ballP1.velocity = triangle.Reflect(lineNormal, ball, "point13");
                        }
                        if (checkLineStart == triangle.line23.start && checkLineEnd == triangle.line23.end)
                        {
                            _ballP1.velocity = triangle.Reflect(lineNormal, ball, "point23");
                        }
                        //reflect on normal from line.

                    }
                }
                ball.y = ball.position.y;
                ball.x = ball.position.x;
            }

        }
        void lineCollision(LineSegment line, bool flipNormal, Ball ball)
        {
            if (ball != null)
            {
                Vec2 differentVector = ball.position.Clone().Sub(line.start);
                Vec2 lineNormal = line.end.Clone().Sub(line.start).Normal();
                float ballDistance = differentVector.Dot(lineNormal);

                float lineLength = line.start.Clone().Sub(line.end).Length();
                Vec2 normalizedLine = line.end.Clone().Sub(line.start).Normalize();
                float AlongLine = differentVector.Dot(normalizedLine);

                Vec2 project = line.start.Clone().Add(normalizedLine.Scale(AlongLine));
                Vec2 difference = ball.position.Clone().Sub(project);

                lineNormal.Scale(flipNormal ? -1 : 1);

                if (AlongLine >= 0 && AlongLine <= lineLength)
                {
                    if (difference.Length() < ball.radius)
                    {
                        //_ballP1.position.x += (-ballDistance + _ballP1.radius) * lineNormal.x;
                        //_ballP1.position.y += (-ballDistance + _ballP1.radius) * lineNormal.y;
                        difference = difference.SetLength(ball.radius);
                        ball.position = project.Add(difference);
                        ball.velocity.Reflect(lineNormal);
                        //reflect on normal from line.
                    }
                }
                else
                {
                    foreach (Ball cap in _lineCaps)
                    {
                        CapCollision(cap, 0.9f,ball);
                    }
                }

                ball.y = ball.position.y;
                ball.x = ball.position.x;
            }
        }
        void CapCollision(Ball lineCap, float bounciness, Ball ball)
        {
            Vec2 Difference = lineCap.position.Clone().Sub(ball.position.Clone());
            float distance = Difference.Length();

            if (distance < (ball.radius + lineCap.radius))
            {
                float separation = ball.radius + lineCap.radius - distance;
                Vec2 normal = Difference.Normalize();
                Vec2 impulse = normal.Clone().Scale(separation);

                ball.position.Sub(impulse);
                ball.velocity = ball.Reflect(ball, normal, bounciness);

            }
        }

        void paddleCollision(LineSegment paddle, Ball ball, bool sidePaddle)
        {
            Vec2 differentVector = ball.position.Clone().Sub(paddle.start);
            Vec2 paddleNormal = paddle.end.Clone().Sub(paddle.start).Normal();
            float BallDistance = differentVector.Dot(paddleNormal);

            float paddleLength = paddle.start.Clone().Sub(paddle.end).Length();
            Vec2 normalizedPaddle = paddle.end.Clone().Sub(paddle.start).Normalize();
            float alongPaddle = differentVector.Dot(normalizedPaddle);

            Vec2 project = paddle.start.Clone().Add(normalizedPaddle.Scale(alongPaddle));
            Vec2 difference = ball.position.Clone().Sub(project);

            if (alongPaddle >= 0 && alongPaddle <= paddleLength)
            {
                if (difference.Length() < ball.radius)
                {
                    difference = difference.SetLength(ball.radius);
                    ball.position = project.Add(difference);
                    ball.velocity = ball.velocity.Reflect(paddleNormal, 100.0f);
                    if (sidePaddle)
                    {
                        ball.velocity.x = (ball.position.x - (paddle.start.x + paddleLength / 2)) * 0.2f;
                    }
                    else
                    {
                        ball.velocity.x = (ball.position.x - (paddle.start.x + paddleLength / 2)) * -0.2f;
                    }
                }
            }
            else
            {
                //foreach (Ball cap in _paddlesP1Caps)
                //{
                //    CapCollision(cap, 4.0f, ball);
                //}
                //foreach (Ball cap in _paddlesP2Caps)
                //{
                //    CapCollision(cap, 4.0f, ball);
                //}
            }
            ball.x = ball.position.x;
            ball.y = ball.position.y;
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
            _ballP2.acceleration = Vec2.zero;
            //Moving(_ballP1);
            //Moving(_ballP2);
            Moving();
            ApplyGravity();
            if (Input.GetMouseButton(0))
            {
                _ballP1.position.Set(Input.mouseX, Input.mouseY);
            }
            if (Input.GetMouseButton(1))
            {
                _ballP2.position.Set(Input.mouseX, Input.mouseY);
            }
            _ballP1.Step();
            _ballP2.Step();
            //else if (lineP2 is true)
            //{
            //    _ballP2.Step();
            //}
            Generator();

            #region mirror
            foreach (Square mirror in _squaresL)
            {
                if (_ballP1 != null && _ballP2 != null)
                {
                    List<LineSegment> mirrorLineList = new List<LineSegment>() { mirror.lineLeftBottomRightBottom, mirror.lineLeftTopLeftBottom, mirror.lineRightBottomRightTop, mirror.lineRightTopLeftTop };
                    
                    foreach (LineSegment mirrorLine in mirrorLineList)
                    {
                        if (SquareCollision(mirror, true, _ballP1))
                        {
                            //foreach (LineSegment line in mirrorLineList)
                            //{
                            //    line.start.Sub(mirrorLine.end);
                            //    line.start.RotateDegrees(45);
                            //    line.start.Add(mirrorLine.end);
                            //}
                            //315 - 90 = 225
                            if (Time.now > (_timerMirror + 2000))
                            {
                                _timerMirror = Time.now;
                                mirror.Destroy();
                                mirror.rotation += 45;
                                if (mirror.rotation > 360)
                                {
                                    mirror.rotation = 45;
                                }
                                mirror.Rotate(mirror.rotation);
                                AddChild(mirror);
                            }
                        }
                        if (_ballP2.HitTest(mirrorLine))
                        {
                            if (Time.now > (_timerMirror + 2000))
                            {
                                _timerMirror = Time.now;
                                mirror.Destroy();
                                mirror.rotation += 45;
                                if (mirror.rotation > 360)
                                {
                                    mirror.rotation = 45;
                                }
                                mirror.Rotate(mirror.rotation);
                                AddChild(mirror);
                            }
                        }
                    }

                }
            }
            #endregion

            #region collisions update
            for (int i = 0; i < _lines.Count; i++)
            {
                lineCollision(_lines[i], true, _ballP1);
                lineCollision(_lines[i], false, _ballP1);
                lineCollision(_lines[i], true, _ballP2);
                lineCollision(_lines[i], false, _ballP2);
            }
            for (int i = 0; i < _squaresL.Count; i++)
            {
                SquareCollision(_squaresL[i], true, _ballP1);
                SquareCollision(_squaresL[i], false, _ballP1);
                SquareCollision(_squaresL[i], true, _ballP2);
                SquareCollision(_squaresL[i], false, _ballP2);
            }
            for (int i = 0; i < _triangleL.Count; i++)
            {
                TriangleCollision(_triangleL[i], true, _ballP1);
                TriangleCollision(_triangleL[i], false, _ballP1);
                TriangleCollision(_triangleL[i], true, _ballP2);
                TriangleCollision(_triangleL[i], false, _ballP2);
            }
            for (int i = 0; i < _circles.Count; i++)
            {
                BumberCollision(_circles[i], _ballP1);
                BumberCollision(_circles[i], _ballP2);
            } 
            for (int i = 0; i < _paddlesP1.Count; i++)
            {
                if (i == 1)
                {
                    paddleCollision(_paddlesP1[i], _ballP1, true);
                    paddleCollision(_paddlesP1[i], _ballP2, true);
                }
                else
                {
                    paddleCollision(_paddlesP1[i], _ballP1, false);
                    paddleCollision(_paddlesP1[i], _ballP2, false);
                }
            }
            for (int i = 0; i < _paddlesP2.Count; i++)
            {
                if (i == 1)
                {
                    paddleCollision(_paddlesP2[i], _ballP1, true);
                    paddleCollision(_paddlesP2[i], _ballP2, true);
                }
                else
                {
                    paddleCollision(_paddlesP2[i], _ballP1, true);
                    paddleCollision(_paddlesP2[i], _ballP2, true);
                }
            }
            #endregion

            #region GetKeys

            //player 1
            if (Input.GetKey(Key.A))
            {
                PaddleL1(-dir);
            }

            if (Input.GetKey(Key.D))
            {
                PaddleL1(dir);
            }
            if (Input.GetKeyDown(Key.W))
            {
                _laser = new Laser(new Vec2(183, game.height - 50),new Vec2(0,-5));
                _laser.FireLaser(true);
                AddChild(_laser);
                _isP1Fired = true;
            }
            if (_isP1Fired)
            {
                _laser.Step();
                if (Input.GetKeyDown(Key.H))
                {
                    _laser.Destroy();
                }
            }
            if (Input.GetKey(Key.S))
            {
                if (Time.now > (_launchTimeP1 + 1000))
                {
                    _launcherP1 = new Vec2(0, -_launchPowerP1);
                    _ballMovesP1 = false;
                    _ballP1.velocity = new Vec2(0, 0);
                    _green.height -= 1;
                    _green.y += 1;
                    _launchPowerP1 += 0.2f;
                    if (_launchPowerP1 >= 15)
                    {
                        _green.height += 1;
                        _green.y -= 1;
                    }
                }
            }
            else
            {
                if (_isFiredP1)
                {
                    if (_ballP1 != null && _launcherP1 != null)
                    {
                        _ballP1.velocity.Add(_launcherP1);
                        _isFiredP1 = false;
                        _isLaunchedP1 = true;
                        _launchPowerP1 = 0;
                    }
                }
                else
                {
                    if (_ballP1 != null && _launcherP1 != null)
                    {
                        if (_ballP1.velocity.y > -5.0f)
                        {
                            _isLaunchedP1 = false;
                            _ballMovesP1 = true;
                        }
                    }
                }
            }
            if (_isFiredP1 == false)
            {
                if (_green.height < 108)
                {
                    if (Time.now > (_launchTimeP1 + 100))
                    {
                        _launchTimeP1 = Time.now;
                        _green.height += 2;
                        _green.y -= 1.8f;
                    }
                }
                else
                {
                    _green.height = 108;
                    _green.y = game.height - 125;
                    _launchPowerP1 = 0;
                    _isFiredP1 = true;
                    _launcherP1 = null;
                   // _ballMoves = true;
                }
                
            }
            Console.WriteLine("ball X: "+_ballP1.velocity.x);
            Console.WriteLine("ball Y: "+_ballP1.velocity.y);
            //player 2
            if (Input.GetKey(Key.LEFT))
            {
                PaddleL2(-dir);
            }

            if (Input.GetKey(Key.RIGHT))
            {
                PaddleL2(dir);
            }
            if (Input.GetKeyDown(Key.UP))
            {
                _isP2Fired = true;
            }
            if (_isP2Fired)
            {
                
            }
            if (Input.GetKey(Key.DOWN))
            {
                if (Time.now > (_launchTimeP2 + 1000))
                {
                    _launcherP2 = new Vec2(0, -_launchPowerP2);
                    _ballMovesP2 = false;
                    _ballP2.velocity = new Vec2(0, 0);
                    _blue.height -= 1;
                    _blue.y += 1;
                    _launchPowerP2 += 0.2f;
                    if (_launchPowerP2 >= 15)
                    {
                        _blue.height += 1;
                        _blue.y -= 1;
                    }
                }
            }
            else
            {
                if (_isFiredP2)
                {
                    if (_ballP2 != null && _launcherP2 != null)
                    {
                        _ballP2.velocity.Add(_launcherP2);
                        _isFiredP2 = false;
                        _isLaunchedP2 = true;
                        _launchPowerP2 = 0;
                    }
                }
                else
                {
                    if (_ballP2 != null && _launcherP2 != null)
                    {
                        if (_ballP2.velocity.y > -5.0f)
                        {
                            _isLaunchedP2 = false;
                            _ballMovesP2 = true;
                        }
                    }
                }
            }
            if (_isFiredP2 == false)
            {
                if (_blue.height < 108)
                {
                    if (Time.now > (_launchTimeP2 + 100))
                    {
                        _launchTimeP2 = Time.now;
                        _blue.height += 2;
                        _blue.y -= 1.8f;
                    }
                }
                else
                {
                    _blue.height = 108;
                    _blue.y = game.height - 125;
                    _launchPowerP2 = 0;
                    _isFiredP2 = true;
                    _launcherP2 = null;
                    // _ballMoves = true;
                }
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

            _lines = new List<LineSegment>();
            _circles = new List<Bumbers>();
            _squaresL = new List<Square>();
            //squareList = new List<List<LineSegment>>();
            _triangleL = new List<Triangle>();
            //triangleList = new List<List<LineSegment>>();

            _levelLines = new List<LineSegment>();
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
