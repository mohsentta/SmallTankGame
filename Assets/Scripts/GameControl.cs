using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    [Header("Game settings")]
    [Range(0, 0.1f)]
    public float EnergyDecreasePerFrame;

    [Range(0, 0.1f)]
    public float EnergyLossGrowPerLevel;

    public float LevelsDuration;

    public int EnemyQuantityPerSpawnArea;

    public float DelayBetweenEverySpawn;

    public GamePlayCanvas Canvas;

    public int CurrentLevel;

    public int MaxCombo;

    [Header("public Variables")]
    public string SpawnAreasTag;

    public bool IsGameEnded = false;


    public float Score;

    public float TimeSpentThisGame;

    public int Combo = 1;

    static float MaxEnergy = 1;

    public static GameControl GameControlInstance;

    public LayerMask TankLayers;

    public int EnemiesLeft;

    [HideInInspector]
    public GameObject[] SpawnAreas = new GameObject[0];

    public float CurrentEnergy;
    [HideInInspector]
    public GameObject PlayerTank;

    private float _timeLeftToSpawn;

    private bool _isSpawnerLocked;

    private float _currentComboTime = 0;

    private float _comboLastDuration;

    private float _currentLevelLeftTime;



    void Start()
    {
        CurrentLevel = 1;

        _comboLastDuration = DelayBetweenEverySpawn * 0.5f;

        _currentLevelLeftTime = LevelsDuration;

        _timeLeftToSpawn = DelayBetweenEverySpawn;
        GameControlInstance = this;
        CurrentEnergy = MaxEnergy;
        SpawnAreas = GameObject.FindGameObjectsWithTag(SpawnAreasTag);
        PlayerTank = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnEnemyPerFrame());


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeSpentThisGame += Time.deltaTime;

        _currentLevelLeftTime -= Time.deltaTime;

        if (_currentLevelLeftTime < 0)
        {
            CurrentLevel++;
            _currentLevelLeftTime = LevelsDuration;
        }

        CurrentEnergy -= (EnergyDecreasePerFrame + (EnergyLossGrowPerLevel * CurrentLevel)) * Time.deltaTime * EnemiesLeft;

        if (CurrentEnergy <= 0)
        {
            CurrentEnergy = 0;
        }

        _timeLeftToSpawn -= Time.deltaTime;

        if (EnemiesLeft <= 0 || _timeLeftToSpawn <= 0 && !_isSpawnerLocked)
        {
            StartCoroutine(SpawnEnemyPerFrame());
            _timeLeftToSpawn = DelayBetweenEverySpawn;
        }



        //update combo time:
        if (_currentComboTime > 0)
        {
            _currentComboTime -= Time.deltaTime;
        }
        else
        {
            ResetCombo();
        }


        if (CurrentEnergy <= 0)
        {
            EndGame();
        }


    }

    private void EndGame()
    {
        Canvas.EnableEndUi();
        Time.timeScale = 0;
        IsGameEnded = true;
    }

    public void ChangeEnergy(float amount)
    {
        CurrentEnergy += amount;
        if (CurrentEnergy > 1)
        {
            CurrentEnergy = 1;
        }
        else if (CurrentEnergy < 0)
        {
            CurrentEnergy = 0;

        }
    }


    public void AddScore(float amount)
    {
        Score += amount * Combo;
        if (_currentComboTime > 0)
        {
            AddCombo();
        }

        _currentComboTime = _comboLastDuration;
    }

    public void AddCombo()
    {
        if (Combo >= MaxCombo)
        {
            return;
        }
        Combo++;
    }

    public float GetNextWaveTime()
    {

        float nextWaveTime = 1 - (_timeLeftToSpawn / DelayBetweenEverySpawn);


        return nextWaveTime;
    }

    public float GetComboTime()
    {
        float comboTime = 0;

        comboTime = _currentComboTime / _comboLastDuration;

        return comboTime;
    }

    public void ResetCombo()
    {
        Combo = 1;
    }

    public IEnumerator SpawnEnemyPerFrame()
    {
        _isSpawnerLocked = true;
        foreach (var spawnArea in SpawnAreas)
        {
            for (int i = 0; i < EnemyQuantityPerSpawnArea; i++)
            {
                var enemyTank = PoolManager.Instance.GetATank().GetComponent<EnemyTank>();

                int randomized = Random.Range(0, 2);

                switch (randomized)
                {
                    case 0:
                        enemyTank.Initialize(ColorContainer.Color.blue);
                        break;
                    case 1:
                        enemyTank.Initialize(ColorContainer.Color.red);
                        break;
                }

                Renderer spawnAreaRenderer = spawnArea.GetComponent<Renderer>();

                Vector3 spawnPosition = new Vector3(Random.Range(spawnAreaRenderer.bounds.min.x,
                        spawnAreaRenderer.bounds.max.x)
                    , -1,
                    Random.Range(spawnAreaRenderer.bounds.min.z,
                        spawnAreaRenderer.bounds.max.z)
                );

                while (Physics.CheckBox(spawnPosition, Vector3.one * 1.5f, enemyTank.transform.rotation, TankLayers))
                {
                    spawnPosition = new Vector3(Random.Range(spawnAreaRenderer.bounds.min.x,
                            spawnAreaRenderer.bounds.max.x)
                        , -1,
                        Random.Range(spawnAreaRenderer.bounds.min.z,
                            spawnAreaRenderer.bounds.max.z));
                    yield return new WaitForEndOfFrame();

                }

                enemyTank.transform.position = spawnPosition;

                enemyTank.transform.LookAt(PlayerTank.transform.position);

                EnemiesLeft++;

                yield return new WaitForEndOfFrame();

            }

        }

        _isSpawnerLocked = false;
        yield return 0;
    }


    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && Score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", (int)GameControl.GameControlInstance.Score);
        }
    }


}
