using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float duration = 1.2f;

    private PlayerController playerController;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    public void Initialize(Vector2 direction, PlayerController controller)
    {
        playerController = controller;

        if (playerController != null)
            playerController.StartAttack();

        // Rotate laser toward direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Find distance to edge of camera
        float distance = GetDistanceToScreenEdge(direction);

        // Stretch laser to that distance (sprite faces RIGHT)
        Vector3 scale = transform.localScale;
        scale.x = distance;
        transform.localScale = scale;
    }

    private void OnDestroy()
    {
        if (playerController != null)
            playerController.EndAttack();
    }

    float GetDistanceToScreenEdge(Vector2 direction)
    {
        Camera cam = Camera.main;

        Vector3 screenCenter = transform.position;
        Vector3 screenEdge = cam.ScreenToWorldPoint(
            new Vector3(
                direction.x > 0 ? Screen.width : 0,
                direction.y > 0 ? Screen.height : 0,
                cam.nearClipPlane
            )
        );

        return Vector2.Distance(screenCenter, screenEdge);
    }
}


