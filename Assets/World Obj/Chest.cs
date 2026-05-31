using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{

    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount;
    
    [SerializeField] private Sprite openSprite;
    [SerializeField] private AudioClip openSfx;

    private PlayerMovement player;
    private bool isInteracting = false;

    private bool isOpened = false;



    public string GetInteractVerb() => "Open";

    public void Interact(){
        if (!IsPlayerFacingChest()) return;
        if (isOpened ) return;

        if (!itemData) {
            Debug.Log("No Item Data Set in Chest Obj");
            return;
        }

        InteractionPromptUI.Instance.Hide();

        if (!AddItem_UI.Instance.IsOpen){
            //show msg and add items
            isInteracting = true;
            player.IsFrozen = true;
            this.GetComponent<AudioSource>().PlayOneShot(openSfx);
            this.GetComponent<SpriteRenderer>().sprite = openSprite;
            AddItem_UI.Instance.Show(itemData.name,amount.ToString(),itemData.icon);
            //TO-DO: Actually add items to the inventory
            for (int i = 0; i < amount; i++)
            {
                player.GetComponent<Inventory>().AddItem(itemData);
            }
        }
        else {
            //close msg
            isInteracting = false;
            AddItem_UI.Instance.Hide();
            player.IsFrozen = false;
            isOpened = true;
        }

        
        
       


    }

    private bool IsPlayerFacingChest(){ 
        if (player == null) return false;

        Vector2 playerPos = player.transform.position;
        Vector2 signPos = transform.position;

        bool isBelow = playerPos.y < signPos.y;
        bool facingNorth = player.CurrentFacing == ActorAnimator.FacingDirection.North;

        return isBelow && facingNorth;
    
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (!collision.CompareTag("Player")) return;

        player = collision.GetComponent<PlayerMovement>();
        Interactable.Current = this;
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (!collision.CompareTag("Player")) return;

        player = null;

        if ((object)this == Interactable.Current) Interactable.Current = null;

        InteractionPromptUI.Instance.Hide();
    }

    private void OnTriggerStay2D(Collider2D collision){
        if (isInteracting) return;
        if (isOpened) return;

        if (IsPlayerFacingChest())
        {
            InteractionPromptUI.Instance.Show(GetInteractVerb(), (Vector2)this.transform.position + new Vector2(0, 1));
        }
        else
        {
            InteractionPromptUI.Instance.Hide();
        }
    }


}
