using UnityEngine;
using System.Collections;

public class TestGUI2 : MonoBehaviour
{
    void OnGUI()
    {
        if (GUILayout.Button("Back"))
        {
            Application.LoadLevel("Scene1");
        }
    }
}
