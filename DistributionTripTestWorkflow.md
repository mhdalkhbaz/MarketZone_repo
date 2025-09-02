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

#### **6. رصيد المنتجات (ProductBalance)**
```json
POST /api/ProductBalance/CreateProductBalance
{
  "productId": 1,
  "qty": 200,
  "availableQty": 200
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
    }
  ]
}
```

**النتيجة المتوقعة:**
- رحلة توزيع جديدة برقم ID
- حالة الرحلة: `Draft` (مسودة)
- `SoldQty` = 0 لجميع المنتجات
- `ReturnedQty` = 0 لجميع المنتجات
- **AvailableQty في ProductBalance نقصت بمقدار 100**

**التحقق من الأخطاء:**
- إذا كان `employeeId` غير موجود: خطأ 404
- إذا كان `carId` غير موجود: خطأ 404
- إذا كان `regionId` غير موجود: خطأ 404
- إذا كان `productId` غير موجود: خطأ 404
- إذا كانت `AvailableQty` غير كافية: خطأ "Insufficient available quantity"

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
- **Qty في ProductBalance نقصت بمقدار 100**

**التحقق من الأخطاء:**
- إذا كانت الرحلة ليست في حالة `Draft`: خطأ "Cannot post trip that is not in Draft status"
- إذا كانت `Qty` غير كافية: خطأ "Insufficient quantity"

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
    }
  ]
}
```

**النتيجة المتوقعة:**
- حالة الرحلة: `GoodsReceived` (تم استلام البضاعة)
- `ReturnedQty` = 15 للمنتج
- `SoldQty` = 0 (لم يتم إنشاء فواتير بعد)

**التحقق من الأخطاء:**
- إذا كانت الرحلة ليست في حالة `Posted`: خطأ "Cannot receive goods for trip that is not in Posted status"
- إذا كانت `returnedQty` > `qty`: خطأ "Returned quantity cannot exceed loaded quantity"

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
- `SoldQty` للمنتج في رحلة التوزيع = 30
- الفاتورة مرتبطة برحلة التوزيع

**التحقق من الأخطاء:**
- إذا كانت رحلة التوزيع ليست في حالة `GoodsReceived`: خطأ "Cannot create distributor invoice for trip that is not in GoodsReceived status"
- إذا كانت الكمية المباعة تتجاوز الكمية المحملة: خطأ "Sold quantity cannot exceed loaded quantity"

---

### **الخطوة 5: إنشاء فاتورة مبيعات موزع (العميل الثاني)**

```bash
POST /api/SalesInvoice/CreateSalesInvoice
```

```json
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

### **الخطوة 6: التحقق من الكميات**

```bash
GET /api/DistributionTrip/ValidateTripQuantities?TripId=1
```

**النتيجة المتوقعة:**
```json
{
  "isValid": true,
  "errors": [],
  "warnings": []
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

**التحقق من الأخطاء:**
- إذا كانت الرحلة ليست في حالة `GoodsReceived`: خطأ "Cannot complete trip that is not in GoodsReceived status"
- إذا كانت هناك تفاصيل بدون مبيعات أو مرتجعات: خطأ "All trip details must have either sold quantity or returned quantity"
- إذا كانت الكميات المباعة والمرجعة تتجاوز الكمية المحملة: خطأ "Invalid quantities"

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
      "customerId": 1,
      "type": "Distributor",
      "distributionTripId": 1,
      "totalAmount": 637.50,
      "status": "Draft"
    }
  ]
}
```

### **التحقق من ProductBalance:**

```bash
GET /api/ProductBalance/GetByProductId/1
```

**النتيجة المتوقعة:**
```json
{
  "id": 1,
  "productId": 1,
  "qty": 100,        // 200 - 100 (من Post)
  "availableQty": 100 // 200 - 100 (من Create)
}
```

---

## ✅ معايير النجاح

### **1. سير العمل:**
- ✅ إنشاء رحلة توزيع بنجاح مع التحقق من الـ IDs
- ✅ التحقق من الكميات المتاحة في ProductBalance
- ✅ نقص AvailableQty عند إنشاء الرحلة
- ✅ نقص Qty عند Post الرحلة
- ✅ استلام البضاعة بنجاح
- ✅ إنشاء فواتير المبيعات بنجاح
- ✅ تحديث `SoldQty` تلقائياً
- ✅ التحقق من الكميات بنجاح
- ✅ إكمال الرحلة بنجاح

### **2. التحقق من الأخطاء:**
- ✅ التحقق من وجود Employee, Car, Region, Product
- ✅ التحقق من الكميات المتاحة
- ✅ التحقق من حالة الرحلة في كل خطوة
- ✅ التحقق من الكميات المباعة والمرجعة
- ✅ التراجع عن التغييرات في حالة الخطأ

### **3. البيانات:**
- ✅ `SoldQty` تُحدث تلقائياً عند إنشاء فواتير المبيعات
- ✅ `ReturnedQty` تُخزن في قاعدة البيانات
- ✅ العلاقات بين الكيانات تعمل بشكل صحيح
- ✅ التحقق من الكميات يعمل بشكل صحيح
- ✅ ProductBalance يتم تحديثه بشكل صحيح

---

## 🚨 اختبار الحالات الاستثنائية

### **1. محاولة إنشاء رحلة توزيع بموظف غير موجود:**
```json
{
  "employeeId": 999,
  "carId": 1,
  "regionId": 1,
  "tripDate": "2025-01-15",
  "loadQty": 500,
  "notes": "اختبار خطأ",
  "details": [
    {
      "productId": 1,
      "qty": 100,
      "expectedPrice": 25.50
    }
  ]
}
```
**النتيجة المتوقعة:** خطأ 404 - Employee not found

### **2. محاولة إنشاء رحلة توزيع بكمية غير متاحة:**
```json
{
  "employeeId": 1,
  "carId": 1,
  "regionId": 1,
  "tripDate": "2025-01-15",
  "loadQty": 500,
  "notes": "اختبار خطأ",
  "details": [
    {
      "productId": 1,
      "qty": 1000,
      "expectedPrice": 25.50
    }
  ]
}
```
**النتيجة المتوقعة:** خطأ - Insufficient available quantity

### **3. محاولة Post رحلة ليست في حالة Draft:**
```json
{
  "id": 1
}
```
**النتيجة المتوقعة:** خطأ - Cannot post trip that is not in Draft status

### **4. محاولة استلام البضاعة لرحلة في حالة Draft:**
```json
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
**النتيجة المتوقعة:** خطأ - Cannot receive goods for trip that is not in Posted status

### **5. محاولة إنشاء فاتورة موزع لرحلة في حالة Posted:**
```json
{
  "type": "Distributor",
  "distributionTripId": 1
}
```
**النتيجة المتوقعة:** خطأ - Cannot create distributor invoice for trip that is not in GoodsReceived status

---

## 📝 ملاحظات الاختبار

1. **تأكد من وجود البيانات الأساسية** قبل بدء الاختبار
2. **تأكد من وجود ProductBalance** مع كميات كافية
3. **احفظ الـ IDs** المستخدمة في الاختبار للتحقق منها
4. **اختبر بالترتيب** المحدد
5. **تحقق من النتائج** بعد كل خطوة
6. **اختبر الحالات الاستثنائية** للتأكد من معالجة الأخطاء
7. **تحقق من قاعدة البيانات** مباشرة للتأكد من صحة البيانات
8. **اختبر التراجع عن التغييرات** في حالة الخطأ

---

## 🎯 النتيجة النهائية

بعد إكمال جميع الخطوات، يجب أن يكون لديك:
- رحلة توزيع مكتملة
- فواتير مبيعات موزع مرتبطة بالرحلة
- كميات مباعة محدثة تلقائياً
- كميات مرجعة مخزنة في قاعدة البيانات
- ProductBalance محدث بشكل صحيح
- نظام يعمل بشكل متكامل ومتناسق مع التحقق من الأخطاء
- التراجع عن التغييرات في حالة الخطأ
