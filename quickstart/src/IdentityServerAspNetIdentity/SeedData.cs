// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using static IdentityServerAspNetIdentity.Constants;

namespace IdentityServerAspNetIdentity
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    #region  Seeding Role and Role Claims

                    if (!roleMgr.Roles.Any())
                    {
                        var role = new IdentityRole(Constants.RoleNames.ADMIN);
                        var roleAddResult = roleMgr.CreateAsync(role).Result;

                        if (roleAddResult.Succeeded)
                        {
                            Log.Debug("Admin Role added successfully!");

                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.AdminPermissions.MANAGE)).Wait();
                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.AdminPermissions.MODIFY_RIDE)).Wait();

                            Log.Debug("Admin Role Permissions added successfully!");
                        }

                        role = new IdentityRole(Constants.RoleNames.EMPLOYEE);
                        roleAddResult = roleMgr.CreateAsync(role).Result;

                        if (roleAddResult.Succeeded)
                        {
                            Log.Debug("Employee Role added successfully!");

                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.EmployeePermissions.VIEW_RIDE)).Wait();

                            Log.Debug("Employee Role Permissions added successfully!");
                        }

                        role = new IdentityRole(Constants.RoleNames.DRIVER);
                        roleAddResult = roleMgr.CreateAsync(role).Result;

                        if (roleAddResult.Succeeded)
                        {
                            Log.Debug("Driver Role added successfully!");

                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.DriverPermissions.ACCEPT_RIDE)).Wait();
                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.DriverPermissions.DISCARD_RIDE)).Wait();

                            Log.Debug("Driver Role Permissions added successfully!");
                        }

                        role = new IdentityRole(Constants.RoleNames.CUSTOMER);
                        roleAddResult = roleMgr.CreateAsync(role).Result;

                        if (roleAddResult.Succeeded)
                        {
                            Log.Debug("Customer Role added successfully!");

                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.CustomerPermissions.VIEW_RIDE)).Wait();
                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.CustomerPermissions.BOOK_RIDE)).Wait();
                            roleMgr.AddClaimAsync(role, new Claim(Constants.CustomClaimNames.PERMISSION, Constants.CustomerPermissions.CANCEL_RIDE)).Wait();

                            Log.Debug("Customer Role Permissions added successfully!");
                        }

                        Log.Debug("Roles and Role Claims created");
                    }
                    else
                    {
                        Log.Debug("Roles already exists");
                    }

                    #endregion

                    #region  Seeding Users and assigning roles

                    var alice = userMgr.FindByNameAsync("alice").Result;
                    if (alice == null)
                    {
                        alice = new ApplicationUser
                        {
                            UserName = "alice",
                            Email = "AliceSmith@email.com",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(alice, "Password@1").Result;

                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(CustomClaimNames.LOCATION, "Dallas, TX")
                        }).Result;

                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        else
                        {
                            Log.Debug("alice created");

                            userMgr.AddToRoleAsync(alice, Constants.RoleNames.ADMIN).Wait();
                            userMgr.AddToRoleAsync(alice, Constants.RoleNames.EMPLOYEE).Wait();

                            Log.Debug("alice was provided the Admin role");
                        }
                    }
                    else
                    {
                        Log.Debug("alice already exists");
                    }

                    var bob = userMgr.FindByNameAsync("bob").Result;
                    if (bob == null)
                    {
                        bob = new ApplicationUser
                        {
                            UserName = "bob",
                            Email = "BobSmith@email.com",
                            EmailConfirmed = true
                        };
                        var result = userMgr.CreateAsync(bob, "Password@1").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim(CustomClaimNames.LOCATION, "Boston, MA")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        else
                        {
                            Log.Debug("bob created");

                            userMgr.AddToRoleAsync(bob, Constants.RoleNames.DRIVER).Wait();
                            userMgr.AddToRoleAsync(bob, Constants.RoleNames.EMPLOYEE).Wait();

                            Log.Debug("bob was provided the Driver role");
                        }
                    }
                    else
                    {
                        Log.Debug("bob already exists");
                    }

                    var jan = userMgr.FindByNameAsync("jan").Result;
                    if (jan == null)
                    {
                        jan = new ApplicationUser
                        {
                            UserName = "jan",
                            Email = "Jenniffer@email.com",
                            EmailConfirmed = true
                        };
                        var result = userMgr.CreateAsync(jan, "Password@1").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(jan, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Jenniffer Connelly"),
                            new Claim(JwtClaimTypes.GivenName, "Jenniffer"),
                            new Claim(JwtClaimTypes.FamilyName, "Connelly"),
                            new Claim(JwtClaimTypes.WebSite, "http://jan.com")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        else
                        {
                            Log.Debug("jan created");

                            userMgr.AddToRoleAsync(jan, Constants.RoleNames.CUSTOMER).Wait();

                            Log.Debug("jan was provided the Customer role");
                        }
                    }
                    else
                    {
                        Log.Debug("jan already exists");
                    }

                    #endregion
                }
            }
        }
    }
}
