using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpInfo : MonoBehaviour
{
    [SerializeField] string unitName;
    [SerializeField] float maxHp;
    [SerializeField] float currentHp;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image hpBarImg;

    public void Init(string unitName, float maxHp)
    {
        this.unitName = unitName;
        this.maxHp = maxHp;
        currentHp = maxHp;

        nameText.SetText(unitName);
        hpText.SetText((int)currentHp + " / " + (int)maxHp);
        hpBarImg.fillAmount = 1f;
    }

    public void AddHp(float delta)
    {
        currentHp = delta < 0 ? Math.Max(currentHp + delta, 0) : Math.Min(currentHp + delta, maxHp);
        
        hpText.SetText((int)currentHp + " / " + (int)maxHp);
        hpBarImg.fillAmount = delta / maxHp;
    }
}
