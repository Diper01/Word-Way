using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LetterSelector : MonoBehaviour
{
    public TextMeshProUGUI CurrentWord;
    public GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private readonly HashSet<GameObject> selectedLetters = new();
    public LineRenderer lineRenderer;
    public List<Vector3> pointPos = new();
    public Material rectTrans;
    private Color color;
    private AudioSource soundChoseLetter;

    private void Start()
    {
        soundChoseLetter = GetComponent<AudioSource>();
        soundChoseLetter.pitch = 1f;
        color = rectTrans.color;
        eventSystem = EventSystem.current;
        if (raycaster == null)
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();
        }
    }

    private void UpdateLine(Vector2 currentFingerPosition)
    {
        lineRenderer.positionCount = pointPos.Count + 1;
        for (int i = 0; i < pointPos.Count; i++)
        {
            lineRenderer.SetPosition(i, pointPos[i]);
        }

        lineRenderer.SetPosition(pointPos.Count, currentFingerPosition);
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerUpdate += HandleFingerSwipe;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= HandleFingerSwipe;
        LeanTouch.OnFingerUp -= HandleFingerUp;
    }

    private void HandleFingerSwipe(LeanFinger finger)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = finger.ScreenPosition
        };
        float screenScaleFactor = Camera.main.orthographicSize * 2f;
        lineRenderer.widthMultiplier = screenScaleFactor;
        List<RaycastResult> results = new List<RaycastResult>();

        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Letter"))
            {
                if (!selectedLetters.Contains(result.gameObject))
                {
                    result.gameObject.GetComponent<Image>().color = color;
                    selectedLetters.Add(result.gameObject);
                    SelectLetter(result.gameObject);
                    pointPos.Add(result.gameObject.GetComponent<RectTransform>().anchoredPosition);
                    soundChoseLetter.Play();
                    soundChoseLetter.pitch += 0.4f;
                }

                break;
            }
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(gameObject.GetComponent<RectTransform>(),
                Input.mousePosition, Camera.main))
        {
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gameObject.GetComponent<RectTransform>(),
                pointerData.position,
                Camera.main,
                out localPoint
            );

            UpdateLine(localPoint);
        }
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        GameManager.Instance.CheckIfWordIsCorrect(CurrentWord.text);
        pointPos.Clear();
        soundChoseLetter.pitch = 1f;
        lineRenderer.positionCount = 0;
        ClearSelectedLetters();
        StartCoroutine(ClearSelection());
    }

    private void ClearSelectedLetters()
    {
        if (selectedLetters.Count != 0)
        {
            foreach (var letter in selectedLetters)
            {
                letter.gameObject.GetComponent<Image>().color = new Color(1, 0, 0, 0);
            }
        }
    }

    private void SelectLetter(GameObject letter) => CurrentWord.text += letter.name;


    private IEnumerator ClearSelection()
    {
        yield return new WaitForSeconds(0.6f);
        //TODO Add animation for word
        selectedLetters.Clear();

        CurrentWord.text = "";
    }
}