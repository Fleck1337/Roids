using System;
using System.Collections.Generic;
#region Using Statements
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
#endregion
namespace Roids
{
    class AllowedKeys
    {
        public static bool IsAllowed(Keys key)
        {
            // Returns true if an alphabetical(En qwerty) key.
            switch (key)
            {
                case Keys.A:
                    return true;
                case Keys.B:
                    return true;
                case Keys.C:
                    return true;
                case Keys.D:
                    return true;
                case Keys.E:
                    return true;
                case Keys.F:
                    return true;
                case Keys.G:
                    return true;
                case Keys.H:
                    return true;
                case Keys.I:
                    return true;
                case Keys.J:
                    return true;
                case Keys.K:
                    return true;
                case Keys.L:
                    return true;
                case Keys.M:
                    return true;
                case Keys.N:
                    return true;
                case Keys.O:
                    return true;
                case Keys.P:
                    return true;
                case Keys.Q:
                    return true;
                case Keys.R:
                    return true;
                case Keys.S:
                    return true;
                case Keys.T:
                    return true;
                case Keys.U:
                    return true;
                case Keys.V:
                    return true;
                case Keys.W:
                    return true;
                case Keys.X:
                    return true;
                case Keys.Y:
                    return true;
                case Keys.Z:
                    return true;
                default:
                    return false;
            }
        }
    }
}
