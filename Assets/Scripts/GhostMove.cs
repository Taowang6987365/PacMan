using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostMove : MonoBehaviour
{
    public GameObject[] wayPointsGos;
    public float speed = 0.2f;

    //store the waypoints
    private List<Vector3> wayPoints = new List<Vector3>();
    private Vector3 startPos;

    private int index = 0;

    private void Start()
    {
        //when start the game, all the ghost will come out in the same position
        //change the come out position with each ghost's y axis
        startPos = transform.position + new Vector3(0, 3, 0);
        LoadPath(wayPointsGos[GameManager.Instance.usingIndex[GetComponent<SpriteRenderer>().sortingOrder-2]]);
    }

    private void LoadPath(GameObject go)
    {
        //everytime load the path, clear the list
        wayPoints.Clear();
        foreach (Transform t in go.transform)
        {
            wayPoints.Add(t.position);
        }
        //insert startPos as the first element of the list
        wayPoints.Insert(0, startPos);
        //at the end of the list, add the startPos in order to close the ghost's move path
        wayPoints.Add(startPos);
    }

    private void FixedUpdate()
    {
        //if ghost's position is not the next waypoint
        if(transform.position != wayPoints[index])
        {
            //move ghost
            Vector2 temp = Vector2.MoveTowards(transform.position, wayPoints[index], speed);
            GetComponent<Rigidbody2D>().MovePosition(temp);
        }
        else
        {
            //when the ghost reach the waypoint, index++, and ghost move to next position
            index++;
            if(index >= wayPoints.Count)
            {
                index = 0;
                //every path has different waypoints, so that the ghost will never touch each other
                //choose different path
                LoadPath(wayPointsGos[Random.Range(0, wayPointsGos.Length)]);
            }
        }

        Vector2 dir = wayPoints[index] - transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman")
        {
            if (GameManager.Instance.isSuperPacman)
            {
                //ghost go home
                transform.position = startPos - new Vector3(0, 3, 0);
                index = 0;
                GameManager.Instance.score += 500;
            }
            else
            {
                //Hide the pacman, if destroy it, it can cause some problem
                collision.gameObject.SetActive(false);
                GameManager.Instance.gamePanel.SetActive(false);
                Instantiate(GameManager.Instance.gameOverPrefab);
                
                Invoke("ReStart", 3f);
            }
            
        }
    }

    private void ReStart()
    {
        SceneManager.LoadScene(0);
    }
}
