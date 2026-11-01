using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip pelletA;
    public AudioClip pelletB;
    public AudioClip powerPellet;
    public AudioClip ghostEaten;
    public AudioClip fruit;
    public AudioClip death;
    public AudioClip gameStart;
    public AudioClip roundClear;
    public AudioClip gameOver;

    private AudioSource source;
    private bool nextPelletIsB;

    private void Awake()
    {
        this.source = GetComponent<AudioSource>();
    }

    public void PlayPellet()
    {
        // Alternate two pitches so a run of pellets doesn't sound like one repeated note
        Play(this.nextPelletIsB ? this.pelletB : this.pelletA);
        this.nextPelletIsB = !this.nextPelletIsB;
    }

    public void Play(AudioClip clip)
    {
        if (clip != null)
        {
            this.source.PlayOneShot(clip);
        }
    }
}
