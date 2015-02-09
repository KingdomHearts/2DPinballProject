﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Vec2
    {
        public static Vec2 zero { get { return new Vec2(0, 0); } }
        public static Vec2 temp = new Vec2();

        public float x = 0;
        public float y = 0;

        public Vec2(float pX = 0, float pY = 0)
        {
            x = pX;
            y = pY;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", x, y);
        }

        public Vec2 Add(Vec2 other)
        {
            x += other.x;
            y += other.y;
            return this;
        }

        public Vec2 Sub(Vec2 other)
        {
            x -= other.x;
            y -= other.y;
            return this;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public Vec2 Normalize()
        {
            if (x == 0 && y == 0)
            {
                return this;
            }
            else
            {
                return Scale(1 / Length());
            }
        }

        public Vec2 Clone()
        {
            return new Vec2(x, y);
        }

        public Vec2 Scale(float scalar)
        {
            x *= scalar;
            y *= scalar;
            return this;
        }

        public Vec2 Set(float pX, float pY)
        {
            x = pX;
            y = pY;
            return this;
        }

        public Vec2 Normal()
        {
            return new Vec2(-y, x).Normalize();
        }

        public float Dot(Vec2 other)
        {
            return this.x * other.x + this.y * other.y;
        }

        public Vec2 Reflect(Vec2 normal, float bounciness = 1)
        {
            Vec2 Result = this.Sub(normal.Clone().Scale((1 + bounciness) * this.Dot(normal)));
            return Result;
        }

        //Rotation
        public void SetAngleRadians(double angleR)
        {
            float length = Length();
            x = (float)Math.Cos(angleR) * length;
            y = (float)Math.Sin(angleR) * length;
        }
        public void SetAngleDegrees(double angleD)
        {
            double angle = Math.PI * angleD / 180;
            float length = Length();
            x = (float)Math.Cos(angle) * length;
            y = (float)Math.Sin(angle) * length;
        }
        public double GetAngleRadians()
        {
            return Math.Atan2(y, x);
        }
        public double GetAngleDegrees()
        {
            return Math.Atan2(y, x) * 180 / Math.PI;
        }
        public void RotateDegrees(double angleD)
        {
            double angle = Math.PI * angleD / 180;

            float x2 = (float)(x * Math.Cos(angle) - y * Math.Sin(angle));
            float y2 = (float)(x * Math.Sin(angle) + y * Math.Cos(angle));

            x = x2;
            y = y2;
        }
        public void RotateRadians(double angleR)
        {
            float x2 = (float)(x * Math.Cos(angleR) - y * Math.Sin(angleR));
            float y2 = (float)(x * Math.Sin(angleR) + y * Math.Cos(angleR));

            x = x2;
            y = y2;
        }
    }
}