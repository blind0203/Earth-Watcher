using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandController : Singleton<HandController>
{
    [SerializeField] private GameObject[] _cardPrefabs;

    [SerializeField] private Image _planetEmission;

    [SerializeField] private AudioSource _fullHandSound;

    private int _cardsCount = 0, _cardLimit = 4;

    private float _screenDifferenceX, _screenDifferenceY;

    private void Start()
    {
        _screenDifferenceX = (float)1920 / Screen.width;
        _screenDifferenceY = (float)1080 / Screen.height;
    }

    public void SpawnCard(Card card) 
    {
        CardComponent cc = Instantiate(_cardPrefabs[card.TeamIndex], transform)
            .transform.GetComponent<CardComponent>();

        cc.InitCard(card);
    }

    public void OnFullHand() 
    {
        _fullHandSound.PlayOneShot(_fullHandSound.clip);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform c = transform.GetChild(i).transform;
            c.DORotate(Vector3.forward * 5, .15f).SetEase(Ease.Linear)
                .OnComplete(() => 
                {
                    c.DORotate(Vector3.zero, .15f).SetEase(Ease.Linear);
                });
        }
    }

    public void TooglePlanetEmission(bool isOn) 
    {
        int mul = isOn ? 1 : 0;
        _planetEmission.DOColor(new Color(.5f, .5f, .5f, .1f) * mul, .25f).SetEase(Ease.OutCubic);
    }

    public void IncreaseCardsCount() => _cardsCount++;
    public void DecreaseCardsCount() => _cardsCount--;

    public int CardLimit() => _cardLimit;
    public bool IsHandFull() => _cardLimit == _cardsCount;

    public Vector2 GetScreenDifference() => new Vector2(_screenDifferenceX, _screenDifferenceY);
}
