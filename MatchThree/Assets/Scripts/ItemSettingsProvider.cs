using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSettingsProvider",
                 menuName = "ScriptableObjects/ItemSettingsProvider",
                 order = 52)]
public class ItemSettingsProvider : ScriptableObject
{
    public IReadOnlyList<ItemScriptableObject> ItemsList => _itemsList;

    [SerializeField] 
    private List<ItemScriptableObject> _itemsList;
}