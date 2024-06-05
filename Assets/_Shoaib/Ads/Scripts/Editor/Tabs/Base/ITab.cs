using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Base
{
    public abstract class ITab
    {
        public abstract GUIContent Title { get; }
        protected abstract string IconBase64 { get; }
        protected Texture2D Texture()
        {
            if (!AllTextures.ContainsKey(this.GetType()))
                Load();
            return AllTextures[this.GetType()];
        }
        public abstract void OnGUI();
        public abstract void OnEnable(AdSettings settings);

        static Dictionary<Type,Texture2D>  AllTextures = new Dictionary<Type,Texture2D>();

        static void Load()
        {
            foreach (var tem in Extensions.GetInstanceOfAllSubClasses<ITab>())
            {
                byte[] imageBytes = Convert.FromBase64String(tem.IconBase64);
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    Debug.LogError($"Failed to load image bytes for type {tem.Title}");
                    continue;
                }

                var texture = new Texture2D(2, 2);
                if (!texture.LoadImage(imageBytes))
                {
                    Debug.LogError($"Failed to load texture for type {tem.Title}");
                    continue;
                }

                AllTextures.Add(tem.GetType(), texture);
            }
        }
    }
}