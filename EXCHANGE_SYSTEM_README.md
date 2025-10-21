# 💱 نظام سعر الصرف والتحويل - MarketZone

## 📋 نظرة عامة

نظام متكامل لإدارة أسعار الصرف وتحويل العملات في نظام MarketZone. يدعم النظام العملتين الأساسيتين: الدولار الأمريكي (USD) والليرة السورية (SYP).

## 🏗️ البنية المعمارية

### **1️⃣ Domain Layer**

#### **Entities:**
- `ExchangeRate` - أسعار الصرف
- `ExchangeTransaction` - معاملات التحويل

#### **Enums:**
- `ExchangeDirection` - اتجاهات التحويل
  - `USD_To_SYP = 0` - دولار إلى سوري
  - `SYP_To_USD = 1` - سوري إلى دولار

### **2️⃣ Application Layer**

#### **Commands:**
- `CreateExchangeRateCommand` - إنشاء سعر صرف جديد
- `CreateExchangeTransactionCommand` - إنشاء معاملة تحويل

#### **Queries:**
- `GetLatestRateQuery` - جلب أحدث سعر صرف

### **3️⃣ Infrastructure Layer**

#### **Repositories:**
- `ExchangeRateRepository` - إدارة أسعار الصرف
- `ExchangeTransactionRepository` - إدارة معاملات التحويل

## 🔧 API Endpoints

### **💱 Exchange Rate Management**

#### **إنشاء سعر صرف جديد:**
```http
POST /api/Cash/CreateExchangeRate
Content-Type: application/json

{
  "rate": 15000,
  "effectiveDate": "2025-01-15T00:00:00Z"
}
```

#### **جلب أحدث سعر صرف:**
```http
GET /api/Cash/GetLatestExchangeRate
```

### **🔄 Exchange Transactions**

#### **تحويل دولار إلى سوري:**
```http
POST /api/Cash/CreateExchangeTransaction
Content-Type: application/json

{
  "cashRegisterId": 1,
  "direction": 0,
  "fromAmount": 100,
  "exchangeRate": 15000,
  "transactionDate": "2025-01-15T00:00:00Z",
  "notes": "تحويل 100 دولار إلى ليرة سورية"
}
```

#### **تحويل سوري إلى دولار:**
```http
POST /api/Cash/CreateExchangeTransaction
Content-Type: application/json

{
  "cashRegisterId": 1,
  "direction": 1,
  "fromAmount": 1500000,
  "exchangeRate": 15000,
  "transactionDate": "2025-01-15T00:00:00Z",
  "notes": "تحويل 1,500,000 ليرة إلى دولار"
}
```

## 💰 تكامل مع نظام المدفوعات

### **دفع فاتورة شراء بعملات مختلفة:**
```http
POST /api/Cash/CreatePayment
Content-Type: application/json

{
  "paymentType": 1,
  "invoiceId": 1,
  "invoiceType": 1,
  "amount": 100,
  "currency": "USD",
  "paymentCurrency": "SYP",
  "exchangeRate": 15000,
  "notes": "دفع فاتورة شراء بالليرة",
  "receivedBy": "المحاسب",
  "paymentDate": "2025-01-15T00:00:00Z",
  "isConfirmed": true
}
```

## 📊 أمثلة عملية

### **مثال 1: إنشاء سعر صرف جديد**
```json
{
  "rate": 15000,
  "effectiveDate": "2025-01-15T00:00:00Z"
}
```
**النتيجة:** 1 دولار = 15000 ليرة سورية

### **مثال 2: تحويل 100 دولار إلى ليرة**
```json
{
  "cashRegisterId": 1,
  "direction": 0,
  "fromAmount": 100,
  "exchangeRate": 15000,
  "transactionDate": "2025-01-15T00:00:00Z",
  "notes": "تحويل دولار إلى ليرة"
}
```
**النتيجة:** 100 دولار × 15000 = 1,500,000 ليرة سورية

### **مثال 3: تحويل 1,500,000 ليرة إلى دولار**
```json
{
  "cashRegisterId": 1,
  "direction": 1,
  "fromAmount": 1500000,
  "exchangeRate": 15000,
  "transactionDate": "2025-01-15T00:00:00Z",
  "notes": "تحويل ليرة إلى دولار"
}
```
**النتيجة:** 1,500,000 ليرة ÷ 15000 = 100 دولار

## 🔄 تدفق العمل

### **1️⃣ إدارة أسعار الصرف:**
1. إنشاء سعر صرف جديد
2. إلغاء تفعيل الأسعار القديمة
3. تفعيل السعر الجديد

### **2️⃣ تحويل العملات:**
1. اختيار اتجاه التحويل
2. إدخال المبلغ المراد تحويله
3. حساب المبلغ المحول تلقائياً
4. تحديث الصندوق النقدي
5. تسجيل المعاملة

### **3️⃣ تكامل المدفوعات:**
1. اختيار نوع الدفع
2. ربط بفاتورة (إذا لزم الأمر)
3. اختيار العملات
4. حساب سعر الصرف
5. تسجيل الدفع

## 🎯 المميزات

### **✅ إدارة شاملة:**
- تتبع أسعار الصرف التاريخية
- إدارة المعاملات
- تحديث الصندوق تلقائياً

### **✅ مرونة في العملات:**
- دعم USD/SYP
- تحويل ثنائي الاتجاه
- حساب تلقائي للمبالغ

### **✅ تكامل متقدم:**
- ربط مع نظام المدفوعات
- دعم العملات المختلفة في الفواتير
- تتبع دقيق للمعاملات

### **✅ أمان وموثوقية:**
- التحقق من صحة البيانات
- تتبع جميع المعاملات
- نسخ احتياطية تلقائية

## 📈 التقارير المتاحة

### **1️⃣ تقارير أسعار الصرف:**
- تاريخ تغيرات الأسعار
- مقارنة الفترات
- تحليل الاتجاهات

### **2️⃣ تقارير التحويلات:**
- معاملات التحويل
- تحليل الاتجاهات
- إحصائيات الصندوق

### **3️⃣ تقارير التكامل:**
- المدفوعات بالعملات المختلفة
- تحليل التكاليف
- مؤشرات الأداء

## 🧪 الاختبار

### **Postman Collections:**
- `Exchange_System_Test_Collection.json` - اختبارات أساسية
- `Complete_Exchange_System_Test.json` - اختبارات شاملة

### **سيناريوهات الاختبار:**
1. إنشاء سعر صرف جديد
2. تحويل دولار إلى سوري
3. تحويل سوري إلى دولار
4. دفع فاتورة بعملات مختلفة
5. قبض من زبون
6. دفع أجور التحميص

## 🚀 التطوير المستقبلي

### **المميزات المخططة:**
- دعم عملات إضافية
- تحليل تلقائي للأسعار
- تنبيهات تغيرات الأسعار
- تقارير متقدمة
- واجهة مستخدم محسنة

---

## 📞 الدعم

للمساعدة أو الاستفسارات، يرجى التواصل مع فريق التطوير.

**تم تطوير النظام بواسطة فريق MarketZone** 🚀
