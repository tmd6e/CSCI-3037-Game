using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask groundLayer;  // The layer that represents the ground.

    public bool IsGrounded()
    {
        // The center of the object's position.
        Vector3 center = transform.position;

        // The length of the raycast.
        float rayLength = 1.4f;

        // Cast a ray from the center of the object downwards.
        RaycastHit hit;
        if (Physics.Raycast(center, Vector3.down, out hit, rayLength, groundLayer))
        {
            // The object is grounded, and the ray hit the ground layer.
            Debug.DrawLine(center, hit.point, Color.green);  // Draw a line from the object's position to the hit point.
            return true;
        }
        else
        {
            Debug.DrawRay(center, Vector3.down * rayLength, Color.red);  // Draw a ray in the downward direction.
        }

        // The object is not grounded.
        return false;
    }
}
