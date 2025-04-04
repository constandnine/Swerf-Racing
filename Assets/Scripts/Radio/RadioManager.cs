using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RadioManager : MonoBehaviour
{
    [Header("Input")]

    private PlayerInput playerInput;


    [Header("stations")]

    [SerializeField] private RadioStations[] radioStations;
    [SerializeField] private AudioSource stationSong;

    public int stationIndex;
    private int lastSongIndex;
    private int SongIndex;


    [Header("UI")]

    [SerializeField] private TextMeshProUGUI stationName;

    [SerializeField] private float fadeOutSpeed;

    private Color stationNameColor;


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


    private void Start()
    {
        ShowStationName();
    }


    public void ONNextstation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            stationIndex++;


            ChangeStation();


            ShowStationName();
        }
    }


    public void ONPreviusstation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            stationIndex--;


            ChangeStation();


            ShowStationName();
        }
    }


    private void ShowStationName()
    {
        StartCoroutine(StationChanged());


        if (stationNameColor.a < .1f)
        {
            stationNameColor.a = 1;
            stationName.enabled = false;
        }
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

    private IEnumerator StationChanged()
    {
        stationName.enabled = true;


        stationName.text = radioStations[stationIndex].name;


        stationNameColor.a -= fadeOutSpeed * Time.deltaTime;


        stationNameColor.a = Mathf.Clamp01(stationNameColor.a);


        stationName.color = stationNameColor;


        yield return new WaitForSeconds(2);
    }
}
