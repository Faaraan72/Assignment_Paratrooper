using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;


public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    public int LevelUPScore;

    [Header("Text")]
    public Text coinText;
    public Text diamondText;
    public Text FinalcoinText;
    public Text FinaldiamondText;
    public Text totalScoreText;
    public Text InfoText;
    public Text LevelText;

    [Header("Buttons")]
    public Button PlayBtn;
    public Button ControlsBtn;
    public Button QuitBtn;
    public Button BackBtn;
    public Slider towerHealth;


    [Header("Panels")]
    public GameObject StartPanel;
    public GameObject ControlsPanel;
    public GameObject WinPanel;

    private int level =0;
    private void Awake()
    {
        instance=this;
    }
    private void Start()
    {
        level = 0;
        LevelText.text = "LEVEL UP: +" + level.ToString();
        StartPanel.SetActive(true);
        ControlsPanel.SetActive(false);
        WinPanel.SetActive(false);
        InfoText.gameObject.SetActive(false);
        PlayBtn.onClick.AddListener(StartPlay);
        ControlsBtn.onClick.AddListener(Controls);
        BackBtn.onClick.AddListener(RestartGame);
        QuitBtn.onClick.AddListener(ExitGame);
    }
    public void setScoreUI()
    {
        //setting values to UI
        coinText.text = GameController.coin.ToString();
        diamondText.text = GameController.diamonds.ToString();
        FinalcoinText.text = GameController.coin.ToString();
        FinaldiamondText.text = GameController.diamonds.ToString();
        totalScoreText.text = (GameController.coin + GameController.diamonds * 2).ToString();
        towerHealth.value = GameController.TowerHealth;


        // level up to 1
        if(GameController.coin > LevelUPScore && !GameController.gameover && level ==0)
        {
            level = 1;
            SetLevelupUI();
            AudioManager.PlaySound(GameController.instance.levelUPsound);
            LevelText.text = "LEVEL UP: +"+level.ToString();
            HelicopterSpawner.instance.spawnPrecentage = 20;
        }


        // level up to 2
        if (GameController.coin > (LevelUPScore + 150) && !GameController.gameover && level ==1)
        {
            level = 2;
            LevelText.text = "LEVEL UP: +" + level.ToString();
            AudioManager.PlaySound(GameController.instance.levelUPsound);
            SetLevelupUI();
            GameController.instance.StartSpawningJets();
        }


        //Gameover
        if (GameController.gameover)
        {
            Invoke(nameof(SetGameOverPanel), 1.5f);
        }
    }
    //Starting the Game
    public void StartPlay()
    {
        StartPanel.SetActive(false);
        HelicopterSpawner.instance.StartSpawningHelicopters();
    }

    // Control Panel UI
    public void Controls()
    {
        if (ControlsPanel.activeInHierarchy)
        {
            ControlsPanel.SetActive(false);
        }
        else
        {
            ControlsPanel.SetActive(true);

        }
    }

    //Animated LevelUP UI
    public void SetLevelupUI()
    {
        InfoText.gameObject.SetActive(true);
        InfoText.text = "Leveling UP!!";
        StartCoroutine(LevelUP_Ani(InfoText.gameObject, 1, 0.5f, 5));
       
    }
    IEnumerator LevelUP_Ani(GameObject obj, float moveSpeed , float growSpeed , float fadeDuration)
    {
        Text texttoFade = obj.GetComponent<Text>();
        Color originalColor = texttoFade.color;
        Vector3 originalScale = obj.transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            obj.transform.position += moveSpeed * Time.deltaTime * Vector3.up; 
            obj.transform.localScale += growSpeed * Time.deltaTime * Vector3.one; 

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); 
            texttoFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.SetActive(false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }
    public void SetGameOverPanel()
    {
        WinPanel.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    
}
