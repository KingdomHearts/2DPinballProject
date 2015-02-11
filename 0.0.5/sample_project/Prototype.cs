using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using UniLua;

namespace GXPEngine
{
    class Prototype : GameObject
    {
        //Lua
        ILuaState _lua;
        string answer = "";
        string message = "";
        Canvas canvas;
        Font font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
        Brush brush = new SolidBrush(Color.White);
        bool running = true;

        private List<NLineSegment> _lines;
        private NLineSegment _middleLine;
        private Ball _ballP1;
        private Ball _ballP2;
        private Ball targetBall;

        private List<Ball> _lineBalls;

        private Vec2 _previousPosition;
        private Canvas _canvas;
        private int width = 0;
        private int height = 0;

        private MouseHandler _ballHandlerP1 = null;
        private MouseHandler _ballHandlerP2 = null;

        private float _ballDownTime = 0;
        private Vec2 _ballDownPositionP1 = null;
        private Vec2 _ballDownPositionP2 = null;

        private Vec2 _gravity = new Vec2(0, 0.002f);
        private bool gravityP1 = false;
        private bool gravityP2 = false;
        private float _speed = 0;

        private List<NLineSegment> _paddles;

        public Prototype()
        {

            _lua = LuaAPI.NewState();
            _lua.L_OpenLibs();

            width = game.width;
            height = game.height;
            _canvas = new Canvas(width, height);
            AddChild(_canvas);

            _lines = new List<NLineSegment>();

            AddLine(new Vec2(0, 0), new Vec2(width, 0));
            AddLine(new Vec2(width, 0), new Vec2(width, height));
            AddLine(new Vec2(width, height), new Vec2(0, height));
            AddLine(new Vec2(0, height), new Vec2(0, 0));

            _middleLine = new NLineSegment(new Vec2(width / 2, 0), new Vec2(width / 2, height));
            AddChild(_middleLine);

            _paddles = new List<NLineSegment>();

            //player one side
            //shoot lines
            AddLine(new Vec2(0 + 50, height), new Vec2(0 + 50, height - 200));
            AddLine(new Vec2(0, height - 200), new Vec2(0 + 20, 250));

            //2 lines laser
            AddLine(new Vec2((width / 2) / 2 + 30, height), new Vec2((width / 2) / 2 + 30, height - 50));
            AddLine(new Vec2((width / 2) / 2 - 20, height), new Vec2((width / 2) / 2 - 20, height - 50));

            //flipper line
            AddLine(new Vec2((width / 2) / 2 - 20, height - 50), new Vec2(50, height - 200));
            AddLine(new Vec2((width / 2) / 2 + 30, height - 50), new Vec2(width / 2, height - 200));
            //flippers
            Paddles(new Vec2((width / 2) / 2 - 20, height - 50), new Vec2((width / 2) / 2 - 60, height - 78), 0xff00ffff);
            Paddles(new Vec2((width / 2) / 2 + 30, height - 50), new Vec2((width / 2) / 2 + 70, height - 78), 0xff00ffff);

            //player two side
            ////flipper line
            AddLine(new Vec2((width / 2) + ((width / 2) / 2) - 30, height - 50), new Vec2((width / 2), height - 200));
            AddLine(new Vec2((width / 2) + ((width / 2) / 2) + 20, height - 50), new Vec2(width - 50, height - 200));
            //flippers
            Paddles(new Vec2((width / 2) + ((width / 2) / 2) - 30, height - 50), new Vec2((width / 2) + ((width / 2) / 2) - 70, height - 78), 0xff00ffff);
            Paddles(new Vec2((width / 2) + ((width / 2) / 2) + 20, height - 50), new Vec2((width / 2) + ((width / 2) / 2) + 60, height - 78), 0xff00ffff);

            //2 lines laser
            AddLine(new Vec2((width / 2) + ((width / 2) / 2) - 30, height - 50), new Vec2((width / 2) + ((width / 2) / 2) - 30, height));
            AddLine(new Vec2((width / 2) + ((width / 2) / 2) + 20, height - 50), new Vec2((width / 2) + ((width / 2) / 2) + 20, height));

            //shoot lines
            AddLine(new Vec2(width - 50, height), new Vec2(width - 50, height - 200));
            AddLine(new Vec2(width, height - 200), new Vec2(width - 20, 250));

            // Overall lines
            AddLine(new Vec2(0 + 20, height - 290), new Vec2(width / 2, 0));
            AddLine(new Vec2(width - 20, height - 290), new Vec2(width / 2, 0));

            _lineBalls = new List<Ball>();

            _ballP1 = new Ball(10, new Vec2(0 + (10 * 2), height - (10 * 10)), null, Color.Yellow);
            AddChild(_ballP1);
            _ballP2 = new Ball(10, new Vec2(width - (10 * 2), height - (10 * 10)), null, Color.Blue);
            AddChild(_ballP2);


            //_ballP1.velocity = new Vec2(5, 5);

            _ballHandlerP1 = new MouseHandler(_ballP1);
            _ballHandlerP1.OnMouseDownOnTarget += onBallMouseDownP1;

            _ballHandlerP2 = new MouseHandler(_ballP2);
            _ballHandlerP2.OnMouseDownOnTarget += onBallMouseDownP2;
        }

        private void Paddles(Vec2 start, Vec2 end, uint color = 0xff00ff00)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _lines.Add(line);
            _paddles.Add(line);
        }

        private void PaddleBehavoir()
        {
            foreach (NLineSegment line in _paddles)
            {
                for (int i = 0; i < 45; i++)
                {
                    line.start.RotateDegrees(i);
                }

            }

        }

        private void AddLine(Vec2 start, Vec2 end, uint color = 0xff00ff00)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _lines.Add(line);
        }

        void Update()
        {

            if (Input.GetMouseButton(0))
            {
                _ballP1.position.Set(Input.mouseX, Input.mouseY);
            }
            if (Input.GetMouseButton(1))
            {

                _ballP2.position.Set(Input.mouseX, Input.mouseY);
            }

            if (Input.GetKeyDown(Key.W))
            {
                PaddleBehavoir();
            }


            _ballP1.Step();
            _ballP2.Step();

            for (int i = 0; i < _lines.Count; i++)
            {
                lineCollision(_lines[i], _ballP1);
            }
            for (int i = 0; i < _lines.Count; i++)
            {
                lineCollision(_lines[i], _ballP2);
            }

            //draw line
            //_canvas.graphics.DrawLine(
            //    Pens.White, _previousPosition.x, _previousPosition.y, _ball.position.x, _ball.position.y
            //)

            _ballP1.y = _ballP1.position.y;
            _ballP1.x = _ballP1.position.x;
            _ballP2.y = _ballP2.position.y;
            _ballP2.x = _ballP2.position.x;

            if (_ballP1.velocity.x > 10)
            {
                _ballP1.velocity.x = 10;
            }
            else if (_ballP1.velocity.x < -10)
            {
                _ballP1.velocity.x = -10;
            }
            if (_ballP1.velocity.y > 10)
            {
                _ballP1.velocity.y = 10;
            }
            else if (_ballP1.velocity.y < -10)
            {
                _ballP1.velocity.y = -10;
            }

            if (_ballP2.velocity.x > 10)
            {
                _ballP2.velocity.x = 10;
            }
            else if (_ballP2.velocity.x < -10)
            {
                _ballP2.velocity.x = -10;
            }
            if (_ballP2.velocity.y > 10)
            {
                _ballP2.velocity.y = 10;
            }
            else if (_ballP2.velocity.y < -10)
            {
                _ballP2.velocity.y = -10;
            }

            if (gravityP1)
            {
                _ballP1.acceleration.Add(_gravity.Clone());
            }
            if (gravityP2)
            {
                _ballP2.acceleration.Add(_gravity.Clone());
            }

            Console.WriteLine(_ballP1.velocity.y);

        }

        void lineCollision(NLineSegment line, Ball _ball)
        {
            Vec2 differenceVector = _ball.position.Clone().Sub(line.start);

            Vec2 lineNormal = line.end.Clone().Sub(line.start).Normal();
            float ballDistance = differenceVector.Dot(lineNormal);

            float lineLength = line.start.Clone().Sub(line.end).Length();
            Vec2 lineNormalized = line.end.Clone().Sub(line.start).Normalize();
            float alongLine = differenceVector.Dot(lineNormalized);
            //Console.WriteLine("LineLenght: " + lineLength);
            //Console.WriteLine("alongLine: " + alongLine);
            Vec2 project = line.start.Clone().Add(lineNormalized.Scale(alongLine));
            Vec2 difference = _ball.position.Clone().Sub(project);

            if (alongLine >= 0 && alongLine <= lineLength)
            {
                if (difference.Length() < _ball.radius)
                {
                    difference = new Vec2(difference.x, difference.y).Normalize().Scale(_ball.radius);
                    _ball.position = project.Add(difference);
                    //_ball.position.x += (-ballDistance + _ball.radius) * lineNormal.x;
                    //_ball.position.y += (-ballDistance + _ball.radius) * lineNormal.y;
                    _ball.velocity.Reflect(lineNormal);
                }
                else
                {
                    for (int i = 0; i < _lineBalls.Count; i++)
                    {
                        Vec2 ballDifference = _lineBalls[i].position.Clone().Sub(_ball.position.Clone());
                        float distance = ballDifference.Length();

                        if (distance < (_ball.radius + _lineBalls[i].radius - 1))
                        {
                            float overlap = _ball.radius + _lineBalls[i].radius - distance;
                            Vec2 normalized = ballDifference.Normalize();
                            Vec2 impulse = normalized.Clone().Scale(overlap);

                            _ball.position.Sub(impulse);
                            _ball.velocity.Reflect(normalized);
                        }
                    }
                }
            }

            _ball.y = _ball.position.y;
            _ball.x = _ball.position.x;

        }

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
            if (target is Ball) targetBall = (Ball)target;
            Console.WriteLine(target + "mouse up, time taken:" + (Time.now - _ballDownTime));
            _ballHandlerP1.OnMouseUp -= onBallMouseUpP1;
            _ballHandlerP1.OnMouseMove -= onBallMouseMoveP1;
            _ballP1.velocity = _ballDownPositionP1.Sub(_ballP1.position).Scale(1 / 10.0f);
            gravityP1 = true;
        }

        //Player 2
        private void onBallMouseDownP2(GameObject target, MouseEventType type)
        {
            _ballHandlerP2.OnMouseMove += onBallMouseMoveP2;
            _ballHandlerP2.OnMouseUp += onBallMouseUpP2;

            _ballDownTime = Time.now;
            _ballDownPositionP2 = _ballP2.position.Clone();
        }

        private void onBallMouseMoveP2(GameObject target, MouseEventType type)
        {
            Vec2 MousePosition2 = new Vec2(Input.mouseX, Input.mouseY);
            Vec2 delta = MousePosition2.Clone().Sub(_ballDownPositionP2);
            Vec2 deltaClone = delta.Clone();
            deltaClone.Normalize().Scale((float)Math.Sqrt((double)delta.Length())).Scale(8);

        }
        private void onBallMouseUpP2(GameObject target, MouseEventType type)
        {
            if (target is Ball) targetBall = (Ball)target;
            Console.WriteLine(target + "mouse up, time taken:" + (Time.now - _ballDownTime));
            _ballHandlerP2.OnMouseUp -= onBallMouseUpP2;
            _ballHandlerP2.OnMouseMove -= onBallMouseMoveP2;
            _ballP2.velocity = _ballDownPositionP2.Sub(_ballP2.position).Scale(1 / 10.0f);
        }
    }
}
