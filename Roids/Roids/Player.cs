#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Flextensions;
#endregion
namespace Roids
{
    class Player
    {
        #region Class Variables
        public int Radius;
        public float Rotation, TimeSinceSpawn;
        public Vector2 Position, Velocity;

        public Color[] PlayerTexData;
        public Matrix shipTransform;

        private Texture2D m_shipImage;
        private Vector2 m_position, m_velocity, m_direction, m_center;
        #endregion

        #region Constructor
        public Player(Texture2D image, int xPos, int yPos)
        {
            m_shipImage = image;
            m_position = new Vector2(xPos, yPos);
            m_velocity = Vector2.Zero;
            m_direction = Vector2.Zero;
            m_center = new Vector2(image.Width / 2, image.Height / 2);
            Rotation = (3f * (float)Math.PI)/2f;      // Rotate by 270 degrees so the ship faces up on spawn.
            Position = m_position;
            Radius = image.Width / 2;

            // This is used for collision detection. NOT MINE
            #region For Per Pixel Check. This is not mine.
            PlayerTexData = new Color[m_shipImage.Width * m_shipImage.Height];
            m_shipImage.GetData(PlayerTexData);
            #endregion
        }
        #endregion

        #region Update and Draw
        public void Update(float friction = 0f)
        {
            TimeSinceSpawn += 1 / 60f;
            // Create a direction vector using our rotation value. Sprites face right at Zero rotation.
            m_direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            m_direction.Normalize();

            #region Player Input
            // Handle Input.
            float throttle = Input.GetRightTrigger();
            if (throttle >= 0.1f)    // Deadzone.
            {
                // Add to current velocity if Right Trigger is pressed.
                m_velocity += (m_direction * 0.2f * throttle);
            }
            if (Input.IsKeyDown(Keys.W))
            {
                m_velocity += m_direction * 0.2f;
            }
            
            // Handle rotation.
            if (Input.IsButtonDown(Buttons.DPadLeft) || Input.IsKeyDown(Keys.A))
                Rotation -= 0.1f;
            if (Input.IsButtonDown(Buttons.DPadRight) || Input.IsKeyDown(Keys.D))
                Rotation += 0.1f;
            if (Input.GetLeftStick().X < -0.1f)
                Rotation += 0.1f * Input.GetLeftStick().X;
            if (Input.GetLeftStick().X > 0.1f)
                Rotation += 0.1f * Input.GetLeftStick().X;
            #endregion

            // Apply friction-like effect.
            m_velocity *= 1-friction;

            #region Cap Velocity
            // Limit Velocity.
            if (m_velocity.Length() > 9)
            {
                m_velocity.Normalize();
                m_velocity *= 9;
            }
            if (m_velocity.Length() < 0.1)
                m_velocity = Vector2.Zero;
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

            #region Update Player Position
            // Update position.
            m_position += m_velocity;
            Position = m_position;
            Velocity = m_velocity;
            #endregion

            // Create player transform. NOT MINE
            #region For Per Pixel Check. This is not mine, from Riemer.
            shipTransform =
                Matrix.CreateTranslation(new Vector3(-m_center, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(m_position, 0.0f));
            #endregion
        }

        public void Draw(SpriteBatch sb)
        {
            Color col = Color.White;
            col *= TimeSinceSpawn / 3;
            sb.Draw(m_shipImage, m_position, null, col, Rotation, m_center, 1, SpriteEffects.None, 0.1f);
        }
        #endregion
    }
}
