using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicScript : MonoBehaviour
{
    // Snapshots in main mixer
    public AudioMixerSnapshot mainSnapshotPause;
    public AudioMixerSnapshot mainSnapshotNormal;

    // Snapshots in music mixer
    public AudioMixerSnapshot[] musicSnapshot;

    private bool pause = false;
    private float drama = 0.0f;
    private float brightness = 1.0f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var c = GetComponent<Camera>();
        c.backgroundColor = new Color(brightness * 0.6f, brightness * 0.1f, brightness * 0.1f, 1.0f);
        brightness += (((pause) ? 0.0f : 1.0f) - brightness) * 0.015f;
    }

    void OnGUI()
    {
        if (GUILayout.Button(pause ? "Resume Game" : "Pause Game"))
        {
            pause = !pause;
            if (pause)
                mainSnapshotPause.TransitionTo(5.0f);
            else
                mainSnapshotNormal.TransitionTo(5.0f);
        }

        GUILayout.Label("Drama slider (move slowly) ;-)");
        float newDrama = GUILayout.HorizontalSlider(drama, 0, musicSnapshot.Length - 1);
        if ((int)newDrama != (int)drama)
            musicSnapshot[(int)newDrama].TransitionTo(1.5f);
        drama = newDrama;

        GUILayout.Label("Mood: " + musicSnapshot[(int)drama].name);
    }
}
