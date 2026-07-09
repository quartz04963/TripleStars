using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] Commander commander;
    [SerializeField] Follower attacker;
    [SerializeField] Follower supporter;
    [SerializeField] GameObject[] unitPrefabs;

    public static GamePlayManager instance;

    public Commander Commander => commander;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
