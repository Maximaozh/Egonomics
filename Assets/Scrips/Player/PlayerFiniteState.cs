using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static BuildingUnit;

public class PlayerFiniteState : PlayerBase
{
    public enum AIMood
    {
        weak,
        normal,
        agressive
    }

    public enum AIStrategy
    {
        Produce,
        ProduceAndSell,
        Reseller,
    }

    public enum MarketDemandState
    {
        High,
        Low
    }

    public enum Appealence
    {
        Good,
        Bad
    }

    public enum AIState
    {
        // Базовое
        TurnStart,
        TurnWaiting,
        DataAnalys,
        Planning,
        TurnEnd,

        // Работа с продажами и закупками
        PriceCorrection,
        PurchaseEditing,
        SellEditing,

        // Работа со зданиями
        BuildingManagement,
        BuildingPowerExpanding,

        // Анализ поведения
        MoodAnalysing,
        CrisisManagement
    }

    public enum HandleResult
    {
        succesed,
        failed,
        interrupt
    }

    private class CompetitorData
    {
        public int TotalSellers { get; set; }
        public int AvailableSellers { get; set; }
        public float AveragePrice { get; set; }
    }


    [Header("Параметры - Модификаторы цены")]
    [SerializeField] private float basePriceModifier = 1.5f; // Базовая цена
    [SerializeField] private float crisisPriceModifier = 2.0f; // Цена во время кризиса
    [SerializeField] private float minPriceModifier = 1.2f; // Минимальный модификатор
    [SerializeField] private float maxPriceModifier = 2.0f; // Максимальный модификатор
    [SerializeField] private bool onlyBasicResouce = false;
    [SerializeField] private bool onlyReseller = false;
    public float BasePriceModifier { get => basePriceModifier; set => basePriceModifier = value; }
    public float CrisisPriceModifier { get => crisisPriceModifier; set => crisisPriceModifier = value; }
    public float MinPriceModifier { get => minPriceModifier; set => minPriceModifier = value; }
    public float MaxPriceModifier { get => maxPriceModifier; set => maxPriceModifier = value; }


    [Header("Параметры - Ход")]
    [SerializeField] private int actionsPerTurnMaximum = 5; // Количество действий на ход. его тратит ОЖИДАНИЕ, КОРРЕКТИРОВКА ЦЕН, СТРОИТЕЛЬСТВО
    [SerializeField] private int actionsLeft = 5; // Осталось действий

    [SerializeField] private int sleepWaitingMaximum = 2;      // Количество ходов, которые ИА тратит на своё восстановление
    [SerializeField] private int currentSleepWaiting = 2;      // Количество ходов, которые ИА тратит на своё восстановление

    [Header("Параметры - Управление")]
    [SerializeField] private float technologyChance = 0.5f; // Шанс добавления научного юнита в шаблон
    [SerializeField] private float lowerResouceLevel = 1.0f; // Шанс выбора ресурса с более высоким уровнем (с какого индекса начинаем выбор ресурсов)
    [SerializeField] private float higherResourceLevel = 0.0f; // Шанс выбора ресурса с более высоким уровнем (с какого индекса начинаем выбор ресурсов)
    [SerializeField] private float aggressive = 0.5f; // Влияет на то, насколько завышает модификаторы цены при победе в конкуренции
    [SerializeField] private int buildingLimit = 20;    // Ограничение зданий, который ИИ строит на карте

    [Header("Параметры - Случайность")]
    [SerializeField] private float screwLooseChanse = 0.2f; // Шанс возникновения "бреда" в голове ИА
    [SerializeField] private float screwLoosePower = 0.2f; // Влияет на отклонения каждого выбора
    [SerializeField] private float randomiserFactor = 0.2f; // Параметр случайности для неигрового влияния

    [Header("Параметры - Стратегия")]
    [SerializeField] private ProductData targetProduct;  // Ресурс, которые отслеживает ИИ на данную итерацию
    [SerializeField] private int turnsBeforeLevelUp = 5;   // Ход перед которым ИИ перейдёт к следующему уровню
    [SerializeField] private int currentProductLevel = -1;   // Стартовый уровень
    [SerializeField] private AIStrategy selectedStrategy = AIStrategy.Produce; // Выбранная Стратегия ИИ

    public int factoryPerTurn = 2;
    public int factoryBuilded = 1;
    public int shopPerTurn = 1;
    public int shopsBuilded = 1;

    public float shopChance = 0.8f;

    [Header("Состояния")]
    [SerializeField] private AIMood currentMood = AIMood.normal; // Оценка поведения ИА или его настроение. 
    [SerializeField] private AIState currentAIState = AIState.TurnStart; // Нынешнее состояние ИА.
    [SerializeField] private AIState lastAIState = AIState.TurnStart; // Прошлое состояние ИА.

    [Header("Кризисное управление")]
    [SerializeField] private bool crisisHandled = false;
    [SerializeField] private int crisisDuration = 3; // Длительность кризиса в ходах
    [SerializeField] private int crisisTimer = 0;
    [SerializeField] private Dictionary<SellUnit, float> originalPriceModifiers = new Dictionary<SellUnit, float>();
    [SerializeField] private Dictionary<SellUnit, bool> originalPrivacySettings = new Dictionary<SellUnit, bool>();

    [SerializeField] private bool isTurnInProgress = false;

    [Header("Лёгкое преимущество перед игроком - поддержка Скайнета")]
    [SerializeField] public int advantage = 1500;

    public int ActionsPerTurnMaximum { get => actionsPerTurnMaximum; set => actionsPerTurnMaximum = value; }
    public int ActionsLeft { get => actionsLeft; set => actionsLeft = value; }
    public int SleepWaitingMaximum { get => sleepWaitingMaximum; set => sleepWaitingMaximum = value; }
    public int CurrentSleepWaiting { get => currentSleepWaiting; set => currentSleepWaiting = value; }
    public float TechnologyChance { get => technologyChance; set => technologyChance = value; }
    public float LowerResouceLevel { get => lowerResouceLevel; set => lowerResouceLevel = value; }
    public float HigherResourceLevel { get => higherResourceLevel; set => higherResourceLevel = value; }
    public float Agressive { get => aggressive; set => aggressive = value; }
    public int BuildingLimit { get => buildingLimit; set => buildingLimit = value; }
    public float ScrewLooseChanse { get => screwLooseChanse; set => screwLooseChanse = value; }
    public float ScrewLoosePower { get => screwLoosePower; set => screwLoosePower = value; }
    public float RandomiserFactor { get => randomiserFactor; set => randomiserFactor = value; }
    public ProductData TargetProduct { get => targetProduct; set => targetProduct = value; }
    public AIStrategy SelectedStrategy { get => selectedStrategy; set => selectedStrategy = value; }
    public AIMood CurrentMood { get => currentMood; set => currentMood = value; }
    public AIState CurrentAIState { get => currentAIState; set => currentAIState = value; }
    public AIState LastAIState { get => lastAIState; set => lastAIState = value; }
    public bool CrisisHandled { get => crisisHandled; set => crisisHandled = value; }
    public int CrisisDuration { get => crisisDuration; set => crisisDuration = value; }
    public int CrisisTimer { get => crisisTimer; set => crisisTimer = value; }
    public Dictionary<SellUnit, float> OriginalPriceModifiers { get => originalPriceModifiers; set => originalPriceModifiers = value; }
    public Dictionary<SellUnit, bool> OriginalPrivacySettings { get => originalPrivacySettings; set => originalPrivacySettings = value; }
    public bool IsTurnInProgress { get => isTurnInProgress; set => isTurnInProgress = value; }
    public int Advantage { get => advantage; set => advantage = value; }
    public bool OnlyBasicResouce { get => onlyBasicResouce; set => onlyBasicResouce = value; }
    public bool OnlyReseller { get => onlyReseller; set => onlyReseller = value; }

    public override void TakeTurn()
    {
        base.TakeTurn();
        shopPerTurn = shopsBuilded;
        factoryPerTurn = factoryBuilded;
        Portfolio.funds += Advantage * UnityEngine.Random.value;
        if (!IsTurnInProgress)
        {
            StartCoroutine(TurnCoroutine());
        }
    }

    private IEnumerator TurnCoroutine()
    {
        DataLogger.Instance.Log($"");
        DataLogger.Instance.Log($"==={this.PlayerName}===");
        DataLogger.Instance.Log($"{this.PlayerName} принял бразды управления! Кажется начинается...");
        IsTurnInProgress = true;
        CrisisHandled = false;
        CurrentAIState = AIState.TurnStart;
        LastAIState = AIState.TurnStart;

        while (true)
        {
            HandleResult handleResult = HandleCurrentState();
            if (handleResult == HandleResult.interrupt) break;
            yield return null; // Оставляем только один yield на всю итерацию
        }

        EndTurn();
        IsTurnInProgress = false;
    }

    private HandleResult HandleCurrentState()
    {
        HandleResult handleResult = HandleResult.failed;
        switch (CurrentAIState)
        {
            // Базовое
            case AIState.TurnStart:
                ////Debug.Log($"{playerName} начал ход");
                handleResult = HandleTurnStart();
                break;
            case AIState.TurnWaiting:
                ////Debug.Log($"{playerName} выжидает");
                handleResult = HandleTurnWaiting();
                break;
            case AIState.DataAnalys:
                ////Debug.Log($"{playerName} анализирует");
                handleResult = HandleDataAnalysis();
                break;
            case AIState.Planning:
                ////Debug.Log($"{playerName} планиурет");
                handleResult = HandlePlanning();
                break;
            case AIState.TurnEnd:
                ////Debug.Log($"{playerName} завершает ход");
                handleResult = HandleTurnEnd();
                break;

            // Цены
            case AIState.PriceCorrection:
                ////Debug.Log($"{playerName} корректирует цены");
                handleResult = HandlePriceCorrection();
                break;
            case AIState.PurchaseEditing:
                ////Debug.Log($"{playerName} управляет покупками");
                handleResult = HandlePurchaseEditing();
                break;
            case AIState.SellEditing:
                ////Debug.Log($"{playerName} управляет продажами");
                handleResult = HandleSellEditing();
                break;

            // Здания
            case AIState.BuildingManagement:
                ////Debug.Log($"{playerName} управляет зданиями");
                handleResult = HandleBuildingManagement();
                break;
            case AIState.BuildingPowerExpanding:
                ////Debug.Log($"{playerName} расширяет здания");
                handleResult = HandleBuildingPowerExpanding();
                break;

            // Поведение
            case AIState.MoodAnalysing:
                ////Debug.Log($"{playerName} оценивает настроение");
                handleResult = HandleMoodAnalysing();
                break;
            case AIState.CrisisManagement:
                ////Debug.Log($"{playerName} управляет кризисом");
                handleResult = HandleCrisisManagement();
                break;

            default:
                ////Debug.LogError($"ДА ГОСПОДИ ПОЧЕМУ ОПЯТЬ Я");
                break;
        }
        return handleResult;
    }
    public override void EndTurn()
    {
        base.EndTurn();
        CurrentSleepWaiting--;
        if (CurrentSleepWaiting < 0)
            CurrentSleepWaiting = SleepWaitingMaximum;
    }

    public void SwitchState(AIState newState)
    {
        LastAIState = CurrentAIState;
        CurrentAIState = newState;
    }

    // === Базовые состояния ===
    private HandleResult HandleTurnStart()
    {
        ActionsLeft = ActionsPerTurnMaximum;

        if (TurnManager.Instance.TurnCount % turnsBeforeLevelUp == 0)
            currentProductLevel++;

        SwitchState(AIState.TurnWaiting);

        return HandleResult.succesed;

    }

    private HandleResult HandleTurnWaiting()
    {
        if (ActionsLeft <= 0)
            SwitchState(AIState.TurnEnd);
        else if (CurrentSleepWaiting > 0)
            SwitchState(AIState.DataAnalys);
        else
            SwitchState(AIState.Planning);

        DataLogger.Instance.Log($"Находился в ожидании и придумал что-то очень умное!\n");
        return HandleResult.succesed;
    }

    //private HandleResult HandleDataAnalysis()
    //{
    //    DataLogger.Instance.Log($"Начинаю анализ производственных мощностей...");
    //    TargetProduct = null;

        
    //    var productionStats = new Dictionary<ProductData, int>();
    //    var ourSellUnits = Portfolio.Buildings
    //        .SelectMany(b => b.ActiveUnits)
    //        .Where(u => u.info is SellUnit)
    //        .Select(u => u.info as SellUnit)
    //        .ToList();

    //    var othersSellUnits = MarketManager.Instance.SellUnitsPlayer
    //        .Select(u => u as SellUnit)
    //        .ToList();

    //    foreach (var sellUnit in ourSellUnits)
    //    {
    //        if (sellUnit.OutputProduct == null) continue;

    //        if (productionStats.ContainsKey(sellUnit.OutputProduct))
    //        {
    //            productionStats[sellUnit.OutputProduct]++;
    //        }
    //        else
    //        {
    //            productionStats[sellUnit.OutputProduct] = 1;
    //        }
    //    }

    //    foreach (var sellUnit in othersSellUnits)
    //    {
    //        if (sellUnit.OutputProduct == null) continue;

    //        if (productionStats.ContainsKey(sellUnit.OutputProduct))
    //        {
    //            productionStats[sellUnit.OutputProduct]++;
    //        }
    //        else
    //        {
    //            productionStats[sellUnit.OutputProduct] = 1;
    //        }
    //    }

    //    foreach (var sellUnit in othersSellUnits)
    //    {
    //        if (sellUnit.OutputProduct == null) continue;

    //        if (productionStats.ContainsKey(sellUnit.OutputProduct) && sellUnit.StoredItems.ContainsKey(sellUnit.OutputProduct))
    //        {
    //            if (sellUnit.orders.Count < 2) 
    //                productionStats[sellUnit.OutputProduct]++;
    //        }
    //        else
    //        {
    //            if (sellUnit.orders.Count < 2)
    //                productionStats[sellUnit.OutputProduct] = 1;
    //        }
    //    }

    //    var allProducts = MarketManager.Instance.GameProductsDatabase
    //        .Where(p => p.Level <= currentProductLevel) 
    //        .OrderByDescending(p => p.Level) 
    //        .ThenByDescending(p => p.Requireds?.Count ?? 0)
    //        .ToList();

    //    List<ProductData> craftItems = new List<ProductData>();
    //    List<ProductData> basicItems = allProducts.Where(x => x.Requireds.Count == 0).ToList<ProductData>();

    //    foreach (var product in allProducts)
    //    {
    //        if (product.Level == 0 && product.Requireds.Count == 0)
    //        {
    //            if (TargetProduct == null)
    //                TargetProduct = product;
    //            continue;
    //        }

    //        bool canProduce = true;
    //        foreach (var requirement in product.Requireds)
    //        {
    //            if (!productionStats.ContainsKey(requirement.product) ||
    //                productionStats[requirement.product] < 1)
    //            {
    //                canProduce = false;
    //                break;
    //            }
    //        }

    //        if (canProduce)
    //        {
    //            int currentProductionCount = productionStats.ContainsKey(product) ?
    //                productionStats[product] : 0;

    //            if (currentProductionCount < 3)
    //            {
    //                craftItems.Add(product);
    //                break;
    //            }
    //        }
    //    }

    //    if (craftItems.Count > 0)
    //    {
    //        SetTarget(craftItems,true);
    //    }
    //    else if (craftItems.Count == 0 && basicItems.Count > 0)
    //    {
    //        SetTarget(basicItems,false);
    //    }
    //    else
    //    {
    //        DataLogger.Instance.Log($"Не удалось выбрать целевой продукт!");
    //    }

    //    ActionsLeft--;
    //    SwitchState(AIState.TurnWaiting);
    //    return HandleResult.succesed;
    //}

private HandleResult HandleDataAnalysis()
{
    DataLogger.Instance.Log($"Начинаю анализ производственных мощностей...");
    TargetProduct = null;

    if(selectedStrategy == AIStrategy.Reseller)
        {
            var unsatisfiedDemand = new Dictionary<ProductData, float>();

            foreach (var marketItem in MarketManager.Instance.MarketItems)
            {
                if (marketItem.product == null) continue;

                float satisfactionRatio = (float)marketItem.Supply / marketItem.Demand;
                float unsatisfied = 1 - satisfactionRatio;

                unsatisfiedDemand[marketItem.product] = unsatisfied;
            }

            var supplierStats = new Dictionary<ProductData, int>();
            var othersSellUnits = MarketManager.Instance.SellUnitsPlayer
                .Where(u => u != null && u.OutputProduct != null && u.IsPrivate == false)
                .ToList();

            foreach (var product in unsatisfiedDemand.Keys)
            {
                supplierStats[product] = othersSellUnits
                    .Count(u => u.OutputProduct == product &&
                           u.StoredItems.ContainsKey(product) &&
                           u.StoredItems[product].Amount > 0);
            }

            var potentialProducts = unsatisfiedDemand.Keys
                .Where(p => p.Level <= currentProductLevel)
                .OrderByDescending(p => unsatisfiedDemand[p])
                .ThenBy(p => supplierStats[p])
                .ThenBy(p => p.CalculateResultPrice()) 
                .ToList();

            if (potentialProducts.Count > 0)
            {
                var topProducts = potentialProducts.Take(3).ToList();

                float totalWeight = topProducts.Sum(p =>
                    Mathf.Pow(unsatisfiedDemand[p], 2) *
                    (1f / (supplierStats[p] + 1)));

                float randomPoint = UnityEngine.Random.value * totalWeight;
                float currentSum = 0f;

                foreach (var product in topProducts)
                {
                    float productWeight = Mathf.Pow(unsatisfiedDemand[product], 2) *
                                        (1f / (supplierStats[product] + 1));

                    currentSum += productWeight;
                    if (currentSum >= randomPoint)
                    {
                        TargetProduct = product;
                        break;
                    }
                }

                DataLogger.Instance.Log($"Выбран товар для перепродажи: {TargetProduct.ItemName} " +
                    $"(неудовлетворенный спрос: {unsatisfiedDemand[TargetProduct]:P0}, " +
                    $"поставщиков: {supplierStats[TargetProduct]}, " +
                    $"цена: {TargetProduct.CalculateResultPrice()})");
            }
            else
            {
                DataLogger.Instance.Log($"Не найдено подходящих товаров для перепродажи!");
            }
        }
        else
        {
            // Собираем статистику по производству и поставщикам
            var productionStats = new Dictionary<ProductData, int>();
            var supplierStats = new Dictionary<ProductData, int>();

            // Наши продающие юниты
            var ourSellUnits = Portfolio.Buildings
                .SelectMany(b => b.ActiveUnits)
                .Where(u => u.info is SellUnit)
                .Select(u => u.info as SellUnit)
                .ToList();

            // Продающие юниты других игроков
            var othersSellUnits = MarketManager.Instance.SellUnitsPlayer
                .Where(u => u != null && u.IsPrivate == false)
                .ToList();

            // Считаем общее производство и поставщиков
            foreach (var product in MarketManager.Instance.GameProductsDatabase)
            {
                int ourProduction = ourSellUnits.Count(u => u.OutputProduct == product);
                int otherProduction = othersSellUnits.Count(u => u.OutputProduct == product);

                productionStats[product] = ourProduction + otherProduction;
                supplierStats[product] = othersSellUnits
                    .Count(u => u.OutputProduct == product &&
                               u.StoredItems.ContainsKey(product) &&
                               u.StoredItems[product].Amount > 0);
            }

            var allProducts = MarketManager.Instance.GameProductsDatabase
                .Where(p => p.Level <= currentProductLevel)
                .ToList();

            List<ProductData> basicItems = allProducts.Where(x => x.Requireds.Count == 0).ToList();
            List<ProductData> craftItems = allProducts.Where(x => x.Requireds.Count > 0).ToList();

            ProductData cheapestBasic = basicItems.OrderBy(x => x.CalculateResultPrice()).FirstOrDefault();

            var productScores = new Dictionary<ProductData, float>();

            foreach (var product in allProducts)
            {
                float score = 0f;

                float priceFactor = 1 / (product.CalculateResultPrice() + 0.1f); // Чем дешевле, тем лучше
                float scarcityFactor = 1 / (supplierStats[product] + 1); // Чем меньше поставщиков, тем лучше

                float strategyModifier = selectedStrategy == AIStrategy.Reseller ? 1.5f : 1f;
                float moodModifier = currentMood == AIMood.agressive ? 1.3f : 1f;

                float cheapestBonus = product == cheapestBasic ? 2f : 1f;

                score = priceFactor * scarcityFactor * strategyModifier * moodModifier * cheapestBonus;

                score *= 1 + (UnityEngine.Random.value - 0.5f) * randomiserFactor;

                productScores[product] = score;
            }

            var potentialTargets = new List<ProductData>();

            potentialTargets.AddRange(basicItems
                .OrderByDescending(p => productScores[p])
                .Take(3));

            foreach (var product in craftItems)
            {
                bool canProduce = true;
                foreach (var requirement in product.Requireds)
                {
                    if (productionStats[requirement.product] < 1)
                    {
                        canProduce = false;
                        break;
                    }
                }

                if (canProduce && productionStats[product] < 3)
                {
                    potentialTargets.Add(product);
                }
            }

            if (potentialTargets.Count > 0)
            {
                float totalScore = potentialTargets.Sum(p => productScores[p]);
                float randomPoint = UnityEngine.Random.value * totalScore;
                float currentSum = 0f;

                foreach (var product in potentialTargets.OrderByDescending(p => productScores[p]))
                {
                    currentSum += productScores[product];
                    if (currentSum >= randomPoint)
                    {
                        TargetProduct = product;
                        break;
                    }
                }
            }

        }

        ActionsLeft--;
    SwitchState(AIState.TurnWaiting);
    return HandleResult.succesed;
}
    
    private HandleResult HandlePlanning()
    {
        if (ActionsLeft <= 0)
        {
            SwitchState(AIState.MoodAnalysing);
            return HandleResult.succesed;
        }

        if (TargetProduct == null)
        {
            SwitchState(AIState.MoodAnalysing);
            return HandleResult.failed;
        }


        float reasonModifier = (1 - Agressive + UnityEngine.Random.value * RandomiserFactor);

        bool isCrisis = this.Portfolio.funds <= this.Portfolio.initialfunds / 2 &&
                       Portfolio.Buildings.Count > 2 &&
                       !CrisisHandled;

        if (this.Portfolio.funds > TargetProduct.CalculateResultPrice() * (5f + reasonModifier) + 2500)
        {
            DataLogger.Instance.Log($"Решил заняться расширением своего бизнеса");
            SwitchState(AIState.BuildingManagement);
            return HandleResult.succesed;
        }
        else if (isCrisis)
        {
            DataLogger.Instance.Log($"Решил заняться своей кризисной ситуацией");
            SwitchState(AIState.CrisisManagement);
            return HandleResult.succesed;
        }
        else
        {
            DataLogger.Instance.Log($"Решил заняться управлениями ценами");
            SwitchState(AIState.PriceCorrection);
            return HandleResult.succesed;
        }
    }


    private HandleResult HandleTurnEnd()
    {
        SwitchState(AIState.TurnEnd);
        return HandleResult.interrupt;
    }

    // === Продажи и закупки ===

    private HandleResult HandlePriceCorrection()
    {
        if (ActionsLeft <= 0 || LastAIState == AIState.PurchaseEditing)
        {
            SwitchState(AIState.Planning);
            return HandleResult.succesed;
        }

        SwitchState(AIState.SellEditing);

        ActionsLeft--;
        return HandleResult.succesed;
    }

    public float HelperPriceMoodAndRandomModifier()
    {
        float aggressiveFactor = Mathf.Pow(Agressive, 0.7f);

        float baseModifier = (0.7f * aggressiveFactor);

        float moodModifier = CurrentMood switch
        {
            AIMood.weak => 0.85f,
            AIMood.normal => 1.0f,
            AIMood.agressive => 1.15f,
            _ => 1.0f
        };

        float randomFactor = 1f + (UnityEngine.Random.value - 0.5f) * RandomiserFactor * 2f;

        float dynamicMax = Mathf.Lerp(1.8f, 2.2f, aggressiveFactor * moodModifier);

        return Mathf.Clamp(
            baseModifier * moodModifier * randomFactor, 
            MinPriceModifier,
            dynamicMax
        );
    }


    //private HandleResult HandleSellEditing()
    //{
    //    DataLogger.Instance.Log($"Корректировал цены продаж");
    //    var ourSellUnits = Portfolio.Buildings
    //        .SelectMany(b => b.ActiveUnits)
    //        .Where(u => u.info is SellUnit)
    //        .ToList();

    //    List<(string, float)> modifiers = new List<(string, float)>();
    //    foreach (var sellUnit in ourSellUnits)
    //    {
    //        if (sellUnit.info.OutputProduct == null) continue;

    //        float minModifier = float.MaxValue;

    //        var purchaseUnitsInBuilding = sellUnit.info.ParentBuilding.ActiveUnits
    //            .Where(u => u.info is PurchaseUnit)
    //            .Select(u => u.info as PurchaseUnit)
    //            .ToList();

    //        foreach (var pu in purchaseUnitsInBuilding)
    //        {
    //            if (pu.OutputProduct == sellUnit.info.OutputProduct)
    //            {
    //                float minModifiercheck = pu.SellerUnit != null ? pu.SuggestedMultiplier : 1.3f;
    //                if (minModifiercheck < minModifier)
    //                {
    //                    minModifier = minModifiercheck;
    //                }
    //            }
    //        }

    //        if (minModifier == float.MaxValue)
    //        {
    //            minModifier = 2f;
    //        }

    //        float baseModifier = HelperPriceMoodAndRandomModifier();
    //        baseModifier = Mathf.Max(baseModifier, minModifier);

    //        var productHistory = MarketManager.Instance.LastSalesRecords
    //            .Where(r => r.Product == sellUnit.info.OutputProduct)
    //            .ToList();

    //        float totalSold = productHistory.Sum(r => r.AmountSold);
    //        float totalDemand = MarketManager.Instance.MarketItems.First(x => x.product == sellUnit.info.OutputProduct).Demand;
    //        float totalRevenue = productHistory.Sum(r => r.TotalRevenue);
    //        float avgAppeal = productHistory.Any() ? productHistory.Average(r => r.Appeal) : 0.5f;

    //        MarketDemandState demand = totalSold > totalDemand * 0.7f ? MarketDemandState.High : MarketDemandState.Low;
    //        Appealence appeal = avgAppeal > 0.7f ? Appealence.Good : Appealence.Bad;

    //        float situationModifier = 1.0f;

    //        if (demand == MarketDemandState.High && appeal == Appealence.Good)
    //            situationModifier *= 1.2f;
    //        else if (demand == MarketDemandState.High && appeal == Appealence.Bad)
    //            situationModifier *= 0.9f;
    //        else if (demand == MarketDemandState.Low && appeal == Appealence.Good)
    //            situationModifier *= 1.1f;
    //        else
    //            situationModifier *= 0.8f;

    //        float finalModifier = Mathf.Clamp(
    //            baseModifier * situationModifier,
    //            Mathf.Max(MinPriceModifier, minModifier),
    //            MaxPriceModifier
    //        );

    //        var su = (SellUnit)sellUnit.info;
    //        su.PriceMultiplier = finalModifier;
    //        modifiers.Add(($"{su.ParentBuilding.BuildingName} [{su.UnitName}] ({su.OutputProduct.ItemName})", finalModifier));
    //    }

    //    foreach (var mod in modifiers)
    //        DataLogger.Instance.Log($"{mod.Item1} с модификатором цены {mod.Item2}");
    //    DataLogger.Instance.Log($"И закончил работу с магазином\n");

    //    SwitchState(AIState.PurchaseEditing);
    //    return HandleResult.succesed;
    //}

    private struct _sales
        {
        public int TotalSellers;
        public float AvgPrice;
    }

    private HandleResult HandleSellEditing()
    {
        DataLogger.Instance.Log($"Корректировал цены продаж");
        var ourSellUnits = Portfolio.Buildings
            .SelectMany(b => b.ActiveUnits)
            .Where(u => u.info is SellUnit)
            .ToList();

        // Сначала собираем статистику по всем нашим товарам
        var ourProducts = ourSellUnits
            .Where(u => u.info.OutputProduct != null)
            .GroupBy(u => u.info.OutputProduct)
            .ToDictionary(
                g => g.Key as ProductData,
                g => new _sales
                (){
                    TotalSellers = g.Count(),
                    AvgPrice = g.Average(u => ((SellUnit)u.info).PriceMultiplier)
                });

        List<(string, float)> modifiers = new List<(string, float)>();
        foreach (var sellUnit in ourSellUnits)
        {
            if (sellUnit.info.OutputProduct == null) continue;

            var product = sellUnit.info.OutputProduct;
            var su = (SellUnit)sellUnit.info;

            float minModifier = GetMinimumPriceModifier(su);

            float baseModifier = CalculateBasePriceModifier(minModifier);

            float marketAdjustment = CalculateMarketAdjustment(product, ourProducts);

            float competitionAdjustment = CalculateCompetitionAdjustment(product, ourProducts);

            float finalModifier = baseModifier * marketAdjustment * competitionAdjustment;
            finalModifier *= 1 + (UnityEngine.Random.value - 0.5f) * RandomiserFactor * 0.5f; // Меньшая случайность

            float currentModifier = su.PriceMultiplier;
            float maxChange = currentModifier * 0.2f;
            finalModifier = Mathf.Clamp(
                finalModifier,
                currentModifier - maxChange,
                currentModifier + maxChange
            );

            finalModifier = Mathf.Clamp(finalModifier, minModifier, MaxPriceModifier);

            if (CrisisHandled && CrisisTimer > 0)
            {
                finalModifier = Mathf.Max(finalModifier, CrisisPriceModifier);
            }

            su.PriceMultiplier = finalModifier;
            modifiers.Add(($"{su.ParentBuilding.BuildingName} [{su.UnitName}] ({su.OutputProduct.ItemName})", finalModifier));
        }

        foreach (var mod in modifiers)
            DataLogger.Instance.Log($"{mod.Item1} с модификатором цены {mod.Item2:F2}");
        DataLogger.Instance.Log($"И закончил работу с магазином\n");

        SwitchState(AIState.PurchaseEditing);
        return HandleResult.succesed;
    }

    private float GetMinimumPriceModifier(SellUnit sellUnit)
    {
        float minModifier = float.MaxValue;

        var purchaseUnits = sellUnit.ParentBuilding.ActiveUnits
            .Where(u => u.info is PurchaseUnit)
            .Select(u => u.info as PurchaseUnit);

        foreach (var pu in purchaseUnits)
        {
            if (pu.OutputProduct == sellUnit.OutputProduct && pu.SellerUnit != null)
            {
                minModifier = Mathf.Min(minModifier, pu.SuggestedMultiplier * 1.15f);
            }
        }

        return minModifier == float.MaxValue ? MinPriceModifier : minModifier;
    }

    private float CalculateBasePriceModifier(float minModifier)
    {
        float moodFactor = CurrentMood switch
        {
            AIMood.weak => 0.8f,      
            AIMood.normal => 1.0f,    
            AIMood.agressive => 1.3f, 
            _ => 1.0f
        };

        float aggressionFactor = 1.0f + Agressive * 0.5f;

        float basePrice = BasePriceModifier * moodFactor * aggressionFactor;

        return Mathf.Max(basePrice, minModifier);
    }

    private float CalculateMarketAdjustment(ProductData product, Dictionary<ProductData, _sales> ourProducts)
    {
        var marketItem = MarketManager.Instance.MarketItems.FirstOrDefault(m => m.product == product);
        if (marketItem == null) return 1.0f;

        float saturation = marketItem.Supply > 0 ?
            (float)marketItem.Demand / marketItem.Supply : 1.0f;

        var salesHistory = MarketManager.Instance.LastSalesRecords
            .Where(r => r.Product == product)
            .ToList();

        float avgAppeal = salesHistory.Any() ? salesHistory.Average(r => r.Appeal) : 0.5f;
        float sellThroughRate = salesHistory.Any() ?
            (float)salesHistory.Sum(r => r.AmountSold) / marketItem.Demand : 0.5f;

        float adjustment = 1.0f;

        if (saturation > 1.2f && avgAppeal > 0.7f)
        {
            adjustment *= 1.2f;
        }

        else if (saturation < 0.8f || avgAppeal < 0.4f)
        {
            adjustment *= 0.8f;
        }

        if (sellThroughRate < 0.3f)
        {
            adjustment *= 0.9f;
        }

        return adjustment;
    }

    private float CalculateCompetitionAdjustment(ProductData product, Dictionary<ProductData, _sales> ourProducts)
    {
        if (!ourProducts.ContainsKey(product)) return 1.0f;

        int ourSellersCount = ourProducts[product].TotalSellers;
        float ourAvgPrice = ourProducts[product].AvgPrice;

        // Если у нас много таких же товаров - снижаем цену для конкуренции
        if (ourSellersCount > 3)
        {
            float competitionFactor = Mathf.Clamp(1.0f - (ourSellersCount - 3) * 0.1f, 0.7f, 1.0f);
            return competitionFactor;
        }

        // Если у нас мало таких товаров - можем держать цену выше
        return 1.0f + (3 - ourSellersCount) * 0.05f;
    }
    private HandleResult HandlePurchaseEditing()
    {
        DataLogger.Instance.Log($"Корректировал цены закупок");
        foreach (var building in Portfolio.Buildings)
        {
            switch (building.BuildingType)
            {
                case BuildingType.Shop:
                    HelperHandleShopBuilding(building);
                    break;
                case BuildingType.Craft:
                    HelperHandleProductionBuilding(building);
                    break;

                default: // Обработка других состояний не требуется
                    break;
            }
        }

        SwitchState(AIState.PriceCorrection);
        return HandleResult.succesed;
    }
    private void HelperHandleShopBuilding(BuildingAbstract build)
    {
        DataLogger.Instance.Log($"И начал обработку для магазина {build.BuildingName}");
        var purchaseUnits = build.ActiveUnits
            .Where(unit => unit.info is PurchaseUnit)
            .ToList();

        // Сбрасываем существующие заказы
        foreach (var unit in purchaseUnits)
        {
            var pu = (PurchaseUnit)unit.info;
            pu.CancelOrder();
        }

        foreach (var purchaseUnit in purchaseUnits)
        {
            var pu = (PurchaseUnit)purchaseUnit.info;
            if (pu.OutputProduct == null) continue;

            // Получаем всех потенциальных продавцов, сортируем по цене
            var allSellers = MarketManager.Instance.GetValidSellersFor(pu.OutputProduct, this)
                .OrderBy(s => s.GetCurrentPrice())
                .ToList();

            // Разделяем продавцов на группы
            var ourSellers = allSellers.Where(s => s.ParentBuilding.Owner == this).ToList();
            var externalSellers = allSellers.Where(s => s.ParentBuilding.Owner != this).ToList();

            // Функция для безопасного выбора случайного элемента
            SellUnit SelectRandomSeller(List<SellUnit> sellers)
            {
                if (sellers.Count == 0) return null;
                int index = Mathf.Min(Mathf.FloorToInt(UnityEngine.Random.value * sellers.Count), sellers.Count - 1);
                return sellers[index];
            }

            // Приоритет 1: наши собственные продавцы
            if (ourSellers.Count > 0)
            {
                var seller = ourSellers
                    .OrderBy(s => s.orders.Count)
                    .FirstOrDefault(s => s.orders.Count < 3); // Макс 3 заказа на продавца

                if (seller != null)
                {
                    pu.SetOrder(seller, pu.OutputProduct, 1.0f); // Внутренние цены без наценки
                    DataLogger.Instance.Log($"[{pu.UnitName}] подключен к ВНУТРЕННЕМУ поставщику {seller.ParentBuilding.BuildingName}");
                    continue;
                }
            }

            // Приоритет 2: внешние продавцы
            if (externalSellers.Count > 0)
            {
                // Берем 5 самых дешевых подходящих продавцов
                var suitableSellers = externalSellers
                    .Where(s => s.PriceMultiplier <= 2 && s.orders.Count < 3)
                    .Take(5)
                    .ToList();

                if (suitableSellers.Count > 0)
                {
                    // Выбираем случайного из топ-5 самых дешевых
                    var seller = SelectRandomSeller(suitableSellers);

                    // Устанавливаем цену как среднее между их ценой и нашим максимумом
                    float finalMultiplier = Mathf.Clamp(seller.PriceMultiplier + HelperPriceMoodAndRandomModifier() * aggressive, 0, 2.0f);
                    pu.SetOrder(seller, pu.OutputProduct, finalMultiplier);
                    DataLogger.Instance.Log($"[{pu.UnitName}] подключен к ВНЕШНЕМУ поставщику {seller.ParentBuilding.Owner.playerName} за {finalMultiplier:F2}x");
                    continue;
                }
            }

            //// Приоритет 3: если не нашли подходящих продавцов - покупаем с рынка
            //DataLogger.Instance.Log($"[{pu.UnitName}] будет закупать {pu.OutputProduct.ItemName} с рынка");
            ////pu.FillFromMarket = true;
        }
    }

    private void HelperHandleProductionBuilding(BuildingAbstract build)
    {
        DataLogger.Instance.Log($"И начал обработку для производства {build.BuildingName}");

        var craftUnit = build.ActiveUnits
            .FirstOrDefault(unit => unit.info is CraftUnit)?.info as CraftUnit;
        if (craftUnit == null || craftUnit.OutputProduct == null) return;


        var ourProduction = Portfolio.Buildings
            .SelectMany(b => b.ActiveUnits)
            .Where(u => u.info is SellUnit)
            .Select(u => u.info as SellUnit)
            .Where(s => s.OutputProduct != null)
            .GroupBy(s => s.OutputProduct)
            .ToDictionary(g => g.Key, g => g.ToList());

        if (craftUnit == null || craftUnit.OutputProduct == null)
            return;

        var sellUnit = build.ActiveUnits
            .FirstOrDefault(unit => unit.info is SellUnit)?.info as SellUnit;
        if (sellUnit == null)
            return;

        var purchaseUnits = build.ActiveUnits
            .Where(unit => unit.info is PurchaseUnit)
            .ToList();


        for (int i = 0; i < craftUnit.OutputProduct.Requireds.Count; i++)
        {
            if (i >= purchaseUnits.Count) break;

            var required = craftUnit.OutputProduct.Requireds[i];
            var pu = (PurchaseUnit)purchaseUnits[i].info;
            pu.CancelOrder();

            if (ourProduction.ContainsKey(required.product))
            {
                var internalSuppliers = ourProduction[required.product]
                    .Where(s => s != pu && s.ParentBuilding != build && s.StoredItems.ContainsKey(required.product) && s.StoredItems[required.product].Amount > 0) 
                    .OrderBy(s => s.GetCurrentPrice())
                    .ToList();

                if (internalSuppliers.Count > 0)
                {
                    var weightedSelection = internalSuppliers
                    .OrderBy(s => s.orders.Count)
                    .ThenBy(s => s.GetCurrentPrice())
                    .First();

                    float internalModifier = CalculateOptimalModifier(weightedSelection, required.product);
                    pu.SetOrder(weightedSelection, required.product, internalModifier);
                    DataLogger.Instance.Log($"[{pu.UnitName}] подключен к ВНУТРЕННЕМУ поставщику " + $"{weightedSelection.ParentBuilding.BuildingName} ({required.product.ItemName})");
                    continue;
                }
            }

            var externalSuppliers = MarketManager.Instance
                .GetValidSellersFor(required.product, this)
                .Where(s => s.ParentBuilding.Owner != this && s.StoredItems.ContainsKey(required.product) && s.StoredItems[required.product].Amount > 0)
                .OrderBy(s => s.GetCurrentPrice())
                .ToList();

            if (externalSuppliers.Count > 0)
            {
                var weightedSelection = externalSuppliers
                    .OrderBy(s => s.orders.Count)
                    .ThenBy(s => s.GetCurrentPrice())
                    .First();

                float externalModifier = CalculateOptimalModifier(weightedSelection, required.product);

                foreach(var seller in externalSuppliers)
                {
                    pu.SetOrder(weightedSelection, required.product, externalModifier);
                }
                pu.SetOrder(weightedSelection, required.product, externalModifier);
                DataLogger.Instance.Log($"[{pu.UnitName}] подключен к ВНЕШНЕМУ поставщику " +
                    $"{weightedSelection.ParentBuilding.Owner.playerName} ({required.product.ItemName})");
                continue;
            }

            ////pu.FillFromMarket = true;
            //DataLogger.Instance.Log($"[{pu.UnitName}] будет закупать {required.product.ItemName} с рынка");
        }
    }

    private float CalculateOptimalModifier(SellUnit seller, ProductData product)
    {
        float baseModifier = seller.PriceMultiplier * (1 + Agressive * 0.2f);
        float randomFactor = 0.1f * UnityEngine.Random.value * RandomiserFactor;
        return Mathf.Clamp(baseModifier + randomFactor, MinPriceModifier, MaxPriceModifier);
    }

    // === Работа со зданиями ===
    private HandleResult HandleBuildingManagement()
    {
        DataLogger.Instance.Log($"Занимался управлением зданиями");
        ActionsLeft--;

        if (Portfolio.Buildings.Count >= BuildingLimit)
        {
            SwitchState(AIState.PriceCorrection);
            return HandleResult.succesed;
        }
        else
        {
            SwitchState(AIState.BuildingPowerExpanding);
            return HandleResult.succesed;
        }
    }

    public HandleResult HandleBuildingPowerExpanding()
    {
        if (ActionsLeft <= 0 || BuildingLimit <= Portfolio.Buildings.Count)
        {
            SwitchState(AIState.Planning);
            return HandleResult.succesed;
        }

        switch (SelectedStrategy)
        {
            case AIStrategy.Reseller:
                HelperResselerWorking();
                break;

            case AIStrategy.ProduceAndSell:
                HelperProduceAndSellWorking();
                break;

            case AIStrategy.Produce:
                HelperProduceWorking();
                break;
        }

        ActionsLeft--;

        SwitchState(AIState.PriceCorrection);
        return HandleResult.succesed;
    }
    private int CalculateResellerPairsCount()
    {
        float baseCount = 2f;

        float aggressiveModifier = Mathf.Lerp(0.8f, 1.5f, Agressive);
        float moodModifier = CurrentMood == AIMood.agressive ? 1.2f : 0.9f;
        float randomFactor = UnityEngine.Random.Range(0.9f, 1.1f);

        int count = Mathf.Clamp(Mathf.RoundToInt(baseCount * aggressiveModifier * moodModifier * randomFactor), 1, 1);

        return count;
    }

    private void HelperResselerWorking()
    {
        var availablePositions = BuildManagerExpanded.Instance.GetAvailableBuildPositions();
        if (availablePositions.Count == 0)
            return;

        Vector3Int buildPosition = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];

        BuildingAbstract shopTemplate = BuildManagerExpanded.Instance.availableBuilds.FirstOrDefault(b =>
            b.BuildingType == BuildingType.Shop);

        if (shopTemplate == null)
            return;

        if (!BuildManagerExpanded.Instance.TryBuild(shopTemplate, buildPosition, this))
            return;

        var newShop = Portfolio.Buildings.Last();

        var shopUnits = newShop.ActiveUnits;
        Unit sellUnit = newShop.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.sell);
        Unit purchaseUnit = newShop.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.purchase);

        int pairCount = CalculateResellerPairsCount();
        for (int i = 0; i < 1; i += 2)
        {
            Unit newSellUnit = Instantiate(sellUnit);
            newSellUnit.info.OutputProduct = TargetProduct;

            Unit newPurchaseUnit = Instantiate(purchaseUnit);
            newPurchaseUnit.info.OutputProduct = TargetProduct;

            newShop.ReplaceUnitByExisting(newShop.ActiveUnits[i], newSellUnit);
            newShop.ReplaceUnitByExisting(newShop.ActiveUnits[i + 1], newPurchaseUnit);

            newShop.AddConnection(newPurchaseUnit.info, newSellUnit.info);
        }
        DataLogger.Instance.Log($"Выполнил строительство зданий согласно стратегии перепродаж {newShop.BuildingName}\n");
    }
    private void HelperProduceWorking()
    {
        var availablePositions = BuildManagerExpanded.Instance.GetAvailableBuildPositions();
        if (availablePositions.Count == 0)
            return;

        Vector3Int buildPosition = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];

        BuildingAbstract produceTemplate = null;
        if (TargetProduct.Requireds.Count == 0 || TargetProduct.Requireds == null)
            produceTemplate = BuildManagerExpanded.Instance.availableBuilds.FirstOrDefault(b => b.BuildingType == BuildingType.Produce);
        else
            produceTemplate = BuildManagerExpanded.Instance.availableBuilds.FirstOrDefault(b => b.BuildingType == BuildingType.Craft);

        if (produceTemplate == null)
            return;

        if (!BuildManagerExpanded.Instance.TryBuild(produceTemplate, buildPosition, this))
            return;

        var newProduce = Portfolio.Buildings.Last();

        var produceBuilding = newProduce;
        var produceUnits = newProduce.ActiveUnits;


        if (produceTemplate.BuildingType == BuildingType.Produce)
        {
            Unit produceUnit = produceBuilding.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.produce);
            produceUnit.info.OutputProduct = targetProduct;
            Unit sellUnit = produceBuilding.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.sell);
            sellUnit.info.OutputProduct = targetProduct;
            int pairCount = CalculateResellerPairsCount();
            for (int i = 0; i < 5; i += 2)
            {
                Unit newProduceUnit = Instantiate(produceUnit);
                newProduceUnit.info.OutputProduct = TargetProduct;

                Unit newSellUnit = Instantiate(sellUnit);
                newSellUnit.info.OutputProduct = TargetProduct;

                newProduce.ReplaceUnitByExisting(newProduce.ActiveUnits[i], newProduceUnit);
                newProduce.ReplaceUnitByExisting(newProduce.ActiveUnits[i + 1], newSellUnit);

                newProduce.AddConnection(newProduceUnit.info, newSellUnit.info);
            }

            if (UnityEngine.Random.value < TechnologyChance)
            {
                Unit scienceUnit = produceBuilding.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.science);
                Unit newScienceUnit = Instantiate(scienceUnit);;
                newProduce.ReplaceUnitByExisting(newProduce.ActiveUnits[newProduce.ActiveUnits.Count()-1], newScienceUnit);
            }
        }
        else
        {
            Unit purchaseUnit = produceBuilding.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.purchase);
            Unit craftUnit = produceBuilding.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.craft);
            Unit sellUnit = produceBuilding.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.sell);

            int i = 0;
            Unit newCraftUnit = Instantiate(craftUnit);
            newCraftUnit.info.OutputProduct = TargetProduct;


            newProduce.ReplaceUnitByExisting(newProduce.ActiveUnits[i], newCraftUnit);
            i++;

            Unit newSellUnit = Instantiate(sellUnit);
            newSellUnit.info.OutputProduct = TargetProduct;

            newProduce.ReplaceUnitByExisting(newProduce.ActiveUnits[i], newSellUnit);
            i++;

            for (int counter = 0; counter < TargetProduct.Requireds.Count; counter++)
            {
                Unit newPurchaseUnit = Instantiate(purchaseUnit);
                newPurchaseUnit.info.OutputProduct = TargetProduct.Requireds[counter].product;
                newProduce.ReplaceUnitByExisting(newProduce.ActiveUnits[i], newPurchaseUnit);
                i++;
                newProduce.AddConnection(newPurchaseUnit.info, newCraftUnit.info);
            }

            newProduce.AddConnection(newCraftUnit.info, newSellUnit.info);

            if (UnityEngine.Random.value < TechnologyChance)
            {
                Unit scienceUnit = produceBuilding.AvaiableUnits.FirstOrDefault(b => b.info.Type == UnitType.science);
                Unit newScienceUnit = Instantiate(scienceUnit);
                newProduce.ReplaceUnitByExisting(newProduce.ActiveUnits[i], newScienceUnit);
            }
        }
        DataLogger.Instance.Log($"Выполнил строительство зданий согласно стратегии производства {newProduce.BuildingName} \n");
    }

    private void HelperProduceAndSellWorking()
    {
        if(factoryPerTurn > 0)
        {
            HelperProduceWorking();
            factoryPerTurn--;
        }
        if(shopPerTurn > 0)
        {
            HelperResselerWorking();
            shopPerTurn--;
        }
    }


    // === Анализ поведения ===
    private HandleResult HandleMoodAnalysing()
    {
        var ourSellUnits = Portfolio.Buildings
            .SelectMany(b => b.ActiveUnits)
            .Where(u => u.info is SellUnit)
            .Select(u => u.info as SellUnit)
            .ToList();

        float fundsRatio = Portfolio.funds / Portfolio.initialfunds;
        bool isFinancialCrisis = fundsRatio < 0.5f;

        float totalSales = MarketManager.Instance.LastSalesRecords
            .Where(r => ourSellUnits.Any(su => su == r.Seller))
            .Sum(r => r.TotalRevenue);

        float avgSalesPerUnit = ourSellUnits.Count > 0 ? totalSales / ourSellUnits.Count : 0;

        AIMood newMood = DetermineNewMood(fundsRatio, avgSalesPerUnit);

        if (newMood != CurrentMood)
        {
            float changeChance = 0.7f - (Agressive * 0.2f) + (RandomiserFactor * 0.1f);
            if (UnityEngine.Random.value < changeChance)
                CurrentMood = newMood;
        }
        DataLogger.Instance.Log($"Сменил настроение на {currentMood.ToString()}\n");
        SwitchState(AIState.TurnEnd);
        return HandleResult.succesed;
    }

    private AIMood DetermineNewMood(float fundsRatio, float avgSales)
    {
        float financialWeight = 0.4f;
        float salesWeight = 0.3f;

        financialWeight += Agressive * 0.1f;
        salesWeight -= Agressive * 0.05f;

        float moodScore = (Mathf.Clamp01(fundsRatio) * financialWeight) +
            (Mathf.Clamp01(avgSales / 100f) * salesWeight) + Agressive;

        moodScore *= 1f + (UnityEngine.Random.value - 0.5f) * RandomiserFactor * 0.5f;

        if (moodScore < 0.4f)
            return AIMood.weak;
        if (moodScore > 0.7f)
            return AIMood.agressive;
        return AIMood.normal;
    }


    private HandleResult HandleCrisisManagement()
    {
        DataLogger.Instance.Log($"Занимался управлением кризисом\n");

        if (CrisisTimer == 0)
        {
            ActivateCrisisMeasures();
            CrisisTimer = CrisisDuration;
        }

        CrisisTimer--;

        if (CrisisTimer <= 0)
        {
            DeactivateCrisisMeasures();
            CrisisHandled = true;
            SwitchState(AIState.Planning);
        }

        return HandleResult.succesed;
    }

    private void ActivateCrisisMeasures()
    {
        OriginalPriceModifiers.Clear();
        OriginalPrivacySettings.Clear();

        var allSellUnits = Portfolio.Buildings
            .SelectMany(b => b.ActiveUnits)
            .Where(u => u.info is SellUnit)
            .Select(u => u.info as SellUnit);

        foreach (var sellUnit in allSellUnits)
        {
            OriginalPriceModifiers[sellUnit] = sellUnit.PriceMultiplier;
            OriginalPrivacySettings[sellUnit] = sellUnit.IsPrivate;

            sellUnit.PriceMultiplier = CrisisPriceModifier;

            sellUnit.AddToMarket();
        }

        CurrentMood = AIMood.agressive;
        Agressive = 1.0f;
        RandomiserFactor = 0.1f;
    }

    private void DeactivateCrisisMeasures()
    {
        foreach (var orig in OriginalPriceModifiers)
        {
            var sellUnit = orig.Key;
            sellUnit.PriceMultiplier = orig.Value;
        }

        foreach (var orig in OriginalPrivacySettings)
        {
            var sellUnit = orig.Key;
            sellUnit.IsPrivate = orig.Value;
            sellUnit.AddToMarket();
        }

        CurrentMood = AIMood.normal;
        Agressive = 0.5f;
        RandomiserFactor = 0.2f;

        OriginalPriceModifiers.Clear();
        OriginalPrivacySettings.Clear();
    }
}