using System.Threading.Tasks;
using UnityEngine;

public class CommanderStateController : UnitStateController
{

    protected override async void Die()
    {
        isAlive = false;
        
        animator.Play(StunnedHash);
        spriteRenderer.color = Color.gray;

        await Task.Yield();

        GameplayManager.instance.GameOver();
    }
}
