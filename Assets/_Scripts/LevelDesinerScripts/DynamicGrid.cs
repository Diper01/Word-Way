using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class DynamicGrid : MonoBehaviour
{
    public GameObject gridItemPrefab;
    public int columns = 4;
    public int rows = 4;
    public int padding = 10;
    
    [HideInInspector] public RectTransform gridContainer;
    [HideInInspector] public GridLayoutGroup gridLayoutGroup;
    [HideInInspector] public WordSearch wordSearch;

    private void OnEnable()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridContainer = GetComponent<RectTransform>();
        wordSearch = GetComponent<WordSearch>();
    }

    public void UpdateGrid()
    {
        UpdateCellSize();
        GenerateGrid();
    }

    private void UpdateCellSize()
    {
        float containerWidth = gridContainer.rect.width;
        float containerHeight = gridContainer.rect.height;

        float cellSize = Mathf.Min(
            (containerWidth - (rows - 1) * padding) / rows,
            (containerHeight - (columns - 1) * padding) / columns
        );

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(padding, padding);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = rows;
    }

    private void GenerateGrid()
    {
        for (int i = gridContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject gridItem = Instantiate(gridItemPrefab, gridContainer);
                gridItem.name = $"GridItem_{i}_{j}";
            }
        }
    }

    public void GetGridForLevel(LevelData levelData)
    {
        columns = levelData.gridSize.rows;
        rows = levelData.gridSize.columns;
        UpdateGrid();
        foreach (var letterData in levelData.letters)
        {
            int index = letterData.row * rows + letterData.column;

            if (index >= 0 && index < gridContainer.childCount)
            {
                GameObject gridItem = gridContainer.GetChild(index).gameObject;
                TextMeshProUGUI textComponent = gridItem.GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    textComponent.text = letterData.letter;
                }
            }
        }
    }
}