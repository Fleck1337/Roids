#region Using Statements
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
#endregion
namespace Roids
{
    static class Sound
    {
        public static SoundEffect Select { get; private set; }
        public static SoundEffect Back { get; private set; }
        public static SoundEffect Hit { get; private set; }
        public static SoundEffect Explode { get; private set; }
        public static SoundEffect Shoot { get; private set; }
        public static SoundEffect GameOver { get; private set; }

        public static Song Music { get; private set; }

        public static void Load(ContentManager content)
        {
            Select = content.Load<SoundEffect>("Sound/Select");
            Back = content.Load<SoundEffect>("Sound/Back");
            Hit = content.Load<SoundEffect>("Sound/Hit");
            Explode = content.Load<SoundEffect>("Sound/Explode");
            Shoot = content.Load<SoundEffect>("Sound/Laser_Shoot");
            GameOver = content.Load<SoundEffect>("Sound/GameOver");

            Music = content.Load<Song>("Sound/The Blue Danube");
        }
    }
}
