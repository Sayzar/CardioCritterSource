using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;

namespace CardioCritters
{
    [Serializable]
    public struct SaveGame
    {
        public int level; 
        public int experience;
        public int fullness;
        public int calmness;
        public int energy;

    }

    public static class FileManager
    {
        static string fileName = "save.file";
        static SaveGame currentSave;

        static FileManager()
        {
            currentSave = Load();

            // TODO :
            // saving and loading in windows mode in the Save and Load methods
        }

        public static void AddExperience(int amount)
        {
            currentSave.experience += amount;
            currentSave.experience = (int)MathHelper.Clamp(currentSave.experience, 0, 100);
            // Update Level up
        }
        public static void AddFullness(int amount)
        {
            currentSave.fullness += amount;
            currentSave.fullness = (int)MathHelper.Clamp(currentSave.fullness, 0, 100);
        }
        public static void AddCalmness(int amount)
        {
            currentSave.calmness += amount;
            currentSave.calmness = (int)MathHelper.Clamp(currentSave.calmness, 0, 100);
        }
        public static void AddEnergy(int amount)
        {
            currentSave.energy += amount;
            currentSave.energy = (int)MathHelper.Clamp(currentSave.energy, 0, 100);
        }

        public static void Save()
        {
            Save(currentSave);
        }

        public static void Save(SaveGame data)
        {
#if !WINDOWS
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Check to see whether the save exists.
                if (storage.FileExists(fileName))
                    // Delete it so that we can create one fresh.
                    storage.DeleteFile(fileName);

                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
                {
                    // Convert the object to XML data and put it in the stream.
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));

                    serializer.Serialize(fileStream, data);

                }
            }
#else

#endif
        }

        public static SaveGame Load()
        {
#if !WINDOWS
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Check to see whether the save exists.
                if (!storage.FileExists(fileName))
                {
                    SaveGame newGame = new SaveGame();
                    // newgame data
                    newGame.level = 1;
                    newGame.experience = 0;
                    newGame.fullness = 100;
                    newGame.calmness = 100;
                    newGame.energy = 100;
                    return newGame;
                }

                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage))
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                    
                    return (SaveGame)serializer.Deserialize(fileStream);
                }
            }
#else
            return new SaveGame();
#endif
        }
    }
}
