using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject",
    menuName = "ScriptableObjects/ItemScriptableObject",
    order = 51)]
public class ItemScriptableObject : ScriptableObject
{
    public Sprite Icon
    {
        get => _icon;
        set { }
    }

    public int NameID
    {
        get => _nameID;
        set { }
    }

    [SerializeField] private Sprite _icon;
    [SerializeField] private int _nameID;
}