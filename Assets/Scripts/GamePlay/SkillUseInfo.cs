using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class SkillUseInfo : MonoBehaviour
{
    [SerializeField] string skillName;
    [SerializeField] float cooldown;
    private KeyControl keyControl;

    [SerializeField] Image iconImg;
    [SerializeField] TextMeshProUGUI iconText;
    [SerializeField] TextMeshProUGUI nameText;

    private bool isCoolingDown;
    private float remainCooldown = 0;

    public KeyControl KeyControl
    {
        get => keyControl;
        set => keyControl = value;
    }

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
            iconText.SetText(keyControl.displayName);
        }
    }

    public void Init(string skillName, float cooldown, KeyControl keyControl)
    {
        this.skillName = skillName;
        this.cooldown = cooldown;
        this.keyControl = keyControl;

        iconText.SetText(keyControl.displayName);
        nameText.SetText(skillName);
    }

    public bool StartCooldown()
    {
        if (isCoolingDown) return false;

        isCoolingDown = true;
        remainCooldown = cooldown;
        return true;
    }
}
