using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class ScorePopup : MonoBehaviour
{
    public float lifetime = 0.8f;
    public float riseSpeed = 1.5f;

    private TextMeshPro label;
    private Color startColor;
    private float elapsed;

    private void Awake()
    {
        this.label = GetComponent<TextMeshPro>();
        this.startColor = this.label.color;
    }

    public void Show(int points)
    {
        this.label.text = "+" + points;
    }

    private void Update()
    {
        this.elapsed += Time.deltaTime;

        if (this.elapsed >= this.lifetime)
        {
            Destroy(this.gameObject);
            return;
        }

        this.transform.position += Vector3.up * this.riseSpeed * Time.deltaTime;

        Color color = this.startColor;
        color.a = 1.0f - this.elapsed / this.lifetime;
        this.label.color = color;
    }
}
