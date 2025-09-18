# تحليل الماستر داتا - المنتجات (Products)

## القسم الأول: بنية المنتج (Product Entity)

### حقول المنتج الأساسية:

| الحقل | النوع | الوصف | ملاحظات |
|-------|------|--------|----------|
| `Id` | long | المعرف الفريد | Auto-generated |
| `CategoryId` | long | معرف الفئة | مطلوب - Foreign Key إلى Category |
| `Name` | string | اسم المنتج | مطلوب - الحد الأقصى 100 حرف |
| `Description` | string | وصف المنتج | اختياري - الحد الأقصى 500 حرف |
| `UnitOfMeasure` | string | وحدة القياس | افتراضي "kg" - الحد الأقصى 100 حرف |
| `PurchasePrice` | decimal? | سعر الشراء | اختياري - رقم عشري (18,2) |
| `SalePrice` | decimal? | سعر البيع | اختياري - رقم عشري (18,2) |
| `MinStockLevel` | decimal | الحد الأدنى للمخزون | افتراضي 5 - رقم عشري (18,2) |
| `IsActive` | bool | حالة النشاط | افتراضي true |
| `NeedsRoasting` | bool | يحتاج تحميص | افتراضي false |
| `RoastingCost` | decimal? | تكلفة التحميص | اختياري - رقم عشري (18,2) |
| `BarCode` | string | الباركود | اختياري - الحد الأقصى 50 حرف |
| `BarCode2` | string | الباركود الثاني | اختياري |
| `RawProductId` | long? | معرف المنتج الخام | للربط مع المنتج المحمص |
| `CommissionPerKg` | decimal? | العمولة لكل كيلو | اختياري - رقم عشري (18,2) |

### العلاقات (Relationships):

1. **مع الفئة (Category)**: `Product.CategoryId → Category.Id`
   - علاقة Many-to-One
   - Delete Behavior: Restrict

2. **مع المنتج الخام (Self-Reference)**: `Product.RawProductId → Product.Id`
   - علاقة Self-Reference للربط بين المنتج الني والمحمص

## القسم الثاني: العمليات المرتبطة بالمنتجات وتأثيرها على الجداول الأخرى

### 1. عمليات الشراء (Purchase Operations)

**الجداول المتأثرة:**
- `PurchaseInvoiceDetail` - تفاصيل فاتورة الشراء
- `ProductBalance` - رصيد المنتج في المخزون
- `InventoryHistory` - تاريخ المخزون

**التأثير:**
```csharp
// عند إضافة فاتورة شراء جديدة
PurchaseInvoiceDetail → ProductBalance.Qty += quantity
PurchaseInvoiceDetail → InventoryHistory (log entry)
```

### 2. عمليات البيع (Sales Operations)

**الجداول المتأثرة:**
- `SalesInvoiceDetail` - تفاصيل فاتورة البيع
- `ProductBalance` - رصيد المنتج في المخزون
- `InventoryHistory` - تاريخ المخزون

**التأثير:**
```csharp
// عند إضافة فاتورة بيع جديدة
SalesInvoiceDetail → ProductBalance.AvailableQty -= quantity
SalesInvoiceDetail → InventoryHistory (log entry)
```

### 3. عمليات التحميص (Roasting Operations)

**الجداول المتأثرة:**
- `RoastingOperation` - عملية التحميص
- `ProductBalance` (المنتج الني) - تقليل الكمية
- `ProductBalance` (المنتج المحمص) - زيادة الكمية
- `InventoryHistory` - تاريخ المخزون

**التأثير:**
```csharp
// عند تنفيذ عملية تحميص
RoastingOperation → RawProduct.Balance -= quantity
RoastingOperation → RoastedProduct.Balance += quantity
RoastingOperation → InventoryHistory (log entries)
```

### 4. عمليات التوزيع (Distribution Operations)

**الجداول المتأثرة:**
- `DistributionTripDetail` - تفاصيل رحلة التوزيع
- `ProductBalance` - رصيد المنتج في المخزون

**التأثير:**
```csharp
// عند إرسال منتجات للتوزيع
DistributionTripDetail → ProductBalance.AvailableQty -= quantity
```

## القسم الثالث: منطق البزنس للمنتجات

### أنواع المنتجات:

#### 1. المنتج الجاهز (Ready Product)
- **الوصف**: منتجات جاهزة للاستهلاك مباشرة (مثل البسكوت)
- **التسعير**:
  - `PurchasePrice`: مطلوب - سعر الشراء من المورد
  - `SalePrice`: مطلوب - سعر البيع للعميل
- **الخصائص**:
  - `NeedsRoasting`: false
  - `RoastingCost`: null
  - `RawProductId`: null

#### 2. المنتج الني (Raw Product - Needs Roasting)
- **الوصف**: منتجات خام تحتاج إلى عملية تحميص (مثل البزر الأبيض الني)
- **التسعير**:
  - `PurchasePrice`: مطلوب - سعر الشراء من المورد
  - `SalePrice`: null - لا يباع مباشرة، يحتاج تحميص أولاً
- **الخصائص**:
  - `NeedsRoasting`: true
  - `RoastingCost`: مطلوب - تكلفة التحميص لكل كيلو
  - `RawProductId`: null

#### 3. المنتج المحمص (Roasted Product)
- **الوصف**: منتجات خرجت من عملية التحميص (مثل البزر الأبيض المحمص)
- **التسعير**:
  - `PurchasePrice`: null - لا يوجد شراء، منتج داخلي
  - `SalePrice`: null - يحدد لاحقاً حسب السوق والتكلفة
- **الخصائص**:
  - `NeedsRoasting`: false
  - `RoastingCost`: null
  - `RawProductId`: مطلوب - يشير إلى المنتج الني الأصلي

### منطق التسعير التفصيلي:

#### للمنتج الجاهز:
```
سعر الشراء: 15.00
سعر البيع: 20.00
هامش الربح: 5.00 (33.33%)
```

#### للمنتج الني:
```
سعر الشراء: 25.50
تكلفة التحميص: 2.00
إجمالي التكلفة: 27.50
```

#### للمنتج المحمص:
```
التكلفة الأساسية: 27.50 (من المنتج الني)
العمولة: 1.00
إجمالي التكلفة: 28.50
سعر البيع: يحدد لاحقاً (عادة 35-40)
```

### العمليات المالية المرتبطة:

1. **عند الشراء**: تحديث `PurchasePrice` وزيادة `ProductBalance`
2. **عند التحميص**: نقل الكمية من المنتج الني للمحمص مع حساب التكلفة
3. **عند البيع**: تحديث `SalePrice` وتقليل `ProductBalance`
4. **حساب التكلفة**: `RawProduct.PurchasePrice + RoastingCost + CommissionPerKg`

## القسم الرابع: التوصيات والتطوير

### التحسينات المقترحة:

1. **إضافة حقل نوع المنتج**: `ProductType` enum (Ready, Raw, Roasted)
2. **تحسين العلاقات**: إضافة navigation properties للعلاقات
3. **إضافة validations**: قواعد عمل للتسعير حسب نوع المنتج
4. **تتبع التكلفة**: إضافة حقول لتتبع التكلفة الفعلية
5. **تحسين الباركود**: إضافة فحص للباركود المكرر

### قواعد العمل المقترحة:

```csharp
public enum ProductType
{
    Ready = 1,      // جاهز
    Raw = 2,        // ني
    Roasted = 3     // محمص
}

// قواعد العمل
- المنتج الجاهز: PurchasePrice و SalePrice مطلوبان
- المنتج الني: PurchasePrice و RoastingCost مطلوبان
- المنتج المحمص: RawProductId مطلوب، الأسعار تحدث لاحقاً
```

---

**ملاحظة**: هذا التحليل مبني على الكود الحالي في المشروع وقد يحتاج إلى تحديثات حسب متطلبات العمل الجديدة.
