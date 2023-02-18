using UnityEngine;
namespace Ninsar.TermsOfPrivacy
{
    public class TermsBehaviour : MonoBehaviour
    {
        [SerializeField]
        private GameObject termsPlane;
        
        [SerializeField]
        private CanvasGroup canvasGroup;
        
        private string AcceptedFlag;
        void OnEnable()
        {
            SplashImage();
        }

        void SplashImage()
        {
            canvasGroup.alpha = 1;
            
            if(!PlayerPrefs.HasKey(AcceptedFlag))
                PlayerPrefs.SetInt(AcceptedFlag, 0);
            
            termsPlane.SetActive(PlayerPrefs.GetInt(AcceptedFlag) == 0);
        }
        public void AcceptPrivacy()
        {
            PlayerPrefs.SetInt(AcceptedFlag,1);
            termsPlane.SetActive(false);
        }

        public void ShowPrivacy()
        {
            Application.OpenURL(Project.main.privacy);
        }
    
    }
}
