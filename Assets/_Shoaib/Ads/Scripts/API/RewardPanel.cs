using SH.API;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace SH.Ads.API.UI
{
    public class RewardPanel : MonoBehaviour
    {
        [SerializeField] Text m_Info;
        [SerializeField] Button m_Yes, m_No;

        Action<bool> m_OnFormComplete;


        void YesClick()
        {
            m_OnFormComplete?.Invoke(true);
            gameObject.SetActive(false);
        }
        void NoClick()
        {
            m_OnFormComplete?.Invoke(false); 
            gameObject.SetActive(false);
        }

        public void Show(Action<bool> OnComplete)
        {
            gameObject.SetActive(true);
            m_OnFormComplete = OnComplete;
            if (m_Yes == null || m_No == null)
            {
                OnComplete?.Invoke(false);
                Debug.LogError("UI Status: Buttons not present. Please assign button reference.");
            }
        }

        public static RewardPanel Load(Transform parent)
        {
            if (parent == null)
                return null;

            if (parent.gameObject.GetInChildren<RewardPanel>())
                return parent.gameObject.GetInChildren<RewardPanel>();

            if (AdSettings.UIRewardAdPanel)
            {
                var Panel = Instantiate(AdSettings.UIRewardAdPanel, parent);
                Panel.gameObject.SetActive(false);
                Panel.m_Yes?.onClick.AddListener(Panel.YesClick);
                Panel.m_No?.onClick.AddListener(Panel.NoClick);
                return Panel;
            }
               

            //Create Parent
            var placeHolder = new GameObject("Rewarded Panel", typeof(RectTransform),typeof(RewardPanel));
            placeHolder.transform.SetParent(parent);

            RewardPanel RewardedPanel = placeHolder.GetComponent<RewardPanel>();
            var rect = placeHolder.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            placeHolder.gameObject.AddComponent<Image>().color =new Color(0,0,0,0.5f);
            placeHolder.gameObject.SetActive(false);

            //Create Child
            placeHolder = new GameObject("Panel", typeof(RectTransform));
            placeHolder.transform.SetParent(RewardedPanel.transform);
            rect = placeHolder.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.2f,0.2f);
            rect.anchorMax = new Vector2(0.8f,0.8f);
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            placeHolder.gameObject.AddComponent<Image>().color = new Color(1, 1, 1, 0.4f);

            // Create Text
            var tem = new GameObject("Title", typeof(RectTransform));
            tem.transform.SetParent(placeHolder.transform);
            rect = tem.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.1f, 0.7f);
            rect.anchorMax = new Vector2(0.9f, 0.9f);
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            RewardedPanel.m_Info= tem.gameObject.AddComponent<Text>();
            RewardedPanel.m_Info.text = $"Are you sure you want to watch a Rewarded Video Ad for {AdSettings.RewardAmount}";
            RewardedPanel.m_Info.color = Color.black;

            RewardedPanel.m_Info.font = Font.CreateDynamicFontFromOSFont(Font.GetOSInstalledFontNames()[0], 50);
            RewardedPanel.m_Info.fontStyle = FontStyle.Bold;
            RewardedPanel.m_Info.fontSize = 80;
            RewardedPanel.m_Info.resizeTextForBestFit = true;
            RewardedPanel.m_Info.alignment = TextAnchor.MiddleCenter;

            tem = new GameObject("Buttons", typeof(RectTransform));
            tem.transform.SetParent(placeHolder.transform);
            rect = tem.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.1f, 0.05f);
            rect.anchorMax = new Vector2(0.9f, 0.15f);
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            var Layout = tem.AddComponent<HorizontalLayoutGroup>();
            Layout.childControlHeight = true;
            Layout.childControlWidth = true;
            Layout.childForceExpandHeight = true;
            Layout.childForceExpandWidth = true;

            Layout.spacing = 50;
            Layout.padding = new RectOffset(20, 10, 0, 0);


            tem = new GameObject("No Button", typeof(RectTransform),typeof(Image));
            tem.transform.SetParent(Layout.transform);

            RewardedPanel.m_No = tem.gameObject.AddComponent<Button>();


            tem = new GameObject("Text", typeof(RectTransform));
            tem.transform.SetParent(RewardedPanel.m_No.transform);
            var text= tem.gameObject.AddComponent<Text>();
            text.text = "No";
            text.color = Color.black;
            text.font = Font.CreateDynamicFontFromOSFont(Font.GetOSInstalledFontNames()[0], 50);
            text.fontStyle = FontStyle.Bold;
            text.fontSize = 80;
            text.resizeTextForBestFit = true;
            text.alignment = TextAnchor.MiddleCenter;


            rect = tem.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;


            tem = new GameObject("Yes Button", typeof(RectTransform),typeof(Image));
            tem.transform.SetParent(Layout.transform);
           

            RewardedPanel.m_Yes = tem.gameObject.AddComponent<Button>();


            tem = new GameObject("Text", typeof(RectTransform));
            tem.transform.SetParent(RewardedPanel.m_Yes.transform);
            text = tem.gameObject.AddComponent<Text>();
            text.text = "Yes";
            text.color = Color.black;
            text.font = Font.CreateDynamicFontFromOSFont(Font.GetOSInstalledFontNames()[0], 50);
            text.fontStyle = FontStyle.Bold;
            text.fontSize = 80;
            text.resizeTextForBestFit = true;
            text.alignment = TextAnchor.MiddleCenter;

            rect = tem.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;



            RewardedPanel.m_Yes?.onClick.AddListener(RewardedPanel.YesClick);
            RewardedPanel.m_No?.onClick.AddListener(RewardedPanel.NoClick);


            return RewardedPanel;
        }
    }
}