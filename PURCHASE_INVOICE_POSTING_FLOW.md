# ما يحدث عند تثبيت فاتورة الشراء (Post Purchase Invoice)

## التدفق الكامل:

### 1. **استدعاء PostPurchaseInvoiceCommand**
```csharp
// من PurchaseInvoiceEndpoint
POST /api/PurchaseInvoice/PostPurchaseInvoice
```

### 2. **PostPurchaseInvoiceCommandHandler**
```csharp
public async Task<BaseResult> Handle(PostPurchaseInvoiceCommand request, CancellationToken cancellationToken)
{
    // 1. جلب فاتورة الشراء مع التفاصيل
    var entity = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
    
    // 2. التحقق من وجود الفاتورة
    if (entity is null) return Error("Invoice not found");
    
    // 3. تغيير حالة الفاتورة إلى Posted
    entity.SetStatus(PurchaseInvoiceStatus.Posted);
    
    // 4. تعديل المخزون
    await inventoryAdjustment.AdjustOnPurchasePostedAsync(entity, cancellationToken);
    
    // 5. حفظ التغييرات
    await unitOfWork.SaveChangesAsync();
    
    return BaseResult.Ok();
}
```

### 3. **InventoryAdjustmentService.AdjustOnPurchasePostedAsync**

#### الخطوات التفصيلية:

**أ) جلب البيانات:**
```csharp
// جلب معرفات المنتجات المميزة من تفاصيل الفاتورة
var productIds = GetDistinctProductIds(invoice);

// جلب معلومات المنتجات
var products = await LoadProductsAsync(productIds, cancellationToken);

// جلب أرصدة المنتجات الموجودة
var balances = await LoadBalancesAsync(productIds, cancellationToken);

// تجميع الكميات حسب المنتج
var totalByProduct = GroupTotalQuantities(invoice);
```

**ب) معالجة كل منتج:**
```csharp
foreach (var (productId, quantity) in totalByProduct)
{
    // التحقق من وجود المنتج
    if (!products.TryGetValue(productId, out var product)) continue;
    
    // زيادة رصيد المنتج
    await IncreaseProductBalanceAsync(balances, productId, quantity, cancellationToken);
}
```

### 4. **IncreaseProductBalanceAsync - المنطق الجديد**

#### الحالة الأولى: منتج جديد (لا يوجد رصيد)
```csharp
if (!cache.TryGetValue(productId, out var balance))
{
    // جلب سعر الوحدة من تفاصيل الفاتورة
    var unitPrice = await dbContext.Set<PurchaseInvoiceDetail>()
        .Where(d => d.ProductId == productId)
        .OrderByDescending(d => d.Id)
        .Select(d => d.UnitPrice)
        .FirstOrDefaultAsync(cancellationToken);
    
    // حساب القيمة الإجمالية
    var value = unitPrice * quantity;
    
    // إنشاء رصيد جديد
    balance = new ProductBalance(productId, quantity, quantity, value);
    await dbContext.Set<ProductBalance>().AddAsync(balance, cancellationToken);
}
```

#### الحالة الثانية: منتج موجود (يوجد رصيد)
```csharp
else
{
    // جلب أحدث سعر للوحدة
    var latestUnitPrice = await dbContext.Set<PurchaseInvoiceDetail>()
        .Where(d => d.ProductId == productId)
        .OrderByDescending(d => d.Id)
        .Select(d => d.UnitPrice)
        .FirstOrDefaultAsync(cancellationToken);
    
    // حساب القيمة المضافة
    var addValue = latestUnitPrice * quantity;
    
    // تحديث الرصيد الموجود
    balance.AdjustWithValue(quantity, quantity, addValue);
}
```

### 5. **ProductBalance.AdjustWithValue**
```csharp
public void AdjustWithValue(decimal qtyDelta, decimal availableDelta, decimal valueDelta)
{
    Qty += qtyDelta;           // زيادة الكمية الإجمالية
    AvailableQty += availableDelta;  // زيادة الكمية المتاحة
    TotalValue += valueDelta;   // زيادة القيمة الإجمالية
}
```

## النتيجة النهائية:

### للمنتجات الجاهزة:
- ✅ **Qty**: تزيد بالكمية المشتراة
- ✅ **AvailableQty**: تزيد بالكمية المشتراة (جاهزة للبيع)
- ✅ **TotalValue**: تزيد بقيمة الشراء

### للمنتجات الني (تحتاج تحميص):
- ✅ **Qty**: تزيد بالكمية المشتراة
- ✅ **AvailableQty**: تزيد بالكمية المشتراة (جاهزة للتحميص)
- ✅ **TotalValue**: تزيد بقيمة الشراء

### للمنتجات المحمصة:
- ❌ **لا تدخل في فاتورة الشراء** (يتم التحقق في CreatePurchaseInvoiceCommandHandler)

## مثال عملي:

### فاتورة شراء تحتوي على:
```
1. بسكوت: 10 كيلو × 15 دينار = 150 دينار
2. بزر ابيض ني: 20 كيلو × 25 دينار = 500 دينار
```

### النتيجة في ProductBalance:
```
بسكوت:
- Qty: 10
- AvailableQty: 10
- TotalValue: 150

بزر ابيض ني:
- Qty: 20
- AvailableQty: 20
- TotalValue: 500
```

## المميزات الجديدة:

1. **جدول موحد**: جميع المنتجات في `ProductBalances`
2. **تتبع القيمة**: حساب القيمة الإجمالية للمخزون
3. **الكمية المتاحة**: تتبع الكمية المتاحة للبيع/التحميص
4. **منع الأخطاء**: المنتجات المحمصة لا تدخل في فاتورة الشراء
5. **مرونة في الأسعار**: استخدام أحدث سعر للوحدة
