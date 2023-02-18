using UnityEngine;
using UnityEngine.UI;
using Yurowm.GameCore;
namespace Ninsar.GameEvents.UI
{
    public abstract class GameEventPanel : MonoBehaviour
    {
        public string Page;

        public Button Button;

        public void Init()
        {
            Button.onClick.AddListener(ShowPage);
        }
        
        private void ShowPage()
        {
            if (!Page.IsNullOrEmpty())
            {
                UIAssistant.main.ShowPage(Page);
            }
        }
    }
}