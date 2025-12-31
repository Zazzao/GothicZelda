using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{

    [SerializeField] private DialogueData dialogue;


    public string GetInteractVerb() => "Talk";
    
    public void Interact()
    {
       // DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        InteractionPromptUI.Instance.Show(GetInteractVerb(), (Vector2)this.transform.position + new Vector2(0, 1));

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        InteractionPromptUI.Instance.Hide();
    }



    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
