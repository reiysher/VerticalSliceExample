namespace VerticalSliceTemplate.Api.Shared.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}