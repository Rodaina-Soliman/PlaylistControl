using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.DTOs
{
    /// <summary>
    /// Representation of a User
    /// </summary>
    /// <param name="Id">Unique identifier of the User</param>
    /// <param name="Username">Username of the User</param>
    /// <param name="Email">Email of the User</param>
    public record UserDto(
        Guid Id,
        string Username,
        string Email);

    /// <summary>
    /// Extension methods for mapping <see cref="User"/> entities to <see cref="UserDto"/>
    /// </summary>
    public static class UserDtoExtensions
    {
        /// <summary>
        /// Projects a <see cref="User"/> into a <see cref="UserDto"/>
        /// </summary>
        /// <param name="user">User entity to project</param>
        /// <returns>A new <see cref="UserDto"/></returns>
        public static UserDto ToUserDto(this User user)
            => new(
                user.Id,
                user.Username,
                user.Email);
    }
}