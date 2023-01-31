using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayCanvas : MonoBehaviour
{
    public Image EnergyBarAmountImage;

    public Image NextWavePanelImage;

    public Image ComboPanelImage;

    public Text ComboText;

    public Text ScoreAmountText;

    public Text EndScore;

    public Text EndHighScore;

    public GameObject EndPanel;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnergyBar();


        UpdateScoreUi();


        UpdateWavePanel();


        UpdateComboUi();
    }

    private void UpdateComboUi()
    {
        ComboPanelImage.fillAmount = GameControl.GameControlInstance.GetComboTime();
        if (GameControl.GameControlInstance.Combo > 1)
        {
            ComboPanelImage.gameObject.SetActive(true);
        }
        else
        {
            ComboPanelImage.gameObject.SetActive(false);
        }

        ComboText.text = "combo \n x" + GameControl.GameControlInstance.Combo.ToString().Replace("0", "O");
    }

    private bool _isTriggerCalled;

    private void UpdateWavePanel()
    {
        NextWavePanelImage.fillAmount = GameControl.GameControlInstance.GetNextWaveTime();
        if (GameControl.GameControlInstance.GetNextWaveTime() < .01 && !_isTriggerCalled)
        {
            NextWavePanelImage.GetComponent<Animator>().SetTrigger("Trigger");
            _isTriggerCalled = true;
        }

        if (!(GameControl.GameControlInstance.GetNextWaveTime() < .01))
        {
            _isTriggerCalled = false;
        }
    }

    private void UpdateScoreUi()
    {
        ScoreAmountText.text = GameControl.GameControlInstance.Score.ToString().Replace("0", "O");
    }

    private void UpdateEnergyBar()
    {
        EnergyBarAmountImage.transform.localScale = new Vector3(GameControl.GameControlInstance.CurrentEnergy, 1, 1);
    }


    public void EnableEndUi()
    {
        EndPanel.SetActive(true);

        EndScore.text = "Score: " + GameControl.GameControlInstance.Score.ToString().Replace("0", "O");
        if (GameControl.GameControlInstance.Score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", (int)GameControl.GameControlInstance.Score);
        }

        EndHighScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString().Replace("0", "O");
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}