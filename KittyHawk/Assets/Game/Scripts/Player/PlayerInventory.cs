using System;
using UnityEngine;

/// <summary>
/// Script keeping track of player inventory
/// Author: Calvin Ferst
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    #region Events

    public static event Action<int> OnLivesChanged = delegate { };
    public static event Action<int> OnCatnipChanged = delegate { };
    public static event Action<int> OnBunniesChanged = delegate { };

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
    private int bunnies;

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

    public int Bunnies
    {
        get { return bunnies; }
        set
        {
            if (0 <= value)
            {
                bunnies = value;
                OnBunniesChanged(value);
            }
        }
    }

    #endregion

    #region Methods

    void Start()
    {
        Lives = DataManager.Instance.Lives;
        Catnip = DataManager.Instance.Catnip;
        Bunnies = DataManager.Instance.Bunnies;
    }

    #endregion

}
