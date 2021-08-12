using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Sprite CardImage;

    [SerializeField]
    private float ImagesRadius;

    public Action<int> OnCardClicked;

    private int PairId;

    private List<GameObject> FaceImages;

    public void Init(int pairId)
    {
        FaceImages = new List<GameObject>();
        PairId = pairId;
        PopulateCardFace();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("card clicked");
        OnCardClicked?.Invoke(PairId);
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
        var positionInDegrees = positionPercent * 360f;
        var newImagePosition = gameObject.transform.position;
        newImagePosition += new Vector3(ImagesRadius * Mathf.Sin(positionInRad),
                                        ImagesRadius * Mathf.Cos(positionInRad));
        var imageGameObject = CreateSpriteRenderer();
        imageGameObject.transform.SetParent(gameObject.transform);
        imageGameObject.transform.SetPositionAndRotation(newImagePosition, Quaternion.Euler(0,0,positionInDegrees + 180));
        return imageGameObject;
    }

    private GameObject CreateSpriteRenderer()
    {
        var gameObject = new GameObject ();
        gameObject.name = "CardImage";
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CardImage;
        spriteRenderer.color = CalculateCardColor();
        spriteRenderer.sortingOrder = 2;
        return gameObject;
    }

    private Color CalculateCardColor()
    {
        // Random color that is equal to all similar pairIds
        // Random color generation created from arbitrary values
        return new Color(
            (((float)PairId * 0.6f) + 0.7f) % 1f,
            (((float)PairId * 0.4f) + 0.6f) % 1f,
            (((float)PairId * 0.8f) + 0.1f) % 1f);
    }
}
