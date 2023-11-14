﻿#pragma warning disable IDE0002
#pragma warning disable IDE0031

using UnityEngine;

namespace trrne.Box
{
    public static partial class Gobject
    {
        public static T Instantiate<T>(this T g, Vector3 p = new(), Quaternion r = new())
            where T : Object => GameObject.Instantiate(g, p, r);
        public static T Instantiate<T>(this T[] gs, Vector3 p = new(), Quaternion r = new())
            where T : Object => GameObject.Instantiate(gs.Choice(), p, r);

        public static T TryInstantiate<T>(this T g, Vector3 p = new(), Quaternion r = new())
            where T : Object => g != null ? g.Instantiate(p, r) : null;
        public static T TryInstantiate<T>(this T[] gs, Vector3 p = new(), Quaternion r = new())
            where T : Object => gs.Length > 0 ? gs.Instantiate(p, r) : null;
    }
}