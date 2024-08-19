using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;
    private string m_name;
    public int highScore = 0;
    public string highScoreName = "";
    public InputField playerName;

    private bool m_GameOver = false;
    public Text highScoreText;



    private void Awake()
    {
        LoadScore();
    }
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            m_name = playerName.text;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (m_Points > highScore)
                {
                    SaveScore();
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > highScore)
        {
            playerName.gameObject.SetActive(true);
        }
    }

    [System.Serializable]
    class SaveData
    {
        public int score;
        public string name;
    }

    public void SaveScore()
    {
        SaveData data = new SaveData();
        data.score = m_Points;
        data.name = m_name;
        string folderPath = Path.Combine(Application.dataPath, "SavedData");

        // Crée le dossier s'il n'existe pas
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.dataPath + "/SavedData/playerData.json", json);
    }

    public void LoadScore()
    {
        string path = Application.dataPath + "/SavedData/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.score;
            highScoreName = data.name;
            highScoreText.text = $"Best Score : {highScoreName} : {highScore}";
        }
    }
}
