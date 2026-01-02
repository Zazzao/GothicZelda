using UnityEngine;

public class Sign : MonoBehaviour, IInteractable
{

    [TextArea]
    [SerializeField] private string message;

    private PlayerMovement player;

    private bool isInteracting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = collision.GetComponent<PlayerMovement>();
        Interactable.Current = this;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = null;

        if ((object)this == Interactable.Current) Interactable.Current = null;

        InteractionPromptUI.Instance.Hide();
    }


    private void OnTriggerStay2D(Collider2D collision){
        if (isInteracting) return;
        
        if (IsPlayerFacingSign())
        {
            InteractionPromptUI.Instance.Show(GetInteractVerb(),(Vector2)this.transform.position + new Vector2(0,1));
        }
        else { 
            InteractionPromptUI.Instance.Hide();
        }
    }



    public string GetInteractVerb() => "Read";

    public void Interact()
    {
        //if (!playerInRange) return;

        // Player must be south of sign and facing north
        if (!IsPlayerFacingSign()) return;

        InteractionPromptUI.Instance.Hide();
        ToggleSign();
    }

    private bool IsPlayerFacingSign()
    {
        if (player == null) return false;

        Vector2 playerPos = player.transform.position;
        Vector2 signPos = transform.position;

        bool isBelow = playerPos.y < signPos.y;
        bool facingNorth = player.CurrentFacing == ActorAnimator.FacingDirection.North;

        return isBelow && facingNorth;
    }

    private void ToggleSign(){
        
        if (SignUI.Instance.IsOpen){
            isInteracting = false;
            SignUI.Instance.Hide();
            player.IsFrozen = false;
        }
        else{
            isInteracting = true;
            player.IsFrozen = true;
            SignUI.Instance.Show(message);
        }

    }





}