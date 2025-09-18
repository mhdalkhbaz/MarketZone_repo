# ملخص التعديلات على منطق فاتورة الشراء

## التعديلات المطلوبة والمطبقة:

### 1. ✅ حذف الكمية للمنتج غير المحمص
- **قبل**: كان هناك جدول منفصل `UnroastedProdcutBalances` للمنتجات الخام
- **بعد**: جميع المنتجات (جاهز + ني) تذهب إلى جدول `ProductBalances` الموحد

### 2. ✅ إضافة Product Balance في الجدول
- **تم تحديث**: `InventoryAdjustmentService` لاستخدام `ProductBalance` فقط
- **تم حذف**: جميع المراجع إلى `UnroastedProdcutBalance`

### 3. ✅ حذف UnroastedProductBalances
- **تم حذف**: `UnroastedProdcutBalance` entity
- **تم حذف**: `IUnroastedProdcutBalanceRepository` interface
- **تم حذف**: `UnroastedProdcutBalanceRepository` implementation
- **تم حذف**: `UnroastedProdcutBalanceConfiguration`
- **تم تحديث**: `ApplicationDbContext` لإزالة `DbSet<UnroastedProdcutBalance>`

### 4. ✅ تحديث عملية الشراء للجاهز + الني فقط
- **تم إضافة**: التحقق في `CreatePurchaseInvoiceCommandHandler` لمنع شراء المنتجات المحمصة
- **المنطق الجديد**: 
  - المنتجات الجاهزة: تدخل في فاتورة الشراء
  - المنتجات الني (تحتاج تحميص): تدخل في فاتورة الشراء
  - المنتجات المحمصة: لا تدخل في فاتورة الشراء (تأتي من فاتورة التحميص)

### 5. ✅ تحديث عملية التحميص للمنتجات المحمصة
- **تم تحديث**: `RoastingService` ليدعم المنتج الخام والمنتج المحمص
- **تم تحديث**: `RoastingOperation` entity ليدعم `RawProductId` و `RoastedProductId`
- **تم تحديث**: `IRoastingService` interface
- **تم تحديث**: جميع معالجات فواتير التحميص لاستخدام `ProductBalance`

## التدفق الجديد:

### فاتورة الشراء:
```
المنتجات الجاهزة + المنتجات الني → ProductBalance
```

### فاتورة التحميص:
```
المنتج الني (من ProductBalance) → المنتج المحمص (إلى ProductBalance)
```

## الملفات المحدثة:

### Domain Layer:
- `RoastingOperation.cs` - إضافة RawProductId و RoastedProductId
- `UnroastedProdcutBalance.cs` - **تم حذف الملف**

### Application Layer:
- `IRoastingService.cs` - تحديث signature
- `CreatePurchaseInvoiceCommandHandler.cs` - إضافة التحقق من نوع المنتج
- `PostRoastingInvoiceCommandHandler.cs` - استخدام ProductBalance
- `CreateRoastingInvoiceCommandHandler.cs` - إزالة UnroastedProdcutBalanceRepository
- `UpdateRoastingInvoiceCommandHandler.cs` - إزالة UnroastedProdcutBalanceRepository
- `DeleteRoastingInvoiceCommandHandler.cs` - إزالة UnroastedProdcutBalanceRepository

### Infrastructure Layer:
- `InventoryAdjustmentService.cs` - استخدام ProductBalance فقط
- `RoastingService.cs` - تحديث منطق التحميص
- `ApplicationDbContext.cs` - إزالة UnroastedProdcutBalances
- `LookupEndpoint.cs` - استخدام ProductBalance للبحث عن المنتجات الني

### Repository Layer:
- `UnroastedProdcutBalanceRepository.cs` - **تم حذف الملف**
- `IUnroastedProdcutBalanceRepository.cs` - **تم حذف الملف**
- `UnroastedProdcutBalanceConfiguration.cs` - **تم حذف الملف**

## الخطوات التالية المطلوبة:

1. **إنشاء Migration**: لحذف جدول `UnroastedProdcutBalances` من قاعدة البيانات
2. **تحديث Configuration**: تحديث `RoastingOperationConfiguration` لدعم الحقول الجديدة
3. **اختبار النظام**: التأكد من عمل جميع العمليات بشكل صحيح
4. **تحديث الواجهات**: تحديث أي واجهات تستخدم UnroastedProductBalance

## ملاحظات مهمة:

- **المنتجات المحمصة** لا تدخل في فاتورة الشراء، بل تأتي من فاتورة التحميص
- **جميع المنتجات** (جاهز + ني + محمص) تستخدم جدول `ProductBalances` الموحد
- **عملية التحميص** تنقل الكمية من المنتج الني إلى المنتج المحمص مع حساب القيمة
- **التحقق من النوع** يمنع إدخال منتجات محمصة في فاتورة الشراء
