using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Prueba5.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using TweetSharp;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Data;

namespace Prueba5.Controllers
{
    public class AccountController : Controller
    {

        private static TranSapoContext transapoContext = new TranSapoContext();
        //Twitter
        private string _consumerKey = "ov8IthavTr6qQQghL7f9A";
        private string _consumerSecret = "72GlljtfAb3HWtyxTSiHkhBZ6pPRM41VmpKcB15s";
        
        //Mail
        private MailAddress fromAddress = new MailAddress("transapo.cl@gmail.com", "TranSapo");
        private const string fromPassword = "proyectoing123";
               
       
        //Twitter
        public ActionResult Authorize()
        {
            // Step 1 - Retrieve an OAuth Request Token
            TwitterService service = new TwitterService(_consumerKey, _consumerSecret);

            var url = Url.Action("AuthorizeCallback", "Account", null, "http");
            // This is the registered callback URL
            OAuthRequestToken requestToken = service.GetRequestToken(url);

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = service.GetAuthorizationUri(requestToken);
            return new RedirectResult(uri.ToString(), false /*permanent*/);
        }

        // This URL is registered as the application's callback at http://dev.twitter.com
        public ActionResult AuthorizeCallback(string oauth_token, string oauth_verifier)
        {
            var requestToken = new OAuthRequestToken { Token = oauth_token };

            // Step 3 - Exchange the Request Token for an Access Token
            TwitterService service = new TwitterService(_consumerKey, _consumerSecret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);

            // Step 4 - User authenticates using the Access Token
            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            TwitterUser user = service.VerifyCredentials();

            FormsAuthentication.SetAuthCookie(user.ScreenName, false);

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/login

        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Cuenta cuenta = GetCuenta(model.Username);

                if ((cuenta == null) || (cuenta.password != model.Password))
                {
                    ModelState.AddModelError("", "Username o Password inválido.");
                }
                else if (cuenta.validado == false)
                {
                    ModelState.AddModelError("", "Aún no has validado tu cuenta. Por favor revisa tu correo y haz click en el enlace de confirmación.");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.AutoLogin);
                    HomeController.Mensajes.Add("¡Inicio de sesión exitoso!");
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        //
        // GET: /Account/register

        public ActionResult register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                bool existeUsername = ExisteUsername(model.Username);
                bool existeEmail = ExisteEmail(model.Email);

                if (existeUsername == false & existeEmail == false)
                {
                   
                    Cuenta cuenta = new Cuenta();
                    cuenta.username = model.Username;
                    cuenta.password = model.Password;
                    cuenta.email = model.Email;
                    cuenta.parametroValidador = (model.Email + "-" + GetParameter()).ToUpper();
                    cuenta.validado = false;
                    transapoContext.Cuentas.Add(cuenta);
                    transapoContext.SaveChanges();
                    HomeController.Mensajes.Add("¡Su cuenta ha sido creada satisfactoriamente!");
                    HomeController.Mensajes.Add("Revisa tu casilla de correo para confirmar tu cuenta");
                    string body = "Estimado " + model.Username + ", \n\n" +
                        "Su cuenta ha sido creada satisfactoriamente. Para finalizar, confirme su cuenta pinchando en el siguiente link:\n\n" +
                        "http://"+Request.Url.Authority+Url.Action("Confirmar")+"/"+cuenta.parametroValidador+" \n\n"+
                        "¡Gracias por ser parte de esta nueva red!";
                    SendEmail(model.Email, model.Username, "Mail de Confirmación", body);
                    return RedirectToAction("Index", "Home");

                }
                if (existeEmail)
                    ModelState.AddModelError("", "Email ya está registrado. Por favor escoger otro");
                if (existeUsername)
                    ModelState.AddModelError("", "Nombre de usuario ya existe. Por favor escoger otro.");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Confirmar(string parametroValidador)
        {
            TranSapoContext db = new TranSapoContext();

            var query = from Cuenta c in db.Cuentas
                        where c.parametroValidador == parametroValidador
                        select c;
            if (query.Count() == 1)
            {
                Cuenta c=new Cuenta();
                foreach (Cuenta cu in query)
                {
                    c = cu; break;
                }

                if (c.validado)
                {
                    HomeController.Mensajes.Add("Ya confirmaste tu cuenta con anterioridad");
                }
                else
                {
                    HomeController.Mensajes.Add("¡Bienvenido a TranSapo " + c.username + "!.\nHas terminado de registrarte con éxito");
                    c.validado = true;
                    db.Entry(c).State = EntityState.Modified;
                    db.SaveChanges();

                    FormsAuthentication.SetAuthCookie(c.username, false);
                    
                 }
            }
            return RedirectToAction("Index", "Home");
        }


        //
        // GET: /Account/LogOff

        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region Metodos privados .. ¿Existe Username?  ¿Existe Email? ¿Usuario y contraseña correctos?

        private Cuenta GetCuenta(string username)
        {
            TranSapoContext tsc = new TranSapoContext();
            var query = from cuenta in tsc.Cuentas where cuenta.username == username select cuenta;
            return query.First<Cuenta>();
        }

        private bool ExisteUsername(string username)
        {
            var query = from cuenta in transapoContext.Cuentas where cuenta.username == username select cuenta;
            return query.Count() > 0;
        }

        private bool ExisteEmail(string email)
        {
            var query = from cuenta in transapoContext.Cuentas where cuenta.email == email select cuenta;
            return query.Count() > 0;
        }

        private bool UsernamePassword(string username, string password)
        {
            var query = from cuenta in transapoContext.Cuentas where cuenta.username == username & cuenta.password == password select cuenta;
            return query.Count() > 0;
        }

        private string GetParameter()
        {
            return Guid.NewGuid().ToString();
        }

        private void SendEmail(string MailTo,string NameTo, string Titulo, string Body)
        {
            try
            {
                
                var toAddress = new MailAddress(MailTo, NameTo);
                
                 string subject = Titulo;
                 string body = Body;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                //return RedirectToAction("CompletionMethod");
            }
            catch (Exception ex)
            {
                ViewData.ModelState.AddModelError("Email Sent Error", ex.ToString());
            }
            //return View();
        }

        #endregion


        #region ClientSide Validacion (Username,Email)
        [HttpPost]
        public JsonResult existeUsername(string username)
        {
            var db = new TranSapoContext();
            var query = from c in db.Cuentas where c.username == username select c;
            return Json(query.Count() == 0);
        }

        [HttpPost]
        public JsonResult existeEmail(string email)
        {
            var db = new TranSapoContext();
            var query = from c in db.Cuentas where c.email == email select c;
            return Json(query.Count() == 0);
        }
        #endregion

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Vaya a http://go.microsoft.com/fwlink/?LinkID=177550 para
            // obtener una lista completa de códigos de estado.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El nombre de usuario ya existe. Escriba un nombre de usuario diferente.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ya existe un nombre de usuario para esa dirección de correo electrónico. Escriba una dirección de correo electrónico diferente.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contraseña especificada no es válida. Escriba un valor de contraseña válido.";

                case MembershipCreateStatus.InvalidEmail:
                    return "La dirección de correo electrónico especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La respuesta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La pregunta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidUserName:
                    return "El nombre de usuario especificado no es válido. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.ProviderError:
                    return "El proveedor de autenticación devolvió un error. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                case MembershipCreateStatus.UserRejected:
                    return "La solicitud de creación de usuario se ha cancelado. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                default:
                    return "Error desconocido. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";
            }
        }
        #endregion
    }
}
