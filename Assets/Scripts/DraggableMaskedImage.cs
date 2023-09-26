using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableMaskedImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    protected Vector3 initialPosition;
    protected RectTransform display;
    protected Canvas canvas;

    private Image targetImage;
    public float scaleSpeed = 0.4f;
    public bool isImageClicked = false;

    public RectTransform mask;

    void Start() {
        canvas = GetComponentsInParent<Canvas>()[0];
        display = GetComponent<RectTransform>();
        targetImage = GetComponent<Image>();
        initialPosition = display.anchoredPosition;
    }

    void Update()
    {
        if (isImageClicked)
        {
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0f)
            {
                float newScale = targetImage.rectTransform.localScale.x + scrollWheel * scaleSpeed;
                Vector3 originalScale = targetImage.rectTransform.localScale;
                targetImage.rectTransform.localScale = new Vector3(newScale, newScale, 1f);

                if (IsOutOfBounds()) {
                    targetImage.rectTransform.localScale = originalScale;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow)) {
                float newScale = targetImage.rectTransform.localScale.x + 0.01f;
                Vector3 originalScale = targetImage.rectTransform.localScale;
                targetImage.rectTransform.localScale = new Vector3(newScale, newScale, 1f);

                if (IsOutOfBounds()) {
                    targetImage.rectTransform.localScale = originalScale;
                }
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                float newScale = targetImage.rectTransform.localScale.x - 0.01f;
                Vector3 originalScale = targetImage.rectTransform.localScale;
                targetImage.rectTransform.localScale = new Vector3(newScale, newScale, 1f);

                if (IsOutOfBounds()) {
                    targetImage.rectTransform.localScale = originalScale;
                }
            }
        }
    }

    public void UpdateAutoSize() {

        while (!IsOutOfBounds()) {
            float newScale = targetImage.rectTransform.localScale.x - 0.1f;
            targetImage.rectTransform.localScale = new Vector3(newScale, newScale, 1f);
        }

        while (IsOutOfBounds()) {
            float newScale = targetImage.rectTransform.localScale.x + 0.1f;
            targetImage.rectTransform.localScale = new Vector3(newScale, newScale, 1f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        initialPosition = display.anchoredPosition;
        FindObjectOfType<CustomizationManager>().LogChange();
    }

    public void OnDrag(PointerEventData eventData) 
    {
        isImageClicked = true;
        Vector3 previousPosition = display.anchoredPosition;
        display.anchoredPosition += eventData.delta / canvas.scaleFactor;
        if (IsOutOfBounds()) {
            display.anchoredPosition = previousPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        isImageClicked = false;

        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }
    private bool IsOutOfBounds()
    {
        // Get the corners of the image and parent RectTransforms in world space
        Vector3[] imageCorners = new Vector3[4];
        display.GetWorldCorners(imageCorners);

        Vector3[] parentCorners = new Vector3[4];
        mask.GetWorldCorners(parentCorners);

        if (imageCorners[0].x > parentCorners[0].x) {
            return true;
        }

        if (imageCorners[2].x < parentCorners[2].x) {
            return true;
        }

        if (imageCorners[0].y > parentCorners[0].y) {
            return true;
        }

        if (imageCorners[2].y < parentCorners[2].y) {
            return true;
        }

        return false;

    }

}
