# 📘 GIẢI THÍCH DỰ ÁN CV MANAGEMENT

## 🎯 Ý TƯỞNG TỔNG QUAN

Dự án này là một **Hệ thống Quản lý CV và Tuyển dụng** với 2 vai trò chính:

### 1. **User (Người tìm việc)**
- Đăng ký tài khoản
- Tạo và quản lý CV của mình
- Xem các Job đang tuyển
- Apply vào các Job (sẽ phát triển)
- Lưu Job yêu thích (SavedJob)

### 2. **Company (Nhà tuyển dụng)**
- Đăng ký tài khoản
- Tạo Company profile
- Đăng Job tuyển dụng
- Xem danh sách ứng viên (sẽ phát triển)

---

## 📦 DTOs LÀ GÌ?

### Định nghĩa
**DTO = Data Transfer Object** là các class dùng để **truyền dữ liệu** giữa Client và Server.

### Tại sao cần DTOs?

#### ❌ Vấn đề khi KHÔNG dùng DTOs:
```csharp
// Trả về trực tiếp Model User
public IActionResult Login() {
    var user = _context.Users.Find(1);
    return Ok(user); // ❌ Nguy hiểm!
}

// Response sẽ bao gồm:
{
  "userId": 1,
  "username": "john",
  "email": "john@example.com",
  "passwordHash": "$2a$10$abc123..." // ❌ Lộ password!
  "createdAt": "2024-01-01",
  // ... tất cả fields trong database
}
```

#### ✅ Giải pháp với DTOs:
```csharp
// Dùng DTO để kiểm soát
public IActionResult Login([FromBody] LoginRequest request) {
    // LoginRequest chỉ nhận username + password
    var user = Authenticate(request.Username, request.Password);
    
    // AuthResponse chỉ trả về thông tin cần thiết
    return Ok(new AuthResponse {
        Token = GenerateToken(user),
        User = new UserInfo {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email
            // ✅ KHÔNG có password!
        }
    });
}
```

---

## 🏗️ KIẾN TRÚC DỰ ÁN

```
cv_management/
│
├── Models/                          # Database Models (Entity Framework)
│   ├── User.cs                      # Bảng Users
│   ├── Company.cs                   # Bảng Companies
│   ├── Job.cs                       # Bảng Jobs
│   ├── Cv.cs                        # Bảng CVs
│   └── ...
│
├── Controllers/                     # API Controllers
│   │
│   ├── Auth/                        # Xác thực
│   │   ├── AuthController.cs       # Login, Register
│   │   └── DTOs/
│   │       ├── LoginRequest.cs     # Input: username, password
│   │       ├── RegisterRequest.cs  # Input: username, email, password, fullName
│   │       └── AuthResponse.cs     # Output: token, user info
│   │
│   ├── Companies/                   # Quản lý công ty
│   │   ├── CompanyController.cs    # CRUD Company
│   │   └── DTOs/
│   │       ├── CreateCompanyRequest.cs   # Input: name, address, description
│   │       ├── UpdateCompanyRequest.cs   # Input: các field cần update
│   │       └── CompanyResponse.cs        # Output: thông tin company
│   │
│   ├── Jobs/                        # Quản lý việc làm
│   │   ├── JobsController.cs       # CRUD Jobs
│   │   └── DTOs/
│   │       ├── CreateJobRequest.cs       # Input: title, description, requirements...
│   │       ├── UpdateJobRequest.cs       # Input: các field cần update
│   │       └── JobResponse.cs            # Output: thông tin job + company
│   │
│   └── CV/                          # Quản lý CV
│       ├── CVController.cs         # CRUD CV
│       └── DTOs/
│           ├── CreateCVRequest.cs        # Input: CV data
│           └── CVResponse.cs             # Output: CV info
│
└── Program.cs                       # Cấu hình ứng dụng (JWT, Database, CORS)
```

---

## 📋 PHÂN TÍCH DTOs TRONG DỰ ÁN

### 1️⃣ AUTH DTOs

#### `LoginRequest.cs` - Input
```csharp
public class LoginRequest
{
    public string Username { get; set; }  // User nhập vào
    public string Password { get; set; }  // User nhập vào
}
```
**Mục đích**: Chỉ nhận 2 thông tin cần thiết để đăng nhập

---

#### `RegisterRequest.cs` - Input
```csharp
public class RegisterRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}
```
**Mục đích**: Nhận thông tin đăng ký, validate trước khi lưu vào DB

---

#### `AuthResponse.cs` - Output
```csharp
public class AuthResponse
{
    public string Message { get; set; }      // "Đăng nhập thành công"
    public string Token { get; set; }        // JWT token
    public UserInfo User { get; set; }       // Thông tin user
}

public class UserInfo
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }  // ["User", "Admin"]
    // ✅ KHÔNG có PasswordHash
}
```
**Mục đích**: 
- Trả về JWT token để client lưu lại
- Trả về thông tin user (KHÔNG bao gồm password)
- Client dùng token này cho các request sau

---

### 2️⃣ JOBS DTOs

#### `CreateJobRequest.cs` - Input
```csharp
public class CreateJobRequest
{
    public string Title { get; set; }           // "Senior .NET Developer"
    public string Description { get; set; }     // Mô tả công việc
    public string? Requirements { get; set; }   // Yêu cầu
    public string? SalaryRange { get; set; }    // "25-35 triệu"
    public string? Location { get; set; }       // "TP.HCM"
    public string? JobType { get; set; }        // "Full-time"
    public string Status { get; set; }          // "Active"
}
```
**Mục đích**: Company điền form này để đăng job mới

**Lưu ý**: 
- KHÔNG cần truyền `CompanyId` vì lấy từ JWT token
- KHÔNG cần truyền `CreatedAt` vì server tự tạo

---

#### `UpdateJobRequest.cs` - Input
```csharp
public class UpdateJobRequest
{
    public string? Title { get; set; }         // Optional
    public string? Description { get; set; }   // Optional
    // ... tất cả đều optional
}
```
**Mục đích**: 
- Cho phép update **từng phần** (partial update)
- Chỉ truyền field nào muốn update
- VD: Chỉ update salary mà không thay đổi title

---

#### `JobResponse.cs` - Output
```csharp
public class JobResponse
{
    public int JobId { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }    // ✅ Kèm theo tên company
    public string Title { get; set; }
    public string Description { get; set; }
    // ... các field khác
}
```
**Mục đích**: 
- Trả về thông tin Job
- **Kèm theo tên Company** (JOIN từ 2 bảng)
- Client không cần gọi thêm API để lấy tên company

---

### 3️⃣ COMPANY DTOs

#### `CreateCompanyRequest.cs` - Input
```csharp
public class CreateCompanyRequest
{
    public string Name { get; set; }          // Tên công ty
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
}
```
**Mục đích**: User tạo company profile để có thể đăng job

---

#### `CompanyResponse.cs` - Output
```csharp
public class CompanyResponse
{
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    // ... các field khác
}
```
**Mục đích**: Trả về thông tin công ty

---

## 🔄 FLOW HOẠT ĐỘNG

### Flow 1: User đăng ký và đăng nhập

```
1. Client gửi RegisterRequest
   POST /api/Auth/register
   {
     "username": "john",
     "email": "john@example.com",
     "password": "123456",
     "fullName": "John Doe"
   }

2. Server xử lý:
   - Validate input
   - Kiểm tra username/email đã tồn tại chưa
   - Hash password
   - Lưu vào database
   - Tạo JWT token
   
3. Server trả về AuthResponse
   {
     "message": "Đăng ký thành công",
     "token": "eyJhbGciOiJIUzI1NiIs...",
     "user": {
       "userId": 1,
       "username": "john",
       "email": "john@example.com",
       "roles": ["User"]
     }
   }

4. Client lưu token vào localStorage/cookie
```

---

### Flow 2: Company tạo Job mới

```
1. User phải đăng nhập trước (có token)

2. User tạo Company profile
   POST /api/Company
   Authorization: Bearer <token>
   {
     "name": "Tech Corp",
     "address": "TP.HCM",
     "description": "Công ty công nghệ"
   }

3. Company tạo Job
   POST /api/Jobs
   Authorization: Bearer <token>
   {
     "title": "Senior .NET Developer",
     "description": "Tuyển developer giỏi .NET",
     "salaryRange": "25-35 triệu"
   }

4. Server xử lý:
   - Lấy UserId từ JWT token
   - Tìm Company của user này
   - Tạo Job mới với CompanyId
   - Lưu vào database
   
5. Server trả về JobResponse
   {
     "jobId": 1,
     "companyId": 1,
     "companyName": "Tech Corp",
     "title": "Senior .NET Developer",
     "status": "Active"
   }
```

---

## 🔐 BẢO MẬT VỚI JWT

### Cách hoạt động:

```
1. User login → Server tạo JWT token
   
2. JWT token chứa:
   - UserId
   - Username
   - Email
   - Roles
   - Expiration time (1 giờ)

3. Mọi request sau đó client gửi kèm token:
   Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

4. Server kiểm tra:
   - Token có hợp lệ không?
   - Token có hết hạn chưa?
   - User có quyền thực hiện action này không?
```

### Ví dụ phân quyền:

```csharp
[HttpPost]
[Authorize]  // ✅ Phải đăng nhập
public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
{
    // Lấy userId từ token
    var userId = User.FindFirst("id")?.Value;
    
    // Kiểm tra user có company không
    var company = await _context.Companies
        .FirstOrDefaultAsync(c => c.UserId == userId);
    
    if (company == null) {
        return BadRequest("Bạn cần tạo Company profile trước");
    }
    
    // Tạo job với CompanyId của user này
    var job = new Job {
        CompanyId = company.CompanyId,
        Title = request.Title,
        // ...
    };
}
```

---

## 🎨 BEST PRACTICES ĐÃ ÁP DỤNG

### ✅ 1. Separation of Concerns
- **Models**: Đại diện cho database tables
- **DTOs**: Đại diện cho API contracts
- **Controllers**: Xử lý logic nghiệp vụ

### ✅ 2. Security
- Không trả về password trong response
- JWT authentication cho các endpoint cần bảo mật
- Kiểm tra quyền sở hữu (user chỉ update/delete được job của mình)

### ✅ 3. Validation
- Validate input trước khi xử lý
- Trả về error message rõ ràng

### ✅ 4. RESTful API Design
```
GET    /api/Jobs           - Lấy danh sách
GET    /api/Jobs/{id}      - Lấy 1 item
POST   /api/Jobs           - Tạo mới
PUT    /api/Jobs/{id}      - Cập nhật
DELETE /api/Jobs/{id}      - Xóa
```

### ✅ 5. Pagination & Filtering
```csharp
GET /api/Jobs?pageNumber=1&pageSize=10&location=HCM&jobType=Full-time
```

### ✅ 6. Include Related Data
```csharp
// Trả về Job kèm tên Company
.Include(j => j.Company)
.Select(j => new JobResponse {
    JobId = j.JobId,
    CompanyName = j.Company.Name  // ✅ JOIN data
})
```

---

## 📊 DATABASE RELATIONSHIPS

```
User (1) ←→ (1) Company
             ↓
Company (1) ←→ (n) Job
             ↓
Job (1) ←→ (n) Application

User (1) ←→ (n) CV
             ↓
CV (1) ←→ (n) Application
```

---

## 🚀 HƯỚNG PHÁT TRIỂN TIẾP

### 1. Application System (Ứng tuyển)
```csharp
POST /api/Jobs/{jobId}/apply
{
  "cvId": 1,
  "coverLetter": "Dear HR..."
}
```

### 2. Saved Jobs (Lưu job yêu thích)
```csharp
POST /api/Jobs/{jobId}/save
DELETE /api/Jobs/{jobId}/unsave
GET /api/Jobs/saved
```

### 3. Job Views (Lượt xem job)
```csharp
POST /api/Jobs/{jobId}/view
GET /api/Jobs/{jobId}/statistics
```

### 4. Search & Advanced Filters
```csharp
GET /api/Jobs/search?keyword=.NET&minSalary=20&maxSalary=40
```

### 5. Email Notifications
- Thông báo khi có người apply
- Thông báo khi job được duyệt

---

## 📝 TÓM TẮT

### DTOs giúp:
1. ✅ **Bảo mật**: Kiểm soát data in/out
2. ✅ **Validation**: Dễ dàng validate input
3. ✅ **Tách biệt**: Models ≠ API contracts
4. ✅ **Linh hoạt**: Có thể thay đổi API mà không ảnh hưởng database
5. ✅ **Chuyên nghiệp**: Chuẩn công nghiệp

### Ý tưởng dự án:
- Hệ thống 2 chiều: **User tìm việc** ↔️ **Company tuyển dụng**
- JWT authentication cho bảo mật
- RESTful API design chuẩn
- Phân quyền rõ ràng
- Có thể mở rộng thêm nhiều tính năng

---

## 📞 LIÊN HỆ & HỖ TRỢ

Nếu có thắc mắc, hãy:
1. Đọc code trong từng Controller để hiểu flow
2. Test API bằng file `jobs_api_test.http`
3. Xem log lỗi trong Console
4. Debug bằng breakpoint trong Visual Studio

Chúc bạn học tập tốt! 🎓

