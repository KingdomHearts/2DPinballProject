﻿using System;
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
        private NLineSegment _line;
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

        private bool _toggleGravity = true;
        private bool _ballMoves = false;
        private Vec2 _gravity = new Vec2(0, 0.2f);
        private bool gravityP1 = false;
        private bool gravityP2 = false;
        private float _speed = 0;

        private List<NLineSegment> _paddlesP1R;
        private List<NLineSegment> _paddlesP1L;
        private List<NLineSegment> _paddlesP2R;
        private List<NLineSegment> _paddlesP2L;

        private List<Bumbers> _bumberList;
        private Bumbers _bumber1;
        private Bumbers _bumber2;

        private List<Triangle> _triangleList;
        private Triangle triangle;

        private List<Square> _squareList;
        private Square _square;

        private RepairKit repairKit;
        private List<RepairKit> repairKitList;

        private BatteryCharge batteryCharge;
        private List<BatteryCharge> batteryChargeList; 

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

            _paddlesP1R = new List<NLineSegment>();
            _paddlesP1L = new List<NLineSegment>();
            _paddlesP2R = new List<NLineSegment>();
            _paddlesP2L = new List<NLineSegment>();

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
            PaddlesP1L(new Vec2((width / 2) / 2 - 20, height - 50), new Vec2((width / 2) / 2 - 60, height - 78), 0xff00ffff);
            PaddlesP1R(new Vec2((width / 2) / 2 + 30, height - 50), new Vec2((width / 2) / 2 + 70, height - 78), 0xff00ffff);

            //player two side
            ////flipper line
            AddLine(new Vec2((width / 2) + ((width / 2) / 2) - 30, height - 50), new Vec2((width / 2), height - 200));
            AddLine(new Vec2((width / 2) + ((width / 2) / 2) + 20, height - 50), new Vec2(width - 50, height - 200));
            //flippers
            PaddlesP2L(new Vec2((width / 2) + ((width / 2) / 2) - 30, height - 50), new Vec2((width / 2) + ((width / 2) / 2) - 70, height - 78), 0xff00ffff);
            PaddlesP2R(new Vec2((width / 2) + ((width / 2) / 2) + 20, height - 50), new Vec2((width / 2) + ((width / 2) / 2) + 60, height - 78), 0xff00ffff);

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

            _ballP1 = new Ball(20, new Vec2(0 + (10 * 2), height - (10 * 10)), null, Color.Yellow);
            AddChild(_ballP1);
            _ballP2 = new Ball(20, new Vec2(width - (10 * 2), height - (10 * 10)), null, Color.Blue);
            AddChild(_ballP2);


            //_ballP1.velocity = new Vec2(5, 5);

            _ballHandlerP1 = new MouseHandler(_ballP1);
            _ballHandlerP1.OnMouseDownOnTarget += onBallMouseDownP1;

            _ballHandlerP2 = new MouseHandler(_ballP2);
            _ballHandlerP2.OnMouseDownOnTarget += onBallMouseDownP2;


            //bumber object test
            _bumberList = new List<Bumbers>();

            _bumber1 = new Bumbers(30, new Vec2(game.width / 2 - 100, game.height / 2 -100), Color.Purple, 1.5f);
            AddChild(_bumber1);
            _bumber2 = new Bumbers(30, new Vec2(game.width / 2 + 100, game.height / 2 - 100), Color.Purple, 1.5f);
            AddChild(_bumber2);
            _bumberList.Add(_bumber1);
            _bumberList.Add(_bumber2);

            //triangle object test
            _triangleList = new List<Triangle>();
            triangle = new Triangle(new Vec2(300, 350), new Vec2(350, 400), new Vec2(250, 400), 10, new List<string>() { "point13", "point12" });
            AddChild(triangle);
            _triangleList.Add(triangle);

            triangle = new Triangle(new Vec2(650, 350), new Vec2(700, 400), new Vec2(600, 400), 10, new List<string>() { "point13", "point12" });
            AddChild(triangle);
            _triangleList.Add(triangle);

            //Sqaure and Mirrior object
            _squareList = new List<Square>();
            _square = new Square(20, (game.width / 2) + (game.width / 2)/2, game.height / 2, 225, 1.5f);
            _squareList.Add(_square);
            AddChild(_square);

            _square = new Square(20, (game.width /2) /2, game.height /2, 315, 1.5f);
            _squareList.Add(_square);
            AddChild(_square);

            //Items
            repairKitList = new List<RepairKit>();
            batteryChargeList = new List<BatteryCharge>();

            repairKit = new RepairKit(new Vec2(200,200));
            AddChild(repairKit);
            repairKitList.Add(repairKit);

            repairKit = new RepairKit(new Vec2(game.width - 200, 200));
            AddChild(repairKit);
            repairKitList.Add(repairKit);

            batteryCharge = new BatteryCharge(new Vec2(200, 300));
            AddChild(batteryCharge);
            batteryChargeList.Add(batteryCharge);
            batteryCharge = new BatteryCharge(new Vec2(game.width - 200, 300));
            AddChild(batteryCharge);
            batteryChargeList.Add(batteryCharge);
        }

        private void PaddlesP1L(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _lines.Add(line);
            _paddlesP1L.Add(line);
        }

        private void PaddlesP1R(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _lines.Add(line);
            _paddlesP1R.Add(line);
        }

        private void PaddlesP2L(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _lines.Add(line);
            _paddlesP2L.Add(line);
        }

        private void PaddlesP2R(Vec2 start, Vec2 end, uint color = 0xff000000)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _lines.Add(line);
            _paddlesP2R.Add(line);
        }

        private void AddLine(Vec2 start, Vec2 end, uint color = 0xff00ff00)
        {
            NLineSegment line = new NLineSegment(start, end, color, 4);
            AddChild(line);
            _lines.Add(line);
        }

        void Update()
        {
            _ballP1.acceleration = Vec2.zero;
            Moving();
            ApplyGravity();
            if (_ballP1.HitTest(repairKit))
            {
                repairKit.Destroy();
            }
            if (_ballP1.HitTest(batteryCharge))
            {
                batteryCharge.Destroy();
            }
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

            for (int i = 0; i < _lines.Count; i++)
            {
                lineCollision(_lines[i], true);
                lineCollision(_lines[i], false);
            }
            for (int i = 0; i < _squareList.Count; i++)
            {
                SquareCollision(_squareList[i],true);
                SquareCollision(_squareList[i],false);
            }
            for (int i = 0; i < _triangleList.Count; i++)
            {
                TriangleCollision(_triangleList[i], true); 
                TriangleCollision(_triangleList[i], false);
            }

            if (Input.GetKey(Key.A))
            {
                PaddleBehavoirP1L();
            }
            else
            {
                for (int i = 0; i < _paddlesP1L.Count; i++)
                {
                    _paddlesP1L[i].RotateBack(0.2f);
                }
            }

            if (Input.GetKey(Key.D))
            {
                PaddleBehavoirP1R();
            }
            else
            {
                for (int i = 0; i < _paddlesP1R.Count; i++)
                {
                    _paddlesP1R[i].RotateBack(-0.2f);
                }
            }

            if (Input.GetKey(Key.LEFT))
            {
                PaddleBehavoirP2L();
            }
            else
            {
                for (int i = 0; i < _paddlesP2L.Count; i++)
                {
                    _paddlesP2L[i].RotateBack(0.2f);
                }
            }

            if (Input.GetKey(Key.RIGHT))
            {
                PaddleBehavoirP2R();
            }
            else
            {
                for (int i = 0; i < _paddlesP2R.Count; i++)
                {
                    _paddlesP2R[i].RotateBack(-0.2f);
                }
            }

            #region troep
            //for (int i = 0; i < _lines.Count; i++)
            //{
            //    lineCollision(_lines[i]);
            //}

            //draw line
            //_canvas.graphics.DrawLine(
            //    Pens.White, _previousPosition.x, _previousPosition.y, _ball.position.x, _ball.position.y
            //)

            //_ballP1.y = _ballP1.position.y;
            //_ballP1.x = _ballP1.position.x;
            //_ballP2.y = _ballP2.position.y;
            //_ballP2.x = _ballP2.position.x;

            //if (_ballP2.velocity.x > 10)
            //{
            //    _ballP2.velocity.x = 10;
            //}
            //else if (_ballP2.velocity.x < -10)
            //{
            //    _ballP2.velocity.x = -10;
            //}
            //if (_ballP2.velocity.y > 10)
            //{
            //    _ballP2.velocity.y = 10;
            //}
            //else if (_ballP2.velocity.y < -10)
            //{
            //    _ballP2.velocity.y = -10;
            //}

            //if (gravityP1)
            //{
            //    _ballP1.acceleration.Add(_gravity.Clone());
            //}
            //if (gravityP2)
            //{
            //    _ballP2.acceleration.Add(_gravity.Clone());
            //}
            #endregion
            Console.WriteLine(_ballP1.velocity.y);

        }

        private void PaddleBehavoirP1L()
        {
            foreach (NLineSegment line in _paddlesP1L)
            {
                lineCollision(line, true);
                line.Rotate(-0.2f);

            }
        }
        private void PaddleBehavoirP1R()
        {
            foreach (NLineSegment line in _paddlesP1R)
            {
                lineCollision(line, true);
                line.Rotate(0.2f);

            }
        }

        private void PaddleBehavoirP2L()
        {
            foreach (NLineSegment line in _paddlesP2L)
            {
                lineCollision(line, true);
                line.Rotate(-0.2f);

            }
        }
        private void PaddleBehavoirP2R()
        {
            foreach (NLineSegment line in _paddlesP2R)
            {
                lineCollision(line, true);
                line.Rotate(0.2f);

            }
        }

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
                       _ballP1.position = project.Add(difference);
                       Vec2 checkLineStart = line.start;
                       Vec2 checkLineEnd = line.end;
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
                   _ballP1.position.x += (-ballDistance) * lineNormal.x;
                   _ballP1.position.y += (-ballDistance) * lineNormal.y;
                   difference = difference.SetLength(_ballP1.radius);
                   //_ballP1.position = project.Add(difference);
                   _ballP1.velocity.Reflect(lineNormal);
                   //reflect on normal from line.

               }
           }
           else
           {
               foreach (Bumbers bumber in _bumberList)
               {
                   BumberCollision(bumber);
               }
           }

           _ballP1.y = _ballP1.position.y;
           _ballP1.x = _ballP1.position.x;

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
            _ballMoves = true;
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
            Console.WriteLine(target + "mouse up, time taken:" + (Time.now - _ballDownTime));
            _ballHandlerP2.OnMouseUp -= onBallMouseUpP2;
            _ballHandlerP2.OnMouseMove -= onBallMouseMoveP2;
            _ballP2.velocity = _ballDownPositionP2.Sub(_ballP2.position).Scale(1 / 10.0f);
        }

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
                //_line.end = null;
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
    }
}
