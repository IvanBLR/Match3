using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemScriptableObject ItemSettings => _itemScriptableObject;
    
    [SerializeField]
    private ItemScriptableObject _itemScriptableObject;
}
