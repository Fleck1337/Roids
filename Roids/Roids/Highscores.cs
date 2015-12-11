#region File Description
//-----------------------------------------------------------------------------
// Highscores.cs
//
// Based on code from a tutorial at:
// http://xnaessentials.com/page/highscores.aspx
// By Chad Carter.
//-----------------------------------------------------------------------------
#endregion
#region Using Statements
using System;
using System.IO;
using System.Xml.Serialization;
#endregion
namespace Roids
{
    public class Highscores
    {
        [Serializable]
        public struct HighscoreData
        {
            public string[] Names;
            public int[] Scores;

            public int Count;

            public HighscoreData(int count)
            {
                Names = new string[count];
                Scores = new int[count];

                Count = count;
            }
        }

        public static void SaveHighScores(HighscoreData data, string filename)
        {
            // Get the path of the file to save to.
            string fullpath = Path.Combine("", filename);

            // Open the file, creating it if it doesn't exist.
            // DO NOT use FileMode.OpenOrCreate as it can cause corrupt files if new is shorter than the last,
            // as described here: http://xboxforums.create.msdn.com/forums/p/22451/131751.aspx
            FileStream stream = File.Open(fullpath, FileMode.Create);
            try
            {
                // Convert the object to XML data and put it in the stream.
                XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close the file.
                stream.Close();
            }
        }

        public static HighscoreData LoadHighScores(string filename)
        {
            HighscoreData data;

            // Get the path of the save game.
            string fullpath = Path.Combine("", filename);

            // Open the file.
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {
                // Read the data from the file.
                XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
                data = (HighscoreData)serializer.Deserialize(stream);
            }

            finally
            {
                // Close the file.
                stream.Close();
            }

            return (data);
        }

        public static void Init(string filename)
        {
            // Get the path of the save game.
            string fullpath = Path.Combine("", filename);

            // Check to see if the save exists.
            if (!File.Exists(fullpath))
            {
                //If the file doesn't exist, make a fake one...
                CreateFile(filename);
            }
        }

        public static void CreateFile(string filename)
        {
            // Create dummy data to save.
            HighscoreData data = new HighscoreData(5);
            data.Names[0] = "Bob";
            data.Scores[0] = 200;

            data.Names[1] = "Nathan";
            data.Scores[1] = 180;

            data.Names[2] = "Ed";
            data.Scores[2] = 130;

            data.Names[3] = "Frank";
            data.Scores[3] = 90;

            data.Names[4] = "Mike";
            data.Scores[4] = 10;

            SaveHighScores(data, filename);
        }

        // Returns true if the input score is larger than one in the list.
        public static bool CheckHighScore(string filename, int score)
        {
            HighscoreData data = LoadHighScores(filename);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.Scores[i])
                {
                    scoreIndex = i;
                    break;
                }
            }
            if (scoreIndex > -1)
            {
                return true;
            }
            else
                return false;
        }

        public static void SaveHighScore(string filename, int score, string name)
        {
            // Create the data to save
            HighscoreData data = LoadHighScores(filename);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.Scores[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.Names[i] = data.Names[i - 1];
                    data.Scores[i] = data.Scores[i - 1];
                }

                data.Names[scoreIndex] = name; //Retrieve User Name Here
                data.Scores[scoreIndex] = score;

                SaveHighScores(data, filename);
            }
        }

    }
}
