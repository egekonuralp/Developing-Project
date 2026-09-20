# TechStore

Dystopia is a layered **ASP.NET Core MVC** e-commerce portfolio project built with **Entity Framework Core**. It implements the core end-to-end flows of a real e-commerce site: product catalog, cart, coupons, order management, and support ticketing.

## Screenshots

### Home Page
<!-- Drop your home page screenshot here -->

### Product Detail
<!-- Drop your product detail screenshot here -->

### Cart
<!-- Drop your cart screenshot here -->

### Admin Panel
<!-- Drop your admin panel screenshot here -->

## Features

### Customer Side
- Product listing with pagination, search, and category filtering
- Product detail page with image gallery, average rating, and related products
- Add/remove cart items and update quantities — prices are always synced server-side from the product's current price
- Coupon code application (percentage/fixed discount, minimum order amount, and usage-limit checks)
- Multi-step checkout flow: delivery information → payment → order confirmation
- View order history and order details
- Wishlist
- Leave reviews/ratings on purchased products
- Create support tickets and view ticket history
- Authentication: register, login, logout, profile update (ASP.NET Core Identity)

### Admin Panel
- Dashboard with summary metrics (order count, total revenue, etc.)
- Product management (CRUD) and multi-image upload/management
- Category management
- Coupon creation and management
- Order listing, search/filtering, and order status updates
- User listing, user detail, and role assignment
- View and respond to support tickets

## Tech Stack

- **Backend:** ASP.NET Core 8 MVC (.NET 8)
- **ORM / Database:** Entity Framework Core 8, SQL Server (LocalDB)
- **Authentication:** ASP.NET Core Identity (role-based: Admin / User)
- **Frontend:** Razor Views, Bootstrap, jQuery
- **Other:** jQuery Validation (client-side validation)

## Architecture

The project follows a clearly separated layered architecture:

```
Controllers/      → Handles HTTP requests, calls services
Services/          → Business logic (Interfaces + Implementations)
Repositories/      → Data access layer (Interfaces + Implementations)
Data/              → DbContext and EF Core migrations
Models/            → Database entities
DTOs/              → Data transfer objects between layers
ViewModels/        → View-specific data models
Views/             → Razor pages
```

This separation means, for example, that changes to the data access logic (Repository) don't affect the business rules (Service) or the controller layer.

### Notable Technical Details
- **Stock consistency:** Stock deduction during order creation is performed atomically at the database level with a conditional update (`WHERE Stock >= quantity`), preventing race conditions on concurrent purchases.
- **Price integrity:** The unit price in the cart is never taken from the client — it's synced server-side with the product's current price on every operation.
- **Authorization:** User-specific data such as orders and carts is always queried scoped to the authenticated user's identity (no IDOR via ID tampering).
- **Upload security:** Product images go through three layers of validation: file extension, MIME type, and file signature (magic bytes).

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (bundled with Visual Studio) or any SQL Server instance

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/egekonuralp/Developing-Project.git
cd Developing-Project

# 2. Restore dependencies
dotnet restore

# 3. Configure the database connection
# Edit ConnectionStrings:DefaultConnection in appsettings.json for your environment
# (the default works out of the box with LocalDB)

# 4. Apply migrations
dotnet ef database update

# 5. Run the project
dotnet run
```

The app will be available at `https://localhost:xxxx` by default.

### Admin Account
To make a user an admin, set the `SeedData:AdminEmail` value (in `appsettings.json` or User Secrets) to the email of an already-registered user; that user is automatically assigned the `Admin` role on startup.

## Payment

The checkout flow collects card details and walks through a full payment step, but no real payment provider is integrated — it's a mock/simulated payment that doesn't process an actual transaction.

## Development Notes

This project is under active development. Planned/possible improvements:
- Unit tests for the service layer
- Real payment gateway integration

## License

This project was built for personal portfolio purposes.
