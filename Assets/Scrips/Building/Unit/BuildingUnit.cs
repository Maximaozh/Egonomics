    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    using static RandomNameGenerator;

    [System.Serializable]
    public class BuildingUnit : MonoBehaviour
    {

        [Header("Общая информация")]
        [SerializeField] private string unitName;
        [SerializeField] private string extraInfo;
        [SerializeField] private UnitType type = UnitType.slot;
        [SerializeField] private int purchaseCost = 100;
        [SerializeField] private int maintenanceCost = 20;
        [SerializeField] private int unitLevel = 1;
        [SerializeField] private int workersLevel = 1;

        [SerializeField] private int basePower = 100;
        [SerializeField] private int maxReasonableLevel = 1000;

        [Header("Хранилище")]

        [SerializeField] private Dictionary<ProductData, ProductInstance> storedItems = new Dictionary<ProductData, ProductInstance>();

        [SerializeField] private int maxStorageCapacity = 50;

        [Header("Входные и выходные ресурсы")]
        [SerializeField] private int outputCapacity = 10;

        [SerializeField] private BuildingAbstract parentBuilding; // Здание, которому принадлежит unit

        [SerializeField] private ProductData outputProduct;

        public string UnitName { get => unitName; set => unitName = value; }
        public string ExtraInfo { get => extraInfo; set => extraInfo = value; }
        public UnitType Type { get => type; set => type = value; }
        public int PurchaseCost { get => purchaseCost; set => purchaseCost = value; }
        public int MaintenanceCost { get => maintenanceCost; set => maintenanceCost = value; }
        public int UnitLevel { get => unitLevel; set => unitLevel = value; }
        public int WorkersLevel { get => workersLevel; set => workersLevel = value; }
        public int BasePower { get => basePower; set => basePower = value; }
        public int MaxReasonableLevel { get => maxReasonableLevel; set => maxReasonableLevel = value; }
        public Dictionary<ProductData, ProductInstance> StoredItems { get => storedItems; set => storedItems = value; }
        public int MaxStorageCapacity { get => maxStorageCapacity; set => maxStorageCapacity = value; }
        public int OutputCapacity { get => outputCapacity; set => outputCapacity = value; }
        public BuildingAbstract ParentBuilding { get => parentBuilding; set => parentBuilding = value; }
        public ProductData OutputProduct { get { return outputProduct; }
        set {
                //Debug.Log(this.unitName + " сменил " + OutputProduct.ItemName + " на " + value); 
                outputProduct = value;
            }
        }

        protected virtual void Awake()
        {
            if (ParentBuilding == null)
                ParentBuilding = GetComponentInParent<BuildingAbstract>();
        }

        public void Start()
        {
            ExtraInfo = UnitNameGenerator.GetRandomUnitName();
            UnitName += $" {UnitNameGenerator.GetRandomNumber()}";
        }

        public void AddProductInstance(ProductInstance newProduct)
        {
            if (!StoredItems.ContainsKey(newProduct.Data))
            {
                newProduct.Amount = Mathf.Clamp(newProduct.Amount, 0, MaxStorageCapacity);
                StoredItems.Add(newProduct.Data, newProduct);

                return;
            }    
                
            ProductInstance existing = StoredItems[newProduct.Data];

            int totalAmount = existing.Amount + newProduct.Amount;

            existing.Brand      = (existing.Brand * existing.Amount  + newProduct.Brand * newProduct.Amount) / totalAmount;
            existing.Quality    = (existing.Quality * existing.Amount + newProduct.Quality * newProduct.Amount) / totalAmount;
            existing.Amount = Mathf.Clamp(totalAmount, 0, MaxStorageCapacity);
        }

        public void RemoveProduct(ProductData item, int amount)
        {
            if (!StoredItems.ContainsKey(item))
                return;

            ProductInstance instance = StoredItems[item];

            instance.Amount -= amount;
            if(instance.Amount <= 0)
            {
                StoredItems.Remove(item);
            }
        }

        public void Normalize()
        {
            List<ProductData> keysToRemove = new List<ProductData>();

            foreach(var product in StoredItems)
            {

                if (StoredItems[product.Key].Amount <= 0)
                    keysToRemove.Add(product.Key);
            }

            foreach(var key in keysToRemove)
            {
                StoredItems.Remove(key);
            }

        }

        public virtual void Execute(BuildingUnit outputTarget)
        {
            int totalMaintenance = MaintenanceCost;
            ParentBuilding.Owner.Portfolio.funds -= totalMaintenance;
        }

        public void Transfer(BuildingUnit target)
        {
            if (target == null || StoredItems.Count == 0) return;

            var productKeys = new List<ProductData>(StoredItems.Keys);

            foreach (var productData in productKeys)
            {
                if (!StoredItems.TryGetValue(productData, out ProductInstance productInstance))
                    continue;

                if (productInstance.Amount <= 0)
                    continue;

                int targetAvailableSpace = target.MaxStorageCapacity;
                if (target.StoredItems.TryGetValue(productData, out ProductInstance targetProduct))
                {
                    targetAvailableSpace -= targetProduct.Amount;
                }

                int amountToTransfer = Mathf.Min(
                    productInstance.Amount,
                    OutputCapacity,
                    targetAvailableSpace
                );

                if (amountToTransfer <= 0)
                    continue;

                var transferProduct = new ProductInstance(
                    productData,
                    productInstance.Quality,
                    productInstance.Brand,
                    amountToTransfer
                );

                target.AddProductInstance(transferProduct);
                RemoveProduct(productData, amountToTransfer);

                //Debug.Log($"Передача {amountToTransfer} {productData.itemName} из {unitName} в {target.unitName}");
            }

            Normalize();
        }

        public int GetCurrentItemAmount(ProductData product)
        {
            if (StoredItems.TryGetValue(product, out ProductInstance instance))
                return instance.Amount;
            return 0;
        }

        public bool TryGetProductInstance(ProductData product, out ProductInstance productInstance)
        {
            productInstance = null;

            if (product == null)
            {
                //Debug.LogWarning("Попытка получить продукт с null-ссылкой");
                return false;
            }

            if (StoredItems.TryGetValue(product, out ProductInstance instance))
            {
                if (instance != null && instance.Amount > 0)
                {
                    productInstance = instance;
                    return true;
                }
            }

            return false;
        }

        public void TransferSelectedProduct(ProductData selectedProduct, BuildingUnit outputTarget)
        {
            if (!StoredItems.TryGetValue(selectedProduct, out ProductInstance productToTransfer))
                return;

            outputTarget.AddProductInstance(new ProductInstance(
                productToTransfer.Data,
                productToTransfer.Quality,
                productToTransfer.Brand,
                productToTransfer.Amount
            ));

            RemoveProduct(selectedProduct, productToTransfer.Amount);
        }

        public void ChangeWorkerLevel(int value)
        {
            int result = WorkersLevel + value;
            WorkersLevel = Mathf.Clamp(result, 0, MaxReasonableLevel);
        }

    public enum UnitType
    {
        sell,
        purchase,
        craft,
        produce,
        science,
        slot
    }
}
