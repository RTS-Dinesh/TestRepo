namespace RentApp.Api.DTOs;

public record ListingDto(
    int Id,
    string Title,
    string Description,
    int CategoryId,
    string CategoryName,
    int OwnerId,
    string OwnerName,
    decimal PricePerDay,
    decimal? PricePerWeek,
    decimal? PricePerMonth,
    string? City,
    string? Country,
    string? ImageUrl,
    bool IsAvailable,
    DateTime CreatedAt);

public record CreateListingRequest(
    string Title,
    string Description,
    int CategoryId,
    int OwnerId,
    decimal PricePerDay,
    decimal? PricePerWeek,
    decimal? PricePerMonth,
    string? City,
    string? Country,
    string? ImageUrl);

public record UpdateListingRequest(
    string? Title,
    string? Description,
    int? CategoryId,
    decimal? PricePerDay,
    decimal? PricePerWeek,
    decimal? PricePerMonth,
    string? City,
    string? Country,
    string? ImageUrl,
    bool? IsAvailable);

public record ListingSearchRequest(
    int? CategoryId,
    string? City,   
    string? Country,
    decimal? MaxPricePerDay,
    bool? IsAvailableOnly = null
    );
