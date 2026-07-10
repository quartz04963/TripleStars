using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] Boss boss;
    [SerializeField] Commander commander;
    [SerializeField] Follower attacker;
    [SerializeField] Follower supporter;
    [SerializeField] GameObject[] unitPrefabs;

    public static GameplayManager instance;

    public Boss Boss => boss;
    public Commander Commander => commander;
    public Follower Attacker => attacker;

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
