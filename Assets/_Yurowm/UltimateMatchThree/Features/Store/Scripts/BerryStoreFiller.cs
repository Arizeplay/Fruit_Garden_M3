using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BerryStoreFiller : MonoBehaviour
{
    public LayoutGroup container;
    public List<string> groups;

    private void OnEnable()
    {
        BerryStore.main.fillStores.AddListener(FillStore);
    }

    private void OnDisable()
    {
        BerryStore.main.fillStores.RemoveListener(FillStore);
    }

    private void FillStore()
    {
        BerryStore.main.FillTheStore(container, groups);
    }
}