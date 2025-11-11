using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardData cardData; // 现在引用的是外部定义的 CardData 类
    
    // 这个方法应该在您的卡牌放置逻辑中被调用
    public void ApplyCardEffect()
    {
        // 将卡牌效果添加到全局资源管理器
        PermanentResourceManager.Instance.AddCardEffect(
            cardData.cost,
            cardData.ecoPerSecond,
            cardData.moneyPerInterval,
            cardData.moneyInterval
        );
        
        Debug.Log($"卡牌 '{cardData.cardName}' 效果已应用");
        
        // 卡牌效果已应用，可以销毁卡牌物体
        Destroy(gameObject);
    }
}