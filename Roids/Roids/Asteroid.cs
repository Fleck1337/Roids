#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion
namespace Roids
{
    class Asteroid
    {
        #region Class Variables
        public int Radius, Size;
        public Vector2 Position;

        private Texture2D m_asteroidImage;
        private Vector2 m_position, m_velocity, m_center;
        private float m_rotation, m_rotationSpeed;
        private int xDir = 1, yDir = 1;

        // These are used for Per Pixel Detection
        public Color[] roidTexData;
        public Matrix roidTransform;
        #endregion

        #region Constructor
        public Asteroid(Texture2D image, int xPos, int yPos, int size, Random RNG)
        {
            // Get random X and Y directions.
            if (RNG.Next(2) > 0)
                xDir *= -1;
            if (RNG.Next(2) > 0)
                yDir *= -1;
            // Set variables. Some to random values.
            m_asteroidImage = image;
            m_position = new Vector2(xPos, yPos);
            m_velocity = new Vector2((float)RNG.NextDouble() * xDir, (float)RNG.NextDouble() * yDir);
            m_velocity = m_velocity * 1 / size; // Smaller asteroids go faster.
            m_rotation = (float)RNG.Next(6);    // Start at a random stepped rotation between 0 and nearly 2pi.
            m_rotationSpeed = (float)(RNG.NextDouble() * 0.05);
            m_center = new Vector2(image.Width / 2, image.Height / 2);
            Size = size;
            Position = m_position;
            Radius = (int)(image.Width / 2);

            // This is used for collision detection. NOT MINE
            #region For Per Pixel Check. This is not mine.
            roidTexData = new Color[m_asteroidImage.Width * m_asteroidImage.Height];
            m_asteroidImage.GetData(roidTexData);
            #endregion
        }
        #endregion

        #region Update and Draw
        public void Update()
        {
            #region Update Position
            // Use yDir to sometimes rotate other way.
            m_rotation += m_rotationSpeed * yDir;
            m_position += m_velocity * 2;
            Position = m_position;
            #endregion

            // Create Asteroid transform. NOT MINE
            #region For Per Pixel Check. This is not mine, from Riemer.
            roidTransform =
                Matrix.CreateTranslation(new Vector3(-m_center, 0.0f)) *
                Matrix.CreateRotationZ(m_rotation) *
                Matrix.CreateTranslation(new Vector3(m_position, 0.0f));
            #endregion

            #region Screen Wrap
            // Screen Wrap.
            if (m_position.X + Radius < 0)
                m_position.X = GameRoot.WIDTH - 1;
            if (m_position.X > GameRoot.WIDTH + Radius)
                m_position.X = 1 - Radius;
            if (m_position.Y + Radius < 0)
                m_position.Y = GameRoot.HEIGHT - 1;
            if (m_position.Y > GameRoot.HEIGHT + Radius)
                m_position.Y = 1 - Radius;
            #endregion
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(m_asteroidImage, m_position, null, Color.White, m_rotation, m_center, 1, SpriteEffects.None, 0.1f);
        }
        #endregion
    }
}
