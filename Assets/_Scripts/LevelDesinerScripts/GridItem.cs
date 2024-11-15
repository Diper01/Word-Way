#if UNITY_EDITOR
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class GridItem : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI letterText;

    private void OnEnable()
    {
        if (letterText != null)
            letterText.raycastTarget = false;
    }

    public void SetLetter(string letter)
    {
        if (letterText != null && !string.IsNullOrEmpty(letter))
        {
            letterText.text = letter.ToUpper();
        }
    }

    private void OnValidate()
    {
        if (letterText == null)
        {
            letterText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
#endif