#region Using Statements
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion
namespace Roids
{
    static class Art
    {
        public static Texture2D PlayerShip { get; private set; }
        public static Texture2D Trail { get; private set; }
        public static Texture2D Projectile { get; private set; }
        public static Texture2D Saucer { get; private set; }
        public static Texture2D[,] Asteroids { get; private set; }

        public static SpriteFont DebugFont { get; private set; }
        public static SpriteFont TitleFont { get; private set; }
        public static SpriteFont SubFont { get; private set; }

        public static void Load(ContentManager content)
        {
            PlayerShip = content.Load<Texture2D>("Art/PlayerShip");
            Trail = content.Load<Texture2D>("Art/Trail");
            Projectile = content.Load<Texture2D>("Art/Projectile");
            Saucer = content.Load<Texture2D>("Art/Saucer");
            // Load a 2d array of Textures, representing variation and size of asteroids.
            Asteroids = new Texture2D[8,3];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 3; j++)
                    Asteroids[i, j] = content.Load<Texture2D>("Art/Asteroid" + i + "-" + (j+1));
            // Load Fonts
            DebugFont = content.Load<SpriteFont>("Fonts/DebugFont");
            TitleFont = content.Load<SpriteFont>("Fonts/TitleFont");
            SubFont = content.Load<SpriteFont>("Fonts/SubFont");
        }
    }
}
