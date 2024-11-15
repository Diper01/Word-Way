using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class CircleLayout : MonoBehaviour
{
    public GameObject letterPrefab;
    public float radius = 300f;
    public List<GameObject> lettersOnBoard = new();
    private RectTransform rectTransform;
    private float size;

    private void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        size = rectTransform.rect.width / 3.6f;
        radius = (rectTransform.rect.width / 2) - (size / 2 + size / 10);
    }

    public void SpawnLetters(List<LetterCount> letters)
    {
        List<LetterCount> letters2 = new();
        foreach (var let in letters)
        {
            for (int i = 0; i < let.count; i++)
            {
                letters2.Add(let);
            }
        }

        letters = letters2.OrderBy(x => Random.value).ToList();


        foreach (var letter in lettersOnBoard)
        {
            Destroy(letter);
        }

        lettersOnBoard.Clear();

        var angularStep = 360f / letters.Count;
        float currentAngle = 0;
        CreateLetter(0, radius, letters[0]);

        for (int i = 1; i < letters.Count; i++)
        {
            currentAngle += angularStep;
            float x = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            float y = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            CreateLetter(x, y, letters[i]);
        }
    }

    private void CreateLetter(float x, float y, LetterCount letterDate)
    {
        GameObject letterObject = Instantiate(letterPrefab, transform);
        RectTransform letterRectTransform = letterObject.GetComponent<RectTransform>();
        letterRectTransform.anchoredPosition = new Vector2(x, y);
        letterRectTransform.name = letterDate.letter + "";
        letterRectTransform.GetComponentInChildren<TextMeshProUGUI>().text = letterDate.letter + "";
        letterRectTransform.sizeDelta = new Vector2(size, size);

        lettersOnBoard.Add(letterObject);
    }
}