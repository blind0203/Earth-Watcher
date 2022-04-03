using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _dropImage;

    private Card _card;

    private bool _isPickable = false;

    private Coroutine _decayCycle;

    private WaitForSeconds _decayDelay = new WaitForSeconds(5);

    public void InitDrop(Card card, Vector3 point) 
    {
        _card = card;

        //_dropImage.sprite = _card.TeamImage;

        transform.DOScale(new Vector3(1, 1, .25f), .5f).From(.1f).SetEase(Ease.OutBack);

        transform.DOMove(point, 1f).From(point + transform.forward * 3).SetEase(Ease.InCubic).OnComplete(() => 
        {
            _isPickable = true;
            _decayCycle = StartCoroutine(DecayCycle());
        });
    }

    private void OnMouseUpAsButton()
    {
        if (_isPickable)
        {
            PickDrop();
        }
    }

    private IEnumerator DecayCycle() 
    {
        yield return _decayDelay;

        DeathAfterDecay();
    }

    private void DeathAfterDecay() 
    {
        _isPickable = false;

        transform.DOScale(0, .25f).SetEase(Ease.InBack).OnComplete(() => 
        {
            Destroy(gameObject);
        });
    }

    private void PickDrop()
    {
        _isPickable = false;

        if (HandController.Instance.IsHandFull())
        {
            HandController.Instance.OnFullHand();

            Vector3 pos = transform.position;

            transform.DOMove(transform.position + transform.forward, .15f).SetEase(Ease.OutCubic)
                .OnComplete(() => 
                {
                    transform.DOMove(pos, .15f).SetEase(Ease.InCubic).OnComplete(() => 
                    {
                        _isPickable = true;
                    });
                });
        }
        else
        {
            HandController.Instance.IncreaseCardsCount();

            GetComponentInChildren<AudioSource>().Play();

            StopCoroutine(_decayCycle);

            transform.DOMove(transform.position + transform.forward * 1.5f, .5f).SetEase(Ease.OutCubic);
            transform.DOScale(0, .25f).SetDelay(.25f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                HandController.Instance.SpawnCard(_card);
                Destroy(gameObject);
            });
        }
    }
}
