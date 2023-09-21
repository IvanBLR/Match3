using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField] 
    public ItemScriptableObject ItemSettings { get; set; }
}