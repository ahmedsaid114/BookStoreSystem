# Bookstore API — دليل الباك إند

ده **باك إند مستقل فقط** على هيكل المشروع المرجعي `Ecommerce535.API`: لا توجد Views ولا Razor pages ولا ربط أو تعديل لموقع الواجهة. الموقع استُخدم كمرجع للكيانات والوظائف الظاهرة: الكتب، الفلاتر، المؤلفين والناشرين، السلة، المفضلة، الخصومات، الحسابات، التقييمات، والطلبات.

## قاعدة البيانات

قاعدة SQL Server باسم `BookStore535`، وتُدار بـ EF Core migrations. تشمل الجداول:

- `Books`, `Categories`, `Authors`, `Publishers`
- `BookReviews`
- `ShoppingCarts`, `CartItems`, `Wishlists`, `WishlistItems`
- `DiscountCodes`, `BookOrders`, `BookOrderItems`
- جداول ASP.NET Identity للحسابات والأدوار

يُنشئ initializer فئات المتجر التسع الظاهرة في الكتالوج، وأدوار `Customer`, `Admin`, `SuperAdmin`. إنشاء حساب مسؤول اختياري عبر `Admin:Email` و`Admin:Password` في User Secrets؛ لا توجد بيانات مسؤول افتراضية داخل المشروع.

## التشغيل

1. اضبط `ConnectionStrings:DefaultConnection` في `appsettings.json`.
2. أنشئ مسؤولًا محليًا باستخدام `dotnet user-secrets set` كما في README.
3. شغّل `dotnet run`. تُطبّق migrations تلقائيًا عند تشغيل الـ API.
4. افتح `/scalar` لتجربة الـ API.

## المصادقة

المسارات تحت `/api/Identity` تستخدم Identity Cookie:

| Method | Route | الوظيفة |
|---|---|---|
| POST | `/api/Identity/Auth/Register` | تسجيل عميل باستخدام الاسم الأول والأخير والبريد وكلمة المرور |
| POST | `/api/Identity/Auth/Login` | تسجيل الدخول |
| POST | `/api/Identity/Auth/Logout` | تسجيل الخروج |
| GET | `/api/Identity/Profile` | بيانات الحساب الحالي |
| PUT | `/api/Identity/Profile/UpdateProfile` | تحديث الاسم والهاتف |

## كتالوج الكتب

المسارات العامة تحت `/api/Customer`:

| Method | Route | الوظيفة |
|---|---|---|
| GET | `/api/Customer/Books` | قائمة صفحات مع بحث وفلاتر `categoryId`, `publisherId`, `authorId`, `year`, `minPrice`, `maxPrice`, `featured`, وفرز |
| GET | `/api/Customer/Books/{id}` | تفاصيل كتاب ومتوسط تقييمه |
| GET | `/api/Customer/Categories` | فئات الكتب |
| GET | `/api/Customer/Authors` | المؤلفون |
| GET | `/api/Customer/Publishers` | الناشرون |
| GET | `/api/Customer/Books/{bookId}/Reviews` | مراجعات كتاب منشورة |
| POST | `/api/Customer/Books/{bookId}/Reviews` | إضافة تقييم بعد توصيل طلب يحتوي الكتاب |

مثال: `/api/Customer/Books?search=rich&categoryId=1&year=1999&page=1&pageSize=12&sortBy=price`.

## السلة والمفضلة والطلبات

تتطلب هذه المسارات تسجيل الدخول:

| Method | Route | الوظيفة |
|---|---|---|
| GET | `/api/Customer/Cart` | عرض السلة والإجمالي، ويمكن تمرير `?discountCode=CODE` |
| POST | `/api/Customer/Cart/items` | إضافة كتاب وكمية |
| PUT | `/api/Customer/Cart/items/{itemId}` | تغيير الكمية |
| DELETE | `/api/Customer/Cart/items/{itemId}` | إزالة عنصر |
| DELETE | `/api/Customer/Cart` | إفراغ السلة |
| POST | `/api/Customer/Cart/ApplyDiscount` | التحقق من كود الخصم وحساب الإجمالي |
| GET | `/api/Customer/Wishlist` | عرض المفضلة |
| POST | `/api/Customer/Wishlist/items` | إضافة كتاب للمفضلة |
| DELETE | `/api/Customer/Wishlist/items/{bookId}` | إزالة كتاب |
| POST | `/api/Customer/Wishlist/items/{bookId}/move-to-cart` | نقل كتاب للسلة |
| POST | `/api/Customer/Orders/Checkout` | إنشاء الطلب من السلة والتحقق من المخزون والخصم |
| GET | `/api/Customer/Orders` | طلباتي |
| GET | `/api/Customer/Orders/{id}` | تفاصيل طلب |
| POST | `/api/Customer/Orders/{id}/Cancel` | إلغاء طلب Pending وإرجاع الكمية للمخزون |

Checkout يقبل حاليًا `CashOnDelivery` فقط. يحسب الخادم الأسعار والخصم والضريبة والشحن من بيانات قاعدة البيانات والإعدادات؛ لا يقبل الإجماليات من العميل.

## إدارة المتجر

كل `/api/Admin` يتطلب دور `Admin` أو `SuperAdmin`:

- CRUD: `/api/Admin/Books`, `/Categories`, `/Authors`, `/Publishers`, `/DiscountCodes`.
- `GET /api/Admin/Orders` و`GET /api/Admin/Orders/{id}` لمراجعة الطلبات.
- `PUT /api/Admin/Orders/{id}/Status` لتحديث حالة الطلب.

حذف الكتاب يعطّله ويوقف مخزونه للمحافظة على تفاصيل الطلبات السابقة. لا يُحذف تصنيف أو مؤلف أو ناشر مرتبط بكتب. أخطاء المدخلات ترجع `400`, العنصر المفقود `404`, التكرار أو تعارض المخزون `409`, والمصادقة المطلوبة `401/403`.

كل الاستجابات التي تحمل محتوى تستخدم DTOs و`ApiResponse<T>` من `DTOs/Response`. ملف `BookStore535.API.http` يحتوي أمثلة طلبات محلية.
