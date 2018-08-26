namespace CSharpeComponents.Auth
{

    public interface IRoleAuth
    {
        int RoleAuthFlag { get; set; }

        object GetRoleId();

        object GetAuthId();

    }

}