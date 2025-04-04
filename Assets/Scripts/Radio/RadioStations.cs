using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RAdio", menuName = "Radio Station")]

public class RadioStations : ScriptableObject
{
    [Header("Songs")]

    [SerializeField] private AudioClip[] _songs;
    public AudioClip[] songs { get{ return _songs; } set{ _songs = value; } }

    [Header("Name")]

    [SerializeField] private string _name;
    public string name { get { return _name; } set { _name = value; } }
}
