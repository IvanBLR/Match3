using UnityEngine;
using UnityEngine.UI;

public class AdvertisementManager : MonoBehaviour
{
    public void ActivateCurrentButton(Button button)
    {
        button.image.color = Color.white;
        Color currentColor = button.image.color;
        currentColor.a = 1;
    }
}