using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Infrastructure.Service
{
    public class FirebaseAuthService
    {
        public FirebaseAuthService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("C:\\Users\\mayurramdham\\Desktop\\SmartData\\Assesment\\Assesment_8MovieApplication\\MayurRamdham\\MovieFullStackApplication\\Backend\\Movie_Application\\Backend\\firebase-adminsdk.json")
                });
            }
        }
        public async Task<FirebaseToken> VerifyTokenAsync(string idToken)
        {
            return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        }
    }
   
}
