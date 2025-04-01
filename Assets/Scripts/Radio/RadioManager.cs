using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadioManager : MonoBehaviour
{
    [Header("Input")]

    private PlayerInput playerInput;


    [Header("stations")]

    [SerializeField] private RadioStations[] radioStations;
    [SerializeField] private AudioSource stationSong;

    private int stationIndex;
    private int lastSongIndex;
    private int SongIndex;


    private void Awake()
    {
        playerInput = new PlayerInput();


        PlayNextSong();
    }


    private void OnEnable()
    {
        playerInput.Enable();
    }


    private void OnDisable()
    {
        playerInput.Disable();
    }


    public void ONNextstation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            stationIndex++;


            ChangeStation();

        }
    }


    public void ONPreviusstation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            stationIndex--;


            ChangeStation();
        }
    }


    private void Update()
    {
        
    }


    private void ChangeStation()
    {
        if (stationIndex > radioStations.Length)
        {
            stationIndex = 0;
        }


        PlayNextSong();
    }


    private void PlayNextSong()
    {
        SongIndex = Random.Range(0, radioStations[stationIndex].songs.Length);


        stationSong.clip = radioStations[stationIndex].songs[SongIndex];
        stationSong.Play();


        StartCoroutine(WaitForEndOfSong());
    }


    private IEnumerator WaitForEndOfSong()
    {
        yield return new WaitForSeconds(stationSong.clip.length);


        PlayNextSong();
    }
}
