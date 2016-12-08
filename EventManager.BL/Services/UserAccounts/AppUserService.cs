﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using BrockAllen.MembershipReboot;
using EventManager.AccountPolicy;
using EventManager.BL.DTOs.UserAccounts;
using EventManager.DAL.Enums;

namespace EventManager.BL.Services.UserAccounts
{
    public class AppUserService : EventManagerService, IAppUserService
    {
        private readonly UserAccountService<DAL.Entities.UserAccount> mCoreService;

        public AppUserService(UserAccountService<DAL.Entities.UserAccount> service)
        {
            mCoreService = service;
        }

        public Guid RegisterUserAccount(UserRegistrationDTO userRegistration, UserRole role = UserRole.Member)
        {
            using (UnitOfWorkProvider.Create())
            {
                var userClaims = new List<Claim>();

                switch (role)
                {
                    case UserRole.Administrator:
                        userClaims.Add(new Claim(ClaimTypes.Role, Claims.Admin));
                        break;
                    case UserRole.Organizer:
                        userClaims.Add(new Claim(ClaimTypes.Role, Claims.Organizer));
                        break;
                    default:
                        userClaims.Add(new Claim(ClaimTypes.Role, Claims.Member));
                        break;
                }

                var account = mCoreService.CreateAccount(null, userRegistration.Password, userRegistration.Email, null,
                    null);

                AutoMapper.Mapper.Map(userRegistration, account);

                foreach (var claim in userClaims)
                {
                    mCoreService.AddClaim(account.ID, claim.Type, claim.Value);
                }

                mCoreService.Update(account);

                return account.ID;
            }
        }

        public Guid AuthenticateUser(UserLoginDTO loginDto)
        {
            DAL.Entities.UserAccount account;
            var result = mCoreService.Authenticate(loginDto.Username, loginDto.Password, out account);
            if (!result)
            {
                Debug.WriteLine($"Failed to authenticate user: {loginDto.Username}");
                return Guid.Empty;
            }
            return account.ID;
        }

        public void UpdateUserRole(Guid userAccountId, string role)
        {
            using (UnitOfWorkProvider.Create())
            {
                Claim userClaim;
                
                switch (role)
                {
                    case Claims.Admin:
                        userClaim = new Claim(ClaimTypes.Role, Claims.Admin);
                        break;
                    case Claims.Organizer:
                        userClaim = new Claim(ClaimTypes.Role, Claims.Organizer);
                        break;
                    default:
                        userClaim = new Claim(ClaimTypes.Role, Claims.Member);
                        break;
                }

                mCoreService.RemoveClaim(userAccountId, ClaimTypes.Role);
                mCoreService.AddClaim(userAccountId, userClaim.Type, userClaim.Value);
            }
        }
    }
}
