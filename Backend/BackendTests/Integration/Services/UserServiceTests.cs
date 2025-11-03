using Backend.Data;
using Backend.Entities;
using Backend.Entities.Enums;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BackendTests.Integration.Services;

public class UserServiceTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly UserService _userService;

    /// <summary>
    /// Assigns the context to the UserServiceTests class
    /// </summary>
    public UserServiceTests()
    {
        // Initialize in-memory database
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dataContext = new DataContext(options);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestingThatIsLongEnough");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _userService = new UserService(mockConfig.Object, _dataContext);
    }

    /// <summary>
    /// Dispose method to delete the database after the tests are run
    /// </summary>
    public void Dispose()
    {
        // Clean up the in-memory database
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    private void SeedData()
    {
        var user = new Tutor
        {
            Id = 1,
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "912345678",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Address = "Rua do João",
            ZipCode = "1234-567",
            Role = UserRole.Tutor
        };

        _dataContext.Tutors.Add(user);
        _dataContext.SaveChanges();
        _dataContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsAuthResponse_WhenCredentialsValid()
    {
        // Arrange
        SeedData();
        var email = "joao@example.com";
        var password = "password";

        // Act
        var result = await _userService.AuthenticateUserAsync(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("João Silva", result.LoginResponse.Name);
        Assert.Equal(UserRole.Tutor, result.LoginResponse.Role);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";

        // Act
        var result = await _userService.AuthenticateUserAsync(email, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsNull_WhenPasswordInvalid()
    {
        // Arrange
        SeedData();
        var email = "joao@example.com";
        var password = "wrongpassword";

        // Act
        var result = await _userService.AuthenticateUserAsync(email, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GenerateToken_ReturnsValidJwt()
    {
        // Arrange
        var userId = 1;
        var userRole = UserRole.Patient;

        // Act
        var token = _userService.GenerateToken(userId, userRole);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
        Assert.NotNull(jwtToken);

        var nameIdentifierClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
        Assert.NotNull(nameIdentifierClaim);
        Assert.Equal(userId.ToString(), nameIdentifierClaim.Value);

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal(userRole.ToString(), roleClaim.Value);

        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Equal("TestAudience", jwtToken.Audiences.First());
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task ValidateDuplicatesAsync_ThrowsValidationException_WhenEmailDuplicate()
    {
        // Arrange
        SeedData();
        var fieldsToCheck = new Dictionary<string, string> { { "Email", "joao@example.com" } };

        // Act & Assert
        await Assert.ThrowsAsync<Backend.Exceptions.ValidationException>(async () =>
        {
            await _userService.ValidateDuplicatesAsync<User>(fieldsToCheck);
        });
    }

    [Fact]
    public async Task ValidateDuplicatesAsync_ThrowsValidationException_WhenPhoneDuplicate()
    {
        // Arrange
        SeedData();
        var fieldsToCheck = new Dictionary<string, string> { { "PhoneNumber", "912345678" } };

        // Act & Assert
        await Assert.ThrowsAsync<Backend.Exceptions.ValidationException>(async () =>
        {
            await _userService.ValidateDuplicatesAsync<User>(fieldsToCheck);
        });
    }

    [Fact]
    public async Task ValidateDuplicatesAsync_DoesNotThrow_WhenNoDuplicates()
    {
        // Arrange
        var fieldsToCheck = new Dictionary<string, string> { { "Email", "unique@example.com" }, { "PhoneNumber", "999999999" } };

        // Act & Assert
        await _userService.ValidateDuplicatesAsync<User>(fieldsToCheck);
        // No exception should be thrown
    }
}
