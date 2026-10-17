using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement { get; private set; }
    public GhostHome home { get; set; }
    public GhostChase chase { get; set; }
    public GhostFrightened frightened { get; set; }
    public GhostScatter scatter { get; set; }
    public GhostBehaviour initialBehaviour;
    public Transform target ;
    public int points = 200;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.chase = GetComponent<GhostChase>();
        this.frightened = GetComponent<GhostFrightened>();
        this.scatter = GetComponent<GhostScatter>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Enable();

        if(this.home != this.initialBehaviour)
        {
            this.home.Disable();
        }

        if(this.initialBehaviour != null)
        {
            this.initialBehaviour.Enable();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (this.frightened.enabled)
            {
                FindAnyObjectByType<GameManager>().GhostEaten(this);
            }
            else
            {
                FindAnyObjectByType<GameManager>().PacmanEaten();
            }
        }
    }
}
