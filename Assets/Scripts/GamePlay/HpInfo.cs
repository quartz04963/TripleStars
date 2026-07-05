using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpInfo : MonoBehaviour
{
    [SerializeField] string unitName;
    [SerializeField] int maxHp;
    [SerializeField] int currentHp;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image hpBarImg;

    public void Init(string unitName, int maxHp)
    {
        this.unitName = unitName;
        this.maxHp = maxHp;
        currentHp = maxHp;

        nameText.SetText(unitName);
        hpText.SetText(currentHp + " / " + maxHp);
        hpBarImg.fillAmount = 1f;
    }

    public void SetHp(int currentHp)
    {
        this.currentHp = currentHp;
        
        hpText.SetText(currentHp + " / " + maxHp);
        hpBarImg.fillAmount = currentHp / maxHp;
    }
}
