using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttontest: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
     [SerializeField] private string hoverMessage = "鼠标悬停在按钮上";
    [SerializeField] private GameObject panelToToggle;
    [SerializeField] private bool useFadeEffect = true;
    [SerializeField] private float fadeDuration = 0.2f;
    
    private Image buttonImage;
    private CanvasGroup panelCanvasGroup;
    private float currentAlpha = 0f;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(hoverMessage);
        UpdateCardSize();
        ShowPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       BackCardSize();
        HidePanel();
    }

    void Start()
    {
        buttonImage = GetComponent<Image>();
        if (!buttonImage)
        {
            buttonImage = gameObject.AddComponent<Image>();
        }

        if (panelToToggle)
        {
            panelCanvasGroup = panelToToggle.GetComponent<CanvasGroup>();
            if (!panelCanvasGroup)
            {
                panelCanvasGroup = panelToToggle.AddComponent<CanvasGroup>();
            }
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.blocksRaycasts = false;
            panelCanvasGroup.interactable = false;
        }
    }

    private void ShowPanel()
    {
        
        if (!panelToToggle) return;
    
        panelToToggle.SetActive(true);
        if (useFadeEffect)
        {
            CancelInvoke("FadeOutPanel");
            InvokeRepeating("FadeInPanel", 0f, 0.01f);
        }
        else
        {
            panelCanvasGroup.alpha = 1f;
            panelCanvasGroup.blocksRaycasts = true;
            panelCanvasGroup.interactable = true;
        }
    }

    private void HidePanel()
    {
        if (!panelToToggle) return;
        
        if (useFadeEffect)
        {
            CancelInvoke("FadeInPanel");
            InvokeRepeating("FadeOutPanel", 0f, 0.01f);
        }
        else
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.blocksRaycasts = false;
            panelCanvasGroup.interactable = false;
            panelToToggle.SetActive(false);
        }
    }

    private void FadeInPanel()
    {
        currentAlpha += 0.01f / fadeDuration;
        panelCanvasGroup.alpha = currentAlpha;
        
        if (currentAlpha >= 1f)
        {
            CancelInvoke("FadeInPanel");
            currentAlpha = 1f;
            panelCanvasGroup.blocksRaycasts = true;
            panelCanvasGroup.interactable = true;
        }
    }

    private void FadeOutPanel()
    {
        currentAlpha -= 0.01f / fadeDuration;
        panelCanvasGroup.alpha = currentAlpha;
        
        if (currentAlpha <= 0f)
        {
            CancelInvoke("FadeOutPanel");
            currentAlpha = 0f;
            panelCanvasGroup.blocksRaycasts = false;
            panelCanvasGroup.interactable = false;
            panelToToggle.SetActive(false);
        }
    }
    void UpdateCardSize(){
       /* Image cardImage = GetComponent<Image>();
            if (cardImage != null)
            {
                //cardImage.color = (i == selectedIndex) ? selectedColor : normalColor;
                    this.localScale = Vector3.one * 1.2f; // 放大
                }*/
                 this.gameObject.GetComponent<RectTransform>().localScale = Vector3.one * 0.25f; // 放大
               /* else
                {
                    cards[i].localScale = Vector3.one; // 恢复正常大小
                }*/
            }
            void BackCardSize(){
             this.gameObject.GetComponent<RectTransform>().localScale = Vector3.one*0.2146333f;
            }
    
}