using UnityEngine;

/// <summary>
/// Checkpoint trigger for Go-Kart track
/// Attach this to each checkpoint object in Unity
/// </summary>
public class TrackCheckpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public int checkpointIndex;
    public bool isFinishLine = false;

    private void OnDrawGizmos()
    {
        // Visualize checkpoint in Unity Editor
        Gizmos.color = isFinishLine ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(10f, 2f, 1f));

        // Draw index label direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);
    }
}
