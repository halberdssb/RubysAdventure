using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    public GameObject winUI;
    public GameObject restartUI;
    public TextMeshProUGUI robotCounter;
    public Animator counterAnimator;

    public RubyController player;

    [HideInInspector]
    public int robotsFixed;
    [HideInInspector]
    public int totalRobots;

    private bool _waitingForRestart = false;

    void Start()
    {
        if (main != null)
        {
            Destroy(gameObject);
        }
        main = this;

        robotsFixed = 0;

        totalRobots = GameObject.FindGameObjectsWithTag("Robot").Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (_waitingForRestart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }

        robotCounter.text = "Fixed Robots: " + robotsFixed.ToString();
    }

    public void WinGame()
    {
        Time.timeScale = 0;
        winUI.SetActive(true);
    }

    public void OnDeath()
    {
        player.allowInput = false;
        restartUI.SetActive(true);
        _waitingForRestart = true;
    }

    private void RestartGame()
    {
        player.allowInput = true;
        restartUI.SetActive(false);
        _waitingForRestart = false;

        // reset ruby
        //player.ChangeHealth(player.maxHealth);
        //player.transform.position = player.startPos;
        SceneManager.LoadScene("MainScene");
    }
}
