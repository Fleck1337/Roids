#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion
namespace Roids
{
    class Saucer
    {
        #region Class Variables
        public int Radius;
        public Vector2 Position;
        public List<Projectile> EnemyProjectiles;

        private int m_shootMaxOffset;
        private float timeToShoot, shootTimer;
        private Texture2D m_saucerImage;
        private Vector2 m_center, m_playerDir;
        private Random RNG;

        public Color[] SaucerTexData;
        public Matrix saucerTransform;
        #endregion

        #region Constructor
        public Saucer(Texture2D image, Random rng)
        {
            m_saucerImage = image;
            Position = new Vector2(GameRoot.WIDTH + image.Width, 25);
            m_center = new Vector2(image.Width / 2, image.Height / 2);
            RNG = rng;
            timeToShoot = 1f;
            shootTimer = 0f;

            Radius = image.Width / 2;
            EnemyProjectiles = new List<Projectile>();

            // This is used for collision detection. NOT MINE
            #region For Per Pixel Check. This is not mine.
            SaucerTexData = new Color[m_saucerImage.Width * m_saucerImage.Height];
            m_saucerImage.GetData(SaucerTexData);
            #endregion
        }
        #endregion

        #region Custom Functions
        public void ShootAt(Vector2 position, Random rand)
        {
            // Using the player position, calculate and reduce the Direction Vector.
            m_playerDir = position - Position;
            m_playerDir.Normalize();
            m_playerDir /= 2;
            RNG = rand;
            m_shootMaxOffset = 50;
            // Create new projectile in direction of player
            EnemyProjectiles.Add(new Projectile((int)Position.X, (int)Position.Y, m_playerDir, 5f, false));
        }
        #endregion

        #region Update and Draw
        public void Update(Vector2 playerPos, Random rand)
        {
            #region Update Position
            Position.X -= 2.5f;
            Position.Y = (float)(Math.Sin(Position.X * 5) * 50) + 50;
            // Once offscreen keep offscreen til we respawn him.
            if (Position.X <= -100)
                Position.X = -100;
            #endregion

            #region Shoot on Timer
            shootTimer += 1 / 60f;
            if (shootTimer >= timeToShoot && Position.X > 0)
            {
                // Shmuggle Player Position.
                playerPos.X += RNG.Next(-m_shootMaxOffset, m_shootMaxOffset);
                playerPos.Y += RNG.Next(-m_shootMaxOffset, m_shootMaxOffset);
                shootTimer = 0;
                ShootAt(playerPos, rand);
            }
            #endregion

            #region Remove Projectiles when expired.
            for (int i = 0; i < EnemyProjectiles.Count(); i++)
            {
                EnemyProjectiles[i].Update();
                if (EnemyProjectiles[i].TimeSinceSpawn > EnemyProjectiles[i].Lifetime)
                    EnemyProjectiles.RemoveAt(i);
            }
            #endregion

            // Create saucer transform. NOT MINE!
            #region For Per Pixel Check. This is not mine, from Riemer.
            saucerTransform =
                Matrix.CreateTranslation(new Vector3(-m_center, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));
            #endregion
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(m_saucerImage, Position, Color.White);
        }
        #endregion
    }
}
