using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace CardioCritters.Code.Minigames
{
    public class Minigame : Screen
    {
        // Describes the current state of the minigame
        public enum STATE
        {
            INSTRUCTIONS,
            GAME,
            GAMEOVER
        }

        private STATE state;
        private int currentInstruction;
        private Texture2D[] instructions;
        private Texture2D background, gameOverBad, gameOverGood;
        public Vector2 scorePosition;
        public bool win;
        public float score;

        protected int amtToUpdateExperience, amtToUpdateFullness, amtToUpdateCalmness, amtToUpdateEnergy;

        public Minigame(String[] instructions, String background, String gameOverBad, String gameOverGood, Vector2 scorePosition)
            : base(instructions[0])
        {
            this.state = STATE.INSTRUCTIONS;
            this.instructions = new Texture2D[instructions.Length];
            for (int i = 0; i < instructions.Length; i++)
                this.instructions[i] = Utility.GetTexture(instructions[i]);
            this.background = Utility.GetTexture(background);
            this.gameOverBad = Utility.GetTexture(gameOverBad);
            this.gameOverGood = Utility.GetTexture(gameOverGood);
            this.scorePosition = scorePosition;
            this.win = false;

            amtToUpdateExperience = amtToUpdateFullness = amtToUpdateCalmness = amtToUpdateEnergy = 0;
        }

        public void SetState(STATE state)
        {
            this.state = state;
            switch (state)
            {
                case STATE.INSTRUCTIONS:
                    this.staticBackground = instructions[0];
                    this.currentInstruction = 0;
                    break;
                case STATE.GAME:
                    this.staticBackground = background;
                    break;
                case STATE.GAMEOVER:
                    this.staticBackground = win ? gameOverGood : gameOverBad;
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            switch (state)
            {
                case STATE.INSTRUCTIONS:
                    this.staticBackground = instructions[currentInstruction];
                    DrawInstructions(sb);
                    break;
                case STATE.GAME:
                    DrawGame(sb);
                    break;
                case STATE.GAMEOVER:
                    DrawGameOver(sb);
                    break;
            }
            base.Draw(sb);
        }

        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE.INSTRUCTIONS:
                    UpdateInstructions(gameTime);
                    HandleInputInstructions();
                    break;
                case STATE.GAME:
                    UpdateGame(gameTime);
                    HandleInputGame();
                    break;
                case STATE.GAMEOVER:
                    UpdateGameOver(gameTime);
                    HandleInputGameOver();
                    break;
            }
        }

        // Overrite these methods to make your own minigame!
        public virtual void UpdateInstructions(GameTime gameTime) { }
        public virtual void UpdateGame(GameTime gameTime) { }
        public virtual void UpdateGameOver(GameTime gameTime) { }

        public virtual void DrawInstructions(SpriteBatch sb) { }
        public virtual void DrawGame(SpriteBatch sb) { }
        public virtual void DrawGameOver(SpriteBatch sb)
        {
            sb.DrawString(Game1.font,
                score.ToString(), 
                scorePosition, 
                Color.Black, 
                0f, 
                Vector2.Zero, 
                2f, 
                SpriteEffects.None, 
                0.1f);
        }

        public virtual void HandleInputInstructions()
        {
            // If we double tap, then go to next instruction
            if (Utility.inputManager.GetGesture(GestureType.DoubleTap)
                || (Utility.inputManager.currentKeyboardState.IsKeyUp(Keys.Space)
                && Utility.inputManager.previousKeyboardState.IsKeyDown(Keys.Space)))
            {
                currentInstruction++;
                if (currentInstruction >= instructions.Length) SetState(STATE.GAME);
            }
        }
        public virtual void HandleInputGame()
        {
        }

        public virtual void HandleInputGameOver()
        {
            // If we double tap, then go to next instruction
            if (Utility.inputManager.GetGesture(GestureType.DoubleTap)
                || Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Space))
            {
                // saving stuff after gameover
                FileManager.AddExperience(amtToUpdateExperience);
                FileManager.AddFullness(amtToUpdateFullness);
                FileManager.AddCalmness(amtToUpdateCalmness);
                FileManager.AddEnergy(amtToUpdateEnergy);
                FileManager.Save();

                ScreenStackManager.PopScene();
            }
        }
    }
}