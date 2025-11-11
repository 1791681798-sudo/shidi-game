using UnityEngine;

[System.Serializable]
public class CardData
{
    public string cardName;
    public float cost;
    public float ecoPerSecond;
    public float moneyPerInterval;
    public float moneyInterval = 5f;
    public float cooldownTime = 10f;
    public GameObject cardPrefab; // 卡牌预制体
}