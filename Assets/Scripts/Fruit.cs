using System;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int points = 30;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            FindAnyObjectByType<GameManager>().FruitEaten(this);
        }
    }
}
