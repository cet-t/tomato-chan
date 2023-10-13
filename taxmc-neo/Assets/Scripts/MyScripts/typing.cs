using System;
using UnityEngine;

namespace trrne.Bag
{
    public static class Typing
    {
        public static T Cast<T>(this object obj)
        {
            return (T)obj;
        }

        public static Vector2 ToVec2(this Vector3 vector)
        {
            return (Vector2)vector;
        }

        /// <summary>
        /// 置換したい文字をスペース二つはさんで記述<br/><br/>
        /// Ex(XYZ -> BBB):<br/>
        /// ("XYZ").ReplaceLump("X  Y  Z", B);<br/>
        /// output: "BBB"
        /// </summary>
        public static string ReplaceLump(this string target, string before, string after)
        {
            string[] pres = before.Split("  ");

            for (int count = 0; count < pres.Length; count++)
            {
                target = target.Replace(pres[count], after);
            }

            return target;
        }

        /// <summary>
        /// 文字列から指定の文字を削除する
        /// </summary>
        public static string Delete(this string target, string be)
        {
            return target.Replace(be, "");
        }

        public static bool Subclass(this object obj, Type t)
        {
            return obj.GetType().IsSubclassOf(t);
        }

        public static bool Subclass(this object[] objs, Type t)
        {
            return objs.GetType().IsSubclassOf(t);
        }

        public static string Join(this object[] objs, string sep)
        {
            return string.Join(sep, objs);
        }

        public static string Link(this char[] objs)
        {
            return string.Join("", objs);
        }
    }
}
