using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    #region Events

    public static event Action<int> OnLivesChanged = delegate { };
    public static event Action<int> OnCatnipChanged = delegate { };

    #endregion

    #region Private Fields

    [SerializeField]
    private int startingLives = 9;
    [SerializeField]
    private int startingCatnip = 0;
    [SerializeField]
    private int maxLives = 9;
    [SerializeField]
    private int maxCatnip = 100;

    private int lives;
    private int catnip;

    #endregion

    #region Properties

    public int Catnip
    {
        get { return catnip; }
        set
        {
            if (0 <= value && value <= maxCatnip)
            {
                catnip = value;
                OnCatnipChanged(value);
            }
        }
    }

    public int Lives
    {
        get { return lives; }
        set
        {
            if (0 <= value && value <= maxLives)
            {
                lives = value;
                OnLivesChanged(value);
            }
        }
    }

    #endregion

    #region Methods

    void Start()
    {
        Lives = startingLives;
        Catnip = startingCatnip;
    }

    #endregion

}
