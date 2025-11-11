using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PermanentResourceManager : MonoBehaviour
{
    public static PermanentResourceManager Instance { get; private set; }
    
    [Header("初始资源")]
    public float initialMoney = 100f;
    public float initialEcoValue = 0f;
    public float maxEcoValue = 700f;
    
    [Header("UI组件")]
    public Text moneyText;
    public Text ecoValueText;
    public Slider ecoValueSlider;
    public GameObject[] images;
    private float currentMoney;
    private float currentEcoValue;
    
    // 存储所有已放置卡牌的累积效果
    private float totalEcoPerSecond = 0f;
    private float totalMoneyPerInterval = 0f;
    private float moneyInterval = 5f;
    
    private Coroutine ecoCoroutine;
    private Coroutine moneyCoroutine;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        currentMoney = initialMoney;
        currentEcoValue = initialEcoValue;
        
        // 初始化Slider
        if (ecoValueSlider != null)
        {
            ecoValueSlider.minValue = 0;
            ecoValueSlider.maxValue = maxEcoValue;
            ecoValueSlider.value = currentEcoValue;
        }
       
        
        UpdateUI();
        
        // 启动资源增长协程
        StartResourceCoroutines();
    }
    void Update(){

         if(currentEcoValue>=100){
            images[0].SetActive(true);
        }
        if(currentEcoValue>=200){
            images[1].SetActive(true);
        }
        if(currentEcoValue>=300){
            images[2].SetActive(true);
        }
        if(currentEcoValue>=400){
            images[3].SetActive(true);
        }
        if(currentEcoValue>=500){
            images[4].SetActive(true);
        }
         if(currentEcoValue>=600){
            images[5].SetActive(true);
        }
         if(currentEcoValue>=700){
            images[6].SetActive(true);
        }
    }
    void StartResourceCoroutines()
    {
        if (ecoCoroutine != null) StopCoroutine(ecoCoroutine);
        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
        
        ecoCoroutine = StartCoroutine(EcoIncreaseCoroutine());
        moneyCoroutine = StartCoroutine(MoneyIncreaseCoroutine());
    }
    
    IEnumerator EcoIncreaseCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            AddEcoValue(totalEcoPerSecond);
        }
    }
    
    IEnumerator MoneyIncreaseCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moneyInterval);
            AddMoney(totalMoneyPerInterval);
        }
    }
    
    public bool CanAfford(float amount)
    {
        return currentMoney >= amount;
    }
    
    public bool SpendMoney(float amount)
    {
        if (CanAfford(amount))
        {
            currentMoney -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateUI();
    }
    
    public void AddEcoValue(float amount)
    {
        currentEcoValue = Mathf.Min(currentEcoValue + amount, maxEcoValue);
        UpdateUI();
    }
    
    // 添加卡牌效果（永久性）
    public void AddCardEffect(float cost, float ecoPerSecond, float moneyPerInterval, float moneyInterval)
    {
        // 消耗金钱
        if (!SpendMoney(cost))
        {
            Debug.LogWarning("金钱不足，无法应用卡牌效果!");
            return;
        }
        
        // 累积效果
        totalEcoPerSecond += ecoPerSecond;
        totalMoneyPerInterval += moneyPerInterval;
        
        // 重新启动协程以确保使用最新的间隔时间
        StartResourceCoroutines();
        
        Debug.Log($"卡牌效果已添加: 每秒生态值+{ecoPerSecond}, 每{moneyInterval}秒金钱+{moneyPerInterval}");
    }
    
    void UpdateUI()
    {
        // 更新金钱文本
        if (moneyText != null)
            moneyText.text = $"X {Mathf.FloorToInt(currentMoney)}";
            
        // 更新生态值文本
        if (ecoValueText != null)
            ecoValueText.text = $"X {Mathf.FloorToInt(currentEcoValue)}";
            
        // 更新Slider
        if (ecoValueSlider != null)
            ecoValueSlider.value = currentEcoValue;
    }
    
    public float GetCurrentMoney()
    {
        return currentMoney;
    }
    
    public float GetCurrentEcoValue()
    {
        return currentEcoValue;
    }
    
    // 获取整数形式的当前资源（用于需要整数的逻辑）
    public int GetCurrentMoneyInt()
    {
        return Mathf.FloorToInt(currentMoney);
    }
    
    public int GetCurrentEcoValueInt()
    {
        return Mathf.FloorToInt(currentEcoValue);
    }
    
    // 获取当前总效果（用于调试或UI显示）
    public string GetTotalEffects()
    {
        return $"总效果: 每秒生态值+{totalEcoPerSecond}, 每{moneyInterval}秒金钱+{totalMoneyPerInterval}";
    }
}