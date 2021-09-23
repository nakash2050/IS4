// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
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
                    Name = "custom_identity_scope",
                    DisplayName = "Custom Identity Claims",
                    UserClaims = new string[] { JwtClaimTypes.Actor, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Role, JwtClaimTypes.Address },
                },
                new IdentityResource
                {
                    Name = "permissions",
                    DisplayName = "User Permissions",
                    UserClaims = new string[] { "permissions" },
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("invoice", "Invoice API")
                {
                    Scopes = { "invoice.read", "invoice.pay", "manage" },
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
                // invoice API specific scopes
                new ApiScope(name: "invoice.read",   displayName: "Reads your invoices."),
                new ApiScope(name: "invoice.pay",    displayName: "Pays your invoices."),

                // customer API specific scopes
                new ApiScope(name: "customer.read",    displayName: "Reads you customers information."),
                new ApiScope(name: "customer.contact", displayName: "Allows contacting one of your customers."),

                // shared scope
                new ApiScope(name: "manage", displayName: "Provides administrative access to invoice and customer data.")
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

                    AccessTokenLifetime = 60,

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
                    AccessTokenLifetime = 60,

                    RedirectUris =           { "http://localhost:4200", "http://localhost:4200/silent-renew.html" },
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedCorsOrigins =     { "http://localhost:4200" },

                    RequireConsent = false,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "customer.read",
                        "invoice.read",
                        "custom_identity_scope",
                        "permissions"
                    }
                },
            };
    }
}