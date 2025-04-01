using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RAdio", menuName = "Radio Station")]

public class RadioStations : ScriptableObject
{
    [Header("songs")]

    [SerializeField] private AudioClip[] _songs;
    public AudioClip[] songs { get{ return _songs; } set{ _songs = value; } }
}
