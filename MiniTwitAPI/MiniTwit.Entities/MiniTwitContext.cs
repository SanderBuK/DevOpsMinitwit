﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MiniTwit.Entities
{
    public partial class MiniTwitContext : DbContext, IMiniTwitContext
    {
        public virtual DbSet<Follower> Followers { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public MiniTwitContext(DbContextOptions<MiniTwitContext> options) : base(options)
        {
            
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);//why do we call this method?
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);//do we need this?

    }
}
