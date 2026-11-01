using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public enum GameState
{
    Title,
    Ready,
    Playing,
    Paused,
    Dying,
    RoundClear,
    GameOver
}

public class GameManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";

    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI titleHighScoreText;
    public GameObject gameOverPanel;
    public GameObject newHighScoreText;
    public GameObject titlePanel;
    public GameObject pausePanel;
    public GameObject readyText;

    public AudioManager sounds;
    public ScorePopup scorePopupPrefab;
    public float readyDuration = 2.0f;
    public float deathDuration = 2.0f;

    public GameObject fruitPrefab;
    public int fruitCount = 3;
    public int maxLives = 5;
    private List<GameObject> spawnedFruits = new List<GameObject>();

    public int ghostMultiplier { get; private set; }
    public int score { get; private set; }
    public int lives { get; private set; } = 3;
    public int highScore { get; private set; }
    public GameState state { get; private set; } = GameState.Title;

    private Coroutine readyRoutine;
    private bool beatHighScore;

    private void Start()
    {
        // Initialize the game state
        score = 0;
        lives = 3;
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        ShowTitle();
    }

    private void Update()
    {
        if ((this.state == GameState.Title || this.state == GameState.GameOver)
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            NewGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
    }

    private void OnDestroy()
    {
        // timeScale and the audio pause outlive this scene
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
    }

    private void ShowTitle()
    {
        this.state = GameState.Title;
        Time.timeScale = 0.0f;

        titlePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        readyText.SetActive(false);

        SetScore(0);
        SetLives(3);
        titleHighScoreText.text = "HIGH SCORE : " + highScore.ToString().PadLeft(2, '0');
    }

    public void NewGame()
    {
        // Enter/Space can also submit the selected button in the same frame
        if (this.state != GameState.Title && this.state != GameState.GameOver)
        {
            return;
        }

        HideFruit();
        CancelInvoke();   // drop every pending timer from the previous game
        titlePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        newHighScoreText.SetActive(false);
        this.beatHighScore = false;
        SetScore(0);
        SetLives(3);
        sounds.Play(sounds.gameStart);
        NewRound();
    }

    private void TogglePause()
    {
        if (this.state == GameState.Playing)
        {
            this.state = GameState.Paused;
            Time.timeScale = 0.0f;
            AudioListener.pause = true;
            pausePanel.SetActive(true);
        }
        else if (this.state == GameState.Paused)
        {
            this.state = GameState.Playing;
            Time.timeScale = 1.0f;
            AudioListener.pause = false;
            pausePanel.SetActive(false);
        }
    }

    private IEnumerator ReadyPause()
    {
        // timeScale 0 holds physics, Invoke timers and animations until the round starts
        this.state = GameState.Ready;
        Time.timeScale = 0.0f;
        readyText.SetActive(true);

        yield return new WaitForSecondsRealtime(this.readyDuration);

        readyText.SetActive(false);
        Time.timeScale = 1.0f;
        this.state = GameState.Playing;
    }

    private void GameOver()
    {
        this.state = GameState.GameOver;
        HideFruit();
        SaveHighScore();
        newHighScoreText.SetActive(this.beatHighScore);
        sounds.Play(sounds.gameOver);

        foreach (Ghost ghost in this.ghosts)
        {
            ghost.gameObject.SetActive(false);
        }
        this.pacman.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void NewRound()
    {
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState();

        if (this.readyRoutine != null)
        {
            StopCoroutine(this.readyRoutine);
        }
        this.readyRoutine = StartCoroutine(ReadyPause());
    }

    private void SetScore(int points)
    {
        this.score = points;
        scoreText.text = "SCORE : " + score.ToString().PadLeft(2, '0');

        if (this.score > this.highScore)
        {
            this.highScore = this.score;
            this.beatHighScore = true;
        }
        highScoreText.text = "HIGH : " + highScore.ToString().PadLeft(2, '0');
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, this.highScore);
        PlayerPrefs.Save();
    }

    private void ShowPopup(int points, Vector3 position)
    {
        position.y += 1.5f;   // clear of Pacman's sprite
        position.z = this.pacman.transform.position.z - 1.0f;
        Instantiate(this.scorePopupPrefab, position, Quaternion.identity).Show(points);
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "LIVES : " + lives.ToString().PadLeft(2, '0');
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;
        SetScore(this.score + points);
        ShowPopup(points, this.pacman.transform.position);
        sounds.Play(sounds.ghostEaten);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        // Two ghosts can reach Pacman on the same physics step
        if (this.state != GameState.Playing)
            return;

        this.state = GameState.Dying;

        foreach (Ghost ghost in this.ghosts)
        {
            ghost.gameObject.SetActive(false);
        }
        this.pacman.PlayDeathSequence();
        sounds.Play(sounds.death);

        SetLives(this.lives - 1);
        if (this.lives > 0)
        {
            if (this.lives == 1)
            {
                CancelInvoke(nameof(SpawnFruit));
                Invoke(nameof(SpawnFruit), 5f);
            }
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            Invoke(nameof(GameOver), this.deathDuration);
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        SetScore(this.score + pellet.points);
        sounds.PlayPellet();

        if (!HasRemainingPellets())
        {
            this.state = GameState.RoundClear;
            sounds.Play(sounds.roundClear);
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
        }
    }

    public void PowerPelletEaten(PowerPellet powerPellet)
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(powerPellet.duration);
        }

        sounds.Play(sounds.powerPellet);
        PelletEaten(powerPellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), powerPellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }


    private void SpawnFruit()
    {
        if (this.lives != 1) return;   // only when you have exactly 1 life

        HideFruit();   // remove leftovers from the last wave

        List<Transform> freeSpots = new List<Transform>();
        foreach (Transform pellet in this.pellets)
        {
            if (!pellet.gameObject.activeSelf)     // new: pellet already eaten = empty tile
            {
                freeSpots.Add(pellet);
            }
        }

        if (freeSpots.Count == 0)   // nothing eaten yet (fresh round), use any pellet tile
        {
            foreach (Transform pellet in this.pellets)
            {
                freeSpots.Add(pellet);
            }
        }

        for (int i = 0; i < fruitCount && freeSpots.Count > 0; i++)
        {
            int index = Random.Range(0, freeSpots.Count);
            Transform spot = freeSpots[index];
            freeSpots.RemoveAt(index);   // so two fruits never share a spot

            GameObject f = Instantiate(fruitPrefab,
                new Vector3(spot.position.x, spot.position.y, 0f),
                Quaternion.identity);

            f.SetActive(true);
            f.GetComponent<SpriteRenderer>().sortingOrder = 50;   // always on top
            spawnedFruits.Add(f);
        }

        CancelInvoke(nameof(HideFruit));
        Invoke(nameof(HideFruit), 10f);    // fruits disappear after 10 seconds
        Invoke(nameof(SpawnFruit), 15f);   // a new wave at new random spots
    }

    private void HideFruit()
    {
        foreach (GameObject f in spawnedFruits)
        {
            if (f != null) Destroy(f);
        }
        spawnedFruits.Clear();
    }

    public void FruitEaten(Fruit fruit)
    {
        spawnedFruits.Remove(fruit.gameObject);
        Destroy(fruit.gameObject);

        if (this.lives < maxLives)
        {
            SetLives(this.lives + 1);
        }

        SetScore(this.score + fruit.points);
        ShowPopup(fruit.points, fruit.transform.position);
        sounds.Play(sounds.fruit);
    }

}
