using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class WordSearch : MonoBehaviour
{
    public DynamicGrid dynamicGrid;
    public List<string> foundWords = new();
    public Dictionary<char, int> letterFrequency = new();
    public List<WordData> foundWordData = new();

    public void FoundInfo()
    {
        FindWords();
        PrintFoundWords();
        PrintLetterFrequencies();
    }

    private void PrintFoundWords()
    {
        foreach (var word in foundWords)
        {
            Debug.Log("Found word: " + word);
        }
    }

    private void PrintLetterFrequencies()
    {
        Debug.Log("Letter frequencies needed to form the words:");
        foreach (var letter in letterFrequency)
        {
            Debug.Log("Letter: " + letter.Key + " - Count: " + letter.Value);
        }
    }

    private void FindWords()
    {
        foundWords.Clear();
        letterFrequency.Clear();
        int rows = dynamicGrid.columns;
        int columns = dynamicGrid.rows;
        char[][] grid = new char[rows][];
        for (int index = 0; index < rows; index++)
        {
            grid[index] = new char[columns];
        }

        // Fill GRID
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var gridItem = dynamicGrid.gridContainer.GetChild(i * columns + j)
                    .GetComponentInChildren<TextMeshProUGUI>();
                grid[i][j] = gridItem != null && !string.IsNullOrEmpty(gridItem.text) ? gridItem.text[0] : ' ';
            }
        }
        
        // Horizontal word search
        for (int i = 0; i < rows; i++)
        {
            List<LetterData> currentWordLetters = new List<LetterData>();
            for (int j = 0; j < columns; j++)
            {
                if (grid[i][j] != ' ')
                {
                    currentWordLetters.Add(new LetterData(i, j, grid[i][j]+""));
                }
                else
                {
                    if (currentWordLetters.Count > 1)
                    {
                        string currentWord = string.Join("", currentWordLetters.Select(l => l.letter));
                        foundWordData.Add(new WordData(currentWord, new List<LetterData>(currentWordLetters)));
                        UpdateLetterFrequencyForWord(currentWord);
                    }

                    currentWordLetters.Clear();
                }
            }

            if (currentWordLetters.Count > 1)
            {
                string currentWord = string.Join("", currentWordLetters.Select(l => l.letter));
                foundWordData.Add(new WordData(currentWord, new List<LetterData>(currentWordLetters)));
                UpdateLetterFrequencyForWord(currentWord);
            }
        }

        // Vertical word search
        for (int j = 0; j < columns; j++)
        {
            List<LetterData> currentWordLetters = new List<LetterData>();
            for (int i = 0; i < rows; i++)
            {
                if (grid[i][j] != ' ')
                {
                    currentWordLetters.Add(new LetterData(i, j, grid[i][j]+ ""));
                }
                else
                {
                    if (currentWordLetters.Count > 1)
                    {
                        string currentWord = string.Join("", currentWordLetters.Select(l => l.letter));
                        foundWordData.Add(new WordData(currentWord, new List<LetterData>(currentWordLetters)));
                        UpdateLetterFrequencyForWord(currentWord);
                    }

                    currentWordLetters.Clear();
                }
            }

            if (currentWordLetters.Count > 1)
            {
                string currentWord = string.Join("", currentWordLetters.Select(l => l.letter));
                foundWordData.Add(new WordData(currentWord, new List<LetterData>(currentWordLetters)));
                UpdateLetterFrequencyForWord(currentWord);
            }
        }
    }


    private void UpdateLetterFrequencyForWord(string word)
    {
        Dictionary<char, int> wordLetterCount = new Dictionary<char, int>();
        foreach (char letter in word)
        {
            if (wordLetterCount.ContainsKey(letter))
                wordLetterCount[letter]++;
            else
                wordLetterCount[letter] = 1;
        }

        foreach (var letter in wordLetterCount)
        {
            if (letterFrequency.ContainsKey(letter.Key))
                letterFrequency[letter.Key] = Mathf.Max(letterFrequency[letter.Key], letter.Value);
            else
                letterFrequency[letter.Key] = letter.Value;
        }
    }
}