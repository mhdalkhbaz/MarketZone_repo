# دليل اختبار شامل: رحلة التوزيع وفواتير المبيعات الموزع

## 📋 نظرة عامة

هذا الدليل يوضح كيفية اختبار سير العمل الكامل لرحلة التوزيع وفواتير المبيعات الموزع باستخدام Postman Collection.

## 🎯 الهدف من الاختبار

اختبار التكامل الكامل بين:
- **رحلة التوزيع** (Distribution Trip)
- **فواتير المبيعات الموزع** (Distributor Sales Invoices)
- **إدارة المخزون** (Inventory Management)
- **API قائمة المنتجات** (Product SelectList API)

## 📊 سيناريو الاختبار الكامل

### **البيانات المطلوبة:**
- موظف (Employee)
- سيارة (Car)
- منطقة (Region)
- عميل (Customer)
- منتج جاهز للبيع (Product)
- رصيد المنتج (Product Balance)

### **سير العمل:**
1. إنشاء رحلة توزيع
2. ترحيل الرحلة
3. استلام البضاعة
4. إنشاء فواتير مبيعات موزع
5. التحقق من الكميات
6. إكمال الرحلة

## 🔧 خطوات الاختبار

### **المرحلة 1: إعداد البيانات الأساسية**

#### **1.1 إنشاء موظف**
```json
POST /api/Employee/CreateEmployee
{
  "firstName": "أحمد",
  "lastName": "محمد",
  "phone": "0123456789",
  "email": "ahmed@marketzone.com",
  "address": "القاهرة",
  "isActive": true
}
```
**النتيجة المتوقعة:** ID = 1

#### **1.2 إنشاء سيارة**
```json
POST /api/Car/CreateCar
{
  "name": "سيارة توزيع 1",
  "plateNumber": "أ ب ج 123",
  "model": "2023",
  "year": 2023,
  "capacityKg": 1000,
  "isAvailable": true
}
```
**النتيجة المتوقعة:** ID = 1

#### **1.3 إنشاء منطقة**
```json
POST /api/Region/CreateRegion
{
  "name": "المنطقة الشمالية",
  "description": "تغطي شمال القاهرة",
  "isActive": true
}
```
**النتيجة المتوقعة:** ID = 1

#### **1.4 إنشاء عميل**
```json
POST /api/Customer/CreateCustomer
{
  "name": "محل البن الطازج",
  "phone": "0987654321",
  "email": "customer1@example.com",
  "address": "شارع النيل، القاهرة",
  "whatsAppPhone": "0987654321"
}
```
**النتيجة المتوقعة:** ID = 1

#### **1.5 إنشاء منتج جاهز للبيع**
```json
POST /api/Product/CreateProduct
{
  "categoryId": 1,
  "name": "بن أرابيكا محمص",
  "description": "بن أرابيكا محمص عالي الجودة",
  "unitOfMeasure": "كيلو",
  "purchasePrice": 20.00,
  "salePrice": 25.50,
  "minStockLevel": 5,
  "isActive": true,
  "needsRoasting": false,
  "roastingCost": null,
  "barCode": "123456789"
}
```
**النتيجة المتوقعة:** ID = 1

#### **1.6 إنشاء رصيد المنتج**
```json
POST /api/ProductBalance/CreateProductBalance
{
  "productId": 1,
  "qty": 200,
  "availableQty": 200
}
```
**النتيجة المتوقعة:** ID = 1

---

### **المرحلة 2: سير عمل رحلة التوزيع**

#### **2.1 إنشاء رحلة توزيع جديدة**
```json
POST /api/DistributionTrip/CreateDistributionTrip
{
  "employeeId": 1,
  "carId": 1,
  "regionId": 1,
  "tripDate": "2025-01-15",
  "loadQty": 100,
  "notes": "رحلة توزيع للمنطقة الشمالية - اختبار شامل",
  "details": [
    {
      "productId": 1,
      "qty": 100,
      "expectedPrice": 25.50
    }
  ]
}
```

**النتيجة المتوقعة:**
- رحلة توزيع جديدة برقم ID
- حالة الرحلة: `Draft`
- `SoldQty` = 0
- `ReturnedQty` = 0
- **AvailableQty في ProductBalance نقصت من 200 إلى 100**

#### **2.2 التحقق من رحلة التوزيع بعد الإنشاء**
```json
GET /api/DistributionTrip/GetPagedListDistributionTrip?PageNumber=1&PageSize=10
```

**النتيجة المتوقعة:**
```json
{
  "data": [
    {
      "id": 1,
      "employeeId": 1,
      "carId": 1,
      "regionId": 1,
      "tripDate": "2025-01-15T00:00:00",
      "loadQty": 100,
      "notes": "رحلة توزيع للمنطقة الشمالية - اختبار شامل",
      "status": "Draft",
      "details": [
        {
          "id": 1,
          "productId": 1,
          "qty": 100,
          "expectedPrice": 25.50,
          "returnedQty": 0,
          "soldQty": 0
        }
      ]
    }
  ]
}
```

#### **2.3 التحقق من رصيد المنتج بعد إنشاء الرحلة**
```json
GET /api/ProductBalance/GetByProductId/1
```

**النتيجة المتوقعة:**
```json
{
  "id": 1,
  "productId": 1,
  "qty": 200,        // لم يتغير
  "availableQty": 100 // نقص من 200 إلى 100
}
```

#### **2.4 ترحيل الرحلة**
```json
PUT /api/DistributionTrip/PostDistributionTrip
{
  "id": 1
}
```

**النتيجة المتوقعة:**
- حالة الرحلة: `Posted`
- الرحلة جاهزة للتنفيذ

#### **2.5 استلام البضاعة (تسجيل المرتجعات)**
```json
PUT /api/DistributionTrip/ReceiveGoods
{
  "tripId": 1,
  "details": [
    {
      "detailId": 1,
      "returnedQty": 15
    }
  ]
}
```

**النتيجة المتوقعة:**
- حالة الرحلة: `GoodsReceived`
- `ReturnedQty` = 15
- `SoldQty` = 0 (لم يتم إنشاء فواتير بعد)

---

### **المرحلة 3: إنشاء فواتير المبيعات الموزع**

#### **3.1 إنشاء فاتورة مبيعات موزع (العميل الأول)**
```json
POST /api/SalesInvoice/CreateSalesInvoice
{
  "invoiceNumber": "INV-DIST-001",
  "customerId": 1,
  "invoiceDate": "2025-01-15",
  "totalAmount": 765.00,
  "discount": 0,
  "paymentMethod": "نقدي",
  "notes": "فاتورة توزيع - العميل الأول",
  "type": "Distributor",
  "distributionTripId": 1,
  "details": [
    {
      "productId": 1,
      "quantity": 30,
      "unitPrice": 25.50,
      "subTotal": 765.00,
      "notes": "طلب عادي"
    }
  ]
}
```

**النتيجة المتوقعة:**
- فاتورة مبيعات جديدة برقم ID
- `SoldQty` للمنتج في رحلة التوزيع = 30
- الفاتورة مرتبطة برحلة التوزيع

#### **3.2 التحقق من رحلة التوزيع بعد الفاتورة الأولى**
```json
GET /api/DistributionTrip/GetPagedListDistributionTrip?PageNumber=1&PageSize=10
```

**النتيجة المتوقعة:**
```json
{
  "data": [
    {
      "id": 1,
      "status": "GoodsReceived",
      "details": [
        {
          "id": 1,
          "productId": 1,
          "qty": 100,
          "expectedPrice": 25.50,
          "returnedQty": 15,
          "soldQty": 30  // تم تحديثه
        }
      ]
    }
  ]
}
```

#### **3.3 إنشاء فاتورة مبيعات موزع (العميل الثاني)**
```json
POST /api/SalesInvoice/CreateSalesInvoice
{
  "invoiceNumber": "INV-DIST-002",
  "customerId": 1,
  "invoiceDate": "2025-01-15",
  "totalAmount": 637.50,
  "discount": 0,
  "paymentMethod": "شيك",
  "notes": "فاتورة توزيع - العميل الثاني",
  "type": "Distributor",
  "distributionTripId": 1,
  "details": [
    {
      "productId": 1,
      "quantity": 25,
      "unitPrice": 25.50,
      "subTotal": 637.50,
      "notes": "طلب عاجل"
    }
  ]
}
```

**النتيجة المتوقعة:**
- فاتورة مبيعات جديدة برقم ID
- `SoldQty` للمنتج في رحلة التوزيع = 55 (30 + 25)
- الفاتورة مرتبطة برحلة التوزيع

---

### **المرحلة 4: التحقق والتحليل**

#### **4.1 التحقق من الكميات في رحلة التوزيع**
```json
GET /api/DistributionTrip/ValidateTripQuantities?TripId=1
```

**النتيجة المتوقعة:**
```json
{
  "succeeded": true,
  "data": {
    "isValid": true,
    "errors": [],
    "warnings": []
  }
}
```

#### **4.2 التحقق من رحلة التوزيع النهائية**
```json
GET /api/DistributionTrip/GetPagedListDistributionTrip?PageNumber=1&PageSize=10
```

**النتيجة المتوقعة:**
```json
{
  "data": [
    {
      "id": 1,
      "status": "GoodsReceived",
      "details": [
        {
          "id": 1,
          "productId": 1,
          "qty": 100,        // محمل
          "expectedPrice": 25.50,
          "returnedQty": 15,  // مرتجع
          "soldQty": 55       // مباع (30 + 25)
        }
      ]
    }
  ]
}
```

#### **4.3 التحقق من فواتير المبيعات**
```json
GET /api/SalesInvoice/GetPagedListSalesInvoice?PageNumber=1&PageSize=10
```

**النتيجة المتوقعة:**
```json
{
  "data": [
    {
      "id": 1,
      "invoiceNumber": "INV-DIST-001",
      "customerId": 1,
      "type": "Distributor",
      "distributionTripId": 1,
      "totalAmount": 765.00,
      "status": "Draft"
    },
    {
      "id": 2,
      "invoiceNumber": "INV-DIST-002",
      "customerId": 1,
      "type": "Distributor",
      "distributionTripId": 1,
      "totalAmount": 637.50,
      "status": "Draft"
    }
  ]
}
```

---

### **المرحلة 5: إكمال رحلة التوزيع**

#### **5.1 إكمال رحلة التوزيع**
```json
PUT /api/DistributionTrip/CompleteTrip
{
  "tripId": 1,
  "details": [
    {
      "detailId": 1,
      "returnedQty": 15
    }
  ]
}
```

**النتيجة المتوقعة:**
- حالة الرحلة: `Completed`
- الرحلة مكتملة بنجاح

#### **5.2 التحقق من رصيد المنتج بعد إكمال الرحلة**
```json
GET /api/ProductBalance/GetByProductId/1
```

**النتيجة المتوقعة:**
```json
{
  "id": 1,
  "productId": 1,
  "qty": 145,        // 200 - 55 (مباع)
  "availableQty": 115 // 100 + 15 (مرتجع)
}
```

---

### **المرحلة 6: اختبار API قائمة المنتجات**

#### **6.1 الحصول على المنتجات الجاهزة للبيع**
```json
GET /api/Lookup/GetProductSelectListForDistribution
```

**النتيجة المتوقعة:**
```json
{
  "succeeded": true,
  "data": [
    {
      "value": "1",
      "label": "بن أرابيكا محمص",
      "qty": 115.00  // الكمية المتاحة
    }
  ]
}
```

#### **6.2 الحصول على المنتجات المتبقية في رحلة التوزيع**
```json
GET /api/Lookup/GetProductSelectListForDistribution?DistributionTripId=1
```

**النتيجة المتوقعة:**
```json
{
  "succeeded": true,
  "data": [
    {
      "value": "1",
      "label": "بن أرابيكا محمص",
      "qty": 30.00  // 100 - 55 - 15 = 30 متبقي
    }
  ]
}
```

---

## 🚨 اختبار الحالات الاستثنائية

### **1. محاولة إنشاء فاتورة موزع لرحلة في حالة Draft**
**النتيجة المتوقعة:** خطأ - "Cannot create distributor invoice for trip that is not in GoodsReceived status"

### **2. محاولة إنشاء فاتورة موزع بكمية تتجاوز المحملة**
**النتيجة المتوقعة:** خطأ - "Sold quantity cannot exceed loaded quantity"

### **3. محاولة إنشاء فاتورة موزع لرحلة غير موجودة**
**النتيجة المتوقعة:** خطأ - "Distribution trip not found"

---

## 📊 ملخص النتائج المتوقعة

### **في نهاية الاختبار:**

| العنصر | القيمة الأولية | القيمة النهائية | التغيير |
|--------|----------------|-----------------|---------|
| **ProductBalance.Qty** | 200 | 145 | -55 (مباع) |
| **ProductBalance.AvailableQty** | 200 | 115 | -85 (محمل) + 15 (مرتجع) |
| **DistributionTrip.Status** | Draft | Completed | ✅ مكتملة |
| **DistributionTripDetail.SoldQty** | 0 | 55 | +55 (من فواتير) |
| **DistributionTripDetail.ReturnedQty** | 0 | 15 | +15 (مرتجع) |
| **فواتير المبيعات** | 0 | 2 | +2 فاتورة موزع |

### **التحقق من صحة البيانات:**
- ✅ الكمية المحملة = المباعة + المرجعة + المتبقية (100 = 55 + 15 + 30)
- ✅ المخزون محدث بشكل صحيح
- ✅ الفواتير مرتبطة برحلة التوزيع
- ✅ الكميات المباعة محدثة تلقائياً
- ✅ API قائمة المنتجات يعمل بشكل صحيح

---

## 🎯 معايير النجاح

### **1. سير العمل:**
- ✅ إنشاء رحلة توزيع بنجاح
- ✅ ترحيل الرحلة بنجاح
- ✅ استلام البضاعة بنجاح
- ✅ إنشاء فواتير المبيعات بنجاح
- ✅ تحديث الكميات المباعة تلقائياً
- ✅ التحقق من الكميات بنجاح
- ✅ إكمال الرحلة بنجاح

### **2. التحقق من الأخطاء:**
- ✅ التحقق من وجود البيانات الأساسية
- ✅ التحقق من الكميات المتاحة
- ✅ التحقق من حالة الرحلة في كل خطوة
- ✅ التحقق من الكميات المباعة والمرجعة
- ✅ معالجة الحالات الاستثنائية

### **3. البيانات:**
- ✅ `SoldQty` تُحدث تلقائياً عند إنشاء فواتير المبيعات
- ✅ `ReturnedQty` تُخزن في قاعدة البيانات
- ✅ العلاقات بين الكيانات تعمل بشكل صحيح
- ✅ التحقق من الكميات يعمل بشكل صحيح
- ✅ ProductBalance يتم تحديثه بشكل صحيح
- ✅ API قائمة المنتجات يعمل بشكل صحيح

---

*تم إعداد هذا الدليل لاختبار شامل لرحلة التوزيع وفواتير المبيعات الموزع*
