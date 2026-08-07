using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] int page = 0;

    [SerializeField] List<Sprite> slides;
    [SerializeField] Image slideImg;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Exit();
        }

        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            Next();
        }

        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            Prev();
        }
    }

    void OnEnable() => Reset();
    
    void Reset()
    {
        page = 0;
        slideImg.sprite = slides[page];
    }

    void Exit()
    {
        gameObject.SetActive(false);
    }

    public void Next()
    {
        if (page == slides.Count - 1)
        {
            Reset();
            return;
        }

        slideImg.sprite = slides[++page];
    }

    public void Prev()
    {
        if (page == 0) return;

        slideImg.sprite = slides[--page];
    }

    public void OnClicked()
    {
        if (page == 0)
        {
            Next();
        }

        if (page == slides.Count - 1)
        {
            Exit();
        }
    }
}
