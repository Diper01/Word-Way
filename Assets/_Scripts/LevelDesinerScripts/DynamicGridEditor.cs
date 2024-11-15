#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

[CustomEditor(typeof(DynamicGrid))]
public class DynamicGridEditor : Editor
{
    private LevelSaver _levelSaver = new();
    private DynamicGrid _grid;
    private int _savedLevel = 1;
    private int _loadLevel = 1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _grid = (DynamicGrid)target;
        if (GUILayout.Button("Reload Grid"))
        {
            _grid.UpdateGrid();
        }

        if (GUILayout.Button("Letter Info"))
        {
            _grid.wordSearch.FoundInfo();
        }
        EditorGUILayout.BeginHorizontal();
        _savedLevel = EditorGUILayout.IntField("Level", _savedLevel);
        if (GUILayout.Button("Save Grid"))
        {
            SaveGrid();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _loadLevel = EditorGUILayout.IntField("Level", _loadLevel);
        if (GUILayout.Button("Load Grid"))
        {
            GetGridForLevel(_loadLevel);
        }

        EditorGUILayout.EndHorizontal();
    }


    async void GetGridForLevel(int number)
    {
        LevelData levelData = await _levelSaver.LoadLevelAsync(number);
        _grid.GetGridForLevel(levelData);
    }

    private void SaveGrid()
    {
        GridSize gridSize = new GridSize(_grid.columns, _grid.rows);
        LetterData[] letterData = GetLetterDataArray();
        _grid.wordSearch.FoundInfo();
        LevelData levelData = new LevelData(_savedLevel, gridSize, letterData, _grid.wordSearch.foundWordData,
            ConvertDictionaryToList(_grid.wordSearch.letterFrequency));
        _levelSaver.SaveLevel(levelData);
        _grid.wordSearch.foundWordData.Clear();
        _grid.wordSearch.foundWords.Clear();
        _grid.wordSearch.letterFrequency.Clear();
    }

    private LetterData[] GetLetterDataArray()
    {
        var letterDataList = new List<LetterData>();

        for (int i = 0; i < _grid.gridContainer.childCount; i++)
        {
            Transform gridItem = _grid.gridContainer.GetChild(i);
            var textComponent = gridItem.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent == null || string.IsNullOrEmpty(textComponent.text)) continue;
            int row = i / _grid.rows;
            int column = i % _grid.rows;
            string letter = textComponent.text;

            letterDataList.Add(new LetterData(row, column, letter));
        }

        return letterDataList.ToArray();
    }

    private List<LetterCount> ConvertDictionaryToList(Dictionary<char, int> wordLetterCount)
    {
        List<LetterCount> letterCountList = new List<LetterCount>();
        foreach (var letter in wordLetterCount)
        {
            letterCountList.Add(new LetterCount(letter.Key, letter.Value));
        }

        return letterCountList;
    }
}
#endif