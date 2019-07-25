using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AudioStateMachine : StateMachineBehaviour
{
    private GameManager gameManager;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip sound;
    [SerializeField]
    private float pitch;
    [SerializeField]
    private float volume;
    [SerializeField]
    private bool loop;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.gameObject.GetComponentInParent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = sound;
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.Play();
            audioSource.loop = loop;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.pitch = 1f;
            audioSource.volume = 1;
        }
    }
}
