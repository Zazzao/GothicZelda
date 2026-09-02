using System.Collections.Generic;
using System.Collections;
using UnityEngine;



[RequireComponent (typeof(BoxCollider2D))]
public class PushableBlock : MonoBehaviour
{

    [SerializeField] private float pushDuration = 0.15f;
    [SerializeField] private AudioClip pushSfx;
    private bool isMoving;

   

    private float blockPushTime = 0.5f;
    private float blockTimer = 0.0f;

    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource> ();
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
       //if (!collision.gameObject.CompareTag("Player")) return;
        //Debug.Log("player touching block");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        
        if (PlayerMotor.Instance.GetMovementState() == PlayerMotor.MovementState.Walking && IsFacingBlock())
        {
            //Debug.Log("player is moving block");
            blockTimer += Time.deltaTime;

            if (blockTimer >= blockPushTime) {
                Vector2 dir = GetPushDirection();
                TryPush(dir);
                blockTimer = 0.0f;
            }

        }
        else { 
            blockTimer = 0.0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        //player = null;
    }



    public bool TryPush(Vector2 direction)
    {
        if (isMoving) return false;

        Vector2 targetPos = (Vector2)transform.position + direction;

        // Check if target space is empty
        if (Physics2D.OverlapBox(
            targetPos,
            Vector2.one * 0.8f,
            0,
            LayerMask.GetMask("Solid")))
        {
            return false;
        }

        StartCoroutine(MoveBlock(targetPos));
        return true;
    }






    private IEnumerator MoveBlock(Vector2 target)
    {
        isMoving = true;
        audioSource.PlayOneShot(pushSfx);

        Vector2 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / pushDuration;
            transform.position = Vector2.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }


    private bool IsFacingBlock(){

        float variance = 0.35f;
        Vector2 toBlock = (Vector2)transform.position - (Vector2)PlayerMotor.Instance.transform.position;

        if (Mathf.Abs(toBlock.x) > Mathf.Abs(toBlock.y)){

            if (toBlock.y > variance) return false;
            if (toBlock.y < -variance) return false;

            // Horizontal
            if (toBlock.x > 0)
                return PlayerMotor.Instance.GetFacing() == ActorAnimator.FacingDirection.East;
            else
                return PlayerMotor.Instance.GetFacing() == ActorAnimator.FacingDirection.West;
        }
        else
        {
            if (toBlock.x > variance) return false;
            if (toBlock.x < -variance) return false;

            // Vertical
            if (toBlock.y > 0)
                return PlayerMotor.Instance.GetFacing() == ActorAnimator.FacingDirection.North;
            else
                return PlayerMotor.Instance.GetFacing() == ActorAnimator.FacingDirection.South;
        }


    }

    private Vector2 GetPushDirection(){

        Vector2 vector = Vector2.zero;
        
        switch (PlayerMotor.Instance.GetFacing()) { 
            case ActorAnimator.FacingDirection.North:
                vector = new Vector2(0,1);
                break;
            case ActorAnimator.FacingDirection.East:
                vector = new Vector2(1, 0);
                break;
            case ActorAnimator.FacingDirection.South:
                vector = new Vector2(0, -1);
                break;
            case ActorAnimator.FacingDirection.West: 
                vector = new Vector2(-1, 0);
                break;
        
        }
            
        return vector;
    }


}
