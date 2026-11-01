using System.Collections;
using UnityEngine;

public class GhostHome : GhostBehaviour
{
    public Transform inside;
    public Transform outside;

    private bool skipExitTransition;

    public void ResetState()
    {
        StopAllCoroutines();

        // Disable without playing the exit animation, the ghost is being teleported to its start
        this.skipExitTransition = true;
        Disable();
        this.skipExitTransition = false;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if (!this.skipExitTransition && this.gameObject.activeInHierarchy)
        {
            StartCoroutine(ExitTransition());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            this.ghost.movement.SetDirection(-this.ghost.movement.direction);
        }
    }

    private IEnumerator ExitTransition()
    {
        this.ghost.movement.SetDirection(Vector2.up, true);
        this.ghost.movement.rigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.ghost.movement.enabled = false;

        Vector3 position = this.transform.position;

        float duration = 0.5f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(position, this.inside.position, elapsed / duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0.0f;

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(
                this.inside.position,
                this.outside.position,
                elapsed / duration
            );
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.ghost.movement.SetDirection(
            new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f),
            true
        );

        this.ghost.movement.rigidbody.bodyType = RigidbodyType2D.Dynamic;
        this.ghost.movement.enabled = true;
    }
}
