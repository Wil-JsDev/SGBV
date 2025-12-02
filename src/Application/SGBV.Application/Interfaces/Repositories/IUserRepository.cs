using SGBV.Domain.Models;

namespace SGBV.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Retrieves a user by their **email address**.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The <see cref="User"/> object with the specified email, or null if not found.</returns>
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if an **email address** is already in use by **another** user.
    /// This is typically used during profile editing to exclude the current user.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="userId">The ID of the current user (the one performing the operation).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the email is in use by a **different** user; otherwise, false.</returns>
    Task<bool> EmailInUseAsync(string email, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the provided **email address** belongs to the **current user**.
    /// Useful for verifying if a user is attempting to update their profile with their own existing email (no change).
    /// </summary>
    /// <param name="userId">The ID of the current user.</param>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the email address is associated with the provided <paramref name="userId"/>; otherwise, false.</returns>
    Task<bool> EmailInUseByYouAsync(Guid userId, string email, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a user's **password** in the database.
    /// The new password is expected to be hashed before persistence.
    /// </summary>
    /// <param name="user">The <see cref="User"/> object whose password will be updated.</param>
    /// <param name="newPassword">The **new password** (which should be or is already hashed).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdatePasswordAsync(User user, string newPassword, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if an **email address** **exists** in the database (regardless of which user owns it).
    /// Useful for registration or password recovery processes.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the email exists; otherwise false.</returns>
    Task<bool> EmailExistAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user along with any **detailed/related information** that may be required (e.g., roles, permissions).
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The <see cref="User"/> object with loaded details, or null if not found.</returns>
    Task<User> GetUserDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<int> GetTotalUserCountAsync(CancellationToken cancellationToken);
}