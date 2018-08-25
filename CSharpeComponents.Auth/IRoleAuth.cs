namespace CSharpeComponents.Auth
{

    public interface IRoleAuth
    {
        RoleAuthFlagEnum RoleAuthFlag { get; set; }

        object GetRoleId();

        object GetAuthId();

    }

}