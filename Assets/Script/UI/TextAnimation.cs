using LitMotion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimation : Singleton<TextAnimation>
{
    /// <summary>
    /// LitMotionでテキストアニメーションするよ
    /// </summary>
    /// <param name="text"></param>
    /// <param name="UIText"></param>
    public void LTextAnimation(string text, TMP_Text UIText)
    {
        LMotion.Create(0, text.Length, 0.5f)
            .Bind(value =>
            {
                UIText.text = text.Substring(0, value);
            })
            .AddTo(this);
    }
}
