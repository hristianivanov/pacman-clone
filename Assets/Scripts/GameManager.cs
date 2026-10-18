using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;

    public GameObject fruitPrefab;
    public int fruitCount = 4;
    public int maxLives = 5;
    private List<GameObject> spawnedFruits = new List<GameObject>();

    public int ghostMultiplier { get; private set; }
    public int score { get; private set; }
    public int lives { get; private set; } = 3;

    private void Start()
    {
        // Initialize the game state
        score = 0;
        lives = 3;

        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        HideFruit();
        CancelInvoke(nameof(SpawnFruit));
        gameOverPanel.SetActive(false);
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void GameOver()
    {
        HideFruit();

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
    }

    private void SetScore(int points)
    {
        this.score = points;
        scoreText.text = "SCORE : " + score.ToString().PadLeft(2, '0');
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
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        if (!this.pacman.gameObject.activeSelf)
            return;

        this.pacman.gameObject.SetActive(false);
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
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        SetScore(this.score + pellet.points);

        if (!HasRemainingPellets())
        {
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
    }

}
