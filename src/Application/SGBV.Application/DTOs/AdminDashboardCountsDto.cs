namespace SGBV.Application.DTOs;

public record AdminDashboardCountsDto(
    int ActiveLoans,
    int OverdueLoans,
    int AvailableResources,
    int TotalUsers
    );