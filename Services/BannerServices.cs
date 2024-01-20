using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;

namespace ANNIE_SHOP.Services
{
    public class BannerServices : IBannerService
    {
        public async Task<string> SubirImgenStorage(Stream archivo, string nombre)
        {
            string email = "estefanochacinl@gmail.com";
            string clave = "Polaris123*";
            string ruta = "annieshop2-a8c9c.appspot.com";
            string api_key = "AIzaSyB2O2tkicgiqd930p9CNiHw0_6lT9oMWsw";


            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
            var a = await auth.SignInWithEmailAndPasswordAsync(email, clave);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage
                (
                    ruta,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true,
                    })
                    .Child("Images_Banners")
                    .Child(nombre)
                    .PutAsync(archivo, cancellation.Token);

            var donwloadURL = await task;
            return donwloadURL;
        }
    }
}