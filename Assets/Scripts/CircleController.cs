using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class CircleController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Color color;

    [SerializeField] private Transform startPoint;
    [SerializeField] private  List<Transform> endPoints;
    [SerializeField] private  GameManager gameManager;
    [SerializeField] private  AudioSource audioSource;
    [SerializeField] private  AudioClip swapClip;

    private int selectedEndpointIndex = 0;
    private static bool isDragging;
    private Vector3 startPosition;
    private bool isSwapped = false;
    private static float lastCollideTime = 0;
    private static float collisionDuration = 0.5f;

    void Start()
    {
        startPosition = startPoint.GetComponent<RectTransform>().anchoredPosition;

    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PointerDown()
    {
        isSwapped = false;
        isDragging = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown();
    }

    public void Drag()
    {
        if (isDragging)
        {
            var mousePosition = Input.mousePosition;
            var scaler = GetComponentInParent<CanvasScaler>();
            Vector2 newPosition = Camera.main.ScreenToViewportPoint(new Vector3(mousePosition.x, mousePosition.y));
            newPosition.x -= 0.5f;
            newPosition.y -= 0.5f;
            newPosition = new Vector2(scaler.referenceResolution.x * newPosition.x, scaler.referenceResolution.y * newPosition.y);

            // calculate the distance between the current position and each endpoint
            float minDistance = Mathf.Infinity;
            for (int i = 0; i < endPoints.Count; i++)
            {
                float distance = (newPosition - endPoints[i].GetComponent<RectTransform>().anchoredPosition).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedEndpointIndex = i;
                }
            }

            // calculate the position of the line based on the selected endpoint
            var pos = Vector2.Lerp(startPoint.GetComponent<RectTransform>().anchoredPosition, endPoints[selectedEndpointIndex].GetComponent<RectTransform>().anchoredPosition,
                (newPosition - startPoint.GetComponent<RectTransform>().anchoredPosition).sqrMagnitude /
                (endPoints[selectedEndpointIndex].GetComponent<RectTransform>().anchoredPosition - startPoint.GetComponent<RectTransform>().anchoredPosition).sqrMagnitude);
            GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Drag();
    }

    public void pointerUp()
    {
        isDragging = false;

        // Reset circle to its original position
        if (!isSwapped)
        {
            GetComponent<RectTransform>().DOAnchorPos(startPosition, 0.2f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerUp();
    }

    private void SwapPositions(CircleController target, bool swapStartAndEnd)
    {
        isDragging = false;

        Vector2 tempPosition = target.startPosition;
        if (swapStartAndEnd)
        {
            Transform tempStart = startPoint;
            startPoint = target.startPoint;
            target.startPoint = tempStart;

            List<Transform> tempEndpoints = endPoints;
            endPoints = target.endPoints;
            target.endPoints = tempEndpoints;
        }
        GetComponent<RectTransform>().DOAnchorPos(tempPosition, 0.2f);
        var tempStartPos = startPosition;
        startPosition = target.startPosition;
        target.GetComponent<RectTransform>().DOAnchorPos(tempStartPos, 0.2f);
        target.startPosition = tempStartPos;

        PlaySound(swapClip);

        gameManager.CheckAllLines(this, target);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (Time.unscaledTime - lastCollideTime > collisionDuration)
        {
            lastCollideTime = Time.unscaledTime;
            Debug.Log("call " + col.name);
            CircleController otherCircle = col.gameObject.GetComponent<CircleController>();
            SwapPositions(otherCircle, true);
            isSwapped = true;
        }
    }
}