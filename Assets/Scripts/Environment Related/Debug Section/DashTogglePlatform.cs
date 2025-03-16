using UnityEngine;
namespace StarterAssets
{
    public class DashTogglePlatform : MonoBehaviour
    {
        [Header("References")]
        public FirstPersonController playerController;  // Reference to the player's FirstPersonController
        public MeshRenderer platformRenderer;            // Reference to the platform's MeshRenderer for color change
        public Color activeColor = Color.green;          // Color to change when dash is enabled
        public Color inactiveColor = Color.gray;        // Color to change when dash is disabled

        private bool isDashActive = false;               // To track whether dash is currently active

        private void OnTriggerEnter(Collider other)
        {
            // Check if the player enters the trigger
            if (other.CompareTag("Player"))
            {
                ToggleDashActivation();
            }
        }

        private void ToggleDashActivation()
        {
            // Toggle dash activation
            if (isDashActive)
            {
                // Set dash input period to 0.001f (disable dash)
                playerController.dashActivationInputPeriod = 0.001f;
                // Change the platform's color to grey
                platformRenderer.material.color = inactiveColor;
            }
            else
            {
                // Set dash input period to 0.3f (enable dash)
                playerController.dashActivationInputPeriod = 0.3f;
                // Change the platform's color to green
                platformRenderer.material.color = activeColor;
            }

            // Toggle the state
            isDashActive = !isDashActive;
        }
    }
}