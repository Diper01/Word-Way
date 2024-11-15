using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class LevelSaver
{
    public string GetFilePath(int levelNumber)
    {
        return Path.Combine(Application.persistentDataPath, $"level_{levelNumber}.json");
    }

    public void SaveLevel(LevelData levelData)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, $"level_{levelData.level}.json");
        Debug.Log("Attempting to save level to StreamingAssets path: " + filePath); 

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Level saved to " + filePath);
    }


    public async Task<LevelData> LoadLevelAsync(int levelNumber) //This logs so useful in testing
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, $"level_{levelNumber}.json");
        string json = null;

        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);

            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                json = www.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Failed to load level: " + www.error);
                return null;
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                json = File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogWarning("Level file not found at " + filePath);
                return null;
            }
        }

        if (!string.IsNullOrEmpty(json))
        {
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            Debug.Log("Level loaded successfully from " + filePath);
            return levelData;
        }

        Debug.LogError("Failed to load level: HAHAHAH");
        return null;
    }
}

public class LevelSettingsSaver
{
    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "SaveLevelSettings.json");
    }

    public void SaveLevelSettings(LevelSettings levelSettings)
    {
        string filePath = GetFilePath();
        Debug.Log("Attempting to save level settings to path: " + filePath);

        string json = JsonUtility.ToJson(levelSettings, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Level settings saved to " + filePath);
    }

    public async Task<LevelSettings> LoadLevelSettingsAsync()
    {
        string filePath = GetFilePath();
        string json = null;

        if (File.Exists(filePath))
        {
            try
            {
                json = await Task.Run(() => File.ReadAllText(filePath));
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load level settings: " + e.Message);
                return new LevelSettings(new List<WordData>(), 1);
            }
        }
        else
        {
            Debug.LogWarning("Level settings file not found at " + filePath);
            return new LevelSettings(new List<WordData>(), 1);
        }

        if (!string.IsNullOrEmpty(json))
        {
            LevelSettings levelSettings = JsonUtility.FromJson<LevelSettings>(json);
            Debug.Log("Level settings loaded successfully from " + filePath);
            return levelSettings;
        }

        return null;
    }
}


#region Data Clases

[Serializable]
public class LevelData
{
    public int level;
    public GridSize gridSize;
    public LetterData[] letters;
    public List<WordData> words;
    public List<LetterCount> wordLetterCount;

    public LevelData(int level, GridSize gridSize, LetterData[] letters, List<WordData> words,
        List<LetterCount> wordLetterCount)
    {
        this.level = level;
        this.gridSize = gridSize;
        this.letters = letters;
        this.words = words;
        this.wordLetterCount = wordLetterCount;
    }
}

[Serializable]
public struct GridSize
{
    public int rows;
    public int columns;

    public GridSize(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
    }
}

[Serializable]
public struct LetterData
{
    public int row;
    public int column;
    public string letter;

    public LetterData(int row, int column, string letter)
    {
        this.row = row;
        this.column = column;
        this.letter = letter;
    }
}

[Serializable]
public class WordData
{
    public string word;
    public List<LetterData> letters;

    public WordData(string word, List<LetterData> letters)
    {
        this.word = word;
        this.letters = letters;
    }
}

[Serializable]
public struct LetterCount
{
    public char letter;
    public int count;

    public LetterCount(char letter, int count)
    {
        this.letter = letter;
        this.count = count;
    }
}


[Serializable]
public class LevelSettings
{
    public List<WordData> wordsData;
    public int level;

    public LevelSettings(List<WordData> wordsData, int level)
    {
        this.wordsData = wordsData;
        this.level = level;
    }
}
public class WordLocation
{
    public List<RectTransform> rectTransformsList;

    public WordLocation(List<RectTransform> rectTransformsList)
    {
        this.rectTransformsList = rectTransformsList;
    }
}

#endregion