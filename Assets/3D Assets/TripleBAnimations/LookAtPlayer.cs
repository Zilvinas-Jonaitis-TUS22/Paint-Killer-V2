using UnityEngine;

public class LookAtPlayerWithAnimation : MonoBehaviour
{
    public Transform player;  // Assign the player's transform in the inspector.
    [Range(0f, 1f)] public float strength = 0.5f;  // How much the object turns towards the player (0 = no tracking, 1 = full tracking).
    public bool rotateOnX = true;
    public bool rotateOnY = true;
    public bool rotateOnZ = true;

    private Quaternion initialRotation; // To store the initial rotation upon start.

    void Start()
    {
        // Store the initial rotation at start, relative to the world.
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Get the current rotation from the Animator as the "new default".
        Quaternion animationRotation = transform.rotation;

        // Calculate the desired look rotation to face the player.
        Vector3 targetDirection = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Apply axis-specific rotation
        Vector3 targetEuler = targetRotation.eulerAngles;
        Vector3 animationEuler = animationRotation.eulerAngles;
        Vector3 blendedEuler = new Vector3(
            rotateOnX ? Mathf.LerpAngle(animationEuler.x, targetEuler.x, strength) : animationEuler.x,
            rotateOnY ? Mathf.LerpAngle(animationEuler.y, targetEuler.y, strength) : animationEuler.y,
            rotateOnZ ? Mathf.LerpAngle(animationEuler.z, targetEuler.z, strength) : animationEuler.z
        );

        // Apply the blended rotation back to the object.
        transform.rotation = Quaternion.Euler(blendedEuler);
    }
}
