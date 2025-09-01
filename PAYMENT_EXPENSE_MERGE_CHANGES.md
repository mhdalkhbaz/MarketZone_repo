# دمج Expense مع Payment - التغييرات المطلوبة

## 📋 ملخص التغييرات

تم دمج كيان `Expense` مع كيان `Payment` لتبسيط إدارة الدفعات والمصروفات في النظام.

## 🔄 التغييرات المنجزة

### 1. **تحديث Payment Entity**
- إضافة `PaymentType` enum (InvoicePayment = 0, Expense = 1)
- إضافة خصائص جديدة:
  - `Description` (للمصروفات)
  - `PaidBy` (للمصروفات)
- جعل `InvoiceId` nullable
- إضافة constructor جديد للمصروفات

### 2. **إنشاء PaymentType Enum**
```csharp
public enum PaymentType : short
{
    InvoicePayment = 0,
    Expense = 1
}
```

### 3. **تحديث DTOs**
- تحديث `PaymentDto` ليشمل الخصائص الجديدة
- حذف `ExpenseDto`

### 4. **تحديث Commands**
- تحديث `CreatePaymentCommand` ليشمل `PaymentType` والخصائص الجديدة
- تحديث `CreatePaymentCommandHandler` للتعامل مع نوعين من الدفعات
- تحديث `PostPaymentCommandHandler` للتعامل مع المصروفات

### 5. **حذف Expense Files**
- حذف `Expense.cs` entity
- حذف `ExpenseDto.cs`
- حذف مجلد `Expenses` بالكامل
- حذف `ExpenseConfiguration.cs`
- حذف `ExpenseRepository.cs`
- حذف `ExpenseFunctionalTests.cs`

### 6. **تحديث Infrastructure**
- تحديث `PaymentConfiguration.cs` للخصائص الجديدة
- تحديث `ApplicationDbContext.cs` لحذف `Expenses` DbSet
- تحديث `CashProfile.cs` لحذف Expense mappings

### 7. **تحديث API Endpoints**
- حذف Expense endpoints من `CashEndpoint.cs`
- الاحتفاظ بـ Payment endpoints فقط

### 8. **تحديث Postman Collection**
- تحديث `CreatePayment` requests لتدعم نوعين:
  - `CreatePayment (Invoice)` - للدفعات المرتبطة بالفواتير
  - `CreatePayment (Expense)` - للمصروفات
- حذف Expense requests

## 🚀 كيفية الاستخدام

### إنشاء دفعة فاتورة:
```json
POST /api/Cash/CreatePayment
{
  "cashRegisterId": 1,
  "paymentType": 0,
  "invoiceId": 1,
  "paymentDate": "2025-01-01",
  "amount": 100,
  "notes": "",
  "receivedBy": "Admin",
  "isConfirmed": true,
  "referenceType": 0
}
```

### إنشاء مصروف:
```json
POST /api/Cash/CreatePayment
{
  "cashRegisterId": 1,
  "paymentType": 1,
  "paymentDate": "2025-01-01",
  "amount": 50,
  "description": "مصروفات مكتبية",
  "paidBy": "Admin",
  "isConfirmed": true,
  "referenceType": 2
}
```

## ⚠️ ملاحظات مهمة

1. **Migration مطلوب**: يجب إنشاء migration جديد لحذف جدول Expenses وتحديث جدول Payments
2. **بيانات موجودة**: إذا كانت هناك بيانات في جدول Expenses، يجب نقلها إلى جدول Payments أولاً
3. **اختبار**: يجب اختبار جميع الوظائف للتأكد من عملها بشكل صحيح

## 🔧 الخطوات المتبقية

1. إنشاء migration جديد:
```bash
dotnet ef migrations add RemoveExpenseEntity --project Src/Infrastructure/MarketZone.Infrastructure.Persistence --startup-project Src/Presentation/MarketZone.WebApi
```

2. تطبيق migration:
```bash
dotnet ef database update --project Src/Infrastructure/MarketZone.Infrastructure.Persistence --startup-project Src/Presentation/MarketZone.WebApi
```

3. اختبار النظام للتأكد من عمل جميع الوظائف

## 📊 الفوائد

- **تبسيط النظام**: كيان واحد بدلاً من كيانين
- **سهولة الصيانة**: كود أقل وتعقيد أقل
- **مرونة أكبر**: يمكن إضافة أنواع جديدة من الدفعات بسهولة
- **تتبع موحد**: جميع الحركات المالية في مكان واحد
