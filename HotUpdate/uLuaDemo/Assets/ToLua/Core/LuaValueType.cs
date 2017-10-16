/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;

namespace LuaInterface
{
    public partial struct LuaValueType
    {
        public const int None = 0;
        public const int Vector3 = 1;
        public const int Quaternion = 2;
        public const int Vector2 = 3;
        public const int Color = 4;
        public const int Vector4 = 5;
        public const int Ray = 6;
        public const int Bounds = 7;
        public const int Touch = 8;
        public const int LayerMask = 9;
        public const int RaycastHit = 10;
        public const int Int64 = 11;
        public const int UInt64 = 12;
        public const int Max = 64;

        private int type;

        public LuaValueType(int value)
        {
            type = value;
        }

        public static implicit operator int(LuaValueType mask)
        {
            return mask.type;
        }

        public static implicit operator LuaValueType(int intVal)
        {
            return new LuaValueType(intVal);
        }

        public override string ToString()
        {
            return LuaValueTypeName.Get(type);
        }
    }

    public static class LuaValueTypeName
    {
        public static string[] names = new string[LuaValueType.Max];

        static LuaValueTypeName()
        {
            names[LuaValueType.None] = "None";
            names[LuaValueType.Vector3] = "Vector3";
            names[LuaValueType.Quaternion] = "Quaternion";
            names[LuaValueType.Vector2] = "Vector2";
            names[LuaValueType.Color] = "Color";
            names[LuaValueType.Vector4] = "Vector4";
            names[LuaValueType.Ray] = "Ray";
            names[LuaValueType.Bounds] = "Bounds";
            names[LuaValueType.Touch] = "Touch";
            names[LuaValueType.LayerMask] = "LayerMask";
            names[LuaValueType.RaycastHit] = "RaycastHit";
        }

        static public string Get(int type)
        {
            if (type >= 0 && type < LuaValueType.Max)
            {
                return names[type];
            }

            return "UnKnownType:" + ConstStringTable.GetNumIntern(type);
        }
    }
}
