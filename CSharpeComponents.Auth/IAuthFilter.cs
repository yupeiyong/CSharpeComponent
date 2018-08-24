namespace CSharpeComponents.Auth
{

    public interface IAuthFilter
    {

        bool CanAccess(object userId, string url);

        void ReloadRoleAuth();

    }

}