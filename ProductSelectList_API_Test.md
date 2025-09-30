# اختبار API قائمة المنتجات للتوزيع

## 📋 نظرة عامة

تم إنشاء API جديد لإرجاع قائمة المنتجات للاختيار مع الكميات المتاحة. يدعم حالتين:
1. **مع DistributionTripId**: إرجاع المنتجات المتبقية في رحلة التوزيع
2. **بدون DistributionTripId**: إرجاع المنتجات الجاهزة للبيع (المحمصة)

## 🔧 API الجديد

### **Endpoint:**
```
GET /api/Lookup/GetProductSelectListForDistribution
```

### **المعاملات:**
- `DistributionTripId` (اختياري): رقم رحلة التوزيع

### **Response Format:**
```json
{
  "succeeded": true,
  "data": [
    {
      "value": "1",
      "label": "بن أرابيكا",
      "qty": 50.00
    },
    {
      "value": "2", 
      "label": "بن روبوستا",
      "qty": 30.00
    }
  ]
}
```

## 🧪 سيناريوهات الاختبار

### **الاختبار 1: الحصول على المنتجات الجاهزة للبيع**

**Request:**
```
GET /api/Lookup/GetProductSelectListForDistribution
```

**النتيجة المتوقعة:**
- إرجاع المنتجات التي `NeedsRoasting = false`
- فقط المنتجات التي لديها `AvailableQty > 0`
- مرتبة حسب الاسم

**مثال Response:**
```json
{
  "succeeded": true,
  "data": [
    {
      "value": "1",
      "label": "بن أرابيكا محمص",
      "qty": 100.00
    },
    {
      "value": "3",
      "label": "بن كولومبيا محمص",
      "qty": 75.00
    }
  ]
}
```

---

### **الاختبار 2: الحصول على المنتجات المتبقية في رحلة التوزيع**

**Request:**
```
GET /api/Lookup/GetProductSelectListForDistribution?DistributionTripId=1
```

**النتيجة المتوقعة:**
- إرجاع المنتجات من رحلة التوزيع المحددة
- فقط المنتجات التي لديها كمية متبقية > 0
- الكمية المتبقية = المحملة - المباعة - المرجعة

**مثال Response:**
```json
{
  "succeeded": true,
  "data": [
    {
      "value": "1",
      "label": "بن أرابيكا",
      "qty": 20.00
    }
  ]
}
```

---

### **الاختبار 3: رحلة توزيع غير موجودة**

**Request:**
```
GET /api/Lookup/GetProductSelectListForDistribution?DistributionTripId=999
```

**النتيجة المتوقعة:**
```json
{
  "succeeded": true,
  "data": []
}
```

---

### **الاختبار 4: رحلة توزيع بدون منتجات متبقية**

**Request:**
```
GET /api/Lookup/GetProductSelectListForDistribution?DistributionTripId=2
```

**النتيجة المتوقعة:**
```json
{
  "succeeded": true,
  "data": []
}
```

## 📊 البيانات المطلوبة للاختبار

### **1. المنتجات:**
```json
// منتج جاهز للبيع (محمص)
{
  "id": 1,
  "name": "بن أرابيكا محمص",
  "needsRoasting": false,
  "isActive": true
}

// منتج يحتاج تحميص
{
  "id": 2,
  "name": "بن أرابيكا خام",
  "needsRoasting": true,
  "isActive": true
}
```

### **2. رصيد المنتجات:**
```json
{
  "productId": 1,
  "availableQty": 100.00,
  "qty": 100.00
}
```

### **3. رحلة التوزيع:**
```json
{
  "id": 1,
  "status": "GoodsReceived",
  "details": [
    {
      "productId": 1,
      "qty": 50.00,        // محمل
      "soldQty": 20.00,    // مباع
      "returnedQty": 10.00 // مرتجع
      // المتبقي = 50 - 20 - 10 = 20
    }
  ]
}
```

## 🔍 التحقق من النتائج

### **للحالة الأولى (بدون DistributionTripId):**
- ✅ إرجاع المنتجات التي `NeedsRoasting = false`
- ✅ إرجاع المنتجات التي `AvailableQty > 0`
- ✅ ترتيب حسب الاسم
- ❌ استبعاد المنتجات التي تحتاج تحميص
- ❌ استبعاد المنتجات بدون كمية متاحة

### **للحالة الثانية (مع DistributionTripId):**
- ✅ إرجاع المنتجات من رحلة التوزيع المحددة
- ✅ حساب الكمية المتبقية بشكل صحيح
- ✅ استبعاد المنتجات بدون كمية متبقية
- ✅ ترتيب حسب الاسم
- ❌ استبعاد المنتجات المباعة بالكامل

## 🚨 اختبار الحالات الاستثنائية

### **1. معامل غير صحيح:**
```
GET /api/Lookup/GetProductSelectListForDistribution?DistributionTripId=abc
```
**النتيجة المتوقعة:** خطأ 400 Bad Request

### **2. معامل سالب:**
```
GET /api/Lookup/GetProductSelectListForDistribution?DistributionTripId=-1
```
**النتيجة المتوقعة:** إرجاع قائمة فارغة

### **3. بدون تفويض:**
```
GET /api/Lookup/GetProductSelectListForDistribution
```
**النتيجة المتوقعة:** خطأ 401 Unauthorized (إذا كان مطلوب)

## 📝 ملاحظات الاختبار

1. **تأكد من وجود البيانات الأساسية** قبل الاختبار
2. **اختبر الحالتين** (مع وبدون DistributionTripId)
3. **تحقق من صحة الحسابات** للكميات المتبقية
4. **اختبر الحالات الاستثنائية**
5. **تحقق من الأداء** مع قوائم كبيرة

## 🎯 النتيجة المتوقعة

بعد إكمال جميع الاختبارات، يجب أن يكون API يعمل بشكل صحيح:
- إرجاع المنتجات الجاهزة للبيع عند عدم تحديد رحلة
- إرجاع المنتجات المتبقية عند تحديد رحلة توزيع
- حساب الكميات بشكل صحيح
- معالجة الحالات الاستثنائية بشكل مناسب
- أداء جيد مع قوائم كبيرة

---

*تم إعداد هذا الدليل لاختبار API قائمة المنتجات للتوزيع الجديد*
