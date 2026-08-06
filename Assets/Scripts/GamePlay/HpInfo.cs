using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpInfo : MonoBehaviour
{
    [SerializeField] int maxHP;
    [SerializeField] int currentHP;

    [SerializeField] TextMeshProUGUI nameTmp;
    [SerializeField] TextMeshProUGUI hpTmp;
    [SerializeField] Image hpBarImg;

    public int CurrentHP => currentHP;

    public void Init(string unitName, int maxHP)
    {
        nameTmp.SetText(unitName);

        this.maxHP = maxHP;
        currentHP = maxHP;

        hpTmp.SetText(currentHP + " / " + maxHP);
        hpBarImg.fillAmount = 1f;
    }

    public void AddHp(float delta)
    {
        currentHP = (int)(delta < 0 ? Math.Max(currentHP + delta, 0) : Math.Min(currentHP + delta, maxHP));
        
        hpTmp.SetText(currentHP + " / " + maxHP);
        hpBarImg.fillAmount = currentHP / (float)maxHP;
    }
}
