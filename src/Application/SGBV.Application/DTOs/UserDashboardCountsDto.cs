namespace SGBV.Application.DTOs;

public record UserDashboardCountsDto(
    int TotalLoans,
    int ActiveLoans,
    int OverdueLoans
    );