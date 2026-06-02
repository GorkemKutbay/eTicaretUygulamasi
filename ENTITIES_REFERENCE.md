# eTicaret Entity Models Reference

## Quick Entity Summary Table

| Entity | Purpose | Key Fields | Key Relations |
|--------|---------|-----------|-----------------|
| **User** | User accounts | Email (unique), FirstName, LastName, Password (hashed), RoleId, Enabled, HasSellerRequest | Role (1:M), Products (as Seller, 1:M), Orders (1:M), CartItems (1:M), Comments (1:M), Blogs (1:M) |
| **Role** | Access control lookup | Name (unique): admin/seller/buyer | Users (M:1) |
| **Product** | Catalog items | SellerId, CategoryId, DiscountId (optional), Name, Price (decimal 18,2), Description, StockAmount, Enabled | Seller (User, M:1), Category (M:1), Discount (M:1), Images (1:M, cascade), Comments (1:M), CartItems (M:N), OrderItems (M:N) |
| **Category** | Product taxonomy | Name, Color, IconCssClass | Products (1:M) - [9 seeded] |
| **Discount** | Price reductions | DiscountRate (%), StartDate, EndDate, Enabled | Products (1:M) - [3 seeded: 10%, 20%, 30%] |
| **ProductImage** | Product gallery | ProductId (FK), Url | Product (M:1, cascade delete) |
| **ProductComment** | Reviews & ratings | ProductId, UserId, Text (max 500), StarCount (1-5, default 3), IsConfirmed | Product (M:1), User (M:1) |
| **CartItem** | Shopping cart | UserId, ProductId, Quantity (default 1) | User (M:1), Product (M:1) |
| **Order** | Purchase header | UserId, OrderCode (unique), Address | User (M:1), OrderItems (1:M) |
| **OrderItem** | Order line item | OrderId, ProductId, Quantity (default 1), UnitPrice (decimal 18,2) | Order (M:1), Product (M:1) - [Unique: OrderId+ProductId] |
| **Blog** | Content publishing | Title (max 100), Content, ImageUrl, UserId, Enabled | User (M:1), BlogComments (1:M, cascade delete) |
| **BlogComment** | Blog comments | BlogId, Name (max 100), Email (max 100), Comment, IsApproved (default false) | Blog (M:1, cascade delete) |
| **ContactForm** | Contact submissions | Name (max 100), Email (max 256), Message (max 1000), SeenAt (nullable) | — |

---

## Detailed Entity Definitions

### User Entity
```csharp
public class UserEntity : EntityBase, IHasEnabled
{
    public string Email { get; set; }              // Unique, max 256 chars
    public string FirstName { get; set; }          // Required, max 50 chars
    public string LastName { get; set; }           // Required, max 50 chars
    public string Password { get; set; }           // BCrypt hashed
    public string? ResetPasswordToken { get; set } // For password recovery
    public int RoleId { get; set; }                // FK to Role (admin/seller/buyer)
    public bool Enabled { get; set; }              // Default: true
    public bool HasSellerRequest { get; set; }     // Default: false - seller access request
    public DateTime CreatedAt { get; set; }        // Auto-set to UtcNow

    // Navigation properties
    public RoleEntity Role { get; set; }           // N:1 relationship
}

// Constraints:
// - Email is UNIQUE
// - Email is indexed for login queries
// - OnDelete(RoleId): NoAction (keep user if role deleted)
```

### Role Entity (Lookup)
```csharp
public class RoleEntity : EntityBase
{
    public string Name { get; set; }               // Unique: "admin", "seller", "buyer"
    public DateTime CreatedAt { get; set; }

    // Seeded data:
    // { Id: 1, Name: "admin", CreatedAt: UtcNow }
    // { Id: 2, Name: "seller", CreatedAt: UtcNow }
    // { Id: 3, Name: "buyer", CreatedAt: UtcNow }
}
```

### Product Entity
```csharp
public class ProductEntity : EntityBase, IHasEnabled
{
    public int SellerId { get; set; }              // FK to User (seller)
    public int CategoryId { get; set; }            // FK to Category
    public int? DiscountId { get; set; }           // Optional FK to Discount
    public string Name { get; set; }               // Required, max 100 chars
    public decimal Price { get; set; }             // decimal(18,2)
    public string Description { get; set; }        // Max 1000 chars
    public byte StockAmount { get; set; }          // 0-255
    public bool Enabled { get; set; }              // Default: true
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public UserEntity Seller { get; set; }
    public CategoryEntity Category { get; set; }
    public DiscountEntity? Discount { get; set; }
    public ICollection<ProductImageEntity> Images { get; set; }
    public ICollection<ProductCommentEntity> Comments { get; set; }
}

// Indexes:
// - SellerId (for seller inventory queries)
// - CategoryId (for category browsing)
// - Price (for sorting/filtering)
```

### Category Entity (Lookup)
```csharp
public class CategoryEntity : EntityBase
{
    public string Name { get; set; }               // Max 100 chars
    public string Color { get; set; }              // CSS color value
    public string IconCssClass { get; set; }       // Max 50 chars

    // Seeded categories (9 total):
    // 1. Fresh Meat (Blue)
    // 2. Vegetables (Red)
    // 3. Fresh Fruits (Green)
    // 4. Dried Fruits & Nuts (Brown)
    // 5. Ocean Foods (Purple)
    // 6. Butter & Eggs (Yellow)
    // 7. Fastfood (Pink)
    // 8. Oatmeal (Grey)
    // 9. Juices (Orange)
}
```

### Discount Entity (Lookup)
```csharp
public class DiscountEntity : EntityBase, IHasEnabled
{
    public byte DiscountRate { get; set; }         // Percentage (0-100)
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Enabled { get; set; }              // Default: true
    public DateTime CreatedAt { get; set; }

    // Seeded discounts (3 total):
    // { Id: 1, DiscountRate: 10%, StartDate: UtcNow, EndDate: +6 months }
    // { Id: 2, DiscountRate: 20%, StartDate: UtcNow, EndDate: +6 months }
    // { Id: 3, DiscountRate: 30%, StartDate: UtcNow, EndDate: +6 months }
}
```

### ProductImage Entity
```csharp
public class ProductImageEntity : EntityBase
{
    public int ProductId { get; set; }             // FK to Product
    public string Url { get; set; }                // Image URL/path
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ProductEntity Product { get; set; }

    // OnDelete: Cascade
    // (Images deleted when product is deleted)

    // Seeded with sample images:
    // /theme/img/product/discount/pd-3.jpg
    // /theme/img/product/product-1.jpg
    // etc.
}
```

### ProductComment Entity (Product Reviews)
```csharp
public class ProductCommentEntity : EntityBase
{
    public int ProductId { get; set; }             // FK to Product
    public int UserId { get; set; }                // FK to User (reviewer)
    public string Text { get; set; }               // Max 500 chars
    public byte StarCount { get; set; }            // 1-5, default 3
    public bool IsConfirmed { get; set; }          // Default: false (awaits approval)
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ProductEntity Product { get; set; }
    public UserEntity User { get; set; }

    // Purpose: Product reviews with ratings
    // Admin must approve IsConfirmed = true before display
}
```

### CartItem Entity
```csharp
public class CartItemEntity : EntityBase
{
    public int UserId { get; set; }                // FK to User
    public int ProductId { get; set; }             // FK to Product
    public byte Quantity { get; set; }             // 1-255, default 1
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public UserEntity User { get; set; }
    public ProductEntity Product { get; set; }

    // Purpose: Shopping cart persistence
    // No price stored (calculated from Product.Price at checkout)
}
```

### Order Entity
```csharp
public class OrderEntity : EntityBase
{
    public int UserId { get; set; }                // FK to User (buyer)
    public string OrderCode { get; set; }          // Unique, max 250 chars
    public string Address { get; set; }            // Delivery address, max 250 chars
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public UserEntity User { get; set; }
    public ICollection<OrderItemEntity> OrderItems { get; set; }

    // OrderCode uniqueness for tracking
    // Separate OrderItem entities for line items
}
```

### OrderItem Entity
```csharp
public class OrderItemEntity : EntityBase
{
    public int OrderId { get; set; }               // FK to Order
    public int ProductId { get; set; }             // FK to Product
    public byte Quantity { get; set; }             // 1-255, default 1
    public decimal UnitPrice { get; set; }         // decimal(18,2) - price at purchase time
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public OrderEntity Order { get; set; }
    public ProductEntity Product { get; set; }

    // Unique constraint: (OrderId, ProductId)
    // Each product appears only once per order
    // UnitPrice captured at order time (historical pricing)
}
```

### Blog Entity
```csharp
public class BlogEntity : EntityBase, IHasEnabled
{
    public string Title { get; set; }              // Max 100 chars
    public string Content { get; set; }            // Rich HTML content
    public string? ImageUrl { get; set; }          // Featured image
    public int UserId { get; set; }                // FK to User (author)
    public bool Enabled { get; set; }              // Default: true
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public UserEntity User { get; set; }
    public ICollection<BlogCommentEntity> Comments { get; set; }

    // Purpose: Blog post management
    // Related to: BlogComment (1:M), BlogTag (M:M), BlogCategory (M:M)
}
```

### BlogComment Entity
```csharp
public class BlogCommentEntity : EntityBase
{
    public int BlogId { get; set; }                // FK to Blog
    public string Name { get; set; }               // Commenter name, max 100 chars
    public string Email { get; set; }              // Commenter email, max 100 chars
    public string Comment { get; set; }            // Comment text
    public bool IsApproved { get; set; }           // Default: false
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public BlogEntity Blog { get; set; }

    // OnDelete(BlogId): Cascade
    // (Comments deleted when blog is deleted)
    // Purpose: Blog comments with approval workflow
}
```

### ContactForm Entity
```csharp
public class ContactFormEntity : EntityBase
{
    public string Name { get; set; }               // Submitter name, max 100 chars
    public string Email { get; set; }              // Contact email, max 256 chars
    public string Message { get; set; }            // Message text, max 1000 chars
    public DateTime? SeenAt { get; set; }          // Nullable - null = unread
    public DateTime CreatedAt { get; set; }

    // Purpose: Contact form submissions
    // Admin can mark as seen/responded
}
```

### Supporting Entities

#### BlogTag Entity
```csharp
public class BlogTagEntity : EntityBase
{
    public string Name { get; set; }               // Tag name
    // Supports M:M relationship with Blog via RelBlogTagEntity
}
```

#### BlogCategory Entity
```csharp
public class BlogCategoryEntity : EntityBase
{
    public string Name { get; set; }               // Category name
    // Supports M:M relationship with Blog via RelBlogCategoryEntity
}
```

#### RelBlogTag Entity
```csharp
public class RelBlogTagEntity : EntityBase
{
    public int BlogId { get; set; }
    public int BlogTagId { get; set; }
    // Join table for Blog ↔ BlogTag M:M relationship
}
```

#### RelBlogCategory Entity
```csharp
public class RelBlogCategoryEntity : EntityBase
{
    public int BlogId { get; set; }
    public int BlogCategoryId { get; set; }
    // Join table for Blog ↔ BlogCategory M:M relationship
}
```

---

## Data Type Reference

| .NET Type | Database Type | Range/Max Length | Usage |
|-----------|---------------|------------------|-------|
| `string` | VARCHAR/NVARCHAR | Varies | Text, max 50-1000 chars |
| `int` | INT | -2.1B to +2.1B | IDs, quantities, prices |
| `byte` | TINYINT | 0-255 | StockAmount, StarCount, Quantity |
| `decimal` | DECIMAL(18,2) | Money fields with 2 decimals | Price, UnitPrice |
| `bool` | BIT | 0=false, 1=true | Enabled, IsConfirmed, IsApproved |
| `DateTime` | DATETIME2 | 0001-01-01 to 9999-12-31 | CreatedAt, StartDate, EndDate |
| `DateTime?` | DATETIME2 | — | Optional dates like SeenAt |

---

## Relationship Types Summary

### One-to-Many (1:M)
```
Role (1) ──→ (M) User
Category (1) ──→ (M) Product
Discount (1) ──→ (M) Product
Product (1) ──→ (M) ProductImage
Product (1) ──→ (M) ProductComment
User (1) ──→ (M) Order
Order (1) ──→ (M) OrderItem
Blog (1) ──→ (M) BlogComment
```

### Many-to-Many (M:M)
```
User (M) ──→ (M) Product (through Order → OrderItem)
User (M) ──→ (M) Product (through CartItem)
Blog (M) ──→ (M) BlogTag (through RelBlogTag)
Blog (M) ──→ (M) BlogCategory (through RelBlogCategory)
```

### Optional Relationships (0..1:M)
```
Product (0..1) ──→ (M) Discount
User (0..1) ──→ (M) Product (as Seller)
```

---

## Cascade Delete Behavior

| Relationship | Delete Behavior | Reason |
|-------------|-----------------|--------|
| ProductImage → Product | **CASCADE** | Images only exist for products |
| BlogComment → Blog | **CASCADE** | Comments are blog-specific |
| OrderItem → Order | **NO ACTION** | Keep order history intact |
| ProductComment → Product | **NO ACTION** | Preserve reviews |
| User → Product/Order | **NO ACTION** | Preserve seller/buyer history |
| Role → User | **NO ACTION** | Keep users even if role deleted |

---

## Database Constraints

### Unique Constraints
- `User.Email` - Only one account per email
- `Role.Name` - Prevents duplicate role names
- `Order.OrderCode` - Unique order tracking codes
- `OrderItem(OrderId, ProductId)` - Each product once per order

### Not Null Constraints
- All navigation properties (FK relationships)
- All Name/Title fields
- Price, StockAmount, CreatedAt on products
- Email, Password on users

### Check Constraints (Implicit via type)
- `StarCount` limited to `byte` (1-255, typically 1-5)
- `DiscountRate` limited to `byte` (0-100%)
- `StockAmount` limited to `byte` (0-255)
- `Quantity` limited to `byte` (1-255)

---

## Computed/Derived Fields (Not Stored)

These values are calculated at query time, not stored in database:

```
Product.TotalPrice = Price * (1 - Discount.DiscountRate/100)
Order.TotalAmount = Sum(OrderItem.UnitPrice * OrderItem.Quantity)
Product.AverageRating = Avg(ProductComment.StarCount WHERE IsConfirmed=true)
CartItem.LineTotal = Product.Price * CartItem.Quantity
```

---

## Database Seeding Data

### Users (in UserEntitySeed)
```
Default admin user seeded with hashed password "1234"
Accessible via seed extension methods
```

### Categories (in CategoryEntitySeed)
```
1. Fresh Meat (Blue)
2. Vegetables (Red)
3. Fresh Fruits (Green)
4. Dried Fruits & Nuts (Brown)
5. Ocean Foods (Purple)
6. Butter & Eggs (Yellow)
7. Fastfood (Pink)
8. Oatmeal (Grey)
9. Juices (Orange)
```

### Discounts (in DiscountEntitySeed)
```
10% off (6 months duration)
20% off (6 months duration)
30% off (6 months duration)
```

### Products & Images (in ProductImageEntitySeed)
```
Products 1-10 with corresponding images
Images point to /theme/img/product/ URLs
```

---

## Query Examples

### Finding Products by Category
```csharp
var products = dbContext.Products
    .Where(p => p.CategoryId == categoryId && p.Enabled)
    .Include(p => p.Images)
    .Include(p => p.Seller)
    .ToList();
```

### Getting Confirmed Reviews
```csharp
var reviews = dbContext.ProductComments
    .Where(c => c.ProductId == productId && c.IsConfirmed)
    .Include(c => c.User)
    .OrderByDescending(c => c.CreatedAt)
    .ToList();
```

### Order History with Items
```csharp
var orders = dbContext.Orders
    .Where(o => o.UserId == userId)
    .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
    .OrderByDescending(o => o.CreatedAt)
    .ToList();
```

### Seller Inventory
```csharp
var sellerProducts = dbContext.Products
    .Where(p => p.SellerId == sellerId)
    .Include(p => p.Category)
    .Include(p => p.Discount)
    .Include(p => p.Images)
    .ToList();
```

### Cart Totals
```csharp
var cartTotal = dbContext.CartItems
    .Where(c => c.UserId == userId)
    .Join(dbContext.Products, 
        c => c.ProductId, 
        p => p.Id,
        (c, p) => c.Quantity * p.Price)
    .Sum();
```

