# 📊 أقسام مشروع  

## 🏢 **أقسام المشروع**

### **1. Master Data (البيانات الأساسية)**
 

---
انواع المنتجات 
جاهزة - نية - محضرة ( ل هي بتكون نية )
## 1️⃣ **المنتجات (Products)**

### 📋 **Entity: Product**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **CategoryId** | ✅ مطلوب |
| **Name** | ✅ مطلوب |
| **Description** | |
| **UnitOfMeasure** | | الوحده
| **PurchasePrice** | |  سعر الشراء “ شو رايك نحذفه لان رح يدخلو ب المشتريات ؟؟
| **SalePrice** | | سعر البيع  مطلووب ل المنتجات الجاهزه 
| **MinStockLevel** | | حاليا مالها تأثير 
| **IsActive** | |
| **NeedsRoasting** | |  لما يكون بزر ني مثل بزر ابيض ني  
| **RawProductId** | |  نختار البزر النيء ل بدو يطلع منه هل منتج 
| **BarCode** | | مالها تأثير 
| **RoastingCost** | |  هي مو منعبيها ب فاتورة التحميص ؟ لشو نخليها هون ؟ 
| **CommissionPerKg** | |  كمان هي نفس ل فوقها ؟ ولا نخليها مشان نقترحها اقتراح ؟ 

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
  - `RawProductId` (long?) - معرف المنتج الخام
  - `CommissionPerKg` (decimal?) - العمولة لكل كيلو
- **التأثير:**
  - ✅ إضافة منتج جديد في قاعدة البيانات


—








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
اذا trip id null 
بجيب المنتجات ل فيها كمية ..  
اما اذا مش null
بجيب المنتجات ب ل هل رحلة 
والكمية بتكون 
                var remainingQty = detail.Qty - detail.SoldQty - detail.ReturnedQty;



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
برجع المنتجات النية + الجاهزة 
---

#### **11. GET `/api/Lookup/GetInStockProductSelectList`**
- **الوصف:** قائمة المنتجات المتوفرة في المخزون
- **الطريقة:** GET
 **التأثير:** قراءة فقط - عرض المنتجات المتوفرة
تعيد الجاهزة + المحضر ل فيه كميه 
---
 
#### **12. GET `/api/Lookup/GetUnroastedProductsWithQty`**
- **الوصف:** قائمة المنتجات الخام مع الكميات غير المحمصة
- **الطريقة:** GET
- **المصادقة:** ❌ لا
- **النتيجة:** قائمة `UnroastedProductDto` مع الكميات
- **التأثير:** قراءة فقط - عرض المنتجات الخام مع الأرصدة
تعيد النية ل فيه كميه 
….. هدول لازم يكونو 
---








## 3️⃣ **العملاء (Customers)**

### 📋 **Entity: Customer**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **Name** | ✅ مطلوب |
| **Phone** | ✅ مطلوب |
| **WhatsAppPhone** | |
| **Email** | |
| **Address** | |
| **Currency** | | Currency enum (0 = SY, 1 = Dollar)
| **IsActive** | |

---

### 🔌 **APIs المتاحة:**

#### **1. GET `/api/Customer` - GetPagedListCustomer**
- **الوصف:** الحصول على قائمة العملاء بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
  - `SearchText` (string) - البحث في الاسم
- **التأثير:** عرض العملاء فقط (قراءة فقط)

---

#### **2. GET `/api/Customer/{id}` - GetCustomerById**
- **الوصف:** الحصول على عميل محدد
- **الطريقة:** GET
- **المعاملات:**
  - `id` (long) - معرف العميل
- **التأثير:** عرض عميل واحد فقط (قراءة فقط)

---







#### **3. POST `/api/Customer` - CreateCustomer**
- **الوصف:** إنشاء عميل جديد
- **الطريقة:** POST
- **المعاملات:**
  - `Name` (string) - ✅ **مطلوب**
  - `Phone` (string) - ✅ **مطلوب**
  - `WhatsAppPhone` (string) - واتساب
  - `Email` (string) - البريد الإلكتروني
  - `Address` (string) - العنوان
  - `Currency` (Currency?) - العملة (enum: 0 = SY, 1 = Dollar)
  - `IsActive` (bool) - نشط
- **التأثير:**
  - ✅ إضافة عميل جديد في قاعدة البيانات

---

#### **4. PUT `/api/Customer` - UpdateCustomer**
- **الوصف:** تحديث عميل موجود
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
  - جميع الحقول الأخرى (نفس CreateCustomer)
- **التأثير:**
  - ✅ تحديث بيانات العميل في قاعدة البيانات

---

#### **5. DELETE `/api/Customer/{id}` - DeleteCustomer**
- **الوصف:** حذف عميل
- **الطريقة:** DELETE
- **المعاملات:**
  - `id` (long) - معرف العميل
- **التأثير:**
  - ✅ حذف العميل من قاعدة البيانات
  - ⚠️ قد يفشل إذا كان العميل لديه فواتير مبيعات

---

#### **6. GET `/api/Customer/GetUnpaidInvoicesByCustomer` - GetUnpaidInvoicesByCustomer**
- **الوصف:** الحصول على الفواتير غير المدفوعة للعميل
- **الطريقة:** GET
- **المعاملات:**
  - `customerId` (long) - معرف العميل
- **التأثير:** قراءة فقط - عرض فواتير المبيعات غير المدفوعة

---

#### **7. GET `/api/Customer/GetActiveCustomersSelectList` - GetActiveCustomersSelectList**
- **الوصف:** قائمة اختيار بالعملاء النشطين
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار

---

#### **8. GET `/api/Lookup/GetCustomerSelectList`**
- **الوصف:** قائمة اختيار بجميع العملاء
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار

**ملاحظة:** حقل `Currency` من نوع enum (`Currency`):
- `0` = SY (ليرة سورية)
- `1` = Dollar (دولار)

## 4️⃣ **الموردين (Suppliers)**

### 📋 **Entity: Supplier**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **Name** | ✅ مطلوب |
| **Phone** | ✅ مطلوب |
| **WhatsAppPhone** | |
| **Email** | |
| **Address** | ✅ مطلوب |
| **Currency** | | Currency enum (0 = SY, 1 = Dollar)
| **IsActive** | |


### 🔌 **APIs المتاحة:**

#### **1. GET `/api/Supplier` - GetPagedListSupplier**
- **الوصف:** الحصول على قائمة الموردين بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
  - `SearchText` (string) - البحث في الاسم
- **التأثير:** عرض الموردين فقط (قراءة فقط)

---



#### **2. GET `/api/Supplier/{id}` - GetSupplierById**
- **الوصف:** الحصول على مورد محدد
- **الطريقة:** GET
- **المعاملات:**
  - `id` (long) - معرف المورد
- **التأثير:** عرض مورد واحد فقط (قراءة فقط)

---

#### **3. POST `/api/Supplier` - CreateSupplier**
- **الوصف:** إنشاء مورد جديد
- **الطريقة:** POST
- **المعاملات:**
  - `Name` (string) - ✅ **مطلوب**
  - `Phone` (string) - ✅ **مطلوب**
  - `WhatsAppPhone` (string) - واتساب
  - `Email` (string) - البريد الإلكتروني
  - `Address` (string) - ✅ **مطلوب**
  - `Currency` (Currency?) - العملة (enum: 0 = SY, 1 = Dollar)
  - `IsActive` (bool) - نشط
- **التأثير:**
  - ✅ إضافة مورد جديد في قاعدة البيانات

---

#### **4. PUT `/api/Supplier` - UpdateSupplier**
- **الوصف:** تحديث مورد موجود
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
  - جميع الحقول الأخرى (نفس CreateSupplier)
- **التأثير:**
  - ✅ تحديث بيانات المورد في قاعدة البيانات

---

#### **5. DELETE `/api/Supplier/{id}` - DeleteSupplier**
- **الوصف:** حذف مورد
- **الطريقة:** DELETE
- **المعاملات:**
  - `id` (long) - معرف المورد
- **التأثير:**
  - ✅ حذف المورد من قاعدة البيانات
  - ⚠️ قد يفشل إذا كان المورد لديه فواتير شراء

---

#### **6. GET `/api/Supplier/GetUnpaidInvoicesBySupplier` - GetUnpaidInvoicesBySupplier**
- **الوصف:** الحصول على الفواتير غير المدفوعة للمورد
- **الطريقة:** GET
- **المعاملات:**
  - `supplierId` (long) - معرف المورد
- **التأثير:** قراءة فقط - عرض فواتير الشراء غير المدفوعة

---

#### **7. GET `/api/Supplier/GetActiveSuppliersSelectList` - GetActiveSuppliersSelectList**
- **الوصف:** قائمة اختيار بالموردين النشطين
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار

---

#### **8. GET `/api/Lookup/GetSupplierSelectList`**
- **الوصف:** قائمة اختيار بجميع الموردين
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار




























## 5️⃣ **الموظفين (Employees)**

### 📋 **Entity: Employee**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **FirstName** | ✅ مطلوب |
| **LastName** | ✅ مطلوب |
| **Phone** | |
| **WhatsAppPhone** | |
| **Email** | |
| **Address** | ✅ مطلوب |
| **JobTitle** | |
| **Salary** | ✅ مطلوب |
| **HireDate** | ✅ مطلوب |
| **IsActive** | |
| **SyrianMoney** | | ل تتبع الرصيد ل المحماصيين 
| **DollarMoney** | | ل تتبع الرصيد ل المحماصيين 
| **SalaryType** | |
| **SalaryPercentage** | |

#### **العلاقات:**
- **RoastingInvoices** → علاقة One-to-Many مع فواتير التحميص
- **DistributionTrips** → علاقة One-to-Many مع رحلات التوزيع

---

### 🔌 **APIs المتاحة:**

#### **1. GET `/api/Employee` - GetPagedListEmployee**
- **الوصف:** الحصول على قائمة الموظفين بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
  - `SearchText` (string) - البحث في الاسم
- **التأثير:** عرض الموظفين فقط (قراءة فقط)

---

#### **2. GET `/api/Employee/{id}` - GetEmployeeById**
- **الوصف:** الحصول على موظف محدد
- **الطريقة:** GET
- **المعاملات:**
  - `id` (long) - معرف الموظف
- **التأثير:** عرض موظف واحد فقط (قراءة فقط)

---
salary type
   Fixed = 1,              // راتب ثابت
   FixedWithPercentage = 2 // راتب ثابت + نسبة من العمولة


#### **3. POST `/api/Employee` - CreateEmployee**
- **الوصف:** إنشاء موظف جديد
- **الطريقة:** POST
- **المعاملات:**
  - `FirstName` (string) - ✅ **مطلوب**
  - `LastName` (string) - ✅ **مطلوب**
  - `Phone` (string) - الهاتف
  - `WhatsAppPhone` (string) - واتساب
  - `Email` (string) - البريد الإلكتروني
  - `Address` (string) - ✅ **مطلوب**
  - `JobTitle` (string) - المسمى الوظيفي
  - `Salary` (decimal) - ✅ **مطلوب**
  - `HireDate` (DateTime) - ✅ **مطلوب**
  - `IsActive` (bool) - نشط
  - `SalaryType` (SalaryType) - نوع الراتب (ثابت/نسبة)
  - `SalaryPercentage` (decimal?)   نسبة الموظف من المبيعات
- **التأثير:**
  - ✅ إضافة موظف جديد في قاعدة البيانات

---

#### **4. PUT `/api/Employee` - UpdateEmployee**
- **الوصف:** تحديث موظف موجود
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
  - جميع الحقول الأخرى (نفس CreateEmployee)
- **التأثير:**
  - ✅ تحديث بيانات الموظف في قاعدة البيانات

---

#### **5. DELETE `/api/Employee/{id}` - DeleteEmployee**
- **الوصف:** حذف موظف
- **الطريقة:** DELETE
- **المعاملات:**
  - `id` (long) - معرف الموظف
- **التأثير:**
  - ✅ حذف الموظف من قاعدة البيانات
  - ⚠️ قد يفشل إذا كان الموظف لديه رحلات توزيع أو فواتير تحميص

---

#### **6. GET `/api/Employee/GetActiveEmployeesSelectList` - GetActiveEmployeesSelectList**
- **الوصف:** قائمة اختيار بالموظفين النشطين
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار

---

#### **7. GET `/api/Employee/GetEmployeesWithRemainingSalary` - GetEmployeesWithRemainingSalary**
- **الوصف:** الحصول على الموظفين مع الرصيد المتبقي من الرواتب
- **الطريقة:** GET
- **المعاملات:**
  - `Year` (int) - السنة
  - `Month` (int) - الشهر
- **التأثير:** قراءة فقط - عرض الموظفين مع الرصيد المتبقي

---

#### **8. GET `/api/Employee/GetPagedListEmployeeSalary` - GetPagedListEmployeeSalary**
- **الوصف:** الحصول على قائمة الرواتب الشهرية بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
- **التأثير:** قراءة فقط - عرض الرواتب الشهرية

---

#### **9. POST `/api/Employee/CreateSalaryPayment` - CreateSalaryPayment**
- **الوصف:** إنشاء دفع راتب للموظف
- **الطريقة:** POST
- **المعاملات:**
  - `EmployeeId` (long) - ✅ **مطلوب**
  - `Year` (int) - ✅ **مطلوب**
  - `Month` (int) - ✅ **مطلوب**
  - `Amount` (decimal) - ✅ **مطلوب**
  - `PaymentDate` (DateTime) - ✅ **مطلوب**
  - `CashRegisterId` (long?) - معرف الصندوق النقدي
  - `DistributionTripId` (long?) - معرف رحلة التوزيع
  - `Notes` (string) - الملاحظات
  - `SalaryType` (SalaryType) - ✅ **مطلوب**
- **التأثير:**
  - ✅ إضافة دفعة راتب في `SalaryPayments`
  - ✅ تحديث `PaidAmount` في `EmployeeSalary`
  - ✅ تحديث رصيد الصندوق النقدي (إذا كان موجوداً)

---

## 6️⃣ **السيارات (Cars)**

### 📋 **Entity: Car**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **Name** | ✅ مطلوب |
| **PlateNumber** | |
| **Model** | |
| **Year** | |
| **CapacityKg** | |
| **IsAvailable** | |

#### **العلاقات:**
- **DistributionTrips** → علاقة One-to-Many مع رحلات التوزيع

---

### 🔌 **APIs المتاحة:**

#### **1. GET `/api/Car` - GetPagedListCar**
- **الوصف:** الحصول على قائمة السيارات بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
  - `SearchText` (string) - البحث في الاسم
- **التأثير:** عرض السيارات فقط (قراءة فقط)

---

#### **2. GET `/api/Car/{id}` - GetCarById**
- **الوصف:** الحصول على سيارة محددة
- **الطريقة:** GET
- **المعاملات:**
  - `id` (long) - معرف السيارة
- **التأثير:** عرض سيارة واحدة فقط (قراءة فقط)

---

#### **3. POST `/api/Car` - CreateCar**
- **الوصف:** إنشاء سيارة جديدة
- **الطريقة:** POST
- **المعاملات:**
  - `Name` (string) - ✅ **مطلوب**
  - `PlateNumber` (string) - رقم اللوحة
  - `Model` (string) - الموديل
  - `Year` (int?) - السنة
  - `CapacityKg` (decimal?) - السعة بالكيلو
  - `IsAvailable` (bool) - متاحة
- **التأثير:**
  - ✅ إضافة سيارة جديدة في قاعدة البيانات

---

#### **4. PUT `/api/Car` - UpdateCar**
- **الوصف:** تحديث سيارة موجودة
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
  - جميع الحقول الأخرى (نفس CreateCar)
- **التأثير:**
  - ✅ تحديث بيانات السيارة في قاعدة البيانات

---

#### **5. DELETE `/api/Car/{id}` - DeleteCar**
- **الوصف:** حذف سيارة
- **الطريقة:** DELETE
- **المعاملات:**
  - `id` (long) - معرف السيارة
- **التأثير:**
  - ✅ حذف السيارة من قاعدة البيانات
  - ⚠️ قد يفشل إذا كانت السيارة مستخدمة في رحلات توزيع

---

#### **6. GET `/api/Lookup/GetCarSelectList`**
- **الوصف:** قائمة اختيار بجميع السيارات
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار

---

## 7️⃣ **المناطق (Regions)**

### 📋 **Entity: Region**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **Name** | ✅ مطلوب |
| **Description** | ✅ مطلوب |
| **IsActive** | |

#### **العلاقات:**
- **DistributionTrips** → علاقة One-to-Many مع رحلات التوزيع

---

### 🔌 **APIs المتاحة:**

#### **1. GET `/api/Region` - GetPagedListRegion**
- **الوصف:** الحصول على قائمة المناطق بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
  - `SearchText` (string) - البحث في الاسم
- **التأثير:** عرض المناطق فقط (قراءة فقط)

---

#### **2. GET `/api/Region/{id}` - GetRegionById**
- **الوصف:** الحصول على منطقة محددة
- **الطريقة:** GET
- **المعاملات:**
  - `id` (long) - معرف المنطقة
- **التأثير:** عرض منطقة واحدة فقط (قراءة فقط)

---

#### **3. POST `/api/Region` - CreateRegion**
- **الوصف:** إنشاء منطقة جديدة
- **الطريقة:** POST
- **المعاملات:**
  - `Name` (string) - ✅ **مطلوب**
  - `Description` (string) - ✅ **مطلوب**
  - `IsActive` (bool) - نشط
- **التأثير:**
  - ✅ إضافة منطقة جديدة في قاعدة البيانات

---

#### **4. PUT `/api/Region` - UpdateRegion**
- **الوصف:** تحديث منطقة موجودة
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
  - `Name` (string) - ✅ **مطلوب**
  - `Description` (string) - ✅ **مطلوب**
  - `IsActive` (bool) - نشط
- **التأثير:**
  - ✅ تحديث بيانات المنطقة في قاعدة البيانات

---

#### **5. GET `/api/Lookup/GetRegionSelectList`**
- **الوصف:** قائمة اختيار بجميع المناطق
- **الطريقة:** GET
- **التأثير:** قراءة فقط - عرض قائمة للاختيار

---

## 8️⃣ **المشتريات (Purchases)**

### 📋 **Entity: PurchaseInvoice**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **InvoiceNumber** | ✅ مطلوب |
| **SupplierId** | ✅ مطلوب |
| **InvoiceDate** | ✅ مطلوب |
| **TotalAmount** | ✅ مطلوب |
| **Discount** | |
| **Notes** | |
| **Currency** | | Currency enum (0 = SY, 1 = Dollar)
| **Status** | ✅ مطلوب |
| **PaymentStatus** | ✅ مطلوب |

#### **العلاقات:**
- **Supplier** → علاقة Many-to-One مع Supplier
- **Details** → علاقة One-to-Many مع PurchaseInvoiceDetail

---

### 📋 **Entity: PurchaseInvoiceDetail**

#### **الحقول:**

| الحقل | مطلوب |
|-------|-------|
| **Id** | ✅ مطلوب |
| **InvoiceId** | ✅ مطلوب |
| **ProductId** | ✅ مطلوب |
| **Quantity** | ✅ مطلوب |
| **UnitPrice** | ✅ مطلوب |
| **TotalPrice** | ✅ مطلوب |
| **Notes** | |

#### **العلاقات:**
- **Invoice** → علاقة Many-to-One مع PurchaseInvoice
- **Product** → علاقة Many-to-One مع Product

---

### 🔌 **APIs المتاحة:**

#### **1. GET `/api/PurchaseInvoice` - GetPagedListPurchaseInvoice**
- **الوصف:** الحصول على قائمة فواتير المشتريات بصفحة
- **الطريقة:** GET
- **المعاملات:**
  - `PageNumber` (int) - رقم الصفحة
  - `PageSize` (int) - حجم الصفحة
  - `SearchText` (string) - البحث في رقم الفاتورة
- **التأثير:** عرض فواتير المشتريات فقط (قراءة فقط)

---

#### **2. GET `/api/PurchaseInvoice/{id}` - GetPurchaseInvoiceById**
- **الوصف:** الحصول على فاتورة مشتريات محددة
- **الطريقة:** GET
- **المعاملات:**
  - `id` (long) - معرف الفاتورة
- **التأثير:** عرض فاتورة واحدة فقط (قراءة فقط)

---

#### **3. POST `/api/PurchaseInvoice` - CreatePurchaseInvoice**
- **الوصف:** إنشاء فاتورة مشتريات جديدة
- **الطريقة:** POST
- **المعاملات:**
  - `InvoiceNumber` (string) - ✅ **مطلوب**
  - `SupplierId` (long) - ✅ **مطلوب**
  - `InvoiceDate` (DateTime?) - تاريخ الفاتورة
  - `TotalAmount` (decimal) - ✅ **مطلوب**
  - `Discount` (decimal) - الخصم (افتراضي: 0)
  - `Notes` (string) - الملاحظات
  - `Currency` (Currency?) - العملة (enum: 0 = SY, 1 = Dollar)
  - `Details` (List<CreatePurchaseInvoiceDetailItem>) - ✅ **مطلوب** - تفاصيل الفاتورة
    - `ProductId` (long) - ✅ **مطلوب**
    - `Quantity` (decimal) - ✅ **مطلوب**
    - `UnitPrice` (decimal) - ✅ **مطلوب**
    - `TotalPrice` (decimal) - ✅ **مطلوب**
    - `Notes` (string) - الملاحظات
- **التأثير:**
  - ✅ إضافة فاتورة مشتريات جديدة في قاعدة البيانات
  - ✅ إضافة تفاصيل الفاتورة
  - ✅ تحديث المخزون (إذا كانت الفاتورة Posted)

---

#### **4. PUT `/api/PurchaseInvoice` - UpdatePurchaseInvoice**
- **الوصف:** تحديث فاتورة مشتريات موجودة
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
  - `InvoiceNumber` (string)
  - `SupplierId` (long?)
  - `InvoiceDate` (DateTime?)
  - `TotalAmount` (decimal?)
  - `Discount` (decimal?)
  - `Notes` (string)
  - `Currency` (Currency?) - العملة (enum: 0 = SY, 1 = Dollar)
  - `Status` (PurchaseInvoiceStatus?)
  - `PaymentStatus` (PurchasePaymentStatus?)
  - `Details` (List<UpdatePurchaseInvoiceDetailItem>)
- **التأثير:**
  - ✅ تحديث بيانات الفاتورة في قاعدة البيانات
  - ✅ تحديث تفاصيل الفاتورة
  - ⚠️ لا يمكن التحديث إذا كانت الفاتورة Posted

---

#### **5. PUT `/api/PurchaseInvoice/Post` - PostPurchaseInvoice**
- **الوصف:** تأكيد (Post) فاتورة مشتريات
- **الطريقة:** PUT
- **المعاملات:**
  - `Id` (long) - ✅ **مطلوب**
- **التأثير:**
  - ✅ تغيير حالة الفاتورة إلى Posted
  - ✅ تحديث المخزون (إضافة الكميات المشتراة)
  - ✅ لا يمكن إلغاء Post بعد التأكيد

---

#### **6. DELETE `/api/PurchaseInvoice/{id}` - DeletePurchaseInvoice**
- **الوصف:** حذف فاتورة مشتريات
- **الطريقة:** DELETE
- **المعاملات:**
  - `id` (long) - معرف الفاتورة
- **التأثير:**
  - ✅ حذف الفاتورة من قاعدة البيانات
  - ⚠️ قد يفشل إذا كانت الفاتورة Posted أو لديه مدفوعات

---

#### **7. GET `/api/Supplier/GetUnpaidInvoicesBySupplier` - GetUnpaidInvoicesBySupplier**
- **الوصف:** الحصول على الفواتير غير المدفوعة للمورد
- **الطريقة:** GET
- **المعاملات:**
  - `supplierId` (long) - معرف المورد
- **التأثير:** قراءة فقط - عرض فواتير المشتريات غير المدفوعة

---

#### **8. GET `/api/Lookup/GetPurchaseInvoicesBySupplier`**
- **الوصف:** قائمة فواتير المشتريات لمورد محدد
- **الطريقة:** GET
- **المعاملات:**
  - `supplierId` (long) - معرف المورد
- **التأثير:** قراءة فقط - عرض فواتير المورد Posted فقط

---
