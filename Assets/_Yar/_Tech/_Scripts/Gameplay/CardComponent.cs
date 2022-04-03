using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _cardBack;
    [SerializeField] private Text _upperText, _lowerText;
    [SerializeField] private AudioSource _onDropAudio;

    private Vector3 _startPos;

    private bool _isPickable = false;

    private Card _card;

    public void InitCard(Card card) 
    {
        _card = card;

        _cardBack.sprite = _card.TeamImage;

        if (_card.TeamIndex == 0)
        {
            _upperText.text = _card.TeamValues[0].ToString();
            _lowerText.text = _card.TeamValues[1].ToString();
        }
        else 
        {
            _upperText.text = _card.TeamValues[1].ToString();
            _lowerText.text = _card.TeamValues[0].ToString();
        }


        transform.DOScale(1, .25f).From(0).SetEase(Ease.OutBack).OnComplete(() => 
        {
            _isPickable = true;
        });
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        HandController.Instance.TooglePlanetEmission(false);
        CamController.Instance.SetRotatingFlag(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isPickable)
        {
            transform.DOScale(1.05f, .25f).SetEase(Ease.OutBack);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isPickable)
        {
            transform.DOScale(1, .25f).SetEase(Ease.OutBack);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isPickable)
        {
            transform.DOScale(1, .25f).SetEase(Ease.OutBack);
            _startPos = transform.localPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isPickable)
        {
            //transform.localPosition += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            Vector2 dif = HandController.Instance.GetScreenDifference();
            transform.localPosition += new Vector3(eventData.delta.x * dif.x , eventData.delta.y * dif.y);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isPickable)
        {
            CheckPlanetHover();
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        HandController.Instance.TooglePlanetEmission(true);
        CamController.Instance.SetRotatingFlag(false);
    }

    private void CheckPlanetHover() 
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition + Vector3.forward);

        if (Physics.Raycast(r, out RaycastHit hit, 100f))
        {
            _isPickable = false;

            _onDropAudio.Play();

            HandController.Instance.DecreaseCardsCount();

            transform.SetParent(transform.parent.parent);

            transform.DOScale(0, .25f).SetEase(Ease.InBack).OnComplete(() => 
            {
                SendCardData();
            });
        }

        else 
        {
            transform.DOLocalMove(_startPos, .25f);
        }
    }

    private void SendCardData() 
    {
        GameController.Instance.ProcessCardData(_card);

        Destroy(gameObject);
    }
}
