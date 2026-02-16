namespace RentApp.Api.DTOs;

public record UserDto(int Id, string Name, string Email, string? Phone, string? City, string? Country, DateTime CreatedAt);

public record CreateUserRequest(string Name, string Email, string? Phone, string? City, string? Country);

public record UpdateUserRequest(string? Name, string? Phone, string? City, string? Country);
