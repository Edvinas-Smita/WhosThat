using Backend.Logic;
using System.Net;
using System.Web.Http;

namespace Backend.Controllers
{
    public class TokenController : ApiController
    {
        // TODO: channge to POST later
        [AllowAnonymous]
        public string Get(string username, string password)
        {
            if (CheckUser(username, password))
            {
                return JwtManager.GenerateToken(username);
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        public bool CheckUser(string username, string password)
        {
            // should check in the database
            return true;
        }
    }
}