using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{

    [SerializeField] private DialogueData dialogue;


    public string GetInteractVerb() => "Talk";
    
    public void Interact()
    {
        
        if (!DialogueManager.Instance.IsDialogueActive)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
        else{
            DialogueManager.Instance.AdvanceDialogue();
        }



            
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable.Current = this;
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
        if (this == Interactable.Current)
            Interactable.Current = null;
    }



    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
