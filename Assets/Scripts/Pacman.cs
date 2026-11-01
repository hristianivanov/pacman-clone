using System;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    public AnimatedSprite deathSequence;

    public Movement movement { get; private set; }
    private SpriteRenderer spriteRenderer;
    private Collider2D circleCollider;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.circleCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Frozen on the title, ready and pause screens
        if (Time.timeScale == 0f)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
            this.movement.SetDirection(Vector2.up);
        } else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
            this.movement.SetDirection(Vector2.down);
        } else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
            this.movement.SetDirection(Vector2.left);
        } else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            this.movement.SetDirection(Vector2.right);
        }

        float angle = (float)Math.Atan2(this.movement.direction.y, this.movement.direction.x);
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void ResetState()
    {
        this.enabled = true;
        this.spriteRenderer.enabled = true;
        this.circleCollider.enabled = true;
        this.deathSequence.spriteRenderer.enabled = false;
        this.movement.ResetState();
        this.gameObject.SetActive(true);
    }

    public void PlayDeathSequence()
    {
        this.enabled = false;
        this.spriteRenderer.enabled = false;
        this.circleCollider.enabled = false;
        this.movement.enabled = false;
        this.movement.rigidbody.linearVelocity = Vector2.zero;
        this.movement.rigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.transform.rotation = Quaternion.identity;

        this.deathSequence.spriteRenderer.enabled = true;
        this.deathSequence.Restart();
    }
}
