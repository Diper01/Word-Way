using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject uiElementPrefab;
    private GameObject settingsPanel;
    [SerializeField] private RectTransform[] objectToHide;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private Slider musicSlider;
    private Slider sfxSlider;

    #region Volume

    private void Start()
    {
        SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFXVolumeKey, 0.5f));
    }

    private void InitializeSliders()
    {
        if (settingsPanel == null) return;

        musicSlider = settingsPanel.transform.Find("ScrollbarMusic").GetComponentInChildren<Slider>();
        sfxSlider = settingsPanel.transform.Find("ScrollbarSFX").GetComponentInChildren<Slider>();

        musicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFXVolumeKey, 0.5f);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMusicVolume(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
        PlayerPrefs.Save();
    }

    #endregion

    #region Settings UI

    public void SpawnUIElement()
    {
        if (settingsPanel == null)
        {
            ShrinkObjects(() =>
            {
                uiElementPrefab.transform.localScale = Vector3.zero;
                settingsPanel = Instantiate(uiElementPrefab, transform.parent);
                settingsPanel.transform.localPosition = Vector3.zero;
                settingsPanel.transform.localScale = Vector3.one;
                TweenManager.Instance.GrowToOriginalSize(settingsPanel.GetComponent<RectTransform>());

                InitializeSliders();
            });
        }
        else
        {
            TweenManager.Instance.ShrinkToMinimum(settingsPanel.GetComponent<RectTransform>(), () =>
            {
                Destroy(settingsPanel);
                increaseObjects(null);
            });
        }
    }

    private void ShrinkObjects(System.Action onComplete)
    {
        int completedTweens = 0;
        foreach (var item in objectToHide)
        {
            TweenManager.Instance.ShrinkToMinimum(item, () =>
            {
                completedTweens++;
                if (completedTweens == objectToHide.Length)
                {
                    onComplete?.Invoke();
                }
            });
        }
    }

    private void increaseObjects(System.Action onComplete)
    {
        int completedTweens = 0;
        foreach (var item in objectToHide)
        {
            TweenManager.Instance.GrowToOriginalSize(item, () =>
            {
                completedTweens++;
                if (completedTweens == objectToHide.Length)
                {
                    onComplete?.Invoke();
                }
            });
        }
    }

    #endregion
}