using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

public class Yandex : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void RateGame();

    [UsedImplicitly]
    public void OnClickedRateGame()
    {
        RateGame();
    }
}