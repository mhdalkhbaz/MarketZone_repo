# MarketZone - نظام إدارة المدرسة الشامل

## 📚 نظرة عامة

MarketZone هو نظام إدارة مدرسي متكامل ومتطور مصمم خصيصاً للمدارس الحديثة. يوفر النظام حلاً شاملاً لإدارة جميع جوانب العملية التعليمية من خلال واجهة عربية سهلة الاستخدام وميزات متقدمة.

## ✨ الميزات الرئيسية

### 🎯 لوحة التحكم الشاملة
- نظرة شاملة على أداء المدرسة
- إحصائيات مباشرة ومحدثة
- مؤشرات الأداء الرئيسية
- تقارير فورية

### 👥 إدارة الطلاب
- تسجيل الطلاب الجدد
- إدارة البيانات الشخصية
- متابعة الحضور والغياب
- إدارة الدرجات والتقييمات
- تقارير الأداء التفصيلية

### 👨‍🏫 إدارة المعلمين
- ملفات المعلمين الشاملة
- إدارة الجداول الدراسية
- متابعة الحضور
- تقييم الأداء
- إدارة المواد الدراسية

### 🏫 إدارة الصفوف
- إنشاء وإدارة الصفوف
- توزيع الطلاب
- إدارة السعة والإشغال
- الجداول الدراسية
- إدارة القاعات

### 📊 نظام الحضور الذكي
- تسجيل الحضور والغياب
- متابعة المتأخرين
- تقارير الحضور الشهرية
- إشعارات أولياء الأمور
- تحليل أنماط الحضور

### 📝 إدارة الدرجات والامتحانات
- إدخال الدرجات
- تحليل الأداء الأكاديمي
- تقارير التقدم
- مقارنات الأداء
- إدارة الامتحانات

### 📅 الجداول الدراسية
- إنشاء الجداول بسهولة
- توزيع الحصص
- إدارة القاعات
- تعديل الجداول بمرونة
- تجنب التعارضات

### 📈 التقارير والتحليلات
- تقارير الحضور والغياب التفصيلية
- تحليل الدرجات والأداء الأكاديمي
- تقارير السلوك والانضباط
- تتبع تقدم الطلاب عبر الزمن
- تقارير قابلة للطباعة والتصدير

### 🔔 نظام الإشعارات
- إرسال إشعارات للطلاب والمعلمين
- إشعارات أولياء الأمور
- جدولة الإشعارات
- متابعة معدل القراءة
- أنواع مختلفة من الإشعارات

## 🛠️ التقنيات المستخدمة

### Backend
- **.NET 8** - إطار العمل الرئيسي
- **Entity Framework Core** - إدارة قاعدة البيانات
- **ASP.NET Core Web API** - واجهة برمجة التطبيقات
- **MediatR** - نمط Mediator للتعامل مع الطلبات
- **AutoMapper** - تحويل البيانات
- **FluentValidation** - التحقق من صحة البيانات

### Frontend
- **React.js** - واجهة المستخدم
- **TypeScript** - لغة البرمجة
- **Material-UI** - مكتبة المكونات
- **Redux Toolkit** - إدارة الحالة
- **React Query** - إدارة البيانات

### Database
- **SQL Server** - قاعدة البيانات الرئيسية
- **Entity Framework Migrations** - إدارة التحديثات

### Authentication & Security
- **ASP.NET Core Identity** - إدارة المستخدمين
- **JWT Tokens** - المصادقة
- **Role-based Authorization** - التحكم في الصلاحيات

## 🚀 التثبيت والتشغيل

### المتطلبات الأساسية
- .NET 8 SDK
- SQL Server 2019 أو أحدث
- Node.js 18 أو أحدث
- npm أو yarn

### خطوات التثبيت

1. **استنساخ المشروع**
```bash
git clone https://github.com/your-username/MarketZone.git
cd MarketZone
```

2. **إعداد قاعدة البيانات**
```bash
# تحديث connection string في appsettings.json
# تشغيل الترحيلات
dotnet ef database update --project Src/Infrastructure/MarketZone.Infrastructure.Persistence
```

3. **تشغيل Backend**
```bash
cd Src/Presentation/MarketZone.WebApi
dotnet run
```

4. **تشغيل Frontend**
```bash
cd frontend
npm install
npm start
```

## 📁 بنية المشروع

```
MarketZone/
├── Src/
│   ├── Core/
│   │   ├── MarketZone.Application/     # طبقة التطبيق
│   │   └── MarketZone.Domain/          # طبقة النطاق
│   ├── Infrastructure/
│   │   ├── MarketZone.Infrastructure.Identity/      # إدارة الهوية
│   │   ├── MarketZone.Infrastructure.Persistence/   # قاعدة البيانات
│   │   └── MarketZone.Infrastructure.Resources/     # الموارد
│   └── Presentation/
│       └── MarketZone.WebApi/          # واجهة برمجة التطبيقات
├── Tests/
│   ├── MarketZone.UnitTests/           # اختبارات الوحدة
│   ├── MarketZone.IntegrationTests/    # اختبارات التكامل
│   └── MarketZone.FunctionalTests/     # اختبارات الوظائف
└── Tools/                              # أدوات مساعدة
```

## 🧪 الاختبارات

### تشغيل الاختبارات
```bash
# اختبارات الوحدة
dotnet test Tests/MarketZone.UnitTests

# اختبارات التكامل
dotnet test Tests/MarketZone.IntegrationTests

# اختبارات الوظائف
dotnet test Tests/MarketZone.FunctionalTests
```

## 📊 API Documentation

### نقاط النهاية الرئيسية

#### المصادقة
- `POST /api/account/login` - تسجيل الدخول
- `POST /api/account/register` - تسجيل مستخدم جديد
- `GET /api/account/users` - قائمة المستخدمين

#### الطلاب
- `GET /api/students` - قائمة الطلاب
- `POST /api/students` - إضافة طالب جديد
- `PUT /api/students/{id}` - تحديث بيانات الطالب
- `DELETE /api/students/{id}` - حذف الطالب

#### المعلمين
- `GET /api/teachers` - قائمة المعلمين
- `POST /api/teachers` - إضافة معلم جديد
- `PUT /api/teachers/{id}` - تحديث بيانات المعلم

#### الصفوف
- `GET /api/classes` - قائمة الصفوف
- `POST /api/classes` - إضافة صف جديد
- `PUT /api/classes/{id}` - تحديث بيانات الصف

#### الحضور
- `GET /api/attendance` - سجل الحضور
- `POST /api/attendance` - تسجيل حضور جديد
- `GET /api/attendance/reports` - تقارير الحضور

## 🎥 الفيديو الترويجي

تم إنشاء سكريبت فيديو ترويجي شامل للنظام يتضمن:

### الملفات المتوفرة
- `video_script_arabic.md` - سكريبت باللغة العربية
- `video_script_english.md` - سكريبت باللغة الإنجليزية
- `video_production_checklist.md` - قائمة مراجعة الإنتاج

### محتوى الفيديو
- مقدمة عن النظام
- عرض الميزات الرئيسية
- المشاهد التقنية
- الدعوة للعمل
- معلومات التواصل

## 🤝 المساهمة

نرحب بمساهماتكم! يرجى اتباع الخطوات التالية:

1. Fork المشروع
2. إنشاء فرع للميزة الجديدة (`git checkout -b feature/AmazingFeature`)
3. Commit التغييرات (`git commit -m 'Add some AmazingFeature'`)
4. Push إلى الفرع (`git push origin feature/AmazingFeature`)
5. فتح Pull Request

## 📝 الترخيص

هذا المشروع مرخص تحت رخصة MIT - راجع ملف [LICENSE](LICENSE) للتفاصيل.

## 📞 التواصل

- **الموقع الإلكتروني**: [www.marketzone.com](https://www.marketzone.com)
- **البريد الإلكتروني**: info@marketzone.com
- **الهاتف**: +966-XX-XXXXXXX

## 🙏 الشكر والتقدير

شكراً لجميع المساهمين والمطورين الذين ساعدوا في تطوير هذا النظام.

---

**MarketZone** - إدارة مدرسية ذكية لمستقبل أفضل 🎓

