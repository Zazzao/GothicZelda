using UnityEngine;

public class Sign : MonoBehaviour, IInteractable
{

    [TextArea]
    [SerializeField] private string message;


    private bool playerInRange;
    private PlayerMovement player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = collision.GetComponent<PlayerMovement>();
        playerInRange = true;
        Interactable.Current = this;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
        player = null;

        if (this == Interactable.Current)
            Interactable.Current = null;
    }




    public void Interact()
    {
        if (!playerInRange) return;

        // Player must be south of sign and facing north
        if (!IsPlayerFacingSign()) return;

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

    private void ToggleSign()
    {
        Debug.Log("toggle sign");
        if (SignUI.Instance.IsOpen)
        {
            SignUI.Instance.Hide();
            player.IsFrozen = false;
        }
        else
        {
            player.IsFrozen = true;
            SignUI.Instance.Show(message);
        }//*/


    }





}