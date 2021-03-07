using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //Each ghost will use the GameManager
    //in order to avoid the random path problem, create singleton of the GameManager
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject pacman;
    public GameObject blicky;
    public GameObject clyde;
    public GameObject inky;
    public GameObject pinky;
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject startCountDownPrefab;
    public GameObject gameOverPrefab;
    public GameObject winPrefab;
    public AudioClip startClip;
    public Text remainText;
    public Text nowText;
    public Text scoreText;
    public int score = 0;
    public bool isSuperPacman = false;

    //usingIndex
    public List<int> usingIndex = new List<int>();
    //Initial Index
    public List<int> rawIndex = new List<int> { 0, 1, 2, 3 };
    private List<GameObject> pacdotGos = new List<GameObject>();

    private int pacdotNum = 0;
    private int nowEat = 0;
   

    private void Awake()
    {
        instance = this;
        Screen.SetResolution(1024,768,false);
        
        RandomPath();

        //Add the dots
        foreach (Transform t in GameObject.Find("Maze").transform)
        {
            pacdotGos.Add(t.gameObject);
        }

        //get the total dots count
        pacdotNum = GameObject.Find("Maze").transform.childCount;

    }

    private void Start()
    {
        SetGameState(false);
    }

    private void Update()
    {
        if(nowEat == pacdotNum && pacman.GetComponent<PacmanMove>().enabled != false)
        {
            Win();
        }

        if (nowEat == pacdotNum)
        {
            Restart();
        }

        if (gamePanel.activeInHierarchy)
        {
            //Display score
            DisplayMsg();
        }    
    }

    private void Win()
    {
        gamePanel.SetActive(false);
        Instantiate(winPrefab);
        StopAllCoroutines();//Security check, stop all coroutines
        SetGameState(false);
    }

    private void Restart()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void DisplayMsg()
    {
        remainText.text = "Remain:\n\n" + (pacdotNum - nowEat);
        nowText.text = "Eaten:\n\n" + nowEat;
        scoreText.text = "Score:\n\n" + score;
    }

    private void RandomPath()
    {
        //make sure every ghost has its own path
        int tempCount = rawIndex.Count;
        for (int i = 0; i < tempCount; i++)
        {
            int tempIndex = Random.Range(0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempIndex]);
            rawIndex.RemoveAt(tempIndex);
        }
    }

    public void OnStartButton()
    {
        StartCoroutine(PlayerPlayCountDown());
        //camera pos in (0,0,-10)
        AudioSource.PlayClipAtPoint(startClip, new Vector3(23, 16, -10));
        startPanel.SetActive(false);   
    }

    IEnumerator PlayerPlayCountDown()
    {
        GameObject go = Instantiate(startCountDownPrefab);//auto play anim
        yield return new WaitForSeconds(4f);//length of the anim is 4 sec
        Destroy(go);
        SetGameState(true);

        //create super dot after 10 sec
        Invoke("CreateSuperPacdot", 10f);
        gamePanel.SetActive(true);
        GetComponent<AudioSource>().Play();
    }


    public void OnExitButton()
    {
        Application.Quit();
    }

    //pacman eat normal dot
    public void onEatPacdot(GameObject go)
    {
        nowEat++;
        score += 100;
        pacdotGos.Remove(go);
    }

    //eat super dot
    //activate in PacDot.cs
    public void onEatSuperPacdot()
    {
        score += 200;
        Invoke("CreateSuperPacdot", 10f);
        isSuperPacman = true;
        FreezeEnermy();
        StartCoroutine(RecoveryEnermy());
    }

    IEnumerator RecoveryEnermy()
    {
        //freeze ghost 3 sec
        yield return new WaitForSeconds(3f);
        DisFreezeEnermy();
        isSuperPacman = false;
    }

    //Super Dot
    //activate in PacDot.cs
    private void CreateSuperPacdot()
    {
        //if there are no more than 5 dots, pass
        if(pacdotGos.Count < 5)
        {
            return;
        }

        int tempIndex = Random.Range(0, pacdotGos.Count);
        //change the scale of the pack dot
        pacdotGos[tempIndex].transform.localScale = new Vector3(3, 3, 3);
        //change the dot status
        pacdotGos[tempIndex].GetComponent<PacDot>().isSuperDot = true;
    }

    //Freeze Enermy
    private void FreezeEnermy()
    {
        GhostMove(false);
        GhostColor(0.7f);
    }

    //Disfreeze Enermy
    private void DisFreezeEnermy()
    {
        GhostMove(true);
        GhostColor(1f);
    }
    
    private void SetGameState(bool state)
    {
        pacman.GetComponent<PacmanMove>().enabled = state;
        GhostMove(state);
    }

    //enable or disable Ghost movemnt
    private void GhostMove(bool state)
    {
        blicky.GetComponent<GhostMove>().enabled = state;
        clyde.GetComponent<GhostMove>().enabled = state;
        inky.GetComponent<GhostMove>().enabled = state;
        pinky.GetComponent<GhostMove>().enabled = state;
    }

    //change Ghost Color
    private void GhostColor(float var)
    {
        Color color = new Color(var, var, var);
        blicky.GetComponent<SpriteRenderer>().color = color;
        clyde.GetComponent<SpriteRenderer>().color = color;
        inky.GetComponent<SpriteRenderer>().color = color;
        pinky.GetComponent<SpriteRenderer>().color = color;
    }
}
