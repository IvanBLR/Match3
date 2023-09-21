using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ItemScriptableObject",
    menuName = "ScriptableObjects/ItemScriptableObject",
    order = 51)]
public class ItemScriptableObject : ScriptableObject
{
    public Sprite Icon// если поменять на авто-свойство, то всё ломается почему-то
    {
        get => _icon;
        set => _icon = value;
    }
    public int ID// не надо использовать [field: SerializeField] !!!
    {
        get => _id;
        set => _id = value;
    }

    [SerializeField] private Sprite _icon;
    [FormerlySerializedAs("_nameID")] [SerializeField] private int _id;
}