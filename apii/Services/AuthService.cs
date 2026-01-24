using apii.Models.DTOs;
using apii.Models.Entities;
using apii.Repositories;
using Microsoft.EntityFrameworkCore;

namespace apii.Services;

/// <summary>
/// Interface cho Authentication Service
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto);
    Task<UserInfoDto?> GetUserByIdAsync(int userId);
}

/// <summary>
/// Authentication Service với Business Logic
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("Email và mật khẩu không được để trống");
            }

            // Find user by email
            var user = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("Email hoặc mật khẩu không đúng");
            }

            // Verify password
            bool isPasswordValid = false;
            
            try
            {
                // Try BCrypt verification first
                isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            }
            catch
            {
                // If BCrypt fails, try plain text comparison (for development/testing only)
                // This allows using plain passwords from database during development
                isPasswordValid = user.PasswordHash == loginDto.Password;
                
                // If plain text matched, update to BCrypt hash for security
                if (isPasswordValid)
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
                    _unitOfWork.Repository<User>().Update(user);
                }
            }
            
            if (!isPasswordValid)
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("Email hoặc mật khẩu không đúng");
            }

            // Check if account is active
            if (!user.IsActive)
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên");
            }

            // Update last login date
            user.LastLoginDate = DateTime.Now;
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Create response
            var response = new LoginResponseDto
            {
                Token = GenerateToken(user),
                UserInfo = new UserInfoDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role
                }
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Đăng nhập thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse($"Lỗi hệ thống: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Validate input
            var validationErrors = ValidateRegisterDto(registerDto);
            if (validationErrors.Any())
            {
                return ApiResponse<string>.ErrorResponse("Dữ liệu không hợp lệ", validationErrors);
            }

            // Check if email exists
            var existingUser = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser != null)
            {
                return ApiResponse<string>.ErrorResponse("Email đã được sử dụng");
            }

            // Create new user
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = "Customer",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(
                "Đăng ký thành công", 
                "Đăng ký thành công! Vui lòng đăng nhập để tiếp tục"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.ErrorResponse($"Lỗi hệ thống: {ex.Message}");
        }
    }

    public async Task<UserInfoDto?> GetUserByIdAsync(int userId)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
        if (user == null) return null;

        return new UserInfoDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        };
    }

    private List<string> ValidateRegisterDto(RegisterDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.FullName))
            errors.Add("Họ tên không được để trống");

        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add("Email không được để trống");
        else if (!IsValidEmail(dto.Email))
            errors.Add("Email không đúng định dạng");

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            errors.Add("Số điện thoại không được để trống");
        else if (!IsValidPhoneNumber(dto.PhoneNumber))
            errors.Add("Số điện thoại không hợp lệ");

        if (string.IsNullOrWhiteSpace(dto.Password))
            errors.Add("Mật khẩu không được để trống");
        else if (dto.Password.Length < 6)
            errors.Add("Mật khẩu phải có ít nhất 6 ký tự");

        if (dto.Password != dto.ConfirmPassword)
            errors.Add("Mật khẩu xác nhận không khớp");

        return errors;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length >= 10 && phoneNumber.All(char.IsDigit);
    }

    private string GenerateToken(User user)
    {
        // Simple token generation (replace with JWT in production)
        return $"{user.Id}_{user.Email}_{Guid.NewGuid()}";
    }
}
