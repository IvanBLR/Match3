using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldController : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;

    [SerializeField]
    private ScriptableObjects[] _playingSets;// не надо так. Надо из PlayerSetttings брать название сета

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < _grid.transform.childCount; i++)
            {
                var child = _grid.transform.GetChild(i);
                child.name = i.ToString();
                child.hideFlags = HideFlags.None;
                Debug.Log(_grid.transform.GetChild(i).name, _grid.transform.GetChild(i));
            }
        }
    }
}
