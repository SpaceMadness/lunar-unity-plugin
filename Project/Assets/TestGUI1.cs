using UnityEngine;
using System.Collections;

public class TestGUI1 : MonoBehaviour
{
    void OnGUI()
    {
        if (GUILayout.Button("Load Scene"))
        {
            Application.LoadLevel("Scene2");
        }
    }
}
