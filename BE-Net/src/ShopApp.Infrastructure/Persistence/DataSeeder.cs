using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Users.Entities;

namespace ShopApp.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        await SeedUsersAsync(db, logger);
        await SeedCatalogAsync(db, logger);
    }

    private static async Task SeedUsersAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Users.AnyAsync())
            return;

        logger.LogInformation("Seeding users...");

        var users = CreateUsers();
        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {UserCount} users.", users.Count);

        logger.LogInformation("Seeding addresses...");
        var addresses = CreateAddresses(users);
        await db.Addresses.AddRangeAsync(addresses);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {AddressCount} addresses.", addresses.Count);
    }

    private static List<Address> CreateAddresses(List<User> users)
    {
        var map = users.ToDictionary(u => u.Email);
        var list = new List<Address>();

        void Add(string email, string fullName, string phone, string line, string city,
                 string province, string country, string? postal, bool isDefault)
        {
            var a = Address.Create(map[email].Id, fullName, phone, line, city, province, country, postal);
            if (isDefault) a.SetAsDefault();
            list.Add(a);
        }

        // Admin
        Add("admin@shopapp.com", "Admin", "0901000000",
            "1 Dinh Tien Hoang", "Ho Chi Minh City", "Ho Chi Minh", "Vietnam", "700000", true);

        // Alice Johnson — 2 địa chỉ
        Add("alice.johnson@example.com", "Alice Johnson", "0901111111",
            "12 Nguyen Hue", "Ho Chi Minh City", "Ho Chi Minh", "Vietnam", "700000", true);
        Add("alice.johnson@example.com", "Alice Johnson", "0901111111",
            "45 Le Loi", "Bien Hoa", "Dong Nai", "Vietnam", "760000", false);

        // Bob Smith — 2 địa chỉ
        Add("bob.smith@example.com", "Bob Smith", "0902222222",
            "88 Tran Phu", "Da Nang", "Da Nang", "Vietnam", "550000", true);
        Add("bob.smith@example.com", "Bob Smith", "0902222222",
            "23 Hai Ba Trung", "Hue", "Thua Thien Hue", "Vietnam", "530000", false);

        // Carol White
        Add("carol.white@example.com", "Carol White", "0903333333",
            "56 Le Duan", "Hanoi", "Ha Noi", "Vietnam", "100000", true);

        // David Brown — 2 địa chỉ
        Add("david.brown@example.com", "David Brown", "0904444444",
            "7 Phan Chu Trinh", "Can Tho", "Can Tho", "Vietnam", "900000", true);
        Add("david.brown@example.com", "David Brown", "0904444444",
            "30 Tran Hung Dao", "My Tho", "Tien Giang", "Vietnam", "840000", false);

        // Emma Davis
        Add("emma.davis@example.com", "Emma Davis", "0905555555",
            "19 Bach Dang", "Hai Phong", "Hai Phong", "Vietnam", "180000", true);

        // Frank Miller — 2 địa chỉ
        Add("frank.miller@example.com", "Frank Miller", "0906666666",
            "33 Nguyen Van Cu", "Nha Trang", "Khanh Hoa", "Vietnam", "650000", true);
        Add("frank.miller@example.com", "Frank Miller", "0906666666",
            "101 Hung Vuong", "Phan Thiet", "Binh Thuan", "Vietnam", "770000", false);

        // Grace Wilson
        Add("grace.wilson@example.com", "Grace Wilson", "0907777777",
            "64 Quang Trung", "Quy Nhon", "Binh Dinh", "Vietnam", "590000", true);

        // Henry Moore
        Add("henry.moore@example.com", "Henry Moore", "0908888888",
            "27 Tran Quoc Toan", "Vung Tau", "Ba Ria - Vung Tau", "Vietnam", "780000", true);

        // Iris Taylor — 2 địa chỉ
        Add("iris.taylor@example.com", "Iris Taylor", "0909999999",
            "5 Ly Thai To", "Hanoi", "Ha Noi", "Vietnam", "100000", true);
        Add("iris.taylor@example.com", "Iris Taylor", "0909999999",
            "78 Nguyen Trai", "Ha Dong", "Ha Noi", "Vietnam", "100000", false);

        // James Anderson
        Add("james.anderson@example.com", "James Anderson", "0900000001",
            "42 Cach Mang Thang 8", "Ho Chi Minh City", "Ho Chi Minh", "Vietnam", "700000", true);

        return list;
    }

    private static List<User> CreateUsers()
    {
        var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        var customerHash = BCrypt.Net.BCrypt.HashPassword("Customer@123");

        var admin = User.Create("admin@shopapp.com", "Admin", adminHash);
        admin.PromoteToAdmin();

        return [
            admin,
            User.Create("alice.johnson@example.com", "Alice Johnson", customerHash),
            User.Create("bob.smith@example.com", "Bob Smith", customerHash),
            User.Create("carol.white@example.com", "Carol White", customerHash),
            User.Create("david.brown@example.com", "David Brown", customerHash),
            User.Create("emma.davis@example.com", "Emma Davis", customerHash),
            User.Create("frank.miller@example.com", "Frank Miller", customerHash),
            User.Create("grace.wilson@example.com", "Grace Wilson", customerHash),
            User.Create("henry.moore@example.com", "Henry Moore", customerHash),
            User.Create("iris.taylor@example.com", "Iris Taylor", customerHash),
            User.Create("james.anderson@example.com", "James Anderson", customerHash),
        ];
    }

    private static async Task SeedCatalogAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Categories.AnyAsync())
        {
            logger.LogInformation("Catalog already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding categories and products...");

        var categories = CreateCategories();
        await db.Categories.AddRangeAsync(categories);

        var categoryMap = categories.ToDictionary(c => c.Name);
        var products = CreateProducts(categoryMap);
        await db.Products.AddRangeAsync(products);

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {CategoryCount} categories and {ProductCount} products.",
            categories.Count, products.Count);
    }

    private static List<Category> CreateCategories() =>
    [
        Category.Create("Electronics", "TVs, air conditioners, refrigerators and home appliances"),
        Category.Create("Computers & Accessories", "Laptops, keyboards, mice and computer peripherals"),
        Category.Create("Mobiles & Accessories", "Smartphones, earphones, smartwatches and phone accessories"),
        Category.Create("Home & Kitchen", "Cookware, kitchen appliances and home essentials"),
        Category.Create("Health & Personal Care", "Healthcare devices, skincare and personal hygiene"),
        Category.Create("Sports & Fitness", "Sports equipment, fitness gear and outdoor accessories"),
        Category.Create("Musical Instruments", "Guitars, keyboards, drums and musical accessories"),
        Category.Create("Office Products", "Printers, stationery and office supplies"),
        Category.Create("Car & Motorbike", "Car accessories, cleaning products and motorbike gear"),
        Category.Create("Toys & Games", "Board games, action figures and educational toys"),
    ];

    private static List<Product> CreateProducts(Dictionary<string, Category> cat)
    {
        const string INR = "INR";
        var list = new List<Product>();

        // ── Electronics (10) ───────────────────────────────────────────────
        list.AddRange([
            Product.Create("Samsung 32\" FHD Smart LED TV",
                "32-inch Full HD 1080p display with Smart TV features, built-in Wi-Fi, Netflix, Prime Video. 3 HDMI and 2 USB ports. Energy efficient A+ rated panel.",
                25999, 17999, 31, INR,
                "https://m.media-amazon.com/images/I/61-Q5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09X1234AB",
                rating: 4.2m, ratingCount: 8543, categoryId: cat["Electronics"].Id),

            Product.Create("Sony Bravia 43\" 4K Ultra HD Smart TV",
                "43-inch 4K Ultra HD Google TV with TRILUMINOS Pro display. Built-in Google Assistant, Chromecast. Dolby Audio, 20W output. 4 HDMI, 2 USB.",
                49999, 34999, 30, INR,
                "https://m.media-amazon.com/images/I/71-Q5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09Y2345BC",
                rating: 4.4m, ratingCount: 12890, categoryId: cat["Electronics"].Id),

            Product.Create("LG 1.5 Ton 5 Star Inverter Split AC",
                "1.5 ton 5-star inverter split AC with dual inverter compressor. Auto clean, Wi-Fi control via ThinQ app. R32 refrigerant, 4-way swing, anti-corrosion blue fin.",
                47990, 33990, 29, INR,
                "https://m.media-amazon.com/images/I/51-Q5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B08Z3456CD",
                rating: 4.3m, ratingCount: 5621, categoryId: cat["Electronics"].Id),

            Product.Create("Whirlpool 265L 3 Star Frost-Free Refrigerator",
                "265-litre double-door frost-free refrigerator. IntelliSense inverter technology, microblock anti-bacterial protection. 6th Sense ActiveFresh technology.",
                28990, 21999, 24, INR,
                "https://m.media-amazon.com/images/I/61-A5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B091A456DE",
                rating: 4.1m, ratingCount: 3892, categoryId: cat["Electronics"].Id),

            Product.Create("Panasonic 23L Convection Microwave Oven",
                "23-litre convection microwave with 360° Heat Wrap, Steam Clean, Inverter Technology. 101 auto-cook menus. Fits a 10-inch dinner plate.",
                11990, 8499, 29, INR,
                "https://m.media-amazon.com/images/I/71-B5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B092B456EF",
                rating: 4.0m, ratingCount: 2341, categoryId: cat["Electronics"].Id),

            Product.Create("Philips 9W LED Bulb Pack of 10",
                "9W LED bulb (60W equivalent), 900 lumens, warm white 2700K. Energy saving up to 85%. Average life 15,000 hours. Pack of 10 bulbs.",
                899, 449, 50, INR,
                "https://m.media-amazon.com/images/I/41-C5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B093C456FG",
                rating: 4.3m, ratingCount: 45821, categoryId: cat["Electronics"].Id),

            Product.Create("Havells 1200mm Ceiling Fan",
                "1200mm high-speed ceiling fan with double-ball bearings. 70W motor, 395 RPM speed, 230 m³/min air delivery. 3 aluminium blades, 2-year warranty.",
                2990, 1999, 33, INR,
                "https://m.media-amazon.com/images/I/51-D5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B094D456GH",
                rating: 4.2m, ratingCount: 9871, categoryId: cat["Electronics"].Id),

            Product.Create("Syska LED Smart Color Bulb B22",
                "7W Wi-Fi smart LED bulb, 16 million colors, works with Alexa and Google Home. Schedule timers, music sync mode. No hub required.",
                1499, 899, 40, INR,
                "https://m.media-amazon.com/images/I/61-E5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B095E456HI",
                rating: 4.0m, ratingCount: 6712, categoryId: cat["Electronics"].Id),

            Product.Create("Bajaj 2000W Induction Cooktop",
                "2000W induction cooktop with 7 power levels. Pre-set menus for Indian cooking, auto-off, anti-magnetic wall. Feather touch buttons, ABS body.",
                2999, 1699, 43, INR,
                "https://m.media-amazon.com/images/I/71-F5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B096F456IJ",
                rating: 4.1m, ratingCount: 8234, categoryId: cat["Electronics"].Id),

            Product.Create("Crompton 5M LED Strip Light Warm White",
                "5-metre self-adhesive LED strip light, 300 LEDs, warm white 3000K. 12V low voltage, waterproof (IP65), cuttable every 3 LEDs. Ideal for TV backlight.",
                1299, 799, 38, INR,
                "https://m.media-amazon.com/images/I/61-G5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B097G456JK",
                rating: 3.9m, ratingCount: 4523, categoryId: cat["Electronics"].Id),
        ]);

        // ── Computers & Accessories (10) ───────────────────────────────────
        list.AddRange([
            Product.Create("HP Pavilion 15 Intel i5 12th Gen Laptop",
                "15.6\" FHD IPS display, Intel Core i5-1235U, 8GB DDR4, 512GB SSD. Intel Iris Xe graphics, Windows 11 Home. Backlit keyboard, USB-C, HDMI.",
                62990, 49999, 21, INR,
                "https://m.media-amazon.com/images/I/71-H5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B098H456KL",
                rating: 4.3m, ratingCount: 15423, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("Dell Inspiron 3511 Intel i3 Laptop",
                "15.6\" FHD WVA display, Intel Core i3-1115G4, 8GB DDR4, 512GB SSD. Intel UHD Graphics, Windows 11 Home. 1-year onsite warranty.",
                45990, 35990, 22, INR,
                "https://m.media-amazon.com/images/I/61-I5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B099I456LM",
                rating: 4.1m, ratingCount: 9823, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("Lenovo IdeaPad Slim 3 Ryzen 5 Laptop",
                "15.6\" FHD IPS display, AMD Ryzen 5 5500U, 8GB DDR4, 512GB SSD. AMD Radeon graphics, Windows 11 Home. Narrow bezel design, 2-year warranty.",
                49999, 39990, 20, INR,
                "https://m.media-amazon.com/images/I/71-J5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09AJ456MN",
                rating: 4.2m, ratingCount: 12341, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("ASUS VivoBook 15 Intel i5 Laptop",
                "15.6\" FHD NanoEdge display, Intel Core i5-1235U, 8GB DDR4, 512GB SSD. Intel Iris Xe, Windows 11, ASUS NumberPad 2.0, backlit keyboard.",
                54999, 44990, 18, INR,
                "https://m.media-amazon.com/images/I/61-K5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09BK456NO",
                rating: 4.2m, ratingCount: 7892, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("Logitech MK215 Wireless Keyboard & Mouse",
                "Reliable 2.4GHz wireless combo with nano USB receiver. Keyboard: 12 function keys, spill-resistant. Mouse: optical, 1000 DPI. 2-year battery life.",
                1995, 999, 50, INR,
                "https://m.media-amazon.com/images/I/71-L5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09CL456OP",
                rating: 4.1m, ratingCount: 32541, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("HP 125 Wired USB Mouse",
                "Wired USB optical mouse, 1600 DPI, plug-and-play. Ambidextrous design, 3 buttons with scroll wheel. Compatible with Windows, Linux, Chrome OS.",
                899, 449, 50, INR,
                "https://m.media-amazon.com/images/I/51-M5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09DM456PQ",
                rating: 4.2m, ratingCount: 87423, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("Seagate Expansion 1TB Portable HDD",
                "1TB USB 3.0 portable external hard drive. Plug-and-play for Windows, Mac compatible with reformatting. Pocket-sized, 3-year rescue data recovery service.",
                5999, 3499, 42, INR,
                "https://m.media-amazon.com/images/I/61-N5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09EN456QR",
                rating: 4.3m, ratingCount: 23891, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("Kingston 8GB DDR4 3200MHz Desktop RAM",
                "8GB 3200MHz DDR4 DIMM. CL22, 1.35V, 288-pin. Plug-and-play auto-detects XMP profiles. Lifetime warranty. Compatible with Intel and AMD platforms.",
                3499, 2199, 37, INR,
                "https://m.media-amazon.com/images/I/71-O5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09FO456RS",
                rating: 4.4m, ratingCount: 18234, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("TP-Link AC750 Dual Band Wi-Fi Router",
                "750Mbps dual-band Wi-Fi (300+450 Mbps). 5 antennas, 4 LAN + 1 WAN ports. Parental controls, guest Wi-Fi, IPv6, WPA3 security. Easy setup with Tether app.",
                2999, 1599, 47, INR,
                "https://m.media-amazon.com/images/I/61-P5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09GP456ST",
                rating: 4.0m, ratingCount: 14567, categoryId: cat["Computers & Accessories"].Id),

            Product.Create("Zebronics ZEB-B10 USB 4-Port Hub",
                "USB 3.0 hub with 4 ports. Supports data transfer up to 5Gbps. Compact plug-and-play design, backward compatible with USB 2.0. Includes 30cm cable.",
                999, 499, 50, INR,
                "https://m.media-amazon.com/images/I/51-Q5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09HQ456TU",
                rating: 3.8m, ratingCount: 5632, categoryId: cat["Computers & Accessories"].Id),
        ]);

        // ── Mobiles & Accessories (10) ─────────────────────────────────────
        list.AddRange([
            Product.Create("Samsung Galaxy M33 5G 6GB 128GB",
                "6.6\" FHD+ display, Samsung Exynos 1280 5G, 6GB RAM, 128GB storage. 50MP quad camera, 5000mAh battery, 25W fast charging. Android 12, One UI 4.1.",
                24999, 17999, 28, INR,
                "https://m.media-amazon.com/images/I/71-R5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09IR456UV",
                rating: 4.1m, ratingCount: 67890, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("Redmi Note 11 4GB 64GB",
                "6.43\" AMOLED display 90Hz, Snapdragon 680, 4GB RAM, 64GB storage. 50MP quad camera, 5000mAh battery, 33W Pro fast charging. MIUI 13, Android 11.",
                17999, 13999, 22, INR,
                "https://m.media-amazon.com/images/I/61-S5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09JS456VW",
                rating: 4.2m, ratingCount: 89234, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("Realme 9i 4GB 64GB",
                "6.6\" LCD FHD+ 90Hz, Snapdragon 680, 4GB+64GB. 50MP AI triple camera, 5000mAh, 33W Dart Charge. realme UI 2.0 based on Android 11.",
                15999, 12499, 22, INR,
                "https://m.media-amazon.com/images/I/71-T5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09KT456WX",
                rating: 4.0m, ratingCount: 34521, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("POCO M4 Pro 5G 6GB 128GB",
                "6.5\" FHD+ DotDisplay, MediaTek Dimensity 810 5G, 6GB RAM, 128GB storage. 50MP dual camera, 5000mAh, 33W Pro fast charging. MIUI 12.5.",
                18999, 14999, 21, INR,
                "https://m.media-amazon.com/images/I/61-U5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09LU456XY",
                rating: 4.2m, ratingCount: 45678, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("OPPO A57 4GB 64GB",
                "6.56\" HD+ 60Hz LCD, MediaTek Helio G35, 4GB RAM, 64GB storage. 13MP dual camera, 5000mAh, 33W SUPERVOOC fast charging. ColorOS 12.1 Android 12.",
                17999, 13999, 22, INR,
                "https://m.media-amazon.com/images/I/71-V5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09MV456YZ",
                rating: 3.9m, ratingCount: 23456, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("boAt Rockerz 450 Bluetooth Headphones",
                "Over-ear wireless headphones, 40mm drivers, 15-hour playback. Foldable design, built-in mic, 40ms low latency mode. Bluetooth 5.0, 500mAh battery.",
                3490, 1499, 57, INR,
                "https://m.media-amazon.com/images/I/61-W5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09NW456ZA",
                rating: 4.1m, ratingCount: 234521, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("Noise ColorFit Pro 4 Smartwatch",
                "1.72\" TFT LCD display, 150+ watch faces, health suite with SpO2, heart rate, sleep tracking. 7-day battery, IP68 waterproof, BT calling, GPS.",
                8999, 3999, 56, INR,
                "https://m.media-amazon.com/images/I/71-X5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09OX456AB",
                rating: 4.0m, ratingCount: 56789, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("Samsung 25W USB-C Super Fast Charger",
                "25W USB Type-C travel adapter with PD 3.0. Compatible with Galaxy S/Note/A series, iPad Pro, MacBook Air. Intelligent temperature control, auto voltage detection.",
                1499, 799, 47, INR,
                "https://m.media-amazon.com/images/I/51-Y5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09PY456BC",
                rating: 4.2m, ratingCount: 78234, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("Anker PowerCore 20000 Power Bank",
                "20000mAh portable charger, dual USB-A output (15W) + USB-C input/output. MultiProtect safety suite, VoltageBoost, PowerIQ 2.0. Charges iPhone 14 five times.",
                3999, 2499, 38, INR,
                "https://m.media-amazon.com/images/I/61-Z5Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09QZ456CD",
                rating: 4.4m, ratingCount: 34521, categoryId: cat["Mobiles & Accessories"].Id),

            Product.Create("Belkin ScreenForce TemperedGlass for iPhone 14",
                "Tempered glass screen protector, compatible with Face ID, 9H hardness. Self-aligning installation tray. Anti-scratch, bubble-free. Pack of 2.",
                1299, 699, 46, INR,
                "https://m.media-amazon.com/images/I/51-A1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09RA456DE",
                rating: 4.1m, ratingCount: 12345, categoryId: cat["Mobiles & Accessories"].Id),
        ]);

        // ── Home & Kitchen (10) ────────────────────────────────────────────
        list.AddRange([
            Product.Create("Prestige 3 Litre Aluminium Pressure Cooker",
                "3-litre aluminium pressure cooker with metallic safety plug, gasket release system. Ergonomic handles, compatible with induction and gas stoves. ISI certified.",
                1895, 999, 47, INR,
                "https://m.media-amazon.com/images/I/71-B1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09SB456EF",
                rating: 4.3m, ratingCount: 156789, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Milton Thermosteel Flip Lid Flask 500ml",
                "500ml thermos flask, maintains temperature 18+ hours. Food-grade 304 stainless steel, leak-proof flip lid. BPA-free, 5-year lid warranty.",
                995, 598, 40, INR,
                "https://m.media-amazon.com/images/I/61-C1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09TC456FG",
                rating: 4.4m, ratingCount: 234567, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Pigeon Stainless Steel Deep Kadai 3 Litre",
                "3-litre stainless steel kadai, hammered finish, deep design. Sturdy handles, compatible with all cooktops including induction. Mirror-polished interior.",
                1199, 599, 50, INR,
                "https://m.media-amazon.com/images/I/71-D1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09UD456GH",
                rating: 4.2m, ratingCount: 89234, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Solimo 3-Piece Non-Stick Cookware Set",
                "3-piece non-stick set: 24cm fry pan, 24cm kadai, 20cm sauce pan. 3-layer non-stick coating, PFOA-free, induction compatible. Tempered glass lids included.",
                2999, 1499, 50, INR,
                "https://m.media-amazon.com/images/I/61-E1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09VE456HI",
                rating: 4.0m, ratingCount: 34521, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Wonderchef Nutri-Blend 400W Mixer Grinder",
                "400W personal blender with stainless steel blade. 2 unbreakable jars (300ml + 500ml), 22000 RPM, overload protection. Compact design, 2-year warranty.",
                3995, 2499, 38, INR,
                "https://m.media-amazon.com/images/I/71-F1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09WF456IJ",
                rating: 4.1m, ratingCount: 67890, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Cello Opalware Dinner Set 27 Pieces",
                "27-piece opalware dinner set: 6 full plates, 6 quarter plates, 6 bowls, 6 katoris, 1 serving bowl, 2 serving plates. Microwave and dishwasher safe.",
                2499, 1399, 44, INR,
                "https://m.media-amazon.com/images/I/61-G1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09XG456JK",
                rating: 4.3m, ratingCount: 45678, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Amazon Basics Microfiber Cleaning Cloth Pack of 6",
                "Pack of 6 microfiber cleaning cloths (40x40cm), 300 GSM. Lint-free, highly absorbent, machine washable. Suitable for kitchen, glass, car cleaning.",
                599, 299, 50, INR,
                "https://m.media-amazon.com/images/I/71-H1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09YH456KL",
                rating: 4.2m, ratingCount: 123456, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Usha Calypso 10L Instant Water Heater",
                "10-litre instant water heater, 2000W, 8 bar pressure, titanium enamel tank. Temperature adjustment knob, thermal cut-off, IPX4 splash proof. 2-year warranty.",
                5990, 3499, 42, INR,
                "https://m.media-amazon.com/images/I/61-I1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B09ZI456LM",
                rating: 4.1m, ratingCount: 23456, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Orient Electric Apex-FX 1200mm Ceiling Fan",
                "1200mm BLDC motor ceiling fan, 28W energy efficient. 5-star rated, remote with 6 speeds, reverse rotation, timer. 3 ABS blades, 3-year warranty.",
                3490, 2299, 34, INR,
                "https://m.media-amazon.com/images/I/71-J1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A1J456MN",
                rating: 4.0m, ratingCount: 15678, categoryId: cat["Home & Kitchen"].Id),

            Product.Create("Bajaj Majesty DX6 1000W Dry Iron",
                "1000W non-stick coated dry iron. Micro-thin design (5.8mm), fabric protector, temperature adjustment for all fabrics. Anti-bacterial sole plate, 2-year warranty.",
                1095, 649, 41, INR,
                "https://m.media-amazon.com/images/I/61-K1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A2K456NO",
                rating: 4.2m, ratingCount: 78901, categoryId: cat["Home & Kitchen"].Id),
        ]);

        // ── Health & Personal Care (10) ────────────────────────────────────
        list.AddRange([
            Product.Create("Gillette Mach3 Turbo Razor with 2 Cartridges",
                "Mach3 Turbo men's razor with 2 replacement cartridges. 3 blades, DuraComfort blade coating, Microfin skin guard, comfort gel strip. Up to 15 shaves per blade.",
                599, 299, 50, INR,
                "https://m.media-amazon.com/images/I/71-L1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A3L456OP",
                rating: 4.3m, ratingCount: 89234, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Oral-B All Rounder Pro Health Toothbrush",
                "Medium bristles, 3D cleaning action, CrissCross bristles reach 16% deeper between teeth. Tongue and cheek cleaner on the back of the head. Pack of 2.",
                499, 249, 50, INR,
                "https://m.media-amazon.com/images/I/61-M1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A4M456PQ",
                rating: 4.2m, ratingCount: 56789, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Dettol Liquid Handwash Original 750ml",
                "750ml liquid hand wash with Dettol germ protection. pH-balanced formula, dermatologically tested. Kills 99.99% germs. Pump dispenser. Original fragrance.",
                299, 179, 40, INR,
                "https://m.media-amazon.com/images/I/71-N1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A5N456QR",
                rating: 4.4m, ratingCount: 234567, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Patanjali Saundarya Aloe Vera Gel 150g",
                "Pure aloe vera gel with neem and cucumber extracts. Moisturizes, soothes sunburn, reduces acne. Paraben-free, no artificial colors. Suitable for all skin types.",
                199, 99, 50, INR,
                "https://m.media-amazon.com/images/I/51-O1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A6O456RS",
                rating: 4.1m, ratingCount: 123456, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Himalaya Purifying Neem Face Wash 150ml",
                "150ml neem face wash with turmeric. Removes excess oil, unclogs pores, controls acne. 100% soap-free, pH-balanced, dermatologically tested. No SLS.",
                175, 125, 29, INR,
                "https://m.media-amazon.com/images/I/61-P1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A7P456ST",
                rating: 4.2m, ratingCount: 189234, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Mamaearth Onion Oil for Hair Growth 250ml",
                "250ml onion oil with redensyl and biotin. Reduces hair fall up to 90% in 8 weeks. Strengthens roots, promotes hair growth. Free of SLS, parabens and mineral oil.",
                599, 349, 42, INR,
                "https://m.media-amazon.com/images/I/71-Q1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A8Q456TU",
                rating: 4.0m, ratingCount: 78901, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("WOW Skin Science Vitamin C Serum 30ml",
                "30ml Vitamin C face serum with hyaluronic acid and niacinamide. Brightens skin, reduces dark spots, boosts collagen. Paraben-free, cruelty-free, vegan.",
                799, 499, 38, INR,
                "https://m.media-amazon.com/images/I/61-R1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0A9R456UV",
                rating: 4.1m, ratingCount: 45678, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Omron HEM-7120 Upper Arm Blood Pressure Monitor",
                "Clinical validated upper arm BP monitor. 60-reading memory, irregular heartbeat detection, body movement detection. IntelliSense technology for precise inflation.",
                2995, 1799, 40, INR,
                "https://m.media-amazon.com/images/I/71-S1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AAS456VW",
                rating: 4.3m, ratingCount: 34521, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Lifelong LLBC99 Glucometer with 25 Test Strips",
                "Glucometer with 25 free test strips. Results in 5 seconds, 0.6μL blood sample. 500-reading memory with date/time, 14/30/90-day averages. No coding required.",
                1599, 899, 44, INR,
                "https://m.media-amazon.com/images/I/61-T1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ABT456WX",
                rating: 4.0m, ratingCount: 23456, categoryId: cat["Health & Personal Care"].Id),

            Product.Create("Dr Trust Digital Flexible Tip Thermometer",
                "Digital thermometer with flexible tip, fever alert beep. Results in 60 seconds, 0.1°C accuracy. Oral, rectal or underarm use. Memory recall, waterproof tip.",
                799, 449, 44, INR,
                "https://m.media-amazon.com/images/I/51-U1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ACU456XY",
                rating: 4.2m, ratingCount: 56789, categoryId: cat["Health & Personal Care"].Id),
        ]);

        // ── Sports & Fitness (10) ──────────────────────────────────────────
        list.AddRange([
            Product.Create("Boldfit Heavy Skipping Rope for Exercise",
                "PVC jump rope with steel ball bearings, adjustable length (up to 9 feet). Ball-bearing handles for smooth rotation. Anti-slip grip, suitable for all fitness levels.",
                999, 499, 50, INR,
                "https://m.media-amazon.com/images/I/61-V1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ADV456YZ",
                rating: 4.1m, ratingCount: 56789, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Nivia Carbonite Football Size 5",
                "Size 5 PU football, thermally bonded 32-panel construction. 360° sewing for consistent flight trajectory. 3-ply butyl bladder for air retention. FIFA approved.",
                1299, 699, 46, INR,
                "https://m.media-amazon.com/images/I/71-W1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AEW456ZA",
                rating: 4.0m, ratingCount: 34521, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Vector X Turbo Cricket Bat English Willow",
                "English willow grade 3 cricket bat, 6 grains, weight 1100-1200g. Pre-knocked, oiled handle, protective cover included. Ideal for leather ball cricket.",
                2999, 1799, 40, INR,
                "https://m.media-amazon.com/images/I/61-X1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AFX456AB",
                rating: 4.1m, ratingCount: 23456, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Cosco Badminton Racket Pair with 3 Shuttles",
                "Pack of 2 full-size badminton rackets, aluminium frame, strung with nylon strings. Comes with 3 nylon shuttlecocks and carry bag. Suitable for beginners.",
                899, 549, 39, INR,
                "https://m.media-amazon.com/images/I/71-Y1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AGY456BC",
                rating: 4.0m, ratingCount: 45678, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Strauss Anti-Skid Yoga Mat 6mm",
                "6mm thick anti-skid yoga mat, 183x61cm. NBR foam material, high-density, lightweight (700g). Includes carrying bag and strap. Sweat-absorbent, easy clean.",
                1299, 799, 38, INR,
                "https://m.media-amazon.com/images/I/61-Z1Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AHZ456CD",
                rating: 4.2m, ratingCount: 89234, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Protoner PVC Dumbbell Set 20kg",
                "20kg PVC dumbbell set with chrome dumbbell rods. Includes 4×2.5kg + 4×2.5kg plates. Adjustable weight, anti-skid grip. Complete home gym starter set.",
                2499, 1599, 36, INR,
                "https://m.media-amazon.com/images/I/71-A2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AIA456DE",
                rating: 4.1m, ratingCount: 34521, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Decathlon Kiprun KS500 Running Shoes",
                "Lightweight running shoes, FOAM+ midsole for energy return. Breathable mesh upper, anatomical last for natural foot shape. Drop: 10mm. Weight: 250g.",
                3999, 2499, 37, INR,
                "https://m.media-amazon.com/images/I/61-B2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AJB456EF",
                rating: 4.2m, ratingCount: 67890, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Wildcraft Trailblazer 30L Hiking Backpack",
                "30-litre hiking backpack, HDPE back frame for rigidity. Padded shoulder straps, rain cover included, hydration sleeve. Suitable for 2-3 day treks.",
                2999, 1799, 40, INR,
                "https://m.media-amazon.com/images/I/71-C2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AKC456FG",
                rating: 4.0m, ratingCount: 23456, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Adidas Freelift Training T-Shirt",
                "100% recycled polyester sports t-shirt. HEAT.RDY technology for sweat management, 4-way stretch for full range of motion. Slim fit, crewneck. Available in multiple colors.",
                1299, 799, 38, INR,
                "https://m.media-amazon.com/images/I/61-D2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ALD456GH",
                rating: 4.1m, ratingCount: 12345, categoryId: cat["Sports & Fitness"].Id),

            Product.Create("Yonex Nanoray 7000i Badminton Racket",
                "Graphite badminton racket, isometric head for larger sweet spot. Ultra-slim frame reduces air resistance. Pre-strung at 22 lbs. Weight 3U (85-89g). Comes with full cover.",
                1699, 1099, 35, INR,
                "https://m.media-amazon.com/images/I/71-E2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AME456HI",
                rating: 4.3m, ratingCount: 15678, categoryId: cat["Sports & Fitness"].Id),
        ]);

        // ── Musical Instruments (10) ───────────────────────────────────────
        list.AddRange([
            Product.Create("Yamaha PSR-E373 61-Key Portable Keyboard",
                "61-key touch-sensitive portable keyboard, 622 voices, 205 styles. USB MIDI, 48-note polyphony, lesson function. Battery/adapter powered. Includes sustain pedal port.",
                14999, 10999, 27, INR,
                "https://m.media-amazon.com/images/I/71-F2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ANF456IJ",
                rating: 4.5m, ratingCount: 23456, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Kadence Frontier Series Acoustic Guitar KAG-3",
                "Full-size dreadnought acoustic guitar. Linden top, back and sides. Die-cast chrome tuners, rosewood fingerboard. Includes bag, picks, strings, and tuner.",
                3999, 2999, 25, INR,
                "https://m.media-amazon.com/images/I/61-G2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AOG456JK",
                rating: 4.2m, ratingCount: 45678, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Banjira Tabla Combo Set 13\" Sheesham",
                "Traditional tabla set: 13\" sheesham wood dayan (right drum) with steel bayan (left drum). Ready-to-play, includes tabla bag and hammer. Suitable for students.",
                4999, 3499, 30, INR,
                "https://m.media-amazon.com/images/I/71-H2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0APH456KL",
                rating: 4.1m, ratingCount: 12345, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Casio CT-S300 61-Key Portable Keyboard",
                "61-key keyboard with 48-note polyphony, 100 tones, 50 rhythms. Dance Music Mode, Step-up lesson function. Battery or AC adapter. Ultra-compact design.",
                5999, 4499, 25, INR,
                "https://m.media-amazon.com/images/I/61-I2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AQI456LM",
                rating: 4.3m, ratingCount: 56789, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Juarez JRZ38C 38\" Acoustic Guitar Starter Pack",
                "38-inch folk acoustic guitar for beginners. Basswood top, rosewood fingerboard, 20 frets. Includes padded bag, extra strings, picks, strap, and online lessons access.",
                2999, 1999, 33, INR,
                "https://m.media-amazon.com/images/I/71-J2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ARJ456MN",
                rating: 4.0m, ratingCount: 34521, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Fender FA-115 Acoustic Guitar Pack",
                "Fender FA-115 dreadnought acoustic guitar pack. Spruce top, laminate back and sides, walnut fingerboard. Includes padded gig bag, tuner, picks, strap, and strings.",
                14999, 11999, 20, INR,
                "https://m.media-amazon.com/images/I/61-K2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ASK456NO",
                rating: 4.3m, ratingCount: 8901, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Pearl Export EXX 5-Piece Drum Set",
                "5-piece standard acoustic drum set. Poplar shell, OptiMount tom suspension, 830-series hardware. Includes hardware pack and Sabian SBR cymbals.",
                39999, 29999, 25, INR,
                "https://m.media-amazon.com/images/I/71-L2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0ATL456OP",
                rating: 4.4m, ratingCount: 4521, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Karman 42-Key Harmonium with Coupler",
                "42-key harmonium with scale changer and coupler. Teak wood body, German reeds, 2 sets of bellows. 9 stops, includes carrying bag. Tuned at A=440Hz.",
                12999, 9999, 23, INR,
                "https://m.media-amazon.com/images/I/61-M2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AUM456PQ",
                rating: 4.2m, ratingCount: 6789, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Remo 8\" Coated Ambassador Practice Pad",
                "8-inch single-sided practice pad. Natural coated drumhead surface for realistic rebound. Rubber base for anti-slip placement. Ideal for snare technique practice.",
                2499, 1799, 28, INR,
                "https://m.media-amazon.com/images/I/71-N2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AVN456QR",
                rating: 4.1m, ratingCount: 9876, categoryId: cat["Musical Instruments"].Id),

            Product.Create("Gibson Hummingbird Original Acoustic Guitar",
                "Spruce top, mahogany back and sides, hummingbird pickguard. Rounded C-profile neck, rosewood fretboard, 24.75\" scale. LR Baggs VTC pickup. Hardshell case included.",
                299999, 249999, 17, INR,
                "https://m.media-amazon.com/images/I/61-O2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AWO456RS",
                rating: 4.7m, ratingCount: 2341, categoryId: cat["Musical Instruments"].Id),
        ]);

        // ── Office Products (10) ───────────────────────────────────────────
        list.AddRange([
            Product.Create("HP 805 Black Original Ink Cartridge",
                "Original HP 805 black ink cartridge. Yield ~120 pages at 5% coverage. Compatible with HP DeskJet 1212, 2332, 2336, 2723, 2729 printers. Anti-smear ink.",
                899, 549, 39, INR,
                "https://m.media-amazon.com/images/I/51-P2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AXP456ST",
                rating: 4.0m, ratingCount: 34521, categoryId: cat["Office Products"].Id),

            Product.Create("Canon Pixma MG2577s All-in-One Inkjet Printer",
                "Print, scan and copy. 8 ipm black / 4 ipm color. USB connectivity, auto power on/off. Compatible with Canon 745/746 cartridges. A4 borderless photo printing.",
                6999, 4499, 36, INR,
                "https://m.media-amazon.com/images/I/71-Q2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AYQ456TU",
                rating: 4.1m, ratingCount: 23456, categoryId: cat["Office Products"].Id),

            Product.Create("Epson EcoTank L3150 Wi-Fi Ink Tank Printer",
                "Wi-Fi all-in-one ink tank printer. Print, scan, copy. 33 ppm black, 15 ppm color. Borderless photo printing, mobile print support. 1-year/50,000-page warranty.",
                13999, 10499, 25, INR,
                "https://m.media-amazon.com/images/I/61-R2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0AZR456UV",
                rating: 4.3m, ratingCount: 45678, categoryId: cat["Office Products"].Id),

            Product.Create("Classmate Pulse Executive Ball Pen Pack of 25",
                "Pack of 25 blue ball pens. Smooth-writing 0.7mm tip, comfortable grip, leak-proof. Long write length (>3km). School and office use.",
                199, 99, 50, INR,
                "https://m.media-amazon.com/images/I/51-S2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B0S456VW",
                rating: 4.1m, ratingCount: 89234, categoryId: cat["Office Products"].Id),

            Product.Create("Maped Jumbo Pencil Sharpener",
                "Jumbo-size plastic pencil sharpener, fits standard pencils up to 10mm. Screw-tight shaving reservoir, metal sharpening mechanism. Pack of 2.",
                149, 79, 47, INR,
                "https://m.media-amazon.com/images/I/61-T2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B1T456WX",
                rating: 4.2m, ratingCount: 56789, categoryId: cat["Office Products"].Id),

            Product.Create("Amazon Basics 3-Shelf Bookcase Shelving Unit",
                "3-shelf adjustable bookcase, 36x24x72 inch. Weight capacity 35 lbs/shelf. Engineered wood CARB-compliant board, tool-free assembly. Multiple color options.",
                4999, 3299, 34, INR,
                "https://m.media-amazon.com/images/I/71-U2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B2U456XY",
                rating: 4.0m, ratingCount: 12345, categoryId: cat["Office Products"].Id),

            Product.Create("Leitz Letter A4 Tray Black",
                "Interlockable letter tray for A4 documents. Sturdy durable plastic, smooth surface, portrait orientation. Stackable design, compatible with Leitz tower range.",
                1299, 899, 31, INR,
                "https://m.media-amazon.com/images/I/51-V2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B3V456YZ",
                rating: 4.1m, ratingCount: 6789, categoryId: cat["Office Products"].Id),

            Product.Create("Oddy A4 Copier Paper 75 GSM 500 Sheets",
                "A4 75 GSM multipurpose copier paper, 500 sheets/ream. Suitable for inkjet, laser printers and copiers. Acid-free, ECF pulp, smooth surface. ISO 9706 archival quality.",
                599, 399, 33, INR,
                "https://m.media-amazon.com/images/I/71-W2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B4W456ZA",
                rating: 4.2m, ratingCount: 34521, categoryId: cat["Office Products"].Id),

            Product.Create("Scotch Magic Tape 8m Pack of 4",
                "Pack of 4 clear magic tape rolls (8m each). Writable surface, invisible when applied. Matte finish, acid-free, removes cleanly from most surfaces. Dispenser included.",
                249, 149, 40, INR,
                "https://m.media-amazon.com/images/I/61-X2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B5X456AB",
                rating: 4.3m, ratingCount: 45678, categoryId: cat["Office Products"].Id),

            Product.Create("Fellowes 5 Gallon Bottled Water Cooler Dispenser",
                "Top-loading water cooler for 5-gallon bottles. Hot (90°C) and cold (10°C) water dispenser. Child safety lock, drip tray, LED indicators. Capacity 16L/day.",
                8999, 6499, 28, INR,
                "https://m.media-amazon.com/images/I/71-Y2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B6Y456BC",
                rating: 3.9m, ratingCount: 8901, categoryId: cat["Office Products"].Id),
        ]);

        // ── Car & Motorbike (10) ───────────────────────────────────────────
        list.AddRange([
            Product.Create("3M Scotchgard Paint Protection Film 300mm x 1.5m",
                "Self-healing paint protection film, 300mm x 1.5m roll. Protects against rock chips, scratches, bug damage. UV-resistant, virtually invisible, 10-year warranty.",
                2999, 1999, 33, INR,
                "https://m.media-amazon.com/images/I/61-Z2Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B7Z456CD",
                rating: 4.1m, ratingCount: 23456, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Meguiar's G7101 Ultimate Liquid Car Wax 473ml",
                "473ml polymer liquid car wax with synthetic polymers. Creates hydrophobic layer, 12-month protection. Safe for clear coats, easy wipe-on/wipe-off application.",
                1999, 1399, 30, INR,
                "https://m.media-amazon.com/images/I/71-A3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B8A456DE",
                rating: 4.2m, ratingCount: 15678, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Turtle Wax ICE Seal N Shine 473ml",
                "473ml hybrid ceramic car wax. 12-month UV protection, water beading technology. Streak-free shine, safe on all exterior surfaces including matte finishes.",
                1299, 899, 31, INR,
                "https://m.media-amazon.com/images/I/61-B3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0B9B456EF",
                rating: 4.3m, ratingCount: 34521, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Vroom Portable Car Vacuum Cleaner 12V",
                "12V DC car vacuum cleaner, 100W motor, HEPA filter. 5-metre power cord, 3 attachments (brush, crevice, nozzle). Suction power 3.5kPa. Includes carry bag.",
                1999, 1199, 40, INR,
                "https://m.media-amazon.com/images/I/71-C3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BAC456FG",
                rating: 3.9m, ratingCount: 9876, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Oscar Oxy Clean Car Shampoo 500ml",
                "500ml pH-neutral car shampoo with wax additive. High-foam formula, water-repelling finish. Safe on all paint types, PPF, vinyl wraps. Dilution ratio 1:100.",
                399, 249, 38, INR,
                "https://m.media-amazon.com/images/I/51-D3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BBD456GH",
                rating: 4.0m, ratingCount: 12345, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Bosch S5 Car Battery 60Ah",
                "60Ah maintenance-free car battery. CCA 540A, capacity 60Ah. PowerFrame grid technology, 36% more cyclic durability. Compatible with stop-start systems. 2-year warranty.",
                6499, 4999, 23, INR,
                "https://m.media-amazon.com/images/I/71-E3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BCE456HI",
                rating: 4.2m, ratingCount: 8901, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Autofy F1 Premium Open Face Helmet ISI",
                "ISI certified open face helmet, ABS outer shell, EPS inner lining. Ventilation slots, scratch-resistant visor, detachable inner liner. Weight 1.2kg.",
                2999, 1899, 37, INR,
                "https://m.media-amazon.com/images/I/61-F3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BCF456IJ",
                rating: 4.1m, ratingCount: 23456, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Studds Shifter Full Face Helmet ISI",
                "ISI certified full face helmet. ABS outer shell, aerodynamic design, anti-scratch visor, UV-resistant paint. Ventilation system, anti-bacterial inner padding.",
                3999, 2799, 30, INR,
                "https://m.media-amazon.com/images/I/71-G3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BDG456JK",
                rating: 4.2m, ratingCount: 34521, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("Michelin Pilot Sport 4 195/55 R16 Tyre",
                "195/55 R16 87V summer performance tyre. Variable contact patch for wet/dry grip, high tread stiffness. Speed rating V (240km/h), load index 87.",
                8499, 6799, 20, INR,
                "https://m.media-amazon.com/images/I/61-H3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BEH456KL",
                rating: 4.4m, ratingCount: 12345, categoryId: cat["Car & Motorbike"].Id),

            Product.Create("STP Ultra 5-in-1 Fuel System Cleaner 354ml",
                "354ml complete fuel system cleaner. Cleans injectors, valves, intake ports. Improves fuel economy, reduces emissions. Safe for all petrol and diesel engines.",
                799, 499, 38, INR,
                "https://m.media-amazon.com/images/I/71-I3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BFI456LM",
                rating: 4.0m, ratingCount: 7654, categoryId: cat["Car & Motorbike"].Id),
        ]);

        // ── Toys & Games (10) ─────────────────────────────────────────────
        list.AddRange([
            Product.Create("LEGO Classic Creative Brick Box 10698",
                "790-piece LEGO classic creative box. Includes bricks, windows, doors, wheels, and roof tiles. Builds cars, houses, animals. Ages 4+. Instruction book included.",
                3999, 2799, 30, INR,
                "https://m.media-amazon.com/images/I/71-J3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BGJ456MN",
                rating: 4.6m, ratingCount: 45678, categoryId: cat["Toys & Games"].Id),

            Product.Create("Funskool Scrabble Original Board Game",
                "Original Scrabble board game, 100 letter tiles, 4 tile racks, drawstring tile bag. 2-4 players, ages 8+. Premium board with turntable base.",
                1299, 799, 38, INR,
                "https://m.media-amazon.com/images/I/61-K3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BHK456NO",
                rating: 4.3m, ratingCount: 23456, categoryId: cat["Toys & Games"].Id),

            Product.Create("Mattel UNO Card Game",
                "Classic UNO card game, 112 cards including Action and Wild cards. 2-10 players, ages 7+. Perfect for family game nights and travel.",
                899, 499, 44, INR,
                "https://m.media-amazon.com/images/I/71-L3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BIL456OP",
                rating: 4.4m, ratingCount: 67890, categoryId: cat["Toys & Games"].Id),

            Product.Create("Hot Wheels 20-Car Gift Pack",
                "Pack of 20 die-cast Hot Wheels 1:64 scale cars. Each car individually detailed, unique color. Random assortment includes race cars, trucks and exotic cars. Ages 3+.",
                2499, 1699, 32, INR,
                "https://m.media-amazon.com/images/I/61-M3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BJM456PQ",
                rating: 4.3m, ratingCount: 34521, categoryId: cat["Toys & Games"].Id),

            Product.Create("Barbie Fashionistas Doll with Accessories",
                "Barbie Fashionistas doll with fashion-forward outfit, shoes, and accessories. Bendable knees for multiple poses. Promotes creativity and imaginative play. Ages 3+.",
                1499, 999, 33, INR,
                "https://m.media-amazon.com/images/I/71-N3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BKN456QR",
                rating: 4.2m, ratingCount: 12345, categoryId: cat["Toys & Games"].Id),

            Product.Create("Nerf N-Strike Elite Disruptor Blaster",
                "6-dart rotating drum blaster, slam-fire action. Fires darts up to 27m/90 feet. Includes 12 elite darts. Easy to reload, tactical rail for attachments. Ages 8+.",
                1999, 1399, 30, INR,
                "https://m.media-amazon.com/images/I/61-O3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BLO456RS",
                rating: 4.1m, ratingCount: 23456, categoryId: cat["Toys & Games"].Id),

            Product.Create("Funskool Monopoly India Classic Board Game",
                "Classic Monopoly board game with Indian cities theme. Includes game board, 8 tokens, 32 houses, 12 hotels, 28 title deed cards, 2 dice. 2-8 players, ages 8+.",
                1499, 999, 33, INR,
                "https://m.media-amazon.com/images/I/71-P3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BMP456ST",
                rating: 4.2m, ratingCount: 45678, categoryId: cat["Toys & Games"].Id),

            Product.Create("Fisher-Price Deluxe Kick and Play Piano Gym",
                "Baby play gym with detachable light-up piano. 5 learning modes, 70+ songs/sounds. Tummy time mirror, plush toys, crinkle butterfly. Grows with baby. Ages 0-3.",
                3999, 2499, 37, INR,
                "https://m.media-amazon.com/images/I/61-Q3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BNQ456TU",
                rating: 4.3m, ratingCount: 15678, categoryId: cat["Toys & Games"].Id),

            Product.Create("Crayola 64-Count Crayons Assorted Colors",
                "64 assorted color crayons with built-in sharpener. Vibrant, long-lasting colors, easy-to-peel label. Non-toxic, AP certified. Includes 4 glitter crayons. Ages 3+.",
                999, 649, 35, INR,
                "https://m.media-amazon.com/images/I/71-R3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BOR456UV",
                rating: 4.4m, ratingCount: 34521, categoryId: cat["Toys & Games"].Id),

            Product.Create("Orbeez Wow World Glow Globe Playset",
                "Glow-in-the-dark Orbeez playset with 1500 mini orbeez, globe container and accessories. Sensory play, develops fine motor skills. Non-toxic water beads. Ages 5+.",
                1999, 1299, 35, INR,
                "https://m.media-amazon.com/images/I/61-S3Q5Q5QL._SX679_.jpg",
                "https://www.amazon.in/dp/B0BPS456VW",
                rating: 4.0m, ratingCount: 9876, categoryId: cat["Toys & Games"].Id),
        ]);

        AddVariantSeedData(list, INR);
        return list;
    }

    private static void AddVariantSeedData(List<Product> products, string currency)
    {
        foreach (var product in products.Take(25))
        {
            var primary = product.PrimaryVariant;
            var variant = product.AddVariant(
                "Bundle",
                primary.ActualPrice.Amount + 500,
                primary.DiscountedPrice.Amount + 350,
                primary.DiscountPercentage,
                currency,
                primary.ProductLink,
                primary.DownloadUrl,
                stock: 40,
                options:
                [
                    ("Edition", "Bundle"),
                    ("Warranty", "Extended")
                ]);
        }

        foreach (var product in products.Skip(25).Take(25))
        {
            var primary = product.PrimaryVariant;
            var variant = product.AddVariant(
                "Premium",
                primary.ActualPrice.Amount + 1000,
                primary.DiscountedPrice.Amount + 750,
                primary.DiscountPercentage,
                currency,
                primary.ProductLink,
                primary.DownloadUrl,
                stock: 30,
                options:
                [
                    ("Edition", "Premium"),
                    ("Support", "Priority")
                ]);
        }
    }
}
