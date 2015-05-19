using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace CardioCritters.Code.Main_and_Misc
{
    public class MusicManager
    {
        private Microsoft.Xna.Framework.Media.Song currentSong;
        private bool isLooping;

        private List<SoundEffectInstance> effects;

        private TimeSpan showTime;
        private TimeSpan timeLeft;

        private float musicvolume;

        /// <summary>
        /// Media player volume, from 0.0f (silence) to 1.0f (full volume relative to the current device volume).
        /// </summary>
        public float MusicVolume
        {
            get { return musicvolume; }
            set 
            { 
                musicvolume = value; 
                MediaPlayer.Volume = value; 
            }
        }

        private float soundvolume;

        public float SoundVolume
        {
            get { return soundvolume; }
            set { soundvolume = value; }
        }

        /// <summary>
        /// Prepare the music player.
        /// </summary>
        public MusicManager()
        {
            MusicVolume = 1.0f;
            SoundVolume = 1.0f;

            effects = new List<SoundEffectInstance>();

            showTime = new TimeSpan(0, 0, 3);
        }

        private TimeSpan sampleDelay = new TimeSpan(0, 0, 60);
        private TimeSpan lastPlay;

        public bool IsPlaying(GameTime gameTime)
        {
            //if (MediaPlayer.State == MediaState.Playing && gameTime.TotalGameTime > lastPlay + sampleDelay)
            //{
            //    lastPlay = gameTime.TotalGameTime;
            //    return true;
            //}
            //else
            //    return false;
            return MediaPlayer.State == MediaState.Playing;
        }

        /// <summary>
        /// Play a song.
        /// </summary>
        /// <param name="song"></param>
        public void PlayMusic(Microsoft.Xna.Framework.Media.Song song)
        {
            if (currentSong == null)
                timeLeft = showTime;
            currentSong = song;
            if (MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(currentSong);
                MediaPlayer.IsRepeating = true;
            }
        }

        public void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        public void ResumeMusic()
        {
            MediaPlayer.Resume();
        }

        /// <summary>
        /// Tell the current song to loop.
        /// </summary>
        public void Loop()
        {
            isLooping = true;
        }

        public void Stop()
        {
            MediaPlayer.Stop();
            isLooping = false;
            currentSong = null;
        }

        public void StopAll()
        {
            effects.Clear();
            Stop();
        }

        public SoundEffectInstance PlaySound(SoundEffect soundEffect)
        {
            return PlaySound(soundEffect, 1.0f);
        }

        public SoundEffectInstance PlaySound(SoundEffect soundEffect, float volume)
        {
            SoundEffectInstance instance = soundEffect.CreateInstance();
            instance.Volume = SoundVolume * volume;
            instance.Play();
            effects.Add(instance);
            return instance;
        }

        public void Update(GameTime gameTime)
        {
            //Update music infobox
            if (timeLeft > TimeSpan.Zero)
            {
                timeLeft -= gameTime.ElapsedGameTime;
                if (timeLeft < TimeSpan.Zero)
                    timeLeft = TimeSpan.Zero;
            }

            //Loop music
            if (isLooping && currentSong != null)
                if (MediaPlayer.State == MediaState.Stopped)
                    MediaPlayer.Play(currentSong);

            //Remove done sound effects
            for (int c = effects.Count - 1; c >= 0; c--)
            {
                if (effects[c].State == SoundState.Stopped)
                {
                    effects[c].Dispose();
                    effects.RemoveAt(c);
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
        }
    }
}
