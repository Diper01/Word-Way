using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject prefab;
    public int rows = 4;
    public int columns = 4;
    private const int spacing = 8;

    public GridLayoutGroup _gridLayoutGroup;
    private LevelData lvlData;
    
    private Dictionary<Vector2Int, Image> cellDictionary = new();


    public void GenerateGirdForLvl(LevelData lvlDate)
    {
        rows = lvlDate.gridSize.rows;
        columns = lvlDate.gridSize.columns;
        UpdateGridLayout();
        GenerateGrid(lvlDate.letters);
    }

    private void UpdateGridLayout()
    {
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = columns;

        RectTransform gridRectTransform = GetComponent<RectTransform>();
        float containerWidth = gridRectTransform.rect.width;
        float containerHeight = gridRectTransform.rect.height;

        float cellSize = Mathf.Min(
            (containerWidth - (columns - 1) * spacing) / columns,
            (containerHeight - (rows - 1) * spacing) / rows
        );

        _gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        _gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }

    private void GenerateGrid(LetterData[] letters)
    {
        cellDictionary.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < rows * columns; i++)
        {
            GameObject cube = Instantiate(prefab, transform);
            Image image = cube.GetComponent<Image>();

            int currentRow = i / columns;
            int currentColumn = i % columns;

            foreach (var letter in letters)
            {
                if (letter.column == currentColumn && letter.row == currentRow)
                {
                    var childObject = cube.transform.GetChild(0).gameObject;
                    cube.GetComponent<Image>().enabled = true;
                    childObject.SetActive(true);
                    cube.GetComponentInChildren<TextMeshProUGUI>().text = letter.letter;
                    childObject.SetActive(false);
                    break;
                }
            }

            Vector2Int cellCoords = new Vector2Int(currentRow, currentColumn);
            cellDictionary[cellCoords] = image;
        }
       
    }

    public void LoadOpenedWords(LevelSettings levelSettings)
    {
        if (levelSettings == null) return;
        foreach (var word in levelSettings.wordsData)
        {
            foreach (var letter in word.letters)
            {
                Image image = GetCellRectTransform(letter.row, letter.column);
                image.transform.GetChild(0).gameObject.SetActive(true);
                TweenManager.Instance.ChangeCellColor(image);
            }
        }
    }


    public Image GetCellRectTransform(int row, int column)
    {
        Vector2Int cellCoords = new Vector2Int(row, column);
        if (cellDictionary.TryGetValue(cellCoords, out Image item))
        {
            return item;
        }
        //Debug.LogWarning($"Cell with coordinates ({row}, {column}) not found.");
        return null;
    }
    
    
}