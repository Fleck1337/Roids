using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flextensions
{
    /// <summary>
    /// A series of helper functions to get various input data.
    /// </summary>
    public static class Input
    {
        private static KeyboardState keyboardState, lastKeyboardState;
        private static MouseState mouseState, lastMouseState;
        private static GamePadState gamepadState, lastGamepadState;

        public static Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y); } }

        /// <summary>
        /// This must be called during Root Game's Update function to work.
        /// </summary>
        public static void Update()
        {
            // Set old state information, if first run, all are null.
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            lastGamepadState = gamepadState;
            // Get new state information.
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamepadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Returns true if the given key has been pressed once.
        /// </summary>
        /// <param name="key">The keyboard key to check for.</param>
        /// <returns>True if the key was pressed since the last frame.</returns>
        public static bool WasKeyPressed(Keys key)
        {
            return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }
        /// <summary>
        /// Returns true if the current key is being pressed down.
        /// </summary>
        /// <param name="key">The keyboard key to check for.</param>
        /// <returns>True as long as the key is held down.</returns>
        public static bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
        /// <summary>
        /// Returns true if the given gamepad button has been pressed once.
        /// </summary>
        /// <param name="button">The gamepad button to check for.</param>
        /// <returns>True if the button was pressed since the last frame.</returns>
        public static bool WasButtonPressed(Buttons button)
        {
            return lastGamepadState.IsButtonUp(button) && gamepadState.IsButtonDown(button);
        }
        /// <summary>
        /// Returns true if the given gamepad button is being pressed down.
        /// </summary>
        /// <param name="button">The gamepad button to check for.</param>
        /// <returns>True as long as the button is held down.</returns>
        public static bool IsButtonDown(Buttons button)
        {
            return gamepadState.IsButtonDown(button);
        }

        /// <summary>
        /// Returns a Vector2 of the Left analogue stick.
        /// </summary>
        public static Vector2 GetLeftStick()
        {
            Vector2 direction = gamepadState.ThumbSticks.Left;
            direction.Y *= -1;      // Invert the y axis

            return direction;
        }
        /// <summary>
        /// Returns a Vector2 of the Right analogue stick.
        /// </summary>
        public static Vector2 GetRightStick()
        {
            Vector2 direction = gamepadState.ThumbSticks.Right;
            direction.Y *= -1;      // Invert the y axis

            return direction;
        }

        /// <summary>
        /// Returns a float of the Left Trigger.
        /// </summary>
        public static float GetLeftTrigger()
        {
            if (gamepadState.IsConnected)
                return gamepadState.Triggers.Left;
            else
                return 0f;
        }
        /// <summary>
        /// Returns a float of the Right Trigger.
        /// </summary>
        public static float GetRightTrigger()
        {
            if (gamepadState.IsConnected)
                return gamepadState.Triggers.Right;
            else
                return 0f;
        }
    }
}
