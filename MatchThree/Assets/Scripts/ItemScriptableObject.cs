using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject",
    menuName = "ScriptableObjects/ItemScriptableObject",
    order = 51)]
public class ItemScriptableObject : ScriptableObject
{
    public Sprite Icon
    {
        get => _icon;
        set { _icon = value; }
    }

    public int NameID
    {
        get => _nameID;
        set { _nameID = value; }
    }

    [SerializeField] private Sprite _icon;
    [SerializeField] private int _nameID;
}