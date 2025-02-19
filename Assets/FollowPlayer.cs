using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    public Vector3 offset = new Vector3(0, 5, -10); // Kamera konumu

    void Update()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}

