using System;

namespace PactNet
{
    public class PactAuthorizationOptions
    {
        public string AccessToken { get; private set; }

        public PactAuthorizationOptions(string accessToken)
        {
            if (String.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("accessToken is null or empty.");
            }

            AccessToken = accessToken;
        }
    }
}