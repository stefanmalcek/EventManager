﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Core.Internal;
using EventManager.BL.DTOs.Filters;
using EventManager.BL.DTOs.Registrations;
using EventManager.BL.DTOs.UserAccounts;
using EventManager.BL.DTOs.Users;
using EventManager.BL.Queries;
using EventManager.BL.Repositories;
using EventManager.BL.Repositories.UserAccount;
using EventManager.DAL.Entities;

namespace EventManager.BL.Services.Users
{
    public class UserService : EventManagerService, IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly UserAccountRepository _userAccountRepository;
        private readonly UserListQuery _userListQuery;
        private readonly UserAccordingToEmailQuery _userAccordingToEmailQuery;

        public UserService(UserRepository userRepository, UserAccountRepository userAccountRepository, UserListQuery userListQuery, UserAccordingToEmailQuery userAccordingToEmailQuery)
        {
            _userRepository = userRepository;
            _userAccountRepository = userAccountRepository;
            _userListQuery = userListQuery;
            _userAccordingToEmailQuery = userAccordingToEmailQuery;
        }

        public void CreateUser(UserRegistrationDTO userDto, Guid accountId)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var userAccount = _userAccountRepository.GetById(accountId);
                var user = Mapper.Map<User>(userDto);
                user.Account = userAccount;
                _userRepository.Insert(user);
                uow.Commit();
            }
        }

        public Guid UpdateUser(UserDTO userDto)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var user = _userRepository.GetById(userDto.Id, i => i.Account, i => i.EventOrganizers);
                Mapper.Map(userDto, user);
                _userRepository.Update(user);
                uow.Commit();
                
                return user.EventOrganizers.IsNullOrEmpty() ? user.Account.ID : new Guid();
            }
        }

        public void DeleteUser(int userId)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                _userRepository.Delete(userId);
                uow.Commit();
            }
        }

        public UserDTO GetUser(int userId)
        {
            using (UnitOfWorkProvider.Create())
            {
                var user = _userRepository.GetById(userId);
                return user == null ? null : Mapper.Map<UserDTO>(user);
            }
        }

        public UserDTO GetUserAccortingToEmail(string email)
        {
            using (UnitOfWorkProvider.Create())
            {
               _userAccordingToEmailQuery.Email = email;
                return _userAccordingToEmailQuery.Execute().SingleOrDefault();
            }
        }

        public IEnumerable<UserDTO> ListUsers(UserFilter filter)
        {
            using (UnitOfWorkProvider.Create())
            {
                _userListQuery.Filter = filter;
                return _userListQuery.Execute() ?? new List<UserDTO>();
            }
        }
    }
}
