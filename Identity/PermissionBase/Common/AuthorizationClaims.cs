using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

namespace PermissionBase.Common
{
    public static class AuthorizationClaims
    {
        public static readonly List<ClaimDetail> ProjectClaims;

        static AuthorizationClaims()
        {
            ProjectClaims = new List<ClaimDetail>();

            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.IsDefined(typeof(NonActionAttribute)) && !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                    .Select(x => new ApiData() { Controller = x.DeclaringType.Name, Action = x.Name, DisplayName = x.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            foreach (ApiData item in controlleractionlist)
            {
                ProjectClaims.Add(new ClaimDetail()
                {
                    Claim = new Claim($"{item.Controller}.{item.Action}", $"{item.Controller}.{item.Action}.value"),
                    DisplayName = item.DisplayName
                });
            }
        }

        public static List<Claim> GetClaims()
        {
            return ProjectClaims.Select(x => x.Claim).ToList();
        }

        private class ApiData
        {
            public string Controller { get; set; }
            public string Action { get; set; }
            public string DisplayName { get; set; }
        }

        public class ClaimDetail
        {
            public Claim Claim { get; set; }
            public string DisplayName { get; set; }
        }
    }
}
