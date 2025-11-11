using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    
    [Header("卡牌槽位列表")]
    public List<GameObject> cardSlots = new List<GameObject>();
    
    [Header("卡牌预制体")]
    public List<GameObject> cardPrefabs = new List<GameObject>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 在指定位置生成卡牌
    public void SpawnCard(CardData cardData, Transform parent, Vector2 position)
    {
        if (cardData.cardPrefab != null)
        {
            GameObject newCard = Instantiate(cardData.cardPrefab, parent);
            RectTransform newCardRT = newCard.GetComponent<RectTransform>();
            newCardRT.anchoredPosition = position;
            
            // 设置卡牌数据
            CardViewer newCardViewer = newCard.GetComponent<CardViewer>();
            if (newCardViewer != null)
            {
                newCardViewer.cardData = cardData;
                // 可以根据需要设置其他属性
            }
            
            CardController newCardController = newCard.GetComponent<CardController>();
            if (newCardController != null)
            {
                newCardController.cardData = cardData;
            }
        }
    }
    
    // 初始化卡牌槽位
    public void InitializeCardSlots()
    {
        // 为每个卡牌槽位生成初始卡牌
        foreach (GameObject slot in cardSlots)
        {
            if (slot != null && cardPrefabs.Count > 0)
            {
                // 随机选择一个卡牌预制体
                GameObject randomCardPrefab = cardPrefabs[Random.Range(0, cardPrefabs.Count)];
                
                // 生成卡牌
                GameObject newCard = Instantiate(randomCardPrefab, slot.transform);
                RectTransform cardRT = newCard.GetComponent<RectTransform>();
                cardRT.anchoredPosition = Vector2.zero;
                
                // 设置卡牌数据
                CardData cardData = newCard.GetComponent<CardController>()?.cardData;
                CardViewer cardViewer = newCard.GetComponent<CardViewer>();
                if (cardViewer != null && cardData != null)
                {
                    cardViewer.cardData = cardData;
                    cardViewer.CardType = cardData.cardName;
                    cardViewer.needCost = Mathf.FloorToInt(cardData.cost);
                }
            }
        }
    }
}