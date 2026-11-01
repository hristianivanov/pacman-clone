using UnityEngine;

public class GhostScatter : GhostBehaviour
{
    private void OnDisable()
    {
        this.ghost.chase.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled && !this.ghost.frightened.enabled)
        {
            if (node.availableDirections.Count == 0)
            {
                return;
            }

            int index = Random.Range(0, node.availableDirections.Count);

            if (
                node.availableDirections.Count > 1
                && node.availableDirections[index] == -this.ghost.movement.direction
            )
            {
                index++;

                if (index >= node.availableDirections.Count)
                {
                    index = 0;
                }
            }

            this.ghost.movement.SetDirection(node.availableDirections[index]);
        }
    }
}
