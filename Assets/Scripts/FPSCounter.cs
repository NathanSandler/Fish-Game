using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    GUIStyle style = new GUIStyle();
    Rect rect;
    float deltaTime = 0.0f;

    void Start()
    {
        int w = Screen.width, h = Screen.height;
        rect = new Rect(10, 10, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(1.0f, 0.5f, 0.0f, 1.0f); // Orange text color
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}