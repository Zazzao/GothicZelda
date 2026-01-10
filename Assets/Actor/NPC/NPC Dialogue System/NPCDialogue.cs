using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{

    [SerializeField] private DialogueData[] dialogue;
    [SerializeField] private GameObject importantInfoIcon;

    private int dialogueIndex = 0;
    
    private PlayerMovement player;
    private bool isInteracting;


    public bool IsInteracting { get { return isInteracting; } set { isInteracting = value; } }

    public string GetInteractVerb() => "Talk";

    public void Interact() {

        if (!IsFacingNPC()) return;

        InteractionPromptUI.Instance.Hide();
        isInteracting = true;

        if (!DialogueManager.Instance.IsDialogueActive) { 
            if (importantInfoIcon != null) importantInfoIcon.SetActive(false);
            DialogueManager.Instance.StartDialogue(this, dialogue[dialogueIndex]); 
        }
        else
            DialogueManager.Instance.AdvanceDialogue();
            
    }


    private void OnTriggerEnter2D(Collider2D collision){
        if (!collision.CompareTag("Player")) return;

        player = collision.GetComponent<PlayerMovement>();
        Interactable.Current = this;

    }


    private void OnTriggerStay2D(Collider2D collision){
        if (!collision.CompareTag("Player")) return;
        if (isInteracting) return;

        if (IsFacingNPC())
            InteractionPromptUI.Instance.Show(GetInteractVerb(), (Vector2)this.transform.position + new Vector2(0.5f, 1));
        else
        {
            InteractionPromptUI.Instance.Hide();
            isInteracting= false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        InteractionPromptUI.Instance.Hide();
        if ((object)this == Interactable.Current)
            Interactable.Current = null;

        isInteracting = false;
    }



    private bool IsFacingNPC(){

        Vector2 toNPC = (Vector2)transform.position - (Vector2)player.transform.position;

        if (Mathf.Abs(toNPC.x) > Mathf.Abs(toNPC.y)){
            // Horizontal
            if (toNPC.x > 0)
                return player.CurrentFacing == ActorAnimator.FacingDirection.East;
            else
                return player.CurrentFacing == ActorAnimator.FacingDirection.West;
        }
        else
        {
            // Vertical
            if (toNPC.y > 0)
                return player.CurrentFacing == ActorAnimator.FacingDirection.North;
            else
                return player.CurrentFacing == ActorAnimator.FacingDirection.South;
        }
       
    }

    public void OnDialogueEnd() {
        isInteracting = false;

        dialogueIndex++;
        if (dialogueIndex >= dialogue.Length - 1) { 
            dialogueIndex = dialogue.Length - 1;
        }
    
    }


}
