using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordManager
{
    private readonly GridManager _gridManager;
    private readonly RectTransform _letterSelector;
    private readonly GameObject _temporaryLetterPrefab;
    private LevelData _levelData;

    public WordManager(GridManager gridManager, RectTransform letterSelector, GameObject temporaryLetterPrefab)
    {
        _gridManager = gridManager;
        _letterSelector = letterSelector;
        _temporaryLetterPrefab = temporaryLetterPrefab;
    }

    public void SetLevelData(LevelData levelData) => _levelData = levelData;

    public void CheckIfWordIsCorrect(string word, List<WordData> words)
    {
        foreach (var wordData in words)
        {
            if (IsNewWord(wordData, word))
            {
                AddWordToSaveAndAnimate(wordData, word);
            }
            else
            {
                //TODO anim when wrong
            }
        }
    }

    private bool IsNewWord(WordData wordData, string word) =>
        wordData.word.Equals(word) && !GameManager.Instance._lvlManager.GetLevelSettings().wordsData.Contains(wordData);

    private void AddWordToSaveAndAnimate(WordData wordData, string word)
    {
        GameManager.Instance._lvlManager.AddWordToSave(wordData);
        var spawnedUI = SpawnUIAtCharacter(_letterSelector.GetComponentInChildren<TextMeshProUGUI>());
        var wordLocation = GenerateWordLocation(word);
        var images = GetOpenedImages(wordData);

        TweenManager.Instance.CorrectWordsAnim(spawnedUI, wordLocation, images,
            () => GameManager.Instance.CheckIfWin());
    }


    private List<Image> GetOpenedImages(WordData word)
    {
        var images = new List<Image>();
        foreach (var letter in word.letters)
        {
            images.Add(_gridManager.GetCellRectTransform(letter.row, letter.column));
        }

        return images;
    }


    private WordLocation GenerateWordLocation(string targetWord)
    {
        foreach (WordData word in _levelData.words)
        {
            if (word.word == targetWord)
            {
                var wordsList = GetWordRectTransforms(word);
                return new WordLocation(wordsList);
            }
        }

        Debug.LogWarning("Word not found in LevelData.");
        return null;
    }

    private List<RectTransform> GetWordRectTransforms(WordData word)
    {
        List<RectTransform> wordsList = new();
        foreach (LetterData letter in word.letters)
        {
            var rect = GetObjectAt(letter.row, letter.column);
            if (rect != null)
            {
                wordsList.Add(rect);
            }
        }

        return wordsList;
    }

    private RectTransform GetObjectAt(int row, int column)
    {
        var gridRect = _gridManager.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridRect);

        int index = row * _gridManager.columns + column;
        return _gridManager.gameObject.transform.GetChild(index) as RectTransform;
    }

    private List<RectTransform> SpawnUIAtCharacter(TextMeshProUGUI currentWord)
    {
        if (currentWord == null) return null;

        var spawnUIChars = new List<RectTransform>();
        var textInfo = currentWord.textInfo;

        for (int i = 0; i < currentWord.text.Length; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var uiPosition = CalculateCharacterUIPosition(currentWord, charInfo);
            spawnUIChars.Add(CreateCharacterUIObject(charInfo.character, uiPosition, currentWord.font));
        }

        return spawnUIChars;
    }

    private Vector2 CalculateCharacterUIPosition(TextMeshProUGUI currentWord, TMP_CharacterInfo charInfo)
    {
        Vector3 charMidBaseline = (charInfo.bottomLeft + charInfo.topRight) / 2;
        Vector3 worldPoint = currentWord.transform.TransformPoint(charMidBaseline);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_letterSelector, worldPoint, null, out var uiPosition);

        return uiPosition;
    }

    private RectTransform CreateCharacterUIObject(char character, Vector2 uiPosition, TMP_FontAsset font)
    {
        var newUIObject = Object.Instantiate(_temporaryLetterPrefab, _letterSelector);
        var newUIObjectText = newUIObject.GetComponent<TextMeshProUGUI>();
        newUIObjectText.font = font;
        newUIObjectText.text = character.ToString();
        newUIObject.GetComponent<RectTransform>().anchoredPosition = uiPosition;

        return newUIObject.GetComponent<RectTransform>();
    }
}