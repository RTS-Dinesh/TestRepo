namespace RentApp.Api.DTOs;

public record CategoryDto(int Id, string Name, string? Description, string? IconName, int? ParentCategoryId);

public record CreateCategoryRequest(string Name, string? Description, string? IconName, int? ParentCategoryId);

public record UpdateCategoryRequest(string? Name, string? Description, string? IconName, int? ParentCategoryId);
