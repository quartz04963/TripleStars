using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpInfo : MonoBehaviour
{
    [SerializeField] string unitName;
    [SerializeField] float maxHp;
    [SerializeField] float hp;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image hpBarImg;

    public float Hp => hp;

    public void Init(string unitName, float maxHp)
    {
        this.unitName = unitName;
        this.maxHp = maxHp;
        hp = maxHp;

        nameText.SetText(unitName);
        hpText.SetText((int)hp + " / " + (int)maxHp);
        hpBarImg.fillAmount = 1f;
    }

    public void AddHp(float delta)
    {
        hp = delta < 0 ? Math.Max(hp + delta, 0) : Math.Min(hp + delta, maxHp);
        
        hpText.SetText((int)hp + " / " + (int)maxHp);
        hpBarImg.fillAmount = hp / maxHp;
    }
}
