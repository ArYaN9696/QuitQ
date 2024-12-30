using quitq_cf.Data;

namespace quitq_cf.Repository
{
    public interface IAuthorisationService
    {
        public Task<TokenResponse> GenerateJWTTokenAsync<T>(T user);
        public Task<string> HashPasswordAsync(string password);
        public Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
    }
}
