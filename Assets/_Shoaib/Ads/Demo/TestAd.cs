using SH.Ads;
using UnityEngine;
using UnityEngine.UI;

public class TestAd : MonoBehaviour
{
    public Text m_text;
    void Start()
    {
      
    }

    public void Intialize()
    {
        m_text.text = "Intializing Ads";
        AdsManager.Initialize(true, true, () => m_text.text = "Intializing Complete Completed");
    }
    public void Baner()
    {
        AdsManager.ShowBanner();
    }
    public void BigBaner()
    {
        AdsManager.ShowBigBanner();
    }
    public void Interstial()
    {
        AdsManager.ShowInterstitial();
    }
    public void Rewarded()
    {
        AdsManager.ShowRewarded((type, am) => { m_text.text = type + "  : " + am; });
    }
    public void RewardedInter()
    {
        AdsManager.ShowRewardedInterstitial((type, am) => { m_text.text = type + "  : " + am; });
    }
    public void AnyRewarded()
    {
        AdsManager.ShowAnyRewarded((type, am) => { m_text.text = type + "  : " + am; });
    }

    public void RateUS()
    {
        AdsManager.RateUs((v) => Debug.LogError("Review is successfull : " + v));
    }
}
