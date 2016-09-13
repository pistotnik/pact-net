using System;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Testing;

namespace Provider.Api.Web.Tests
{
    public abstract class BaseApiTestFixture : IDisposable
    {
        protected TestServer Server { get; set; }

        protected IDataProtector DataProtector { get; set; }

        protected virtual void AfterServerSetup()
        {
            // Nothing
        }

        public virtual void Dispose()
        {
            if (Server != null)
            {
                Server.Dispose();
            }
        }
    }
}