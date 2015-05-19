using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CardioCritters
{
    public class AnimatedEntity : Entity
    {
        protected Dictionary<string, Animation> animations;
        protected Animation currentAnimation;
        public string CurrentAnimationName 
        { 
            get 
            { 
                return currentAnimationName; 
            } 
        }
        private string currentAnimationName;
        public String IdleAnimationName { get; set; }

        public AnimatedEntity(String filepath)
            : base(filepath)
        {
            //currentAnimation = idleAnimation = new Animation(sprite, 1, true);
            animations = new Dictionary<string, Animation>();
        }

        public void AddAnimation(String name, String filepath, int numFrames, float length, bool loop)
        {
            Animation a = new Animation(Utility.LoadAnimation(filepath, numFrames), length, loop);
            animations.Add(name, a);
        }

        public virtual void PlayAnimation(String name)
        {
            animations.TryGetValue(name, out currentAnimation);
            if (currentAnimation == null)
                throw new Exception("Trying to play a non-exisiting animation");
            currentAnimationName = name;
            currentAnimation.Start();
        }

        public override void Update(GameTime gameTime)
        {
            currentAnimation.Update(gameTime);

            if (currentAnimation.isDone)
                PlayAnimation(IdleAnimationName);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            sprite = currentAnimation.GetSprite();
            base.Draw(sb);
        }

        // a specific draw function we could use for drawing the accessories
        public void Draw(SpriteBatch sb, int indexToDraw)
        {
            sprite = currentAnimation.GetSprite(indexToDraw);
            base.Draw(sb);
        }
    }
}