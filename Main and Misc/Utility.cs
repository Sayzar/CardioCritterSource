using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CardioCritters.Screens;
using Microsoft.Xna.Framework;
using CardioCritters.Code.Main_and_Misc;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace CardioCritters
{
    public static class Utility
    {
        // Access to the content manager
        public static ContentManager Content;
        public static InputManager inputManager;
        public static MusicManager musicManager;
        public static int ScreenWidth, ScreenHeight;
        //public static Color clearColor = Color.White;
        public static bool showBoundingBoxes = false;
        public static int MINIGAME_HUNGER_POINTS = 0,MINIGAME_INERNAL_POINTS = 0,MINIGAME_ENERGY_POINTS = 0;


        static Utility()
        {
            inputManager = new InputManager();
            musicManager = new MusicManager();
        }

        public static Dictionary<String, Song> songAtlas = new Dictionary<String, Song>();
        public static Dictionary<String, SoundEffect> soundEffectAtlas = new Dictionary<String, SoundEffect>();

        public static Dictionary<String, Texture2D> textureAtlas = new Dictionary<String, Texture2D>();

        public static Texture2D[] LoadAnimationFromSpriteSheet(String filepath)
        {
            AddNewTexture("AccessoriesSS");
            return new Texture2D[5];
        }

        // Given the content manager, filepath to the folder and animation prefix, gets the appropriate number of frames,
        // and returns an array of the texutre2Ds.
        public static Texture2D[] LoadAnimation(String filepath, int frames)
        {
            Texture2D[] result = new Texture2D[frames];
            for (int i = 1; i <= frames; i++)
            {
                result[i - 1] = Utility.GetTexture(filepath + i);
                Utility.AddNewTexture(filepath + i);
            }
            return result;
        }

        public static Texture2D GetTexture(String name)
        {
            // we should just always try to add when we're getting a texture...
            // this is mainly because sometimes people get textures but they forget to create yet
            // it shouldn't be a bad modification since AddNewTexture doesn't create it if it exists already
            AddNewTexture(name);

            Texture2D toReturn;
            textureAtlas.TryGetValue(name, out toReturn);
            return toReturn;
        }

        // Tries adding a texture
        public static bool AddNewTexture(String path)
        {
            if (textureAtlas.ContainsKey(path)) return false;

            textureAtlas.Add(path, Content.Load<Texture2D>(path));
            return true;
        }

        public static Song GetSong(String name)
        {
            Song toReturn;
            songAtlas.TryGetValue(name, out toReturn);
            return toReturn;
        }

        public static bool AddNewSong(String path)
        {
            if (soundEffectAtlas.ContainsKey(path)) return false;
            
            songAtlas.Add(path, Content.Load<Song>(path));
            return true;
        }

        public static SoundEffect GetSoundEffect(String name)
        {
            SoundEffect toReturn;
            soundEffectAtlas.TryGetValue(name, out toReturn);
            return toReturn;
        }

        public static bool AddNewSoundEffect(String path)
        {
            if (soundEffectAtlas.ContainsKey(path)) return false;

            soundEffectAtlas.Add(path, Content.Load<SoundEffect>(path));
            return true;
        }
    }
}