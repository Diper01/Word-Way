using System.Collections;
using System.Collections.Generic;
using DigitalRuby.Tween;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Action = System.Action;


//Animation Class
public class TweenManager : Singleton<TweenManager>
{
    public void CorrectWordsAnim(List<RectTransform> currentWordTxt, WordLocation wordLocation, List<Image> openedItems,
        Action onComplete = null)
    {
        int animationCount = 0;
        for (int i = 0; i < wordLocation.rectTransformsList.Count; i++)
        {
            MoveImageToTarget(currentWordTxt[i], wordLocation.rectTransformsList[i], openedItems[i],
                () =>
                {
                    animationCount++;
                    if (animationCount == wordLocation.rectTransformsList.Count)
                    {
                        onComplete?.Invoke();
                    }
                });
        }
    }

    private void MoveImageToTarget(RectTransform movingObject, RectTransform targetObject, Image image,
        Action onComplete = null)
    {
        Vector3 startPosition = movingObject.position;
        Vector3 targetPosition = targetObject.position;

        TweenFactory.Tween(
            key: movingObject,
            start: startPosition,
            end: targetPosition,
            duration: 0.5f,
            scaleFunc: TweenScaleFunctions.QuadraticEaseInOut,
            progress: (tween) => { movingObject.position = tween.CurrentValue; },
            completion: (tween) =>
            {
                targetObject.transform.GetChild(0).gameObject.SetActive(true);
                Destroy(movingObject.gameObject);
                ChangeCellColor(image);
                onComplete?.Invoke();
            }
        );
    }

    public void ChangeCellColor(Image cellImage)
    {
        TweenFactory.Tween(
            key: cellImage,
            start: cellImage.color,
            end: new Color(1.0f, 0.7f, 0.6f),
            duration: 0.2f,
            scaleFunc: TweenScaleFunctions.QuadraticEaseInOut,
            progress: (tween) => { cellImage.color = tween.CurrentValue; },
            completion: (tween) => { Debug.Log("Color change completed."); }
        );
    }

    public void ShrinkToMinimum(RectTransform targetObject, Action onComplete = null)
    {
        Vector3 initialScale = targetObject.localScale;
        Vector3 targetScale = Vector3.zero;

        TweenFactory.Tween(
            key: targetObject,
            start: initialScale,
            end: targetScale,
            duration: 0.5f,
            scaleFunc: TweenScaleFunctions.QuadraticEaseInOut,
            progress: (tween) => { targetObject.localScale = tween.CurrentValue; },
            completion: (tween) => { onComplete?.Invoke(); }
        );
    }

    public void GrowToOriginalSize(RectTransform targetObject, Action onComplete = null)
    {
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        TweenFactory.Tween(
            key: targetObject,
            start: initialScale,
            end: targetScale,
            duration: 0.5f,
            scaleFunc: TweenScaleFunctions.QuadraticEaseInOut,
            progress: (tween) => { targetObject.localScale = tween.CurrentValue; },
            completion: (tween) => { onComplete?.Invoke(); }
        );
    }

    public void AnimateWinText(TextMeshProUGUI winText, Action onComplete = null)
    {
        winText.gameObject.SetActive(true);
        float duration = 1f;
        GrowToOriginalSize(winText.GetComponent<RectTransform>(), () =>
        {
            StartCoroutine(WaitFor(() =>
            {
                ShrinkToMinimum(winText.GetComponent<RectTransform>(),
                    () =>
                    {
                        onComplete?.Invoke();
                        winText.gameObject.SetActive(false);
                    });
            }, 0.65f));
        });
    }

    IEnumerator WaitFor(Action newAction, float time)
    {
        yield return new WaitForSeconds(time);
        newAction?.Invoke();
    }


    private Dictionary<RectTransform, List<ITween>> activeTweens = new Dictionary<RectTransform, List<ITween>>();

    public void AnimateCircleObjectsLoop(RectTransform[] circleObjects, Action onInterrupt = null)
    {
        foreach (var circleObject in circleObjects)
        {
            AnimateCircleLoop(circleObject, onInterrupt);
        }
    }

    private void AnimateCircleLoop(RectTransform circleObject, Action onInterrupt = null)
    {
        Vector3 initialScale = Vector3.one;
        Vector3 maxScale = Vector3.one * 1.1f;
        float duration = 1f;

        TweenFactory.Tween(
            key: circleObject,
            start: initialScale,
            end: maxScale,
            duration: duration,
            scaleFunc: TweenScaleFunctions.QuadraticEaseInOut,
            progress: (tween) => { circleObject.localScale = tween.CurrentValue; },
            completion: (tween) =>
            {
                TweenFactory.Tween(
                    key: circleObject,
                    start: maxScale,
                    end: initialScale,
                    duration: duration,
                    scaleFunc: TweenScaleFunctions.QuadraticEaseInOut,
                    progress: (reverseTween) => { circleObject.localScale = reverseTween.CurrentValue; },
                    completion: (reverseTween) => { AnimateCircleLoop(circleObject, onInterrupt); }
                );
            }
        );

        onInterrupt?.Invoke();
    }
}