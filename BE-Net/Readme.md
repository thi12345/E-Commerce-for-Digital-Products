● Để nắm đủ context, tôi đọc thêm một số file chính.                                                                                                                                                                                
                                                                                                                                                                                                                                    
  Read 6 files (ctrl+o to expand)                                                                                                                                                                                                   
                                                                                                                                                                                                                                    
● Đủ rồi. Đây là toàn bộ luồng chạy của dự án:   
                                                                                                                                                                                                                                    
  ---                                                                                     
  Kiến trúc tổng quan                                                                                                                                                                                                               
                                                                                                                                                                                                                                    
  Dự án theo Clean Architecture, chia làm 4 layer. Dependency chỉ đi từ ngoài vào trong — layer bên trong không biết layer bên ngoài tồn tại:                                                                                       
                                                                                                                                                                                                                                    
  HTTP Request                                              
       │
       ▼
  ┌──────────────┐
  │   ShopApp.API │   ← Tiếp nhận request, trả response
  └──────┬───────┘
         │ gửi Command/Query qua MediatR
         ▼
  ┌──────────────────────┐
  │ ShopApp.Application  │   ← Điều phối use case, không có business logic
  └──────┬───────────────┘
         │ gọi Domain để xử lý logic
         ▼
  ┌──────────────────┐
  │  ShopApp.Domain  │   ← Business logic thuần, không biết DB hay HTTP
  └──────────────────┘
         ▲
         │ implements interface của Domain
  ┌──────────────────────────┐
  │ ShopApp.Infrastructure   │   ← EF Core, PostgreSQL, Repository
  └──────────────────────────┘

  ---
  Luồng chi tiết — Ví dụ POST /api/products

  Bước 1 — HTTP Request vào Controller

  Client gửi POST /api/products
  Body: { "name": "Ebook C#", "price": 99, "currency": "USD", "downloadUrl": "..." }

  ProductsController nhận request, không xử lý gì cả, chỉ đóng gói thành CreateProductCommand rồi ném vào MediatR:

  // ProductsController.cs
  var result = await sender.Send(command, ct);

  ---
  Bước 2 — MediatR Pipeline (Middleware nội bộ)

  MediatR không đưa thẳng vào Handler. Nó chạy qua ValidationBehavior trước — đây là pipeline behavior đăng ký sẵn:

  MediatR nhận CreateProductCommand
          │
          ▼
  ValidationBehavior.Handle()
    → Tìm tất cả IValidator<CreateProductCommand>
    → Chạy CreateProductCommandValidator
        ✓ Name không rỗng, tối đa 200 ký tự
        ✓ Price > 0
        ✓ Currency đúng 3 ký tự
        ✓ DownloadUrl không rỗng
    → Nếu lỗi: throw ValidationException → Program.cs bắt → trả 400
    → Nếu pass: gọi next() → đến Handler

  ---
  Bước 3 — Application Handler xử lý

  CreateProductCommandHandler chạy:

  // 1. Log bắt đầu
  logger.LogInformation("Creating product: Name={Name}...", ...);

  // 2. Gọi Domain để tạo entity (business logic nằm ở đây)
  var product = Product.Create(name, description, price, currency, downloadUrl);

  // 3. Lưu vào repository
  await productRepository.AddAsync(product, ct);
  await unitOfWork.SaveChangesAsync(ct);

  // 4. Log thành công
  logger.LogInformation("Product created: Id={ProductId}...", ...);

  // 5. Map Entity → DTO rồi trả về
  return ToDto(product);

  ---
  Bước 4 — Domain xử lý business logic

  Bên trong Product.Create() — tầng này không biết gì về DB hay HTTP:

  // Domain/Catalog/Entities/Product.cs
  var product = new Product {
      Id = Guid.NewGuid(),
      Name = ProductName.Create(name),   // Value Object — validate tên
      Price = Money.Create(price, currency), // Value Object — validate tiền
      Status = ProductStatus.Draft       // mặc định là Draft
  };

  // Ghi nhận sự kiện domain đã xảy ra
  product.RaiseDomainEvent(new ProductCreatedDomainEvent(...));

  ProductName và Money là Value Object — chứa validation và logic riêng của chúng.

  ---
  Bước 5 — Infrastructure lưu xuống DB

  UnitOfWork.SaveChangesAsync() gọi AppDbContext.SaveChangesAsync() của EF Core. EF Core dùng configuration trong ProductConfiguration để biết cách map:

  Product entity
    → Name (Value Object)  → cột "Name" VARCHAR(200)
    → Price (Value Object) → cột "Price" DECIMAL(18,2) + "Currency" VARCHAR(3)
    → Status (Enum)        → lưu dạng string ("Draft", "Active", ...)
    → CategoryId           → FK nullable tới Categories

  ---
  Bước 6 — Response trả về Client

  Handler trả ProductDto
    → Controller nhận: return CreatedAtAction(...)
    → HTTP 201 Created
    → Header Location: /api/products/{id}
    → Body: { "id": "...", "name": "Ebook C#", "price": 99, ... }

  ---
  Luồng xử lý lỗi

  Lỗi không cần try/catch ở handler — Program.cs có global exception handler bắt tất cả:

  ValidationException  → 400 Bad Request  + danh sách lỗi validation
  DomainException      → 422 Unprocessable + message lỗi business
  Exception khác       → 500 Internal Server Error

  ---
  Tóm tắt vai trò từng layer

  ┌────────────────┬─────────────────────────────────────┬────────────────────────────────┐
  │     Layer      │               Làm gì                │          Không làm gì          │
  ├────────────────┼─────────────────────────────────────┼────────────────────────────────┤
  │ API            │ Nhận HTTP, trả HTTP                 │ Không có logic                 │
  ├────────────────┼─────────────────────────────────────┼────────────────────────────────┤
  │ Application    │ Điều phối use case, log, map DTO    │ Không có business rule         │
  ├────────────────┼─────────────────────────────────────┼────────────────────────────────┤
  │ Domain         │ Business logic, validation, sự kiện │ Không biết DB, không biết HTTP │
  ├────────────────┼─────────────────────────────────────┼────────────────────────────────┤
  │ Infrastructure │ Kết nối DB, implement repository    │ Không có business logic        │
  └────────────────┴─────────────────────────────────────┴────────────────────────────────┘
