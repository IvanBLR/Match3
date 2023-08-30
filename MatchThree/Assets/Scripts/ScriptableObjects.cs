using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects", order = 51)]
public class ScriptableObjects : ScriptableObject
{
    public Sprite Icon => _icon;
    public string Name => _name;

    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private string _name;
}
