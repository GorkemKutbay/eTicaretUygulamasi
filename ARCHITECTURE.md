# eTicaret Application Architecture Documentation

## Table of Contents
1. [System Architecture](#system-architecture)
2. [Entity Relationship Model](#entity-relationship-model)
3. [Data Flow Patterns](#data-flow-patterns)
4. [Service Architecture](#service-architecture)
5. [API Contract Details](#api-contract-details)
6. [Security Architecture](#security-architecture)
7. [Development Patterns](#development-patterns)

---

## System Architecture

### Layered Architecture Overview
```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
│  ┌──────────────────────┐          ┌──────────────────────┐ │
│  │  App.Eticaret        │          │  App.Admin           │ │
│  │  (Customer Portal)   │          │  (Admin Panel)       │ │
│  │  MVC with Views      │          │  MVC with Views      │ │
│  └──────────────────────┘          └──────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
              ↓ HTTP Requests            ↓ HTTP Requests
┌──────────────────────────────┐  ┌──────────────────────────────┐
│      APPLICATION LAYER       │  │    API APPLICATION LAYER     │
│  Controllers handle:         │  │  Controllers handle:         │
│  - View rendering           │  │  - HTTP requests             │
│  - Form submission          │  │  - JSON serialization        │
│  - Auth redirection         │  │  - Status codes              │
│  - Model to DTO mapping     │  │  - Error handling            │
└──────────────────────────────┘  └──────────────────────────────┘
              ↓                              ↓
┌─────────────────────────────────────────────────────────────┐
│                    SERVICE LAYER                            │
│  - IAuthService          - IOrderService                   │
│  - IProductService       - ICartService                    │
│  - ICategoryService      - ICommentService                 │
│  - IUserService          - IFileService                    │
│  - IProfileService       - IMailService                    │
│                                                             │
│  Business Logic:                                           │
│  - Validation & authorization                             │
│  - Cross-entity operations                                │
│  - Email notifications                                    │
│  - File operations                                        │
└─────────────────────────────────────────────────────────────┘
              ↓
┌──────────────────────────────────┐  ┌──────────────────────┐
│  DATA PERSISTENCE LAYER          │  │  FILE STORAGE LAYER  │
│  - App.Data DbContext            │  │  - File API Service  │
│  - Entity Configurations         │  │  - /uploads/         │
│  - LINQ queries                  │  │  - File system       │
│  - Data aggregation              │  │                      │
└──────────────────────────────────┘  └──────────────────────┘
              ↓
┌──────────────────────────────────────────────────────────────┐
│                    DATA STORAGE LAYER                        │
│  SQL Server Database with:                                  │
│  - User, Product, Order, Cart, Blog tables                 │
│  - Relationships & constraints                             │
│  - Indexed columns for performance                         │
└──────────────────────────────────────────────────────────────┘
```

### Cross-Cutting Concerns
- **Authentication**: JWT validation middleware
- **Authorization**: Role-based access control
- **Validation**: FluentValidation in DTOs
- **Error Handling**: Result pattern for consistent responses
- **Logging**: Implicit via framework

---

## Entity Relationship Model

### Complete Entity Diagram
```
┌─────────────┐
│    Role     │ (Lookup: admin, seller, buyer)
│─────────────│
│ Id (PK)     │
│ Name        │
│ CreatedAt   │
└─────────────┘
     ▲
     │ 1:many
     │
┌─────────────┐        ┌──────────────┐
│    User     │───────→│   Product    │ (as Seller)
│─────────────│        │──────────────│
│ Id (PK)     │        │ Id (PK)      │
│ Email (U)   │        │ SellerId(FK) │──┐
│ FirstName   │        │ CategoryId(FK)  │
│ LastName    │        │ DiscountId(FK)  │
│ Password    │        │ Name         │  │
│ RoleId(FK)  │        │ Price        │  │
│ Enabled     │        │ Description  │  │
│ HasSellerReq│        │ StockAmount  │  │
│ CreatedAt   │        │ Enabled      │  │
└─────────────┘        │ CreatedAt    │  │
     ▲                 └──────────────┘  │
     │ 1:many               ▲    ▲       │
     │                      │    │       │
     │            ┌─────────┘    │       │
     │            │              │       │
     │ 1:many  ┌──────────────────┘    │
     │         │                       │
├─────────────┤ 1:many         ┌──────────────────────┐
│  CartItem   │                │   ProductImage       │
│─────────────│                │──────────────────────│
│ Id (PK)     │                │ Id (PK)              │
│ UserId(FK)  │                │ ProductId(FK)        │
│ ProductId   │                │ Url                  │
│ Quantity    │                │ CreatedAt            │
│ CreatedAt   │                │ (Cascade Delete)     │
└─────────────┘                └──────────────────────┘
     
     │ 1:many             ┌──────────────────────┐
     └────────────────────│ ProductComment       │
                          │──────────────────────│
                          │ Id (PK)              │
                          │ ProductId(FK)        │
                          │ UserId(FK)           │
                          │ Text                 │
                          │ StarCount (1-5)      │
                          │ IsConfirmed          │
                          │ CreatedAt            │
                          └──────────────────────┘

┌──────────────┐
│   Category   │ (Lookup: 9 seeded)
│──────────────│
│ Id (PK)      │
│ Name         │
│ Color        │
│ IconCssClass │
│ CreatedAt    │
└──────────────┘
     ▲
     │ 1:many
     │ (FK from Product)

┌──────────────┐
│   Discount   │ (Lookup: 3 seeded)
│──────────────│
│ Id (PK)      │
│ DiscountRate │
│ StartDate    │
│ EndDate      │
│ Enabled      │
│ CreatedAt    │
└──────────────┘
     ▲
     │ 0..1:many
     │ (FK from Product)

User (1) ───────→ (many) Order
│                         │
│                    1:many
│                         │
└──────────→ OrderItem ←──┘

Order ────────→ OrderItem
                    │
                    └──→ Product (referenced, no delete)

Blog (created by User) ────→ BlogComment (Cascade Delete)
                                  │
                            (Comments on blogs)

User ─────→ Blog, ProductComment, CartItem, Order, BlogComment
            (1:many relationships for audit trail)
```

### Database Constraints & Indexes
```
UNIQUE CONSTRAINTS:
- User.Email
- Role.Name
- Order.OrderCode
- OrderItem(OrderId, ProductId) - Composite unique

INDEXES:
- User.Email (for login queries)
- Product.SellerId (for seller inventory)
- Product.CategoryId (for category browsing)
- Order.UserId (for order history)
- Order.OrderCode (for lookup)
- CartItem(UserId, ProductId) - For quick lookups

FOREIGN KEY CONSTRAINTS:
- All relationships have OnDelete.NoAction
  (except ProductImage and BlogComment with Cascade)
- Prevents accidental cascading deletes
```

---

## Data Flow Patterns

### 1. User Login Flow
```
Client Browser (POST /auth/login)
  ↓
App.Eticaret Controller
  ├─→ Validate input (BaseController)
  ├─→ Call AuthApiService.LoginAsync()
  │    ├─→ HttpClient POST to Api.Data /auth/login
  │    │    ├─→ Api.Data AuthController
  │    │    ├─→ IAuthService.LoginAsync()
  │    │    │    ├─→ Find user by email
  │    │    │    ├─→ Verify BCrypt password
  │    │    │    ├─→ Generate JWT token
  │    │    │    └─→ Return AuthLoginResult (token)
  │    │    └─→ Return Result<AuthLoginResult>
  │    └─→ Extract token from response
  └─→ Set "auth-token" HttpOnly cookie
  └─→ Redirect to dashboard

Server-side validation:
  └─→ Bearer token in header or "auth-token" cookie
  └─→ Extract claims from token
  └─→ [Authorize] middleware validates expiration
```

### 2. Product Creation Flow (Seller)
```
Admin/Seller Form (multipart POST /products)
  ↓
App.Eticaret ProductController
  ├─→ [Authorize(Roles = "seller")]
  ├─→ Bind ProductCreateViewModel
  ├─→ Validate model
  ├─→ Call FileApiService.UploadAsync(image)
  │    └─→ POST to Api.File /upload
  │    └─→ Returns FileUploadResult {FileName}
  ├─→ Call ProductApiService.CreateAsync()
  │    ├─→ HttpClient POST to Api.Data /products
  │    │    ├─→ Api.Data ProductController
  │    │    ├─→ IProductService.CreateAsync(ProductCreateRequest)
  │    │    │    ├─→ Find seller (User)
  │    │    │    ├─→ Validate category exists
  │    │    │    ├─→ Check stock amount > 0
  │    │    │    ├─→ Create ProductEntity
  │    │    │    ├─→ Create ProductImageEntity
  │    │    │    ├─→ dbContext.SaveChangesAsync()
  │    │    │    └─→ Return ProductCreateResult
  │    │    └─→ Return Result<ProductCreateResult>
  │    └─→ Extract product ID from response
  └─→ Set success message
  └─→ Redirect to product detail

Data persists:
  └─→ ProductEntity inserted with SellerId = current user
  └─→ ProductImageEntity linked via ProductId (FK)
  └─→ File stored in /uploads/ directory
```

### 3. Shopping Cart to Order Flow
```
Customer clicks "Checkout"
  ↓
CartController.CheckoutAsync()
  ├─→ [Authorize]
  ├─→ GET cart items for UserId
  │    ├─→ Api.Data /cart
  │    └─→ ICartService.GetCartAsync(UserId)
  │    └─→ Query CartItem entities for user
  │    └─→ Join with Product, ProductImage
  │    └─→ Return CartGetResult[] { ProductName, Price, Quantity, Images[] }
  │
  ├─→ POST /orders (OrderPlaceRequest {items, address})
  │    ├─→ Api.Data OrderController
  │    ├─→ IOrderService.PlaceOrderAsync(OrderPlaceRequest)
  │    │    ├─→ Verify all products exist
  │    │    ├─→ Verify stock > requested quantity
  │    │    ├─→ Calculate order total with discounts
  │    │    ├─→ Create OrderEntity with unique OrderCode
  │    │    ├─→ For each CartItem:
  │    │    │    ├─→ Create OrderItemEntity (captures UnitPrice)
  │    │    │    ├─→ Decrement Product.StockAmount
  │    │    │    └─→ Delete CartItemEntity
  │    │    ├─→ dbContext.SaveChangesAsync()
  │    │    ├─→ Send confirmation email (IMailService)
  │    │    └─→ Return OrderPlaceResult {OrderId, OrderCode}
  │    └─→ Return Result<OrderPlaceResult>
  │
  ├─→ Clear cart (delete all CartItem)
  ├─→ Send order confirmation email
  └─→ Redirect to /order/{orderId}

Data changes:
  ├─→ NEW: OrderEntity + OrderItemEntity rows
  ├─→ UPDATED: ProductEntity.StockAmount decremented
  ├─→ DELETED: CartItemEntity rows for this user
  └─→ EMAIL: Sent via IMailService (SMTP)
```

### 4. Product Review Flow
```
Customer reviews product
  ├─→ POST /products/{id}/reviews
  │    ├─→ Api.Data ProductController
  │    ├─→ [Authorize]
  │    ├─→ ICommentService.CreateReviewAsync(ProductReviewRequest)
  │    │    ├─→ Verify ProductId exists
  │    │    ├─→ Check user hasn't already reviewed
  │    │    ├─→ Create ProductCommentEntity
  │    │    ├─→ IsConfirmed = false (awaits admin approval)
  │    │    ├─→ StarCount = request.Stars (1-5)
  │    │    ├─→ Text = request.ReviewText
  │    │    ├─→ dbContext.SaveChangesAsync()
  │    │    └─→ Return CommentCreateResult {CommentId}
  │    └─→ Return Result<CommentCreateResult>
  │
  └─→ Admin dashboard shows pending reviews
       ├─→ GET /comments (IsConfirmed == false)
       └─→ POST /comments/{id}/approve
            ├─→ ICommentService.ApproveAsync(commentId)
            ├─→ Update ProductCommentEntity.IsConfirmed = true
            ├─→ dbContext.SaveChangesAsync()
            └─→ Review visible on product page

Query for product reviews:
  └─→ ProductEntity.Comments (ICollection<ProductCommentEntity>)
  └─→ WHERE IsConfirmed == true
  └─→ Include User, Product
  └─→ Calculate average rating
```

### 5. Admin Blog Management Flow
```
Admin creates blog post
  ├─→ POST /blogs
  │    ├─→ Api.Data BlogController
  │    ├─→ [Authorize(Roles = "admin")]
  │    ├─→ IBlogService.CreateAsync(BlogCreateRequest)
  │    │    ├─→ Create BlogEntity
  │    │    ├─→ UserId = current admin
  │    │    ├─→ Enabled = true
  │    │    ├─→ ImageUrl = file path from upload
  │    │    ├─→ dbContext.SaveChangesAsync()
  │    │    └─→ Return BlogCreateResult
  │    └─→ Return Result<BlogCreateResult>
  │
Visitors comment on blog:
  ├─→ POST /blogs/{id}/comments
  │    ├─→ BlogCommentEntity created
  │    ├─→ IsApproved = false (needs admin review)
  │    ├─→ dbContext.SaveChangesAsync()
  │    └─→ Return CommentCreateResult
  │
Admin approves blog comments:
  └─→ GET /admin/comments?pending=true
       ├─→ Admin sees BlogCommentEntity.IsApproved == false
       ├─→ POST /comments/{id}/approve
       ├─→ Update BlogCommentEntity.IsApproved = true
       └─→ Comment visible on blog

Data relationships:
  └─→ BlogEntity (1) ← BlogCommentEntity (many, Cascade Delete)
  └─→ On blog delete → all comments auto-deleted
```

---

## Service Architecture

### Service Layer Patterns

#### 1. IAuthService Implementation
```csharp
// Interface defined in App.Services/Abstract/
public interface IAuthService
{
    Task<Result<AuthLoginResult>> LoginAsync(AuthLoginRequest request);
    Task<Result> RegisterAsync(AuthRegisterRequest request);
    Task<Result> ForgotPasswordAsync(AuthForgotPasswordRequest request);
    Task<Result> ResetPasswordAsync(AuthResetPasswordRequest request);
    Task<Result<AuthRefreshTokenResult>> RefreshTokenAsync(AuthRefreshTokenRequest request);
}

// Implementation: App.Services/Concrete/AuthApiService
public class AuthApiService : AppServiceBase, IAuthService
{
    private HttpClient Client { get; }

    public async Task<Result<AuthLoginResult>> LoginAsync(AuthLoginRequest request)
    {
        // Forward HTTP request to Api.Data endpoint
        var response = await Client.PostAsJsonAsync("api/v1/auth/login", request);
        
        // Api.Data.AuthController.Login() handles:
        // 1. Find user by email
        // 2. Verify BCrypt password
        // 3. Generate JWT token
        // 4. Return token
        
        return await response.Content.ReadAsAsync<Result<AuthLoginResult>>();
    }
}
```

#### 2. Service Registration Pattern
```csharp
// In Program.cs
builder.Services
    .AddScoped<IAuthService, AuthApiService>()
    .AddScoped<IProductService, ProductApiService>()
    .AddScoped<ICartService, CartApiService>()
    // ... all services registered
    .AddHttpClient("Api.Data", client =>
    {
        client.BaseAddress = new Uri(apiDataUrl);
    });
```

#### 3. AppServiceBase Pattern
```csharp
public abstract class AppServiceBase
{
    // Provides common functionality:
    // - Logging
    // - Error handling
    // - HttpClient access
    // - Configuration access
}
```

### Service Interaction Diagram
```
MVC Controller
      ↓
IProductService (interface)
      ├─→ Implementation: ProductApiService
      │   └─→ Uses HttpClient("Api.Data")
      │   └─→ Calls Api.Data REST endpoints
      │
Api.Data ProductController
      ├─→ Uses IProductService (local implementation)
      │   └─→ Accesses IProductService in App.Api.Data
      │
App.Data DbContext
      └─→ EF Core queries & saves
```

---

## API Contract Details

### Authentication Flow
```
1. POST /api/v1/auth/login
   Request:  { email, password, rememberMe }
   Response: { isSuccess, value: { token }, errors }
   
2. Browser sets cookie: auth-token = {token}
   
3. Subsequent requests:
   Header: Authorization: Bearer {token}
   OR
   Cookie: auth-token={token}
   
4. JwtBearerDefaults.AuthenticationScheme validates:
   ├─→ Signature (HMAC)
   ├─→ Issuer (matches config)
   ├─→ Expiration (ClockSkew = TimeSpan.Zero)
   ├─→ Claims extracted for [Authorize] checks
   └─→ User identity set in HttpContext.User
```

### Result Pattern Response
```json
Success Response:
{
  "isSuccess": true,
  "value": {
    "id": 123,
    "name": "Product Name",
    // ... data fields
  },
  "errors": []
}

Error Response:
{
  "isSuccess": false,
  "value": null,
  "errors": [
    "Product not found",
    "Insufficient stock"
  ]
}

Paged Response:
{
  "isSuccess": true,
  "value": {
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "items": [ /* array of results */ ]
  },
  "errors": []
}
```

### DTO Validation Examples

#### AuthLoginRequest
```csharp
public class AuthLoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

public class AuthLoginRequestValidator : AbstractValidator<AuthLoginRequest>
{
    public AuthLoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(4);
    }
}
```

#### ProductGetResult
```csharp
public class ProductGetResult
{
    public int Id { get; set; }
    public int SellerId { get; set; }
    public int CategoryId { get; set; }
    public int? DiscountId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public byte StockAmount { get; set; }
    public bool Enabled { get; set; } = true;
}
```

---

## Security Architecture

### JWT Token Structure
```
Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload (Claims):
{
  "sub": "user@example.com",
  "name": "John Doe",
  "role": ["buyer"],
  "nbf": 1717324800,
  "exp": 1717411200,
  "iat": 1717324800,
  "iss": "your-app-name"
}

Signature: HMAC-SHA256(header + payload, secret_key)
```

### Authentication Middleware
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,          // Check expiration
        ClockSkew = TimeSpan.Zero,        // No grace period
        ValidateIssuer = true,            // Verify issuer
        ValidIssuer = config["Jwt:Issuer"],
        ValidateAudience = false,         // Open API
        ValidateIssuerSigningKey = false, // Custom validation
        SignatureValidator = (token, _) => new JsonWebToken(token)
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Read token from cookie if not in header
            context.Token = context.Request.Cookies["auth-token"];
            return Task.CompletedTask;
        }
    };
});
```

### Password Security
```csharp
// Hashing (registration):
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

// Verification (login):
bool isValid = BCrypt.Net.BCrypt.Verify(rawPassword, storedHash);

// Properties:
// - Each hash includes unique salt
// - Work factor prevents brute force
// - No plaintext passwords stored
```

### Authorization Attributes
```csharp
// Public endpoints
[AllowAnonymous]
public IActionResult Login() { ... }

// Authenticated users only
[Authorize]
public IActionResult Profile() { ... }

// Role-specific
[Authorize(Roles = "admin,seller")]
public IActionResult ManageProducts() { ... }

// Claim-based
[Authorize(Policy = "SellerOnly")]
public IActionResult SellerDashboard() { ... }
```

---

## Development Patterns

### Adding a New Feature (Example: Product Wishlist)

#### 1. Create Entity (App.Data/Entities/WishlistEntity.cs)
```csharp
public class WishlistEntity : EntityBase
{
    public int UserId { get; set; }
    public int ProductId { get; set; }

    public UserEntity User { get; set; } = null!;
    public ProductEntity Product { get; set; } = null!;
}

internal class WishlistEntityConfiguration : IEntityTypeConfiguration<WishlistEntity>
{
    public void Configure(EntityTypeBuilder<WishlistEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.ProductId).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasOne(d => d.User).WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(d => d.Product).WithMany()
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
```

#### 2. Create DTOs (App.Models.DTO/Wishlist/)
```csharp
namespace App.Models.DTO.Wishlist
{
    public class WishlistAddRequest
    {
        public int ProductId { get; set; }
    }

    public class WishlistGetResult
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}
```

#### 3. Create Service Interface (App.Services/Abstract/IWishlistService.cs)
```csharp
public interface IWishlistService
{
    Task<Result> AddToWishlistAsync(int userId, WishlistAddRequest request);
    Task<Result> RemoveFromWishlistAsync(int userId, int productId);
    Task<Result<IList<WishlistGetResult>>> GetWishlistAsync(int userId);
}
```

#### 4. Create API Controller (App.Api.Data/Controllers/WishlistController.cs)
```csharp
[Route("api/v1/[controller]")]
[ApiController]
public class WishlistController(IWishlistService wishlistService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [TranslateResultToActionResult]
    public async Task<Result> AddToWishlist(WishlistAddRequest request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        return await wishlistService.AddToWishlistAsync(userId, request);
    }

    [HttpGet]
    [Authorize]
    [TranslateResultToActionResult]
    public async Task<Result<IList<WishlistGetResult>>> GetWishlist()
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        return await wishlistService.GetWishlistAsync(userId);
    }
}
```

#### 5. Implement Service (App.Api.Data/Services/WishlistService.cs)
```csharp
public class WishlistService(ApplicationDbContext dbContext) : IWishlistService
{
    public async Task<Result> AddToWishlistAsync(int userId, WishlistAddRequest request)
    {
        var product = await dbContext.Products.FindAsync(request.ProductId);
        if (product == null)
            return Result.NotFound();

        var existing = await dbContext.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == request.ProductId);
        if (existing != null)
            return Result.Conflict("Already in wishlist");

        var wishlistItem = new WishlistEntity
        {
            UserId = userId,
            ProductId = request.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Wishlists.Add(wishlistItem);
        await dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<IList<WishlistGetResult>>> GetWishlistAsync(int userId)
    {
        var items = await dbContext.Wishlists
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
            .Select(w => new WishlistGetResult
            {
                Id = w.Id,
                ProductId = w.Product.Id,
                ProductName = w.Product.Name,
                Price = w.Product.Price
            })
            .ToListAsync();

        return Result.Success(items);
    }
}
```

#### 6. Register Service (App.Api.Data/Program.cs)
```csharp
builder.Services.AddScoped<IWishlistService, WishlistService>();
```

#### 7. Update Frontend (App.Eticaret/Controllers/WishlistController.cs)
```csharp
public class WishlistController(IHttpClientFactory factory) : BaseController
{
    private HttpClient Client => factory.CreateClient("Api.Data");

    [HttpPost("add/{productId:int}")]
    [Authorize]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var response = await Client.PostAsJsonAsync("/api/v1/wishlist", 
            new { productId });
        // Handle response
    }
}
```

#### 8. Run Migrations
```bash
Add-Migration AddWishlist
Update-Database
```

---

## Performance Optimization Patterns

### Indexing Strategy
```sql
-- High-cardinality columns
CREATE INDEX IX_User_Email ON Users(Email);
CREATE INDEX IX_Product_SellerId ON Products(SellerId);
CREATE INDEX IX_Product_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Order_UserId ON Orders(UserId);

-- Foreign key lookups
CREATE INDEX IX_CartItem_UserId_ProductId ON CartItems(UserId, ProductId);
CREATE INDEX IX_OrderItem_OrderId_ProductId ON OrderItems(OrderId, ProductId);
```

### Query Optimization Patterns
```csharp
// ❌ N+1 Query Problem
var products = dbContext.Products.ToList(); // Query 1
foreach(var product in products)
{
    var seller = dbContext.Users.Find(product.SellerId); // Query N times
}

// ✅ Use Include for eager loading
var products = dbContext.Products
    .Include(p => p.Seller)
    .Include(p => p.Images)
    .ToList(); // Single query with joins

// ✅ Use Select for projection (reduces columns)
var productDtos = dbContext.Products
    .Select(p => new ProductGetResult
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    })
    .ToList(); // Only fetches needed columns
```

---

## Common Development Tasks

### Debugging
```csharp
// Enable SQL logging in DbContext
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
{
    // In Program.cs:
    builder.Services.AddLogging(config =>
    {
        config.AddConsole();
        config.AddDebug();
    });
}
```

### Testing Service Layer
```csharp
// Mock interface for unit tests
[Test]
public async Task LoginAsync_ValidCredentials_ReturnsToken()
{
    // Arrange
    var mockAuthService = new Mock<IAuthService>();
    mockAuthService.Setup(s => s.LoginAsync(It.IsAny<AuthLoginRequest>()))
        .ReturnsAsync(Result.Success(new AuthLoginResult { Token = "fake-token" }));

    // Act
    var result = await mockAuthService.Object.LoginAsync(new AuthLoginRequest());

    // Assert
    Assert.That(result.IsSuccess, Is.True);
}
```

### Entity Auditing
```csharp
// Override SaveChangesAsync to track modifications
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries<EntityBase>();
    foreach (var entry in entries)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedAt = DateTime.UtcNow;
        }
    }
    return await base.SaveChangesAsync(cancellationToken);
}
```

---

## References
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [JWT Authentication in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt)
- [Result Pattern](https://github.com/ardalis/Result)
- [AutoMapper Best Practices](https://docs.automapper.org/)
- [FluentValidation](https://fluentvalidation.net/)

