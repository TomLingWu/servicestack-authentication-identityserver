﻿namespace ServiceStack.Core.Authentication.IdentityServer.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Interfaces;
    using Logging;

    internal class RefreshTokenClient : IRefreshTokenClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RefreshTokenClient));

        private readonly IIdentityServerAuthProviderSettings appSettings;

        public RefreshTokenClient(IIdentityServerAuthProviderSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public async Task<TokenRefreshResult> RefreshToken(string refreshToken)
        {
            var client = new TokenClient(appSettings.RequestTokenUrl, appSettings.ClientId, appSettings.ClientSecret);
            var result = await client.RequestAsync(new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken}
            }).ConfigureAwait(false);

            if (result.IsError)
            {
                Log.Error($"An error occurred while refreshing the access token - {result.Error}");
                return new TokenRefreshResult();
            }

            return new TokenRefreshResult
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(result.ExpiresIn)
            };
        }
    }
}