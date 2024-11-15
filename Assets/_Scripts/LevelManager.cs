using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

public class LevelManager
{
    private readonly LevelSaver levelSaver = new();
    private readonly LevelSettingsSaver lvlSettingsSaver = new();
    private LevelData _currentLevelData;
    private LevelSettings _levelSettings;

    public int currentLvl;


    public async Task LoadLevel()
    {
        if (_currentLevelData == null)
        {
            await LoadLvlSettings();
            currentLvl = _levelSettings.level;
        }

        _currentLevelData = await levelSaver.LoadLevelAsync(currentLvl);
    }


    public async Task LoadLvlSettings()
    {
        _levelSettings = await lvlSettingsSaver.LoadLevelSettingsAsync();
        if (_levelSettings == null)
        {
            _levelSettings = new LevelSettings(new List<WordData>(), 1);
        }
    }

    public void SaveLvlSettings()
    {
        lvlSettingsSaver.SaveLevelSettings(new LevelSettings(_levelSettings.wordsData, currentLvl));
    }

    public void AddWordToSave(WordData wordData)
    {
        _levelSettings.wordsData.Add(wordData);
    }

    public LevelSettings GetLevelSettings() => _levelSettings;
    public LevelData GetLevelData() => _currentLevelData;
    [CanBeNull] public List<LetterCount> GetLetterCount() => _currentLevelData.wordLetterCount;
}