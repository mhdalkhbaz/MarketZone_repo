# 📊 هيكل مشروع MarketZone - تقسيم تفصيلي

---

## 1️⃣ **المنتجات (Products)**

### 📋 **Entity: Product**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **CategoryId** | ✅ مطلوب |
| **Name** | ✅ مطلوب |
| **Description** | |
| **UnitOfMeasure** | |
| **PurchasePrice** | |
| **SalePrice** | |
| **MinStockLevel** | |
| **IsActive** | |
| **NeedsRoasting** | |
| **RoastingCost** | |
| **RawProductId** | |
| **CommissionPerKg** | |
| **BarCode** | |

#### **العلاقات:**
- **Category** → علاقة Many-to-One مع Category
- **RawProduct** → علاقة Many-to-One مع Product (للربط بين المنتج الخام والمحضر)

---

### 🔌 **APIs المتاحة:**

#### **1. GET `/api/Product` - GetPagedListProduct**
- **الوصف:** الحصول على قائمة المنتجات بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
  - `SearchText` (string) - البحث في الاسم
- **التأثير:** عرض المنتجات فقط (قراءة فقط)

---

#### **2. GET `/api/Product/{id}` - GetProductById**
- **الوصف:** الحصول على منتج محدد
- **الطريقة:** GET
- **المعاملات:**
  - `id` (long) - معرف المنتج
- **التأثير:** عرض منتج واحد فقط (قراءة فقط)

---

#### **3. POST `/api/Product` - CreateProduct**
- **الوصف:** إنشاء منتج جديد
- **الطريقة:** POST
- **المعاملات:**
  - `CategoryId` (long?) - معرف الفئة (افتراضي: 1)
  - `Name` (string) - ✅ **مطلوب**
  - `Description` (string) - الوصف
  - `UnitOfMeasure` (string) - وحدة القياس (افتراضي: "kg")
  - `PurchasePrice` (decimal?) - سعر الشراء
  - `SalePrice` (decimal?) - سعر البيع
  - `MinStockLevel` (decimal) - الحد الأدنى للمخزون (افتراضي: 5)
  - `IsActive` (bool) - نشط (افتراضي: true)
  - `NeedsRoasting` (bool) - يحتاج تحميص (افتراضي: false)
  - `RoastingCost` (decimal?) - تكلفة التحميص
  - `BarCode` (string) - الباركود
  - `BarCode2` (string) - باركود إضافي
  - `RawProductId` (long?) - معرف المنتج الخام
  - `CommissionPerKg` (decimal?) - العمولة لكل كيلو
- **التأثير:**
  - ✅ إضافة منتج جديد في قاعدة البيانات
  - ✅ إنشاء سجل في `InventoryHistory` (إذا لزم الأمر)
  - ✅ إنشاء `ProductBalance` جديد برصيد صفر

---

#### **4. PUT `/api/Product` - UpdateProduct**
- **الوصف:** تحديث منتج موجود
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
  - جميع الحقول الأخرى (نفس CreateProduct)
- **التأثير:**
  - ✅ تحديث بيانات المنتج في قاعدة البيانات
  - ✅ تحديث `LastModified` و `LastModifiedBy`

---

#### **5. DELETE `/api/Product/{id}` - DeleteProduct**
- **الوصف:** حذف منتج
- **الطريقة:** DELETE
- **المعاملات:**
  - `id` (long) - معرف المنتج
- **التأثير:**
  - ✅ حذف المنتج من قاعدة البيانات
  - ⚠️ قد يفشل إذا كان المنتج مستخدماً في فواتير أو رحلات

---

### 🔗 **APIs إضافية في Lookup:**

#### **6. GET `/api/Lookup/GetProductSelectList`**
- **الوصف:** قائمة اختيار بجميع المنتجات
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار

---

#### **7. GET `/api/Lookup/GetProductSelectListForDistribution`**
- **الوصف:** قائمة منتجات للتوزيع (مع الكميات المتاحة)
- **الطريقة:** GET
- **المعاملات:**
  - `TripId` (long?) - معرف الرحلة
- **التأثير:** قراءة فقط - عرض المنتجات المتاحة للتوزيع

---

#### **8. GET `/api/Lookup/GetProductReadyByRawProductSelectList`**
- **الوصف:** قائمة المنتجات المحضرة من منتج خام معين
- **الطريقة:** GET
- **المعاملات:**
  - `productId` (long) - معرف المنتج الخام
- **التأثير:** قراءة فقط - عرض المنتجات المرتبطة

---

#### **9. GET `/api/Lookup/GetUnroastedProducts`**
- **الوصف:** قائمة المنتجات التي تحتاج تحميص
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض منتجات التحميص

---

#### **10. GET `/api/Lookup/GetAllProductsForPurchase`**
- **الوصف:** قائمة المنتجات الخام (للشراء)
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض المنتجات الخام

---

#### **11. GET `/api/Lookup/GetInStockProductSelectList`**
- **الوصف:** قائمة المنتجات المتوفرة في المخزون
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض المنتجات المتوفرة

---

#### **12. GET `/api/Lookup/GetUnroastedProductsWithQty`**
- **الوصف:** قائمة المنتجات الخام مع الكميات غير المحمصة
- **الطريقة:** GET
- **المصادقة:** ❌ لا
- **النتيجة:** قائمة `UnroastedProductDto` مع الكميات
- **التأثير:** قراءة فقط - عرض المنتجات الخام مع الأرصدة

---

### 📊 **ملخص التأثيرات:**

| العملية | التأثير على قاعدة البيانات | التأثيرات الأخرى |
|---------|---------------------------|------------------|
| **CreateProduct** | إضافة سجل جديد في `Products` | إنشاء `ProductBalance` جديد |
| **UpdateProduct** | تحديث سجل في `Products` | تحديث `LastModified` |
| **DeleteProduct** | حذف سجل من `Products` | قد يفشل إذا كان مستخدماً |
| **GetPagedListProduct** | قراءة فقط | - |
| **GetProductById** | قراءة فقط | - |
| **GetProductSelectList** | قراءة فقط | - |

---

### 🔄 **التكامل مع الوحدات الأخرى:**

- **Purchases:** يتم استخدام المنتجات في فواتير الشراء
- **Sales:** يتم استخدام المنتجات في فواتير البيع
- **Roasting:** المنتجات الخام والمحضرة للتحميص
- **Logistics:** المنتجات في رحلات التوزيع
- **Inventory:** تتبع حركة المخزون للمنتجات

---

**ملاحظة:** هذا مثال عن المنتجات. سيتم إضافة باقي الوحدات بنفس التفصيل لاحقاً.
