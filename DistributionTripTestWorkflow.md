# اختبار سير عمل رحلات التوزيع - MarketZone

## 🧪 سيناريو الاختبار الشامل

### **البيانات المطلوبة للاختبار:**

#### **1. الموظفين (Employees)**
```json
POST /api/Employee/CreateEmployee
{
  "name": "أحمد محمد",
  "phone": "0123456789",
  "email": "ahmed@marketzone.com",
  "address": "القاهرة"
}
```

#### **2. السيارات (Cars)**
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

#### **3. المناطق (Regions)**
```json
POST /api/Region/CreateRegion
{
  "name": "المنطقة الشمالية",
  "description": "تغطي شمال القاهرة",
  "isActive": true
}
```

#### **4. العملاء (Customers)**
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

#### **5. المنتجات (Products)**
```json
POST /api/Product/CreateProduct
{
  "name": "بن أرابيكا",
  "description": "بن أرابيكا عالي الجودة",
  "categoryId": 1,
  "unitOfMeasure": "كيلو",
  "barCode": "123456789",
  "barCode2": "987654321"
}
```

---

## 🔄 سير العمل الكامل للاختبار

### **الخطوة 1: إنشاء رحلة توزيع جديدة**

```bash
POST /api/DistributionTrip/CreateDistributionTrip
```

```json
{
  "employeeId": 1,
  "carId": 1,
  "regionId": 1,
  "tripDate": "2025-01-15",
  "loadQty": 500,
  "notes": "رحلة توزيع للمنطقة الشمالية - اختبار شامل",
  "details": [
    {
      "productId": 1,
      "qty": 100,
      "expectedPrice": 25.50
    },
    {
      "productId": 2,
      "qty": 50,
      "expectedPrice": 30.00
    }
  ]
}
```

**النتيجة المتوقعة:**
- رحلة توزيع جديدة برقم ID
- حالة الرحلة: `Draft` (مسودة)
- `SoldQty` = 0 لجميع المنتجات
- `ReturnedQty` = 0 لجميع المنتجات

---

### **الخطوة 2: ترحيل الرحلة**

```bash
PUT /api/DistributionTrip/PostDistributionTrip
```

```json
{
  "id": 1
}
```

**النتيجة المتوقعة:**
- حالة الرحلة: `Posted` (مرحلة)
- الرحلة جاهزة للتنفيذ

---

### **الخطوة 3: استلام البضاعة (تسجيل المرتجعات)**

```bash
PUT /api/DistributionTrip/ReceiveGoods
```

```json
{
  "tripId": 1,
  "details": [
    {
      "detailId": 1,
      "returnedQty": 15
    },
    {
      "detailId": 2,
      "returnedQty": 5
    }
  ]
}
```

**النتيجة المتوقعة:**
- حالة الرحلة: `GoodsReceived` (تم استلام البضاعة)
- `ReturnedQty` = 15 للمنتج الأول
- `ReturnedQty` = 5 للمنتج الثاني
- `SoldQty` = 0 (لم يتم إنشاء فواتير بعد)

---

### **الخطوة 4: إنشاء فاتورة مبيعات موزع (العميل الأول)**

```bash
POST /api/SalesInvoice/CreateSalesInvoice
```

```json
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
- `SoldQty` للمنتج الأول في رحلة التوزيع = 30
- `SoldQty` للمنتج الثاني = 0

---

### **الخطوة 5: إنشاء فاتورة مبيعات موزع (العميل الثاني)**

```bash
POST /api/SalesInvoice/CreateSalesInvoice
```

```json
{
  "invoiceNumber": "INV-DIST-002",
  "customerId": 2,
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
- `SoldQty` للمنتج الأول في رحلة التوزيع = 55 (30 + 25)
- `SoldQty` للمنتج الثاني = 0

---

### **الخطوة 6: التحقق من الكميات**

```bash
GET /api/DistributionTrip/ValidateTripQuantities?TripId=1
```

**النتيجة المتوقعة:**
```json
{
  "isValid": true,
  "errors": [],
  "warnings": [
    {
      "detailId": 2,
      "productName": "بن أرابيكا",
      "warningMessage": "لم يتم تسجيل أي مبيعات أو مرتجعات لهذا المنتج"
    }
  ]
}
```

---

### **الخطوة 7: إكمال الرحلة**

```bash
PUT /api/DistributionTrip/CompleteTrip
```

```json
{
  "tripId": 1
}
```

**النتيجة المتوقعة:**
- حالة الرحلة: `Completed` (مكتملة)
- الرحلة مكتملة بنجاح

---

## 📊 التحقق من النتائج

### **التحقق من رحلة التوزيع:**

```bash
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
      "loadQty": 500,
      "notes": "رحلة توزيع للمنطقة الشمالية - اختبار شامل",
      "status": "Completed",
      "details": [
        {
          "id": 1,
          "productId": 1,
          "qty": 100,
          "expectedPrice": 25.50,
          "returnedQty": 15,
          "soldQty": 55
        },
        {
          "id": 2,
          "productId": 2,
          "qty": 50,
          "expectedPrice": 30.00,
          "returnedQty": 5,
          "soldQty": 0
        }
      ]
    }
  ]
}
```

### **التحقق من فواتير المبيعات:**

```bash
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
      "customerId": 2,
      "type": "Distributor",
      "distributionTripId": 1,
      "totalAmount": 637.50,
      "status": "Draft"
    }
  ]
}
```

---

## ✅ معايير النجاح

### **1. سير العمل:**
- ✅ إنشاء رحلة توزيع بنجاح
- ✅ ترحيل الرحلة بنجاح
- ✅ استلام البضاعة بنجاح
- ✅ إنشاء فواتير المبيعات بنجاح
- ✅ تحديث `SoldQty` تلقائياً
- ✅ التحقق من الكميات بنجاح
- ✅ إكمال الرحلة بنجاح

### **2. البيانات:**
- ✅ `SoldQty` تُحدث تلقائياً عند إنشاء فواتير المبيعات
- ✅ `ReturnedQty` تُخزن في قاعدة البيانات
- ✅ العلاقات بين الكيانات تعمل بشكل صحيح
- ✅ التحقق من الكميات يعمل بشكل صحيح

### **3. API Endpoints:**
- ✅ جميع الـ Endpoints تعمل بشكل صحيح
- ✅ التحقق من الصلاحيات يعمل
- ✅ معالجة الأخطاء تعمل

---

## 🚨 اختبار الحالات الاستثنائية

### **1. محاولة إنشاء فاتورة موزع لرحلة غير موجودة:**
```json
{
  "distributionTripId": 999,
  "type": "Distributor"
}
```
**النتيجة المتوقعة:** خطأ 404 - رحلة التوزيع غير موجودة

### **2. محاولة إنشاء فاتورة موزع لرحلة في حالة Draft:**
```json
{
  "distributionTripId": 1,
  "type": "Distributor"
}
```
**النتيجة المتوقعة:** خطأ - لا يمكن إنشاء فاتورة موزع لرحلة في حالة Draft

### **3. محاولة استلام البضاعة لرحلة في حالة Draft:**
```json
{
  "tripId": 1
}
```
**النتيجة المتوقعة:** خطأ - لا يمكن استلام البضاعة لرحلة في حالة Draft

---

## 📝 ملاحظات الاختبار

1. **تأكد من وجود البيانات الأساسية** قبل بدء الاختبار
2. **احفظ الـ IDs** المستخدمة في الاختبار للتحقق منها
3. **اختبر الحالات الاستثنائية** للتأكد من معالجة الأخطاء
4. **تحقق من قاعدة البيانات** مباشرة للتأكد من صحة البيانات
5. **اختبر الأداء** مع كميات كبيرة من البيانات

---

## 🎯 النتيجة النهائية

بعد إكمال جميع الخطوات، يجب أن يكون لديك:
- رحلة توزيع مكتملة
- فواتير مبيعات موزع مرتبطة بالرحلة
- كميات مباعة محدثة تلقائياً
- كميات مرجعة مخزنة في قاعدة البيانات
- نظام يعمل بشكل متكامل ومتناسق
