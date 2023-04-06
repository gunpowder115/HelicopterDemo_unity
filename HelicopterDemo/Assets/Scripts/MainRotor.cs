using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRotor : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        RotateMainRotor();
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void RotateMainRotor()
    {
        transform.Rotate(new Vector3(0, 10, 0));
    }
}
