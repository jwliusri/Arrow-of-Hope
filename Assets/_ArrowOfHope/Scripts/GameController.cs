using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private WallController wall;

    public static GameController Instance;

    public WallController Wall => wall;


    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
