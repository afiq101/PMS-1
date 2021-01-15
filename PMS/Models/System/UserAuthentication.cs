﻿using Microsoft.Owin.Security;
using Newtonsoft.Json;
using PMS.Models.Database;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace PMS.Models
{
    public class UserAuthentication
    {
        private static ClaimsIdentity Identity(User user)
        {
            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            var userDataJson = identity.FindFirst("UserDataJson");
            var userName = identity.FindFirst(ClaimTypes.Name);
            var userEmail = identity.FindFirst(ClaimTypes.Email);
            var userProfilePic = identity.FindFirst("ProfilePicUrl");

            if (userDataJson != null)
            {      
                photogEntities db = new photogEntities();
                user = JsonConvert.DeserializeObject<User>(userDataJson.Value);
                user = db.Users.FirstOrDefault(x => x.id == user.id);

                identity.RemoveClaim(userDataJson);
                identity.RemoveClaim(userName);
                identity.RemoveClaim(userEmail);
                identity.RemoveClaim(userProfilePic);
            }

            var userObj = new User { id = user.id, email = user.email, name = user.name, dateofbirth = user.dateofbirth, isVerified = user.isVerified, imgprofile = user.imgprofile, phonenumber = user.phonenumber };

            var userData = JsonConvert.SerializeObject(userObj, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            string urlPic = (string.IsNullOrWhiteSpace(user.imgprofile)) ? "https://storagephotog2.blob.core.windows.net/user-data/default/default-profile.jpg" : String.Format("https://storagephotog2.blob.core.windows.net/user-data/{0}/{1}", user.id, user.imgprofile);

            var claimsIdentity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name , user.name),
                        new Claim(ClaimTypes.Email, user.email),
                        new Claim(type: "UserDataJson", value: userData),
                        new Claim(type: "ProfilePicUrl", value: urlPic)
                    }, "ApplicationCookie");

            return claimsIdentity;
        }

        public static bool SignIn(HttpContextBase context, string email, string password, bool rememberMe)
        {
            photogEntities db = new photogEntities();
            try
            {
                var user = db.Users.FirstOrDefault(x => x.email.ToLower() == email && x.password == password);

                if (user != null)
                {
                    var identity = Identity(user);            
                    var ctx = context.Request.GetOwinContext();
                    var authManager = ctx.Authentication;

                    authManager.SignIn(identity);

                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public static void SignOut(HttpContextBase context)
        {
            var ctx = context.Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut();
        }

        public static User Identity()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var userData = identity.Claims.FirstOrDefault(x => x.Type == "UserDataJson");
            if (userData != null)
            {
                var userJson = JsonConvert.DeserializeObject<User>(userData.Value);
                photogEntities db = new photogEntities();
                return db.Users.FirstOrDefault(X => X.id == userJson.id);
            }

            return null;
        }

        public static void UpdateClaim()
        {
            var identity = Identity(null);
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = false });
        }
    }

    public class UserRoleProvider : RoleProvider
    {
        photogEntities db = new photogEntities();

        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            return UserAuthentication.Identity().UserSystemRoles.Select(x => x.SystemRole.name).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }

    public class AnonymousOnly : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
              new { action = "Index", controller = "Home" }));
            }
        }
    }
    
    public static class IdentityExtensions
    {
        public static string GetProfilePhotoLink (this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).FindFirst("ProfilePicUrl")?.Value;
        }
    }
}