using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCoins : MonoBehaviour
{
    public void Add(int count)
    {
        CurrentUser.main[ItemID.coin] += count;
        if (CurrentUser.main[ItemID.coin] > 9999)
        {
            CurrentUser.main[ItemID.coin] = 9999;
        }
    }
}
