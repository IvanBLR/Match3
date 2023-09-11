using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemScriptableObject ItemSettings
    {
        get
        {
            return _itemScriptableObject;
        }
        set
        {
            _itemScriptableObject = value;
        }
    }
    
    [SerializeField]
    private ItemScriptableObject _itemScriptableObject;
}
