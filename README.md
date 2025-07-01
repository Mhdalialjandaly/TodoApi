نظام متكامل لإدارة المهام اليومية باستخدام ASP.NET Core Web API مع معماريـة متكاملة وتصميم احترافي.

المميزات الرئيسية ✨
✅ إدارة المهام (إنشاء، قراءة، تحديث، حذف)

🔐 نظام مصادقة متكامل باستخدام JWT

👥 إدارة مستخدمين مع أدوار مختلفة (Owner, Guest)

🗃️ تصنيف المهام حسب الأولوية والفئة

🔍 بحث وتصفية مع ترقيم الصفحات

📊 لوحة تحكم إحصائية

🐳 دعم Docker للتنصيب السهل

التقنيات المستخدمة 🛠️
Backend
ASP.NET Core 8.0

Entity Framework Core

AutoMapper

FluentValidation

JWT Authentication

Swagger/OpenAPI

Database
SQL Server / PostgreSQL

Entity Framework Core Migrations

التطوير
xUnit (Unit/Integration Tests)

Docker & Docker Compose

Git Version Control

الهيكل المعماري 🏗️
text
TodoList.Api/
├── Core/          # منطق التطبيق الأساسي
├── DataAccess/# البنية التحتية (قواعد البيانات، الريبوزيتوريز)
├── WebApi/        # طبقة العرض (Controllers, Services)
├── Models/        # طبقة الكائنات لنقل البيانات بين الطبقات أو عبر الشبكة/API  (DTO (Data Transfer Object))
└── Tests/         # الاختبارات
متطلبات التشغيل ⚙️
.NET 8.0 SDK

Docker (اختياري)

قاعدة بيانات (SQL Server أو PostgreSQL)

كيفية التنصيب 🚀
1. بدون استخدام Docker:
bash
# استنساخ المستودع
git clone https://github.com/yourusername/todolist-api.git
cd todolist-api

# استعادة الحزم
dotnet restore

# تعديل سلسلة الاتصال في appsettings.json

# تطبيق الهجرات
dotnet ef database update

# تشغيل التطبيق
dotnet run
2. باستخدام Docker:
bash
docker-compose up --build
كيفية الاستخدام 📖
1. التسجيل كمستخدم جديد:
text
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "P@ssw0rd123!",
  "confirmPassword": "P@ssw0rd123!",
  "firstName": "John",
  "lastName": "Doe",
  "role": "Guest"
}
2. تسجيل الدخول:
text
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "P@ssw0rd123!"
}
3. إنشاء مهمة جديدة:
text
POST /api/todos
Authorization: Bearer {token}
{
  "title": "اجتماع فريق العمل",
  "description": "مناقشة متطلبات المشروع القادم",
  "priority": "High",
  "categoryId": 1
}
وثائق API 📚
يمكنك الوصول إلى وثائق API التفاعلية عبر Swagger UI بعد تشغيل التطبيق:

text
http://localhost:5000/swagger
الاختبارات 🧪
لتشغيل الاختبارات:

bash
dotnet test
نماذج الاختبار مع Postman
يمكنك استيراد مجموعة Postman الجاهزة من مجلد /postman في المستودع.

المساهمة 🤝
ترحب المساهمات! يرجى اتباع الخطوات التالية:

عمل Fork للمشروع

إنشاء فرع جديد (git checkout -b feature/AmazingFeature)

عمل Commit للتغييرات (git commit -m 'Add some AmazingFeature')

Push إلى الفرع (git push origin feature/AmazingFeature)

فتح طلب دمج (Pull Request)
