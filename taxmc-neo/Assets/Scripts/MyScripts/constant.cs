using System;
using System.Collections.Generic;
using trrne.Bag;
using UnityEngine;
using AnotherScenes = trrne.Bag.Scenes;

namespace trrne
{
    public static partial class Constant
    {
        public static string[] Paths2
        {
            get
            {
                List<string> scenes = new(AnotherScenes.Total(Counting.Built));
                AnotherScenes.names.ForEach(
                    scene => SimpleRunner.BoolAction(scene.Contains(Scenes.Prefix), () => scenes.Add(scene)));

                return scenes.ToArray();
            }
        }

        public readonly struct Scenes
        {
            public static string
            Prefix = "Game",
            StageSelect = "StageSelect";
        }

        public readonly struct Layers
        {
            public const int
            None = 1 << 0,
            Player = 1 << 6,
            Ground = 1 << 7,
            Object = 1 << 8,
            Creature = 1 << 9;
        }

        public readonly struct Keys
        {
            public static string
            Horizontal = "Horizontal",
            Vertical = "Vertical",
            Wheel = "Mouse ScrollWheel",
            Jump = "Jump",
            Down = "Down",
            Zoom = "Zoom",
            Button = "Button";
        }

        public readonly struct Tags
        {
            public static string
            Player = "Player",
            Ladder = "Ladder",
            Pad = "Pad",
            MainCamera = "MainCamera",
            Enemy = "Enemy",
            Panel = "Panel",
            Ice = "Ice";
        }

        public readonly struct Animations
        {
            public static string
            Idle = "Idle",
            Jump = "Jump",
            Walk = "Walk";
        }

        public readonly struct SpawnPositions
        {
            public static Vector2
            Stage1 = Coordinate.zero;
        }
    }
}