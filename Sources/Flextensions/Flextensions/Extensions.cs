using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flextensions
{
    public static class Extensions
    {
        /// <summary>
        /// Gets an angle in radians of the given vector.
        /// </summary>
        /// <param name="vector">The vector to get the angle of.</param>
        /// <returns>The angle to point a sprite at along a given vector.</returns>
        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
    }
}
