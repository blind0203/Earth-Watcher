using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameController : Singleton<GameController> 
{
    [SerializeField] private GameObject _gameScreen, _menuScreen, _endScreen;

    [SerializeField] private Text[] _agressionTexts;

    [SerializeField] private int _maxArgessionLevel = 100;
    [SerializeField] private Image[] _teamSliders;

    [SerializeField] private Vector3 _ingameCamPosition, _warCamPosition;

    [SerializeField] private GameObject[] _dropPrefabs;

    [SerializeField] private CardsLibrary _cardsLibrary;

    private WaitForSeconds _tickDelay = new WaitForSeconds(1f), _spawnDelay = new WaitForSeconds(5f);

    private int[] _teamAgressions;

    // [SerializeField]private Card[] _cards;

    private float _randKoef = .5f;

    public enum GameState 
    {
        BeforeStart,
        GameLoop,
        War,
        GameEnd
    }

    private GameState _gameState = GameState.BeforeStart;

    private void Start()
    {
        //GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
        
    }

    public void StartGame() 
    {
        //_cards = _cardsLibrary.cards;

        _menuScreen.SetActive(false);

        Camera.main.transform.DOMove(_ingameCamPosition, .5f).SetEase(Ease.OutCubic).OnComplete(() => 
        {
            StartCoroutine(Ticker());
            StartCoroutine(Spawner());
        });
    }

    private IEnumerator Ticker() 
    {
        _gameState = GameState.GameLoop;
        _gameScreen.SetActive(true);

        _gameScreen.transform.DOScale(1f, .5f).SetEase(Ease.OutCubic).From(15);
        
        _teamAgressions = new int[2] { 0, 0 };

        while (_gameState == GameState.GameLoop)
        {
            yield return _tickDelay;

            float _maxAgrPercent = 0;

            for (int i = 0; i < _teamAgressions.Length; i++)
            {
                int id = i;
                _teamAgressions[id]++;
                //Debug.Log(_teamAgressions[id]);
                _teamSliders[id].DOFillAmount(1 / (float)_maxArgessionLevel * (float)_teamAgressions[id], .25f).SetEase(Ease.OutCubic);

                _agressionTexts[id].text = _teamAgressions[id] + "%";

                float mAP = 1f / (float)_maxArgessionLevel * (float)_teamAgressions[id];

                _maxAgrPercent = mAP > _maxAgrPercent ? mAP : _maxAgrPercent;

                MusicController.Instance.SetAgressionPercent(_maxAgrPercent);

                if (_teamAgressions[id] >= _maxArgessionLevel) 
                {
                    War();
                }
            }
        }
    }

    private IEnumerator Spawner()
    {
        while (_gameState == GameState.GameLoop)
        {
            yield return _spawnDelay;

            for (int i = 0; i < 2; i++)
            {
                SpawnDrop(i);
            }
        }
    }

    private void SpawnDrop(int teamIndex) 
    {
        Vector3 spawnPoint = new Vector3(Random.Range(-1f, 1f), Random.Range(-.2f, .2f), Random.Range(-1f, 1f));
        spawnPoint = spawnPoint.normalized * 5.25f;

        DropComponent dc = Instantiate(_dropPrefabs[teamIndex]
            , spawnPoint
            , Quaternion.LookRotation(spawnPoint)
            , null)
            .transform.GetComponent<DropComponent>();

        float r = Random.Range(0f, 1f);
        Card cToSpawn;

        if (r < _randKoef)
        {
            //blue
            _randKoef = Mathf.Clamp01(_randKoef - .1f);

            int rCardIndex = Random.Range(1, _cardsLibrary.cards.Length);

            if (rCardIndex % 2 == 1) rCardIndex--;

            cToSpawn = _cardsLibrary.cards[rCardIndex];
        }

        else 
        {
            //red

            _randKoef = Mathf.Clamp01(_randKoef + .1f);

            int rCardIndex = Random.Range(2, _cardsLibrary.cards.Length);

            if (rCardIndex % 2 == 0) rCardIndex--;

            cToSpawn = _cardsLibrary.cards[rCardIndex];
        }

        dc.InitDrop(cToSpawn, spawnPoint);
    }

    public void ProcessCardData(Card card) 
    {
        for (int i = 0; i < _teamAgressions.Length; i++)
        {
            _teamAgressions[i] += card.TeamValues[i];

            _teamAgressions[i] = Mathf.Clamp(_teamAgressions[i], 0, _maxArgessionLevel);
        }
    }

    private void War() 
    {
        Camera.main.transform.DOMove(_warCamPosition, .25f).SetEase(Ease.OutCirc);
        Camera.main.transform.DORotateQuaternion(Quaternion.Euler(Vector3.zero), .25f).SetEase(Ease.OutCirc);

        DropComponent[] dropComponents = FindObjectsOfType<DropComponent>();

        for (int i = 0; i < dropComponents.Length; i++) 
        {
            Destroy(dropComponents[i].gameObject);
        }

        _gameState = GameState.War;
        _gameScreen.SetActive(false);
        StopAllCoroutines();
        NukeController.Instance.StartWar();
    }

    public void EndGame() 
    {
        _gameState = GameState.GameEnd;
        _endScreen.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public GameState CurrentGameState() => _gameState;
    public void SetGameState(GameState gameState) => _gameState = gameState;
}
