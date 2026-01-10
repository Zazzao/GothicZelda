using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string spawnPointId;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        LevelManager.Instance.TransitionToLevel(
            targetScene,
            spawnPointId
        );
    }
}
