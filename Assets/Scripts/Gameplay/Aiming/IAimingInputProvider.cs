using UnityEngine;

namespace DiveCat.Gameplay.Aiming
{
    /// <summary>
    /// Interface for providing aiming target information.
    /// Decouples the aiming logic from the specific input method (Mouse, Gamepad, AI).
    /// </summary>
    public interface IAimingInputProvider
    {
        /// <summary>
        /// Returns the world position where the player is currently aiming.
        /// </summary>
        Vector2 GetAimWorldPosition();

        /// <summary>
        /// Returns true if the aiming system should be active/visible.
        /// </summary>
        bool IsAiming { get; }
    }
}
