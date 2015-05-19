using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace CardioCritters
{
    public class Animation
    {
        public Texture2D[] sprites;
        public float length;
        public bool loop, isDone;
        public int renderIndex;
        public Stopwatch timer;

        public Animation(Texture2D[] sprites, float length, bool loop)
        {
            this.sprites = sprites;
            this.loop = loop;
            this.length = length;
            renderIndex = 0;
            timer = new Stopwatch();
            isDone = false;
        }

        public Animation(Texture2D sprite, float length, bool loop)
            : this(new Texture2D[] {sprite}, length, loop)
        {
        }

        public void Start()
        {
            timer.Start();
            renderIndex = 0;
            isDone = false;
        }

        public void StopAndReset()
        {
            timer.Stop();
            timer.Reset();
            renderIndex = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (isDone && !loop) return;

            if (timer.ElapsedMilliseconds >= (length * 1000) / (float) sprites.Length)
            {
                renderIndex++;
                if (renderIndex >= sprites.Length && loop)
                {
                    StopAndReset();
                    Start();
                }
                else if (renderIndex >= sprites.Length)
                {
                    isDone = true;
                    timer.Stop();
                }

                timer.Reset();
                timer.Start();
            }
        }

        public void SetIsLooping(bool loop)
        {
            this.loop = loop;
        }

        public Texture2D GetSprite()
        {
            return sprites[renderIndex];
        }

        public Texture2D GetSprite(int index)
        {
            if (index < 0 || index >= sprites.Length)
                return null;

            return sprites[index];
        }

    }
}