﻿using System;
using System.Collections.Generic;
using System.Linq;
using MeetingWebsite.DAL.EF;
using MeetingWebsite.DAL.Interfaces;
using MeetingWebsite.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingWebsite.DAL.Repositories
{
    public class UserProfileRepository : IRepository<UserProfile>
    {
        private MeetingDbContext _db;

        public UserProfileRepository(MeetingDbContext db)
        {
            _db = db;
        }

        public IEnumerable<UserProfile> GetAll()
        {
            return _db.UserProfiles;
        }

        public UserProfile Get(int id)
        {
            return _db.UserProfiles.Find(id);
        }

        public IEnumerable<UserProfile> Find(Func<UserProfile, bool> predicate)
        {
            return _db.UserProfiles.Where(predicate);
        }

        public void Create(UserProfile item)
        {
            _db.UserProfiles.AddRange(item);
        }

        public void Update(UserProfile item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var userProfile = _db.UserProfiles.Find(id);
            if (userProfile != null)
                _db.UserProfiles.Remove(userProfile);
        }
    }
}