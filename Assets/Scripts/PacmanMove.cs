using UnityEngine;

public class PacmanMove : MonoBehaviour
{
    //Pacman movement speed
    public float speed = 0.35f;
    //pacman's next position
    private Vector2 dest = Vector2.zero;

    private void Start()
    {
        //stop pacman movement when the game start
        //assign destination with the start pos
        dest = this.transform.position;
    }

    private void FixedUpdate()
    {
        PacManMovement();
    }

    private void PacManMovement()
    {
        //get pacman's movement data
        Vector2 temp = Vector2.MoveTowards(transform.position, dest, speed);

        //assign the movemnt data to the method
        GetComponent<Rigidbody2D>().MovePosition(temp);
        
        if ((Vector2)transform.position == dest)
        {
            //resign dest's value
            dest = ReachedDes(dest);  
        }
    }

    private Vector2 ReachedDes(Vector2 dest)
    {
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && Valid(Vector2.up))
        {
            dest = (Vector2)transform.position + Vector2.up;
        }
        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && Valid(Vector2.down))
        {
            dest = (Vector2)transform.position + Vector2.down;
        }
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && Valid(Vector2.left))
        {
            dest = (Vector2)transform.position + Vector2.left;
        }
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && Valid(Vector2.right))
        {
            dest = (Vector2)transform.position + Vector2.right;
        }

        //get direction
        Vector2 dir = dest - (Vector2)transform.position;

        playAnim(dir);

        return dest;
    }

    private void playAnim(Vector2 dir)
    {
        //set anim bases on the dirction
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }


    //detect if reach the edge
    private bool Valid(Vector2 dir)
    {        
        //shoot the ray in front of the pacman
        //if it hit the wall, return true

        // Hit wall:
        // O |<---- 

        // Hit PacMan
        // O <----

        //get pacman's pos
        Vector2 pos = transform.position;

        //shoot the ray in front of the pacman
        RaycastHit2D hit= Physics2D.Linecast(pos + dir, pos);
        return (hit.collider == GetComponent<Collider2D>());
    }
}
