using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaUtility;

namespace CardioCritters
{
    class SSAnimatedEntity
    {
        String baseName;
        Texture2D spriteSheet;
        Dictionary<String, List<TextureRegion>> animations = new Dictionary<String, List<TextureRegion>>();
        //Should be a better way to do this... One string maps to a list of texture regions and a set of animInfo...
        Dictionary<String, AnimInfo> animInfos = new Dictionary<String, AnimInfo>();
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public float Rotation { get; set; }

        public String CurAnimName { get; set; }

        /// <summary>
        /// -1 is unset, otherwise override current animation's frame with this number
        /// </summary>
        public int FrameOverride;

        private int curFrameNum = 0;

        struct AnimInfo
        {
            public float totalLength;
            public bool isLooping;
            public AnimInfo(float totalLength, bool isLooping)
            {
                this.totalLength = totalLength;
                this.isLooping = isLooping;
            }
        }

        public SSAnimatedEntity(String baseName, String itemName, String[] animationNames)
        {
            FrameOverride = -1;
            this.CurAnimName = animationNames[0];
            this.baseName = baseName;
            //TODO: Use preloaded stuff
            spriteSheet = Utility.Content.Load<Texture2D>(baseName);
            TextureAtlas textureAtlas = new TextureAtlas(baseName + "_XML.xml");

            foreach (String animName in animationNames)
            {
                int count = 1;
                TextureRegion animFrame;
                while (true)
                {
                    animFrame = textureAtlas.GetRegion(itemName + "/" + animName + "/" + count + ".png");
                    if (animFrame != null)
                    {
                        AddAnimationFrame(animName, animFrame);
                    }
                    else
                    {
                        count = 1;
                        break; //Finished loading all this animation's frames
                    }
                    count++;
                }
            }
        }

        /// <summary>
        /// This function assumes animation frames will be added in order.
        /// </summary>
        /// <param name="animName"></param>
        /// <param name="animFrame"></param>
        private void AddAnimationFrame(String animName, TextureRegion animFrame)
        {
            List<TextureRegion> frames;
            if (!animations.TryGetValue(animName, out frames))
            { //If this animation doesn't exist yet, initialize it
                frames = new List<TextureRegion>();
                animations.Add(animName, frames);
            }
            frames.Add(animFrame);
        }

        public void SetAnimationInfo(String animName, float totalLength, bool isLooping)
        {
            List<TextureRegion> frames;
            if (!animations.TryGetValue(animName, out frames))
            { //If this animation doesn't exist, error
                throw new Exception("Trying to set animation info for an animation that doesn't exist");
            }
            animInfos.Add(animName, new AnimInfo(totalLength, isLooping));
        }

        public void Update(GameTime gameTime)
        {
            //TODO: Update current frame's animation
            //TODO: Create Animation subclass and Entity subclass that makes use of spritesheet system. Right now it's just standalong kinda hacked in class...
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureRegion curFrame;
            if (FrameOverride == -1)
                curFrame = animations[CurAnimName][curFrameNum];
            else
                curFrame = animations[CurAnimName][FrameOverride];
            spriteBatch.Draw(spriteSheet, Position, curFrame.Bounds, Color.White,
                Rotation, curFrame.OriginTopLeft, Scale, SpriteEffect, 0);
        }
    }
}
