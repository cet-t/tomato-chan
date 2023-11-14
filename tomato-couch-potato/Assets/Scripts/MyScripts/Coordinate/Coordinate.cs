using System;
using UnityEngine;

namespace trrne.Box
{
    public static partial class Coordinate
    {
        public static Vector3 V3X => new(1, 0, 0);
        public static Vector3 V3Y => new(0, 1, 0);
        public static Vector3 V3Z => new(0, 0, 1);
        public static Vector3 V30 => new(0, 0, 0);
        public static Vector3 V31 => new(1, 1, 1);

        /// <summary>
        /// [xX]<br/>[yY]<br/>[zZ]<br/>[zZ]ero<br/>[oO]ne
        /// </summary>
        public static Vector3 V3(string xyz01) => xyz01 switch
        {
            "x" or "X" => new(1, 0, 0),
            "y" or "Y" => new(1, 0, 0),
            "z" or "Z" => new(1, 0, 0),
            "zero" or "Zero" or "0" => new(1, 0, 0),
            "one" or "One" or "1" => new(1, 0, 0),
            _ => throw null,
        };

        /// <summary>
        /// [xX]<br/>[yY]<br/>[zZ]ero<br/>[oO]ne
        /// </summary>
        public static Vector2 V2(string xy01) => xy01 switch
        {
            "x" or "X" => new(1, 0),
            "y" or "Y" => new(1, 0),
            "zero" or "Zero" or "0" => new(1, 0),
            "one" or "One" or "1" => new(1, 0),
            _ => throw null,
        };

        public static Vector2 V2X => new(1, 0);
        public static Vector2 V2Y => new(0, 1);
        public static Vector2 V20 => new(0, 0);
        public static Vector2 V21 => new(1, 1);

        public static Quaternion QX => new(1, 0, 0, 0);
        public static Quaternion QY => new(0, 1, 0, 0);
        public static Quaternion QZ => new(0, 0, 1, 0);
        public static Quaternion QW => new(0, 0, 0, 1);
        public static Quaternion Q0 => new(0, 0, 0, 0);
        public static Quaternion Q1 => new(1, 1, 1, 1);

        /// <summary>
        /// [xX]<br/>[yY]<br/>[zZ]<br/>[wW]<br/>[zZ]ero<br/>[oO]ne
        /// </summary>
        public static Quaternion Q(string xyzw01) => xyzw01 switch
        {
            "x" or "X" => new Quaternion(1, 0, 0, 0),
            "y" or "Y" => new Quaternion(0, 1, 0, 0),
            "z" or "Z" => new Quaternion(0, 0, 1, 0),
            "w" or "W" => new Quaternion(0, 0, 0, 1),
            "zero" or "Zero" or "0" => new Quaternion(0, 0, 0, 0),
            "one" or "One" or "1" => new Quaternion(1, 1, 1, 1),
            _ => throw null
        };

        /// <summary>
        /// 重力加速度
        /// </summary>
        public static float GravitationalAcceleration => 9.80665f;

        /// <summary>
        /// 重力加速度
        /// </summary>
        public static Vector3 Gravity => -V3Y * GravitationalAcceleration;

        public static bool Twins(Vector3 n1, Vector3 n2)
        => Maths.Twins(n1.x, n2.x) && Maths.Twins(n1.y, n2.y) && Maths.Twins(n1.z, n2.z);

        [Obsolete] public static Vector2 Direction(Vector2 target, Vector2 origin) => target - origin;
    }
}