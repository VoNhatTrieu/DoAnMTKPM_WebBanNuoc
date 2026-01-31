namespace apii.Models;

/// <summary>
/// Interface for entities that support ownership-based access control
/// </summary>
public interface IOwnable
{
    /// <summary>
    /// Owner ID of the entity
    /// </summary>
    int OwnerId { get; set; }
}

/// <summary>
/// User context interface for accessing current user information
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the current user ID from authentication context
    /// </summary>
    int? GetCurrentUserId();
    
    /// <summary>
    /// Gets the current user's role
    /// </summary>
    string? GetCurrentUserRole();
    
    /// <summary>
    /// Checks if current user is admin
    /// </summary>
    bool IsAdmin();
    
    /// <summary>
    /// Checks if current user can access resource with given OwnerId
    /// </summary>
    bool CanAccess(int ownerId);
}

/// <summary>
/// Simple implementation of user context (will be replaced with proper authentication)
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetCurrentUserId()
    {
        // Get from claims when authentication is implemented
        var userIdClaim = _httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }

    public string? GetCurrentUserRole()
    {
        // Get from claims when authentication is implemented
        return _httpContextAccessor.HttpContext?.Request.Headers["X-User-Role"].FirstOrDefault();
    }

    public bool IsAdmin()
    {
        return GetCurrentUserRole()?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
    }

    public bool CanAccess(int ownerId)
    {
        // Admin can access everything
        if (IsAdmin())
            return true;
            
        // Regular users can only access their own data
        var currentUserId = GetCurrentUserId();
        return currentUserId.HasValue && currentUserId.Value == ownerId;
    }
}

/// <summary>
/// Exception thrown when ownership validation fails
/// </summary>
public class OwnershipViolationException : UnauthorizedAccessException
{
    public OwnershipViolationException(string message) : base(message)
    {
    }
    
    public OwnershipViolationException(int resourceId, int requestedOwnerId, int? actualUserId)
        : base($"Access denied. User {actualUserId} cannot access resource {resourceId} owned by {requestedOwnerId}")
    {
    }
}

/// <summary>
/// Helper class for ownership validation logic
/// </summary>
public static class OwnershipValidator
{
    /// <summary>
    /// Validates that current user can access the resource
    /// </summary>
    public static void ValidateAccess(IUserContext userContext, int ownerId, string resourceName = "Resource")
    {
        if (!userContext.CanAccess(ownerId))
        {
            throw new OwnershipViolationException(
                $"Access denied to {resourceName}. User does not have permission to access this resource.");
        }
    }
    
    /// <summary>
    /// Gets the appropriate OwnerId for filtering based on user role
    /// Returns null for admin (see all), specific ID for regular users (see only owned)
    /// </summary>
    public static int? GetFilterOwnerId(IUserContext userContext)
    {
        if (userContext.IsAdmin())
            return null; // Admin sees all
            
        return userContext.GetCurrentUserId(); // Users see only their own
    }
}
