using UnityEngine;
using UnityEditor;
using System.IO;

public class ImageToBase64Editor : EditorWindow
{
    private Texture2D texture;
    Texture2D resizedTexture;
    private string base64String;
    private Vector2 scrollPos;
    private int targetWidth = 256;
    private int targetHeight = 256;

    [MenuItem("SH/Image to Base64 Converter")]
    private static void ShowWindow()
    {
        var window = GetWindow<ImageToBase64Editor>();
        window.titleContent = new GUIContent("Image to Base64 Converter");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Convert Image to Base64", EditorStyles.boldLabel);

        if (GUILayout.Button("Load Image"))
        {
            string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");
            if (!string.IsNullOrEmpty(path))
            {
                LoadImage(path);
            }
        }

        if (texture != null)
        {
            GUILayout.Label("Selected Image:", EditorStyles.boldLabel);
            GUILayout.Label(texture, GUILayout.Width(100), GUILayout.Height(100));

            GUILayout.Label("Resize Options:", EditorStyles.boldLabel);
            targetWidth = EditorGUILayout.IntField("Width", targetWidth);
            targetHeight = EditorGUILayout.IntField("Height", targetHeight);

            if (GUILayout.Button("Convert to Base64"))
            {
                ConvertImageToBase64();
            }
        }
        if (resizedTexture != null)
        {
            GUILayout.Label("Converted Image:", EditorStyles.boldLabel);
            GUILayout.Label(resizedTexture);
        }
        if (!string.IsNullOrEmpty(base64String))
        {
            GUILayout.Label("Base64 String:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.TextArea(base64String, GUILayout.Height(200));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Copy to Clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = base64String;
                Debug.Log("Base64 string copied to clipboard!");
            }
        }

       
    }

    private void LoadImage(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // This will auto-resize the texture dimensions.
    }

    private void ConvertImageToBase64()
    {
        resizedTexture = ResizeTexture(texture, targetWidth, targetHeight);
        byte[] imageBytes = resizedTexture.EncodeToPNG(); // Convert the texture to a PNG byte array
        base64String = System.Convert.ToBase64String(imageBytes);
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        Graphics.Blit(originalTexture, rt);
        RenderTexture.active = rt;

        Texture2D newTexture = new Texture2D(width, height);
        newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        newTexture.Apply();

        RenderTexture.active = null;
        rt.Release();

        return newTexture;
    }
}
