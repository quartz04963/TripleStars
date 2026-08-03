using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class SkillUseInfo : MonoBehaviour
{
    [SerializeField] float cooldown;

    [SerializeField] Image iconImg;
    [SerializeField] TextMeshProUGUI iconTmp;
    [SerializeField] TextMeshProUGUI nameTmp;

    private KeyControl skillKey;

    private bool isCoolingDown;
    private float remainCooldown = 0;

    public KeyControl SkillKey
    {
        get => skillKey;
        set => skillKey = value;
    }

    void Update()
    {
        if (!isCoolingDown) return;

        if (remainCooldown > 0)
        {
            remainCooldown -= Time.deltaTime;
            iconImg.fillAmount = 1f - remainCooldown / cooldown;
            iconTmp.SetText("" + (int)(remainCooldown + 1));
        }
        else
        {
            isCoolingDown = false;
            iconTmp.SetText(skillKey.displayName);
        }
    }

    public void Init(string skillName, KeyControl keyControl, float cooldown)
    {
        this.skillKey = keyControl;
        this.cooldown = cooldown;

        nameTmp.SetText(skillName);
        iconTmp.SetText(keyControl.displayName);
    }

    public bool StartCooldown()
    {
        if (isCoolingDown) return false;

        isCoolingDown = true;
        remainCooldown = cooldown;
        return true;
    }
}
