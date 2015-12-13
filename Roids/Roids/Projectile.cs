#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion
namespace Roids
{
    class Projectile
    {
        #region Class Variables
        public int Radius;
        public Vector2 Position;

        private Texture2D m_image;
        private Vector2 m_position, m_velocity, m_center;
        private bool screenWrap;
        public float Lifetime, TimeSinceSpawn;

        public Color[] TexData;
        public Matrix Transform;
        #endregion

        #region Constructor
        public Projectile(int xPos, int yPos, Vector2 direction, float lifetime, bool wrapScreen = true)
        {
            direction.Normalize();
            m_image = Art.Projectile;
            m_position = new Vector2(xPos, yPos);
            // Offset spawn position to front of ship rather than center point.
            m_position += direction * 15f;
            m_velocity = direction * 6f;
            m_center = new Vector2(m_image.Width / 2, m_image.Height / 2);
            screenWrap = wrapScreen;
            Lifetime = lifetime;
            Radius = m_image.Width / 2;

            // This is used for collision detection. NOT MINE
            #region For Per Pixel Check. This is not mine.
            TexData = new Color[m_image.Width * m_image.Height];
            m_image.GetData(TexData);
            #endregion
        }
        #endregion

        #region Update and Draw
        public void Update()
        {
            TimeSinceSpawn += 1 / 60f;
            m_position += m_velocity;
            Position = m_position;

            #region Screen Wrap
            // Screen Wrap if required.
            if (screenWrap)
            {
                if (m_position.X + Radius < 0)
                    m_position.X = GameRoot.WIDTH - 1;
                if (m_position.X > GameRoot.WIDTH + Radius)
                    m_position.X = 1 - Radius;
                if (m_position.Y + Radius < 0)
                    m_position.Y = GameRoot.HEIGHT - 1;
                if (m_position.Y > GameRoot.HEIGHT + Radius)
                    m_position.Y = 1 - Radius;
            }
            else
            {
                if (m_position.X + Radius < -10)
                    m_position.X = -10;
                if (m_position.X > GameRoot.WIDTH + Radius + 10)
                    m_position.X = GameRoot.WIDTH + Radius + 10;
                if (m_position.Y + Radius < -10)
                    m_position.Y = -10;
                if (m_position.Y > GameRoot.HEIGHT + Radius + 10)
                    m_position.Y = GameRoot.HEIGHT + Radius + 10;
            }
            #endregion  

            // Create Projectile transform. NOT MINE!
            #region For Per Pixel Check. This is not mine, from Riemer.
            Transform =
                Matrix.CreateTranslation(new Vector3(-m_center, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(m_position, 0.0f));
            #endregion
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(m_image, m_position, null, Color.White, 0, m_center, 1, SpriteEffects.None, 0.1f);
        }
        #endregion
    }
}
