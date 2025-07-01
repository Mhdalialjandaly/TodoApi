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
add-migration initial 
ويجب اضافة معلومات الاتصال بال sql server  في الكلاس(ApiDbExtendedContext) الخاص بالاتصال بال sql server 

// Uncomment the next line only when adding a new migration
//optionsBuilder.UseSqlServer("Server=??,1433;Database=DBNam;User Id=UserName;Password=Passwordofuser;TrustServerCertificate=true;");

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
  "email": "Admin@email2.com",
  "password": "Admin@123456",
  "confirmPassword": "Admin@123456",
  "firstName": "John",
  "lastName": "Doe",
  "role": "owner"
}
2. تسجيل الدخول:
text
POST /api/auth/login
{
  "email": "Admin@email2.com",
  "password": "Admin@123456"
}
الافتراضي :

POST /api/auth/login
{
  "email": "Admin@email.com",
  "password": "Admin@12345"
}

4. إنشاء مهمة جديدة:
text
POST /api/todos
Authorization: Bearer {token}
{
  "title": "اجتماع فريق العمل",
  "description": "مناقشة متطلبات المشروع القادم",
  "priority": "3",
  "categoryId": 1
}
وثائق API 📚
يمكنك الوصول إلى وثائق API التفاعلية عبر Swagger UI بعد تشغيل التطبيق:

text
api url : http://localhost:53874/swagger/index.html
الاختبارات 🧪
لتشغيل الاختبارات:

bash
dotnet test
نماذج الاختبار مع Postman
يمكنك استيراد مجموعة Postman الجاهزة من مجلد /postman في المستودع.

