using UnityEngine;
using System.Collections;

public class BossL1LaserController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxLaserLength = 190f; // Adjust the length as needed

    public void ActivateLaser(float specialBulletDuration)
    {
        // Set the starting and ending positions of the laser relative to the fire point
        lineRenderer.SetPosition(0, Vector3.zero); // Start at the fire point
        lineRenderer.SetPosition(1, transform.right * maxLaserLength);

        // Enable the LineRenderer
        lineRenderer.enabled = true;

        // Disable the laser after a set duration
        StartCoroutine(DisableAfterTime(specialBulletDuration)); // 3 seconds duration
    }

    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
        Destroy(gameObject); // Destroy the laser GameObject after use
    }
}