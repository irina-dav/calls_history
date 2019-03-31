using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CallsHistory.Infrastucture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace CallsHistory.Services
{
    public class LdapService
    {
        private const string MemberOfAttribute = "memberOf";
        private const string DisplayNameAttribute = "displayName";
        private const string SAMAccountNameAttribute = "sAMAccountName";
        private const string TitleAttribute = "title";
        private const string MailAttribute = "mail";

        private readonly LdapConfig config;
        private readonly LdapConnection connection;
        private readonly IHttpContextAccessor httpContextAccessor;
        ILogger<LdapService> logger;

        public LdapService(IOptions<LdapConfig> cfg, IHttpContextAccessor httpCtxAccessor, ILogger<LdapService> logger)
        {
            httpContextAccessor = httpCtxAccessor;           
            config = cfg.Value;
            this.logger = logger;
            connection = new LdapConnection
            {
                SecureSocketLayer = false
            };
        }

        public bool Login(string username, string password)
        {
            bool bSuccess = false;
            connection.Connect(config.Url, LdapConnection.DEFAULT_PORT);
            connection.Bind(config.BindDn, config.BindCredentials);

            var searchFilter = string.Format(config.AuthFilter, username);
            var result = connection.Search(
                config.SearchBase,
                LdapConnection.SCOPE_SUB,
                searchFilter,
                new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute, TitleAttribute, MailAttribute },
                false
            );

            try
            {
                var user = result.Next();
                if (user != null)
                {
                    connection.Bind(user.DN, password);
                    bSuccess = true;
                }
            }
            catch (LdapException ex)
            {
                if (ex.ResultCode == LdapException.INVALID_CREDENTIALS)
                {
                    throw new Exception("Wrong username or password");
                }
                else if (ex.ResultCode == LdapException.LOCAL_ERROR)
                {
                    throw new Exception("You are not in an access group");
                }
                else
                {
                    throw new Exception(ex.resultCodeToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return bSuccess;
        }

        public string GetUserName()
        {
            string account = httpContextAccessor.HttpContext.User.Identity.Name;
            return account;
        }
    }
}
