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
    private int startingLives = 3;
    [SerializeField]
    private int startingCatnip = 0;
    [SerializeField]
    private int maxLives = 3;
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
            int newCatnip = catnip + value;

            if (!(newCatnip < 0) && !(newCatnip > maxCatnip))
            {
                catnip = newCatnip;
                OnCatnipChanged(Catnip);
            }
        }
    }

    public int Lives
    {
        get { return lives; }
        set
        {
            int newLives = lives + value;

            if (!(newLives > maxLives))
            {
                lives = newLives;
                OnLivesChanged(Lives);
            }

        }
    }

    #endregion

    #region Methods

    void Start()
    {
        lives = startingLives;
        catnip = startingCatnip;
    }

    #endregion

}
