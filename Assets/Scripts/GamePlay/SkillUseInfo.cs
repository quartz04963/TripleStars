using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUseInfo : MonoBehaviour
{
    private bool isCoolingDown;
    private float remainCooldown = 0;
    
    [SerializeField] string skillName;
    [SerializeField] string keyName;
    [SerializeField] float cooldown;

    [SerializeField] Image iconImg;
    [SerializeField] TextMeshProUGUI iconText;
    [SerializeField] TextMeshProUGUI nameText;

    void Update()
    {
        if (!isCoolingDown) return;

        if (remainCooldown > 0)
        {
            remainCooldown -= Time.deltaTime;
            iconImg.fillAmount = 1f - remainCooldown / cooldown;
            iconText.SetText("" + (int)(remainCooldown + 1));
        }
        else
        {
            isCoolingDown = false;
            iconText.SetText(keyName);
        }
    }

    public void Init(string skillName, string keyName, float cooldown)
    {
        this.skillName = skillName;
        this.keyName = keyName;
        this.cooldown = cooldown;

        iconText.SetText(keyName);
        nameText.SetText(skillName);
    }

    public void StartCooldown()
    {
        if (isCoolingDown) return;

        isCoolingDown = true;
        remainCooldown = cooldown;
    }
}
