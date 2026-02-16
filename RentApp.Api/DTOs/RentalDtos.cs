using RentApp.Api.Models;

namespace RentApp.Api.DTOs;

public record RentalDto(
    int Id,
    int ListingId,
    string ListingTitle,
    int RenterId,
    string RenterName,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalAmount,
    RentalStatus Status,
    string? Notes,
    DateTime CreatedAt);

public record CreateRentalRequest(int ListingId, int RenterId, DateTime StartDate, DateTime EndDate, string? Notes);

public record UpdateRentalStatusRequest(RentalStatus Status);
