#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flextensions;
#endregion

namespace Roids
{
    class Trail
    {
        #region Class Variables
        private Texture2D m_image;
        private Vector2 m_position, m_center;
        private float m_rotation, m_scale;
        public float Lifetime, TimeSinceSpawn;
        #endregion

        #region Constructor
        public Trail(int xPos, int yPos, Vector2 direction, float lifetime, float scale = 1)
        {
            m_image = Art.Trail;
            m_position = new Vector2(xPos, yPos);
            // Move to rear of ship.
            direction.Normalize();
            m_position -= direction * 25;
            m_center = new Vector2(m_image.Width / 2, m_image.Height / 2);
            Lifetime = lifetime;
            m_rotation = direction.ToAngle();
            m_scale = scale;
        }
        #endregion

        #region Update and Draw
        public void Update()
        {
            TimeSinceSpawn += 1 / 60f;

        }

        public void Draw(SpriteBatch sb)
        {
            Color col = Color.White;
            col *= 1 - (TimeSinceSpawn / 3);
            sb.Draw(m_image, m_position, null, col, m_rotation, m_center, m_scale, SpriteEffects.None, 0.1f);
        }
        #endregion
    }
}
