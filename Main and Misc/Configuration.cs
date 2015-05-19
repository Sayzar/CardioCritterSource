using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardioCritters
{
    public static class Configuration
    {
        public static int OPACITY_CHANGE = 3;
        public static int SOLID_TIME = 1500;

        // FALLING GRUB
        // NOTE: SPEEDS, TO_SPAWN_TIME_DIFFERENCE_IN_MS, BAD_SPAWN_RATIO & DIFFICULTY_BY_POINTS
        // must have the same length
        public static float FF_FALLING_ITEM_WIDTH = 0.1f;
        public static float FF_FALLING_ITEM_HEIGHT = 0.15f;
        public static int[] FF_SPEEDS = { 2, 4, 6, 8 };
        public static int[] FF_TO_SPAWN_TIME_DIFFERENCE_IN_MS = { 3000, 2000, 1500, 1000 };
        public static float[] FF_BAD_SPAWN_RATIO = { 0.2f, 0.45f, 0.65f, 0.85f };
        public static int[] FF_DIFFICULTY_BY_POINTS = { 1000, 2000, 4000, 8000 };
        public static int FF_POINTS_PER_ITEM = 100;
        public static float FF_ACCELEROMETER_CHANGE_NEEDED_TO_MOVE = 0.25f;
        public static int FF_MOVEMENT_SPEED = 9;

        // CHOLESTEROL HUNTER
        public static float[] CC_ENEMY_SIZES_X = { 0.10f, 0.05f, 0.025f };
        public static float[] CC_ENEMY_SIZES_Y = { 0.15f, 0.075f, 0.0375f };
        public static float[] CC_ENEMY_SPEEDS = { 0.01f, 0.005f, 0.001f };
        public static float[] CC_HEART_SIZES_X = { 0.10f, 0.05f, 0.025f };
        public static float[] CC_HEART_SIZES_Y = { 0.15f, 0.075f, 0.0375f };
        public static int[] CC_KILLS_NEEDED_TO_GROW = { 0, 30, 70 };
        public static float CC_HUNTER_SIZE_X = 0.10f;
        public static float CC_HUNTER_SIZE_Y = 0.15f;
        public static float[] CC_HUNTER_SPEEDS = { 0.02f, 0.01f, 0.002f };
        public static int[] CC_TO_SPAWN = { 1, 2, 3, 3, 5, 6 };
        public static int CC_GAME_TIME = 60;
        public static int CC_SPAWN_INTERVAL = 1;
        public static float CC_TILT_NEEDED = 0.25f;
    }
}