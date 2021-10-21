// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServerAspNetIdentity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile() { Required = true },
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResource
                {
                    Name = "permissions",
                    DisplayName = "User Permissions",
                    UserClaims = new string[] { "permission", "location", JwtClaimTypes.Role },
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("invoice", "Ride Booking API")
                {
                    Scopes = { "ride.view", "ride.modify", "manage", "ride.accept", "ride.discard", "ride.book", "ride.cancel" },
                    UserClaims = new [] { JwtClaimTypes.Role }
                },

                new ApiResource("customer", "Customer API")
                {
                    Scopes = { "customer.read", "customer.contact", "manage" },
                    UserClaims = new [] { JwtClaimTypes.Address } // Rather than using this, load claims on ApiScope level
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                // Ride Booking API specific scopes
                new ApiScope(name: "ride.view",   displayName: "View the ride"),
                new ApiScope(name: "ride.modify",    displayName: "Modify the ride."),
                new ApiScope(name: "ride.accept",    displayName: "Accept the ride"),
                new ApiScope(name: "ride.discard", displayName: "Discard the ride"),
                new ApiScope(name: "ride.book", displayName: "Book the ride"),
                new ApiScope(name: "ride.cancel", displayName: "Cancel the ride"),

                // shared scope
                new ApiScope(name: "manage", displayName: "Manage the rides")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "client",
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RequirePkce = true,

                    RequireConsent = true,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    // AlwaysIncludeUserClaimsInIdToken = true,

                    AllowOfflineAccess = true, // for refresh tokens

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "custom_identity_scope",
                        "invoice.pay",
                        "customer.read"
                    },


                },
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,

                    RedirectUris =           { "https://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "https://localhost:5003/index.html" },
                    AllowedCorsOrigins =     { "https://localhost:5003" },

                    RequireConsent = true,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1",
                        "custom_identity_scope"
                    }
                },
                new Client
                {
                    ClientId = "postman",
                    ClientName = "Postman Client",

                    // Resource Owner (grant_type=password) needs client id, client secret, username and password
                    // Client Credentials (grant_type=client_credentials) needs only client id, client secret. No Identity scopes or offline_access scope are supported
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,

                    AllowOfflineAccess = true,
                    RequireClientSecret = true,

                    AccessTokenLifetime = 3600,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // scopes that client has access to
                    AllowedScopes = {
                        "invoice.read",
                        "customer.contact",
                        "manage",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "permissions"
                    }
                },
                // Angular
                new Client
                {
                    ClientId = "angular",
                    ClientName = "Angular Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 600,

                    RedirectUris =           { "http://localhost:4200", "http://localhost:4200/silent-renew.html" },
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedCorsOrigins =     { "http://localhost:4200" },

                    RequireConsent = true,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "ride.modify",
                        "ride.book",
                        "custom_identity_scope",
                        "permissions"
                    }
                },
            };
    }
}