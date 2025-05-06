using System.Collections;
using UnityEngine;

public class PlayerHazardDetector : MonoBehaviour
{
    [Header("Hazard Detection Settings")]
    public string hazardTag = "Hazard";
    public float capsuleRadius = 1.0f;
    public float capsuleHeight = 2.0f;
    public Vector3 capsuleOffset = Vector3.zero;
    public float damageCooldown = 1f;

    private PlayerHealth playerHealth;
    private bool canTakeDamage = true;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on the player!");
        }
    }

    private void Update()
    {
        CheckForHazards();
    }

    private void CheckForHazards()
    {
        Vector3 point1 = transform.position + capsuleOffset + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = transform.position + capsuleOffset - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);

        Collider[] hitColliders = Physics.OverlapCapsule(point1, point2, capsuleRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag(hazardTag))
            {
                ApplyHazardDamage();
                break;
            }
        }
    }

    private void ApplyHazardDamage()
    {
        if (canTakeDamage && playerHealth != null)
        {
            playerHealth.TakeDamage(1, null);
            Debug.Log("Player took damage from a hazard!");
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
        Debug.Log("Damage cooldown complete.");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection capsule in the Scene view for easier debugging
        Gizmos.color = Color.red;

        Vector3 point1 = transform.position + capsuleOffset + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = transform.position + capsuleOffset - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);

        Gizmos.DrawWireSphere(point1, capsuleRadius);
        Gizmos.DrawWireSphere(point2, capsuleRadius);
        Gizmos.DrawLine(point1 + Vector3.forward * capsuleRadius, point2 + Vector3.forward * capsuleRadius);
        Gizmos.DrawLine(point1 - Vector3.forward * capsuleRadius, point2 - Vector3.forward * capsuleRadius);
        Gizmos.DrawLine(point1 + Vector3.right * capsuleRadius, point2 + Vector3.right * capsuleRadius);
        Gizmos.DrawLine(point1 - Vector3.right * capsuleRadius, point2 - Vector3.right * capsuleRadius);
    }
}
