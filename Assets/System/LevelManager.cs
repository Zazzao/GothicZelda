using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    //private PlayerMovement player;
    private string pendingSpawnId;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

  
    void Start()
    {
        //player = FindFirstObjectByType<PlayerMovement>();
            
    }

    public void TransitionToLevel(string sceneName, string spawnId)
    {
        pendingSpawnId = spawnId;
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. Freeze player
        PlayerMotor.Instance.IsFrozen = true;

        // 2. Fade out
        // yield return FadeManager.Instance.FadeOut();

        // 3. Load scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 4. Move player to spawn
        PlacePlayerAtSpawn();

        // 5. Update camera bounds
        //UpdateCameraBounds();

        // 6. Fade in
        //yield return FadeManager.Instance.FadeIn();

        // 7. Unfreeze
        PlayerMotor.Instance.IsFrozen = false;
    }

    private void PlacePlayerAtSpawn()
    {
        PlayerSpawnPoint[] spawns = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);

        foreach (var spawn in spawns) { 
            if (spawn.Id == pendingSpawnId){
                PlayerMotor.Instance.transform.position = spawn.transform.position;
                return;
            }
        }

        Debug.LogWarning("Spawn point not found!");
    }




}
