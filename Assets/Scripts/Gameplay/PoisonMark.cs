using System.Collections.Generic;
using UnityEngine;

public class PoisonMark : MonoBehaviour
{
    [SerializeField] List<Effector> effectors;
    

    void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SetMarks(int num)
    {
        foreach (var effector in effectors)
        {
            effector.gameObject.SetActive(false);
        }

        for (int i = 0; i < num; i++)
        {
            effectors[i].gameObject.SetActive(true);
            effectors[i].PlayEffect(100, "Poison Drop", 999);
        }
    }
}
