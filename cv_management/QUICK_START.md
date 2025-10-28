# 🚀 QUICK START - Jobs & Company API

## Bước 1: Chạy ứng dụng

```bash
cd cv_management
dotnet run
```

Ứng dụng sẽ chạy tại: `http://localhost:5000` (hoặc cổng khác)

---

## Bước 2: Test API bằng file HTTP

1. Mở file `jobs_api_test.http`
2. Chạy từng request theo thứ tự

### Flow cơ bản:

```
┌─────────────────────┐
│  1. ĐĂNG KÝ / LOGIN │
│   → Lấy JWT Token   │
└──────────┬──────────┘
           │
           ↓
┌─────────────────────┐
│  2. TẠO COMPANY     │
│   → Company Profile │
└──────────┬──────────┘
           │
           ↓
┌─────────────────────┐
│  3. TẠO JOB         │
│   → Đăng tuyển      │
└─────────────────────┘
```

---

## Bước 3: Copy Token

Sau khi đăng nhập, copy token từ response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."  // ← Copy cái này
}
```

Dán vào biến `@token` trong file `jobs_api_test.http`:

```http
@token = eyJhbGciOiJIUzI1NiIs...
```

---

## 📌 API Endpoints

### Authentication
- `POST /api/Auth/register` - Đăng ký
- `POST /api/Auth/login` - Đăng nhập

### Company
- `GET /api/Company` - Danh sách companies
- `GET /api/Company/{id}` - Chi tiết company
- `GET /api/Company/my-company` - Company của tôi 🔒
- `POST /api/Company` - Tạo company 🔒
- `PUT /api/Company/{id}` - Cập nhật company 🔒
- `DELETE /api/Company/{id}` - Xóa company 🔒

### Jobs
- `GET /api/Jobs` - Danh sách jobs (có filter & pagination)
- `GET /api/Jobs/{id}` - Chi tiết job
- `GET /api/Jobs/company/{companyId}` - Jobs của 1 company
- `POST /api/Jobs` - Tạo job mới 🔒
- `PUT /api/Jobs/{id}` - Cập nhật job 🔒
- `DELETE /api/Jobs/{id}` - Xóa job 🔒

🔒 = Yêu cầu JWT token (phải đăng nhập)

---

## 🧪 Test Cases

### 1. Đăng ký user
```http
POST http://localhost:5000/api/Auth/register
Content-Type: application/json

{
  "username": "company_user",
  "email": "company@example.com",
  "password": "password123",
  "fullName": "Tech Corp Owner"
}
```

### 2. Tạo Company
```http
POST http://localhost:5000/api/Company
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "name": "Tech Corp Vietnam",
  "address": "TP.HCM",
  "description": "Công ty công nghệ"
}
```

### 3. Tạo Job
```http
POST http://localhost:5000/api/Jobs
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "title": "Senior Backend Developer (.NET)",
  "description": "Tuyển developer giỏi .NET",
  "requirements": "3+ years experience",
  "salaryRange": "25-35 triệu VND",
  "location": "TP.HCM",
  "jobType": "Full-time",
  "status": "Active"
}
```

---

## 🔍 Filtering & Pagination

### Lọc Jobs theo location:
```http
GET http://localhost:5000/api/Jobs?location=HCM
```

### Lọc Jobs theo jobType:
```http
GET http://localhost:5000/api/Jobs?jobType=Full-time
```

### Phân trang:
```http
GET http://localhost:5000/api/Jobs?pageNumber=1&pageSize=10
```

### Kết hợp nhiều filters:
```http
GET http://localhost:5000/api/Jobs?location=HCM&jobType=Full-time&status=Active&pageNumber=1&pageSize=5
```

---

## ⚠️ Lưu ý quan trọng

1. **Token expires sau 1 giờ** - Phải đăng nhập lại nếu hết hạn
2. **Phải có Company profile** trước khi tạo Job
3. **Chỉ Company owner** mới có thể update/delete Job của mình
4. **1 User chỉ có 1 Company** - Không thể tạo nhiều company

---

## 🐛 Troubleshooting

### Lỗi: "Bạn cần tạo Company profile trước"
→ Phải tạo Company trước khi tạo Job

### Lỗi: 401 Unauthorized
→ Token không hợp lệ hoặc đã hết hạn, đăng nhập lại

### Lỗi: 403 Forbidden
→ Không có quyền (VD: đang cố sửa job của người khác)

### Lỗi: 500 Internal Server Error
→ Xem Console log để biết chi tiết lỗi

---

## 📚 Tài liệu chi tiết

Xem file `GIAI_THICH_DU_AN.md` để hiểu:
- DTOs là gì
- Tại sao cần DTOs
- Kiến trúc dự án
- Best practices
- Hướng phát triển tiếp

---

## ✅ Checklist

- [ ] Chạy được ứng dụng
- [ ] Đăng ký user thành công
- [ ] Đăng nhập và lấy được token
- [ ] Tạo được Company profile
- [ ] Tạo được Job mới
- [ ] Xem được danh sách Jobs
- [ ] Update được Job
- [ ] Filter Jobs theo location/jobType

Good luck! 🎉

