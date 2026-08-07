using System.Threading.Tasks;
using UnityEngine;

public class CommanderStateController : UnitStateController
{

    protected override async void Die()
    {
        isAlive = false;
        
        spriteRenderer.sprite = stunnedSprite;
        spriteRenderer.color = Color.gray;

        await Task.Yield();

        GameplayManager.instance.GameOver();
    }
}
