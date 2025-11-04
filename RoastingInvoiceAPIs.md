# 📚 APIs الخاصة بفاتورة التحميص (Roasting Invoice APIs)

## 🔗 Base URL
```
/api/RoastingInvoice
```

---

## 1️⃣ GET - الحصول على قائمة بصفحات

### Endpoint
```
GET /api/RoastingInvoice/GetPagedList
```

### Query Parameters
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `pageNumber` | int | No | 1 | رقم الصفحة |
| `pageSize` | int | No | 10 | حجم الصفحة |

### Example Request
```
GET /api/RoastingInvoice/GetPagedList?pageNumber=1&pageSize=10
```

### Response
```json
{
  "data": [
    {
      "id": 1,
      "invoiceNumber": "RI-2024-001",
      "invoiceDate": "2024-01-15T10:00:00Z",
      "totalAmount": 1000.00,
      "notes": "ملاحظات",
      "status": 1,
      "paymentStatus": 0,
      "employeeId": 5,
      "createdDateTime": "2024-01-15T10:00:00Z",
      "paidAmount": 500.00,
      "unpaidAmount": 500.00,
      "details": [],
      "payments": []
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5
}
```

### Description
يرجع قائمة جميع فواتير التحميص مع pagination

---

## 2️⃣ GET - الحصول على فاتورة حسب المعرف

### Endpoint
```
GET /api/RoastingInvoice/GetById
```

### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | long | Yes | معرف الفاتورة |

### Example Request
```
GET /api/RoastingInvoice/GetById?id=1
```

### Response
```json
{
  "success": true,
  "data": {
    "id": 1,
    "invoiceNumber": "RI-2024-001",
    "invoiceDate": "2024-01-15T10:00:00Z",
    "totalAmount": 1000.00,
    "notes": "ملاحظات",
    "status": 1,
    "paymentStatus": 0,
    "employeeId": 5,
    "createdDateTime": "2024-01-15T10:00:00Z",
    "paidAmount": 500.00,
    "unpaidAmount": 500.00,
    "details": [
      {
        "id": 1,
        "rawProductId": 10,
        "rawProductName": "قهوة خام",
        "quantityKg": 50.0,
        "receivedQuantityKg": 45.0,
        "remainingQuantity": 5.0,
        "roastingCost": 20.0,
        "notes": "تفاصيل"
      }
    ],
    "receipts": [
      {
        "id": 1,
        "readyProductId": 20,
        "readyProductName": "قهوة محمصة",
        "quantityKg": 45.0,
        "salePricePerKg": 30.0,
        "roastingCostPerKg": 20.0,
        "commissionPerKg": 5.0,
        "netSalePricePerKg": 25.0
      }
    ],
    "payments": []
  }
}
```

### Description
يرجع تفاصيل فاتورة واحدة مع جميع التفاصيل والإيصالات والمدفوعات

---

## 3️⃣ GET - الحصول على الفواتير غير المدفوعة حسب الموظف

### Endpoint
```
GET /api/RoastingInvoice/GetUnpaidInvoicesByEmployee
```

### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `employeeId` | long | Yes | معرف الموظف |

### Example Request
```
GET /api/RoastingInvoice/GetUnpaidInvoicesByEmployee?employeeId=5
```

### Response
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "invoiceNumber": "RI-2024-001",
      "unpaidAmount": 500.00
    },
    {
      "id": 2,
      "invoiceNumber": "RI-2024-002",
      "unpaidAmount": 300.00
    }
  ]
}
```

### Description
يرجع قائمة بالفواتير غير المدفوعة للموظف مع المعرف ورقم الفاتورة والمبلغ غير المدفوع فقط

### Business Logic
- الفلترة: `Status = Posted` و `UnpaidAmount > 0`
- الترتيب: حسب ID تنازلي
- حساب `UnpaidAmount` = `TotalAmount - Sum(PaidAmounts)`

---

## 4️⃣ POST - إنشاء فاتورة جديدة

### Endpoint
```
POST /api/RoastingInvoice/Create
Authorization: Bearer {token}
```

### Request Body
```json
{
  "invoiceNumber": "RI-2024-001",  // اختياري - يتم توليده تلقائياً إذا فارغ
  "invoiceDate": "2024-01-15T10:00:00Z",
  "totalAmount": 1000.00,
  "notes": "ملاحظات",
  "employeeId": 5,  // اختياري
  "details": [
    {
      "rawProductId": 10,
      "quantityKg": 50.0,
      "notes": "منتج خام للتحميص",
      "roastingCost": 20.0
    }
  ]
}
```

### Request Body Schema
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `invoiceNumber` | string | No | رقم الفاتورة (يُولد تلقائياً إذا فارغ) |
| `invoiceDate` | DateTime | Yes | تاريخ الفاتورة |
| `totalAmount` | decimal | Yes | المبلغ الإجمالي |
| `notes` | string | No | ملاحظات |
| `employeeId` | long? | No | معرف الموظف |
| `details` | array | Yes | قائمة التفاصيل |

#### Details Schema
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `rawProductId` | long | Yes | معرف المنتج الخام |
| `quantityKg` | decimal | Yes | الكمية بالكيلوغرام |
| `notes` | string | No | ملاحظات |
| `roastingCost` | decimal | Yes | تكلفة التحميص |

### Response
```json
{
  "success": true,
  "data": 1  // Invoice ID
}
```

### Description
ينشئ فاتورة جديدة بحالة `Draft`

### Business Logic
- إذا كان `invoiceNumber` فارغاً، يتم توليده تلقائياً
- لكل منتج خام في التفاصيل:
  - التحقق من توفر الكمية (`AvailableQty`)
  - خصم الكمية من المخزون (`Adjust(0, -quantityKg)`)
- الحالة الابتدائية: `Status = Draft`, `PaymentStatus = InProgress`

---

## 5️⃣ PUT - تحديث فاتورة

### Endpoint
```
PUT /api/RoastingInvoice/Update
Authorization: Bearer {token}
```

### Request Body
```json
{
  "id": 1,
  "invoiceNumber": "RI-2024-001",
  "invoiceDate": "2024-01-15T10:00:00Z",
  "totalAmount": 1200.00,
  "notes": "ملاحظات محدثة",
  "employeeId": 5,
  "details": [
    {
      "id": 1,  // إذا موجود = تحديث، إذا null = إضافة جديد
      "rawProductId": 10,
      "quantityKg": 60.0,
      "notes": "تحديث",
      "roastingCost": 25.0
    },
    {
      "id": null,  // تفصيل جديد
      "rawProductId": 11,
      "quantityKg": 30.0,
      "notes": "منتج جديد",
      "roastingCost": 15.0
    }
  ]
}
```

### Request Body Schema
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `id` | long | Yes | معرف الفاتورة |
| `invoiceNumber` | string | Yes | رقم الفاتورة |
| `invoiceDate` | DateTime | Yes | تاريخ الفاتورة |
| `totalAmount` | decimal | Yes | المبلغ الإجمالي |
| `notes` | string | No | ملاحظات |
| `employeeId` | long? | No | معرف الموظف |
| `details` | array | Yes | قائمة التفاصيل |

### Response
```json
{
  "success": true,
  "data": 1  // Invoice ID
}
```

### Description
يحدث فاتورة موجودة

### Business Logic
- يمكن التحديث فقط إذا كانت الحالة `Draft`
- يتم تحديث المخزون حسب التغييرات في التفاصيل
- التفاصيل التي `id = null` = إضافة جديدة
- التفاصيل التي `id` موجود = تحديث

---

## 6️⃣ DELETE - حذف فاتورة

### Endpoint
```
DELETE /api/RoastingInvoice/Delete
Authorization: Bearer {token}
```

### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | long | Yes | معرف الفاتورة |

### Example Request
```
DELETE /api/RoastingInvoice/Delete?id=1
```

### Response
```json
{
  "success": true
}
```

### Description
يحذف فاتورة موجودة

### Business Logic
- يمكن الحذف فقط إذا كانت الحالة `Draft`
- يتم إرجاع الكميات إلى المخزون

---

## 7️⃣ POST - ترحيل فاتورة (Posting)

### Endpoint
```
POST /api/RoastingInvoice/Post
Authorization: Bearer {token}
```

### Request Body
```json
{
  "id": 1,
  "details": [
    {
      "detailId": 1,
      "readyDetails": [
        {
          "rawProductId": 10,
          "readyProductId": 20,
          "actualQuantityAfterRoasting": 45.0,
          "commissionPerKg": 5.0,
          "netSalePricePerKg": 25.0,
          "roastingCostPerKg": 20.0,
          "salePricePerKg": 30.0
        }
      ]
    }
  ]
}
```

### Request Body Schema
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `id` | long | Yes | معرف الفاتورة |
| `details` | array | Yes | قائمة التفاصيل |

#### Details Schema
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `detailId` | long | Yes | معرف التفصيل من الفاتورة |
| `readyDetails` | array | Yes | قائمة المنتجات الجاهزة |

#### ReadyDetails Schema
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `rawProductId` | long | Yes | معرف المنتج الخام |
| `readyProductId` | long | Yes | معرف المنتج المحمص (الجاهز) |
| `actualQuantityAfterRoasting` | decimal | Yes | الكمية الفعلية بعد التحميص |
| `commissionPerKg` | decimal | Yes | العمولة لكل كيلو |
| `netSalePricePerKg` | decimal | Yes | سعر البيع الصافي لكل كيلو |
| `roastingCostPerKg` | decimal | Yes | تكلفة التحميص لكل كيلو |
| `salePricePerKg` | decimal | Yes | سعر البيع لكل كيلو |

### Response
```json
{
  "success": true,
  "data": 1  // Invoice ID
}
```

### Description
يرحل فاتورة التحميص ويقوم بتنفيذ عملية التحميص

### Business Logic
- يمكن الترحيل فقط إذا كانت الحالة `Draft`
- لكل `ReadyDetail`:
  - خصم الكمية الفعلية من المنتج الخام
  - حساب القيمة: (تكلفة الخام المستخدم) + (العمولة)
  - إضافة المنتج المحمص إلى المخزون مع القيمة المحسوبة
  - إنشاء `RoastingInvoiceDetailReceipt`
- تحديث الحالة إلى `Posted`
- تحديث رصيد الموظف: `employee.SyrianMoney += totalRoastingCost`
- إنشاء سجل في `InventoryHistory`

---

## 📊 حالات الفاتورة (Status)

| Value | Name | Description |
|-------|------|-------------|
| 0 | `Draft` | مسودة - تم الإنشاء فقط |
| 1 | `Posted` | تم الترحيل - تم تنفيذ العملية |
| 2 | `Receiving` | قيد الاستلام |
| 3 | `Received` | تم الاستلام |

---

## 💰 حالات الدفع (PaymentStatus)

| Value | Name | Description |
|-------|------|-------------|
| 0 | `InProgress` | لم يتم الدفع بعد |
| 1 | `PartialPayment` | دفع جزئي |
| 2 | `CompletePayment` | دفع كامل |

---

## 🔐 Authentication

جميع Endpoints التالية تتطلب **Authorization Token**:
- `POST /api/RoastingInvoice/Create`
- `PUT /api/RoastingInvoice/Update`
- `DELETE /api/RoastingInvoice/Delete`
- `POST /api/RoastingInvoice/Post`

### Authorization Header
```
Authorization: Bearer {token}
```

---

## 📝 ملاحظات مهمة

1. **عند الإنشاء (Create):**
   - يتم "حجز" الكمية فقط (خصم من `AvailableQty`)
   - الفاتورة بحالة `Draft`

2. **عند الترحيل (Post):**
   - يتم خصم فعلي من المخزون
   - يتم إضافة المنتج المحمص للمخزون
   - يتم تحديث رصيد الموظف (تكلفة التحميص)

3. **عند الدفع (Payment):**
   - يتم تحديث `PaymentStatus`
   - يتم تحديث رصيد الموظف بالمبلغ المدفوع
   - **يدعم الدفع بالعملتين:**
     - إذا تم الدفع بالليرة السورية → يضاف المبلغ إلى `employee.SyrianMoney`
     - إذا تم الدفع بالدولار → يضاف المبلغ إلى `employee.DollarMoney`

4. **التحديث والحذف:**
   - مسموح فقط للفواتير بحالة `Draft`

## 💰 الدفع بفاتورة التحميص

### الدفع بالليرة السورية
```json
{
  "paymentType": "RoastingPayment",
  "invoiceId": 1,
  "invoiceType": "RoastingInvoice",
  "amount": 500000.00,
  "currency": "SY",
  "paymentCurrency": "SY",
  "paymentDate": "2024-01-15T10:00:00Z",
  "cashRegisterId": 1,
  "notes": "دفعة بالليرة السورية",
  "receivedBy": "محمد أحمد"
}
```
- المبلغ يضاف إلى `employee.SyrianMoney`

### الدفع بالدولار
```json
{
  "paymentType": "RoastingPayment",
  "invoiceId": 1,
  "invoiceType": "RoastingInvoice",
  "amount": 100.00,
  "currency": "SY",
  "paymentCurrency": "Dollar",
  "exchangeRate": 15000.00,
  "paymentDate": "2024-01-15T10:00:00Z",
  "cashRegisterId": 1,
  "notes": "دفعة بالدولار",
  "receivedBy": "محمد أحمد"
}
```
- المبلغ يضاف إلى `employee.DollarMoney`
- `AmountInPaymentCurrency` = `amount * exchangeRate` (للحسابات)

### ملاحظات الدفع
- **Currency**: عملة الفاتورة (عادة `SY`)
- **PaymentCurrency**: العملة التي تم الدفع بها فعلياً (`SY` أو `Dollar`)
- **ExchangeRate**: مطلوب فقط عند الدفع بعملة مختلفة عن عملة الفاتورة
- **AmountInPaymentCurrency**: يتم حسابه تلقائياً عند وجود `ExchangeRate`

---

## 🔗 Related Endpoints

### Payments (المدفوعات)
- `POST /api/Payment/Create` - إنشاء دفعة لفاتورة التحميص
- `POST /api/Payment/Post` - ترحيل الدفعة

### Employees (الموظفين)
- `GET /api/Employee/GetById` - الحصول على معلومات الموظف مع الرصيد
- `GET /api/Lookup/GetRoastingEmployeesWithBalance` - الحصول على موظفي التحميص مع الأرصدة

### Products (المنتجات)
- `GET /api/Lookup/GetUnroastedProductsWithQty` - الحصول على المنتجات الخام مع الكميات المتاحة

