# MarketZone - Roasting Operations Postman Collection

## 📋 نظرة عامة

هذا الملف يحتوي على Postman Collection شامل لاختبار جميع عمليات التحميص وفواتير التحميص في نظام MarketZone.

## 🚀 الإعداد الأولي

### 1. استيراد Collection
1. افتح Postman
2. انقر على "Import"
3. اختر ملف `MarketZone_Roasting_Postman_Collection.json`
4. تأكد من استيراد Collection بنجاح

### 2. إعداد المتغيرات البيئية
قم بتعديل المتغيرات التالية في Collection Variables:

```json
{
  "baseUrl": "https://localhost:7001",
  "jwtToken": ""
}
```

## 🔐 المصادقة

### تسجيل الدخول
1. قم بتشغيل طلب **Login** أولاً
2. استخدم البيانات التالية:
   ```json
   {
     "userName": "abd",
     "password": "123123"
   }
   ```
3. انسخ JWT Token من الاستجابة
4. ضعه في متغير `jwtToken` في Collection Variables

## 🔥 عمليات التحميص

### إنشاء عملية تحميص جديدة
**Endpoint:** `POST /api/roasting`

**Body:**
```json
{
  "productId": 1,
  "quantityKg": 50.0,
  "roastPricePerKg": 2.5,
  "roastDate": "2024-01-15T10:00:00Z",
  "notes": "تحميص بن برازيلي درجة متوسطة"
}
```

**الاستجابة المتوقعة:**
```json
{
  "success": true,
  "data": 1,
  "errors": []
}
```

## 📄 فواتير التحميص

### 1. الحصول على قائمة فواتير التحميص
**Endpoint:** `GET /api/roasting-invoice`

**Query Parameters:**
- `pageNumber`: رقم الصفحة (افتراضي: 1)
- `pageSize`: حجم الصفحة (افتراضي: 10)
- `invoiceNumber`: رقم الفاتورة للبحث
- `status`: حالة الفاتورة (0 = Draft, 1 = Posted)

### 2. إنشاء فاتورة تحميص جديدة
**Endpoint:** `POST /api/roasting-invoice`

**Body:**
```json
{
  "invoiceNumber": "ROAST-2024-001",
  "invoiceDate": "2024-01-15T10:00:00Z",
  "totalAmount": 1250.00,
  "notes": "فاتورة تحميص للبن البرازيلي",
  "details": [
    {
      "productId": 1,
      "quantityKg": 50.0,
      "roastPricePerKg": 2.5,
      "notes": "تحميص درجة متوسطة"
    },
    {
      "productId": 2,
      "quantityKg": 30.0,
      "roastPricePerKg": 3.0,
      "notes": "تحميص درجة خفيفة"
    }
  ]
}
```

### 3. تحديث فاتورة تحميص
**Endpoint:** `PUT /api/roasting-invoice`

**Body:**
```json
{
  "id": 1,
  "invoiceNumber": "ROAST-2024-001-UPDATED",
  "invoiceDate": "2024-01-15T10:00:00Z",
  "totalAmount": 1300.00,
  "notes": "فاتورة تحميص محدثة للبن البرازيلي",
  "details": [
    {
      "productId": 1,
      "quantityKg": 55.0,
      "roastPricePerKg": 2.5,
      "notes": "تحميص درجة متوسطة محدث"
    }
  ]
}
```

### 4. حذف فاتورة تحميص
**Endpoint:** `DELETE /api/roasting-invoice/{id}`

### 5. ترحيل فاتورة التحميص
**Endpoint:** `POST /api/roasting-invoice/post`

**Body:**
```json
{
  "id": 1,
  "details": [
    {
      "detailId": 1,
      "actualQuantityAfterRoasting": 47.5
    },
    {
      "detailId": 2,
      "actualQuantityAfterRoasting": 28.5
    }
  ]
}
```

## 📊 التقارير والإحصائيات

### تقرير عمليات التحميص
**Endpoint:** `GET /api/roasting/report`

**Query Parameters:**
- `fromDate`: تاريخ البداية
- `toDate`: تاريخ النهاية

### إحصائيات فواتير التحميص
**Endpoint:** `GET /api/roasting-invoice/statistics`

**Query Parameters:**
- `status`: حالة الفاتورة (0 = Draft, 1 = Posted)

## 🔄 سير العمل المقترح

### 1. إنشاء فاتورة تحميص
1. قم بتشغيل طلب "Create Roasting Invoice"
2. احفظ معرف الفاتورة من الاستجابة

### 2. تحديث الفاتورة (اختياري)
1. استخدم معرف الفاتورة في طلب "Update Roasting Invoice"
2. قم بتعديل البيانات المطلوبة

### 3. ترحيل الفاتورة
1. قم بتشغيل طلب "Post Roasting Invoice"
2. أدخل الكميات الفعلية بعد التحميص
3. ستتغير حالة الفاتورة من Draft إلى Posted

### 4. مراجعة التقارير
1. استخدم طلبات التقارير لمراجعة الأداء
2. تحقق من الإحصائيات

## ⚠️ ملاحظات مهمة

### حالات فواتير التحميص
- **Draft (0)**: مسودة - يمكن التعديل والحذف
- **Posted (1)**: مرحلة - لا يمكن التعديل

### حالات الدفع
- **InProgress (0)**: قيد التنفيذ
- **PartialPayment (1)**: دفع جزئي
- **CompletePayment (2)**: دفع كامل

### التحقق من صحة البيانات
- تأكد من وجود المنتجات قبل إنشاء الفواتير
- تحقق من صحة التواريخ والأرقام
- تأكد من أن الكميات الفعلية بعد التحميص منطقية

## 🛠️ استكشاف الأخطاء

### مشاكل شائعة:
1. **401 Unauthorized**: تأكد من صحة JWT Token
2. **400 Bad Request**: تحقق من صحة البيانات المرسلة
3. **404 Not Found**: تأكد من وجود المعرفات المستخدمة
4. **500 Internal Server Error**: تحقق من سجلات الخادم

### نصائح للاختبار:
1. ابدأ دائماً بتسجيل الدخول
2. اختبر العمليات بالترتيب المنطقي
3. احفظ المعرفات المهمة في متغيرات
4. تحقق من الاستجابات بعناية

## 📞 الدعم

للمساعدة أو الاستفسارات، يرجى التواصل مع فريق التطوير.

---

**MarketZone Roasting Operations** - نظام إدارة التحميص المتكامل 🔥
