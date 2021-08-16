using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Sprite CardImage;

    [SerializeField]
    private GameObject ImagesHolderGameObject;

    [SerializeField]
    private float ImagesRadius;

    [SerializeField]
    private GameObject BackCardGameObject;

    public Action<GameObject,int> OnCardClicked;

    private int PairId;

    private List<GameObject> FaceImages;

    private bool isFading = false;

    const string TurnUpAnim = "TurnUp";

    const string TurnDownAnim = "TurnDown";
    public void Init(int pairId)
    {
        FaceImages = new List<GameObject>();
        PairId = pairId;
        PopulateCardFace();
    }

    private void FixedUpdate() {
        if (isFading)
        {
            var spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRender in spriteRenderers)
            {
                var newColor = spriteRender.color;
                newColor.a = newColor.a - 0.01f;
                spriteRender.color = newColor;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnCardClicked?.Invoke(gameObject,PairId);
    }

    private void PopulateCardFace()
    {
        for (int i = 0; i <= PairId; i++)
        {
            var newImageGameObject = InstantiateNewImage(i);
            FaceImages.Add(newImageGameObject);
        }
    }

    private GameObject InstantiateNewImage(int index)
    {
        var positionPercent = (float)index / ((float)PairId + 1f);
        var positionInRad = positionPercent * 2f * Mathf.PI;
        var newImagePosition = gameObject.transform.position;
        newImagePosition += new Vector3(ImagesRadius * Mathf.Sin(positionInRad),
                                        ImagesRadius * Mathf.Cos(positionInRad));
        var imageGameObject = CreateSpriteRenderer();
        imageGameObject.transform.SetParent(ImagesHolderGameObject.transform);
        var positionInDegrees = positionPercent * -360f;
        imageGameObject.transform.localPosition = newImagePosition;
        imageGameObject.transform.localEulerAngles = new Vector3(0,0,positionInDegrees);
        imageGameObject.transform.localScale = new Vector3(40, 40, 40);
        return imageGameObject;
    }

    private GameObject CreateSpriteRenderer()
    {
        var cardGameObject = new GameObject ();
        cardGameObject.name = "CardImage";
        var spriteRenderer = cardGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CardImage;
        spriteRenderer.color = CalculateCardColor();
        return cardGameObject;
    }

    private Color CalculateCardColor()
    {
        // Random color that is equal to all similar pairIds
        // Random color generation created from testing ColorTest scene and finding good numbers
        return new Color(
            (((float)PairId * 0.68f) + 0.32f) % 1f,
            (((float)PairId * 0.41f) + 0.42f) % 1f,
            (((float)PairId * 0.86f) + 0.39f) % 1f);
    }

    public void TurnCardUp()
    {
        GetComponent<Animator>().Play(TurnUpAnim);
    }

    public void TurnCardDown()
    {
        GetComponent<Animator>().Play(TurnDownAnim);
    }

    public void StartFade()
    {
        isFading = true;
    }
}
