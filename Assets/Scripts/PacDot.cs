using UnityEngine;

public class PacDot : MonoBehaviour
{
    public bool isSuperDot = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Pacman")
        {
            if (isSuperDot)
            {
                //if PacMan eats super dot, then it can eat teh ghost 
                GameManager.Instance.onEatPacdot(gameObject);
                GameManager.Instance.onEatSuperPacdot();
                Destroy(gameObject);
            }
            else
            {
                GameManager.Instance.onEatPacdot(gameObject);
                Destroy(gameObject);
            }
        }
    }
}
