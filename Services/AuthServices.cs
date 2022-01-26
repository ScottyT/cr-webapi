using cr_app_webapi.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace cr_app_webapi.Services
{
    public class AuthServices
    {
        private string? cred;
        private string? projectId;
        //private readonly FirebaseApp _firebaseApp;
        private readonly FirebaseAuth _auth;

        public AuthServices(IOptions<AuthSettings> settings)
        {
            cred = settings.Value.CredentialPath;
            projectId = settings.Value.ProjectId;

            var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("./code-red.json"),
                ProjectId = projectId
            });
            _auth = FirebaseAuth.GetAuth(defaultApp);
        }

        public async Task<string> Verify(string idToken)
        {
            FirebaseToken decodedToken = await _auth.VerifyIdTokenAsync(idToken);
            return decodedToken.Uid;
        }
    }
}