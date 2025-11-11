using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSwitcher : MonoBehaviour
{
    [Header("图片设置")]
    public Sprite defaultSprite;    // 默认图片
    public Sprite switchedSprite;   // 切换后的图片
    public GameObject pictures;
    [Header("切换设置")]
    public bool isSwitched = false; // 当前状态
    public bool toggleMode = true;  // 是否为切换模式（点击后保持状态）
    
    [Header("边距设置")]
    public float switchedTopMargin = 420.2024f;    // 切换后的上边距
    public float switchedBottomMargin = -567.0776f; // 切换后的下边距
    
    private Image buttonImage;      // 按钮的Image组件
    private RectTransform buttonRectTransform; // 按钮的RectTransform组件
    private Vector4 originalMargins; // 原始边距 (x: left, y: bottom, z: right, w: top)
    
    void Start()
    {
        // 获取按钮上的Image组件
        buttonImage = GetComponent<Image>();
        buttonRectTransform = GetComponent<RectTransform>();
        
        // 保存原始边距
        if (buttonRectTransform != null)
        {
            originalMargins = new Vector4(
                buttonRectTransform.offsetMin.x,  // left
                buttonRectTransform.offsetMin.y,  // bottom
                buttonRectTransform.offsetMax.x,  // right
                buttonRectTransform.offsetMax.y   // top
            );
        }
        
        // 设置初始图片和边距
        if (buttonImage != null)
        {
            buttonImage.sprite = isSwitched ? switchedSprite : defaultSprite;
        }
        
        // 设置初始边距
        ApplyMargins(isSwitched);
    }
    
    // 公共方法，用于切换图片和边距
    public void SwitchImage()
    {
        if (toggleMode)
        {
            // 切换模式：点击后在两种状态间切换
            isSwitched = !isSwitched;
            ApplySwitchedState();
            pictures.SetActive(isSwitched);
        }
        else
        {
            // 非切换模式：点击时显示切换图片和边距
            ApplySwitchedState(true);
        }
    }
    
    // 公共方法，用于恢复默认图片和边距（在非切换模式下使用）
    public void RevertToDefault()
    {
        if (!toggleMode)
        {
            buttonImage.sprite = defaultSprite;
            ApplyMargins(false);
        }
    }
    
    // 公共方法，用于外部直接设置状态
    public void SetImageState(bool switched)
    {
        isSwitched = switched;
        ApplySwitchedState();
    }
    
    // 应用切换状态
    private void ApplySwitchedState(bool forceSwitched = false)
    {
        bool state = forceSwitched ? true : isSwitched;
        
        // 切换图片
        if (buttonImage != null)
        {
            buttonImage.sprite = state ? switchedSprite : defaultSprite;
        }
        
        // 应用边距
        ApplyMargins(state);
    }
    
    // 应用边距
    private void ApplyMargins(bool isSwitchedState)
    {
        if (buttonRectTransform != null)
        {
            if (isSwitchedState)
            {
                // 应用切换后的边距
                buttonRectTransform.offsetMin = new Vector2(
                    originalMargins.x, // 保持原始left
                    switchedBottomMargin
                );
                buttonRectTransform.offsetMax = new Vector2(
                    originalMargins.z, // 保持原始right
                    -switchedTopMargin
                );
            }
            else
            {
                // 恢复原始边距
                buttonRectTransform.offsetMin = new Vector2(
                    originalMargins.x, // left
                    originalMargins.y  // bottom
                );
                buttonRectTransform.offsetMax = new Vector2(
                    originalMargins.z, // right
                    originalMargins.w  // top
                );
            }
        }
    }
}