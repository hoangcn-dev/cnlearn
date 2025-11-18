# Hướng Dẫn Cài Đặt và Sử Dụng Trang Login

## 📦 Cài Đặt NG-ZORRO

```bash
npm install ng-zorro-antd --save
```

## 🎨 Import CSS vào angular.json

Thêm vào `angular.json` trong phần `styles`:

```json
"styles": [
  "node_modules/ng-zorro-antd/ng-zorro-antd.min.css",
  "src/styles.css"
]
```

## 🎯 Các Tính Năng Đã Implement

### ✅ 1. Toast Service (Reusable)

**Vị trí:** `src/app/core/services/toast.service.ts`

**Cách sử dụng:**

```typescript
// Inject service
constructor(private toastService: ToastService) {}

// Success message
this.toastService.success('Đăng nhập thành công!');

// Error message
this.toastService.error('Email hoặc mật khẩu không chính xác!');

// Warning message
this.toastService.warning('Vui lòng điền đầy đủ thông tin!');

// Info message
this.toastService.info('Đang xử lý...');

// Notification với title và content
this.toastService.successNotification(
  'Đăng nhập thành công',
  'Chào mừng bạn quay trở lại!'
);

this.toastService.errorNotification(
  'Đăng nhập thất bại',
  'Vui lòng kiểm tra lại thông tin'
);
```

### ✅ 2. Trang Login

**URL:** `/dang-nhap`

**Vị trí:** `src/app/features/auth/pages/login/`

**Chức năng:**
- ✅ Đăng nhập với Email & Password
- ✅ Validation form (email hợp lệ, password tối thiểu 6 ký tự)
- ✅ Toggle hiển thị/ẩn mật khẩu
- ✅ Checkbox "Ghi nhớ đăng nhập"
- ✅ Link "Quên mật khẩu"
- ✅ Đăng nhập với Google
- ✅ Toast thông báo khi đăng nhập thành công/thất bại
- ✅ Loading state khi xử lý
- ✅ Responsive design

### ✅ 3. Auth Service (Enhanced)

**Vị trí:** `src/app/core/services/auth.service.ts`

**Tính năng:**
- Login với email/password
- Login với Google OAuth (mock)
- Logout
- Check authentication status
- Get current user info
- Observable streams cho auth state

**Sử dụng:**

```typescript
// Đăng nhập
this.authService.login(email, password);

// Đăng nhập Google
this.authService.loginWithGoogle().subscribe(response => {
  this.authService.handleGoogleCallback(response);
});

// Đăng xuất
this.authService.logout();

// Kiểm tra trạng thái
this.authService.isAuthenticated();

// Lấy user hiện tại
this.authService.getCurrentUser();

// Subscribe vào auth state
this.authService.isAuthenticated$.subscribe(isAuth => {
  console.log('Is authenticated:', isAuth);
});
```

## 🎨 Giao Diện

### Design Features:
- ✅ Modern gradient background
- ✅ Card-based layout với shadow
- ✅ Animated inputs with icons
- ✅ Google login button với icon
- ✅ Responsive design cho mobile
- ✅ Professional color scheme
- ✅ Smooth transitions và hover effects

### NG-ZORRO Components Sử Dụng:
- `nz-form` - Form container
- `nz-input` - Input fields
- `nz-button` - Buttons
- `nz-checkbox` - Remember me checkbox
- `nz-icon` - Icons
- `nz-divider` - Divider
- `nz-card` - Card container
- `nz-message` - Toast messages
- `nz-notification` - Notifications

## 🔒 Cấu Hình Google OAuth (Production)

Để sử dụng Google OAuth thật, cần:

1. **Tạo Google OAuth Client ID:**
   - Vào [Google Cloud Console](https://console.cloud.google.com)
   - Tạo project mới hoặc chọn project có sẵn
   - Enable Google+ API
   - Tạo OAuth 2.0 Client ID
   - Thêm authorized redirect URIs

2. **Cài đặt Google Sign-In:**
```bash
npm install @abacritt/angularx-social-login
```

3. **Cấu hình trong app.config.ts:**
```typescript
import { SocialAuthServiceConfig, GoogleLoginProvider } from '@abacritt/angularx-social-login';

providers: [
  {
    provide: 'SocialAuthServiceConfig',
    useValue: {
      autoLogin: false,
      providers: [
        {
          id: GoogleLoginProvider.PROVIDER_ID,
          provider: new GoogleLoginProvider('YOUR_GOOGLE_CLIENT_ID')
        }
      ]
    } as SocialAuthServiceConfig,
  }
]
```

4. **Update LoginComponent:**
```typescript
import { SocialAuthService, GoogleLoginProvider } from '@abacritt/angularx-social-login';

constructor(private socialAuthService: SocialAuthService) {}

loginWithGoogle(): void {
  this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID)
    .then(user => {
      // Handle successful login
      console.log(user);
    })
    .catch(error => {
      this.toastService.error('Đăng nhập Google thất bại');
    });
}
```

## 🚀 Testing

### Test Login:
1. Truy cập `http://localhost:4200/dang-nhap`
2. Nhập email: `test@example.com`
3. Nhập password: `123456` (tối thiểu 6 ký tự)
4. Click "Đăng nhập"
5. Xem toast thông báo và được redirect đến dashboard

### Test Google Login:
1. Click nút "Đăng nhập với Google"
2. Xem loading state
3. Xem notification thông báo
4. Được redirect đến dashboard

## 📝 TODO - Các Tính Năng Cần Thêm

- [ ] Implement real API integration
- [ ] Add forgot password page (`/quen-mat-khau`)
- [ ] Add register page (`/dang-ky`)
- [ ] Implement real Google OAuth flow
- [ ] Add remember me functionality
- [ ] Add password strength indicator
- [ ] Add CAPTCHA for security
- [ ] Add rate limiting
- [ ] Add email verification
- [ ] Add 2FA (Two-Factor Authentication)

## 🎯 Best Practices Đã Áp Dụng

1. ✅ **Separation of Concerns:** Service riêng cho Toast, Auth
2. ✅ **Reusable Components:** Toast service có thể dùng ở mọi nơi
3. ✅ **Type Safety:** Interfaces và types đầy đủ
4. ✅ **Lazy Loading:** Route lazy load cho performance
5. ✅ **Reactive Forms:** Validation mạnh mẽ
6. ✅ **Observable Patterns:** RxJS cho state management
7. ✅ **Standalone Components:** Modern Angular architecture
8. ✅ **Professional UI/UX:** NG-ZORRO components
9. ✅ **Error Handling:** Toast notifications cho user feedback
10. ✅ **Loading States:** Visual feedback khi xử lý

## 📚 Tài Liệu Tham Khảo

- [NG-ZORRO Documentation](https://ng.ant.design/docs/introduce/en)
- [Angular Forms](https://angular.dev/guide/forms)
- [Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2)
- [RxJS](https://rxjs.dev/)

## 💡 Tips

1. **Customize Theme:** Có thể customize NG-ZORRO theme trong `styles.css`
2. **i18n:** NG-ZORRO hỗ trợ đa ngôn ngữ, có thể chuyển sang tiếng Việt
3. **Icons:** Sử dụng `nz-icon` cho consistent icons
4. **Responsive:** Test trên nhiều devices
5. **Performance:** Sử dụng lazy loading và standalone components
