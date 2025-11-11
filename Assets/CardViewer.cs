using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CardViewer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private Transform startParent;
    public string CardType;
    public int needCost;
    public int zengzhang;
    public GameManager GM;
    public Image fillImage; // 拖入你的Image组件
    public float fillDuration = 2.0f; // 填充完成所需的时间（秒）

    // 卡牌数据引用
    public CardData cardData;

    // 新增：透明度与交互控制变量
    private Image cardImage; // 卡牌的主Image组件（控制透明度）
    private BoxCollider2D cardCollider; // 控制碰撞/点击
    [SerializeField] private float transparentTime = 5f; // 半透明持续时间（默认5秒）
    [SerializeField] private float transparentAlpha = 0.3f; // 半透明时的Alpha值（0=完全透明，1=不透明）
    private Color originalColor; // 保存卡牌原始颜色（用于恢复）
    private bool isTransparentState; // 是否处于半透明禁用状态


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // 关键：获取卡牌的Image组件（必须确保预制体上有Image）
        cardImage = GetComponent<Image>();
        if (cardImage == null)
        {
            Debug.LogError($"卡牌{gameObject.name}缺少Image组件！请在预制体上添加");
            return;
        }
        originalColor = cardImage.color; // 保存原始颜色（含Alpha值）
        
        // 获取BoxCollider2D（控制点击交互）
        cardCollider = GetComponent<BoxCollider2D>();
        if (cardCollider == null)
        {
            Debug.LogError($"卡牌{gameObject.name}缺少BoxCollider2D！请在预制体上添加");
        }
    }


    // 开始拖动：半透明状态下禁止
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isTransparentState) return;
        
        startPosition = rectTransform.anchoredPosition;
        startParent = rectTransform.parent;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }


    // 拖动中：半透明状态下禁止
    public void OnDrag(PointerEventData eventData)
    {
        if (isTransparentState) return;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }


    // 结束拖动：半透明状态下禁止
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isTransparentState) return;
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("DropZone"))
            {
                hit.collider.gameObject.GetComponent<GrowthLogic>().IsGrowth = CardType;
                
                // 应用卡牌效果
                CardController cardController = GetComponent<CardController>();
                if (cardController != null)
                {
                    cardController.ApplyCardEffect();
                }
                
                // 生成新卡牌（生成后自动进入半透明状态）
                RespawnCard();
                
                // 禁用当前卡牌
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            rectTransform.anchoredPosition = startPosition;
            rectTransform.SetParent(startParent);
        }
    }


    private void RespawnCard()
    {
        // 基础校验：避免空引用错误
        if (cardData == null || cardData.cardPrefab == null)
        {
            Debug.LogWarning("卡牌数据或预制体为空，无法生成新卡牌");
            return;
        }
        if (startParent == null)
        {
            Debug.LogWarning("初始父对象为空，无法生成新卡牌");
            return;
        }

        // 1. 实例化新卡牌到原始父对象
        GameObject newCard = Instantiate(cardData.cardPrefab, startParent);
        RectTransform newCardRT = newCard.GetComponent<RectTransform>();
        if (newCardRT == null)
        {
            Debug.LogError("新卡牌缺少RectTransform组件，无法设置位置");
            Destroy(newCard);
            return;
        }
        newCardRT.anchoredPosition = startPosition; // 恢复到原始位置

        // 2. 启用新卡牌并激活所有组件
        newCard.SetActive(true);
        EnableAllComponents(newCard);

        // 3. 复制新卡牌的核心属性与引用
        CardViewer newCardViewer = newCard.GetComponent<CardViewer>();
        if (newCardViewer == null)
        {
            Debug.LogError("新卡牌缺少CardViewer组件，无法控制透明度和交互");
            Destroy(newCard);
            return;
        }
        // 复制卡牌基础属性
        newCardViewer.CardType = CardType;
        newCardViewer.needCost = needCost;
        newCardViewer.zengzhang = zengzhang;
        newCardViewer.cardData = cardData;
        newCardViewer.GM = GM;
        // 复制透明度配置（确保新卡牌和原卡牌半透明效果一致）
        newCardViewer.transparentTime = this.transparentTime;
        newCardViewer.transparentAlpha = this.transparentAlpha;
        // 传递Image和Collider引用（关键：控制透明度和交互）
        newCardViewer.cardImage = newCard.GetComponent<Image>();
        newCardViewer.cardCollider = newCard.GetComponent<BoxCollider2D>();
        // 保存新卡牌的原始颜色
        if (newCardViewer.cardImage != null)
        {
            newCardViewer.originalColor = newCardViewer.cardImage.color;
        }

        // 4. 初始化CanvasGroup（确保基础交互正常）
        CanvasGroup newCanvasGroup = newCard.GetComponent<CanvasGroup>();
        if (newCanvasGroup == null)
        {
            Debug.LogWarning("新卡牌缺少CanvasGroup组件，自动添加");
            newCanvasGroup = newCard.AddComponent<CanvasGroup>();
        }
        newCardViewer.canvasGroup = newCanvasGroup;
        newCanvasGroup.alpha = 1f;
        newCanvasGroup.blocksRaycasts = true;

        // 5. 初始化CardController
        CardController newCardController = newCard.GetComponent<CardController>();
        if (newCardController != null)
        {
            newCardController.cardData = cardData;
        }

        // 6. 关键：新卡牌生成后，立即启动半透明禁用协程
        newCardViewer.StartCoroutine(newCardViewer.TransparentAndDisableCoroutine());
    }


    // 新增：协程 - 控制半透明+禁用，5秒后恢复
    private IEnumerator TransparentAndDisableCoroutine()
    {
        // 标记为半透明状态（屏蔽拖拽）
        isTransparentState = true;

        // 步骤1：设置半透明（修改Image的Alpha值）
        if (cardImage != null)
        {
           /* Color transparentColor = originalColor; // 基于原始颜色修改Alpha
            transparentColor.a = transparentAlpha; // 设为半透明Alpha值
            cardImage.color = transparentColor;*/
              float timer = 0f;
        // 初始填充量为0
        fillImage.fillAmount = 0f;
        cardCollider.enabled = false;
        while (timer < fillDuration)
        {
            // 更新计时器
            timer += Time.deltaTime;
            // 计算当前的填充量，使用Lerp实现0到1的平滑过渡
            fillImage.fillAmount = Mathf.Lerp(0f, 1f, timer / fillDuration);
            // 等待下一帧
            yield return null;
        }

        // 确保最终填充量精确设置为1
        fillImage.fillAmount = 1f;
         if (cardCollider != null)
        {
            cardCollider.enabled = true;
        }
isTransparentState = false;
        }

        // 步骤2：禁用BoxCollider2D（无法点击/碰撞）
       /* if (cardCollider != null)
        {
            cardCollider.enabled = false;
        }*/

        // 步骤3：等待指定时长（默认5秒）
       // yield return new WaitForSeconds(transparentTime);

        // 步骤4：恢复正常状态
        // 恢复原始颜色（含Alpha=1）
       /* if (cardImage != null)
        {
            cardImage.color = originalColor;
        }*/
        // 启用Collider（允许点击/拖拽）
       

        // 取消半透明状态（允许交互）
       // isTransparentState = false;
    }


    // 原有方法：递归启用所有组件（确保新卡牌组件正常）
    private void EnableAllComponents(GameObject target)
    {
        if (target == null) return;

        // 启用对象本身
        target.SetActive(true);

        // 启用对象上所有组件（跳过Transform）
        Component[] components = target.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp != null && !(comp is Transform))
            {
                var enabledProperty = comp.GetType().GetProperty("enabled");
                if (enabledProperty != null && enabledProperty.CanWrite)
                {
                    enabledProperty.SetValue(comp, true, null);
                }
            }
        }

        // 递归处理子对象
        for (int i = 0; i < target.transform.childCount; i++)
        {
            EnableAllComponents(target.transform.GetChild(i).gameObject);
        }
    }
}