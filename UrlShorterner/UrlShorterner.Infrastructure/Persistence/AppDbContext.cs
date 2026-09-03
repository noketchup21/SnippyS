using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Link> Links => Set<Link>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<LinkClick> LinkClicks => Set<LinkClick>();
        public DbSet<LinkClickDailyStat> LinkClickDailyStats => Set<LinkClickDailyStat>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<AdminLog> AdminLogs => Set<AdminLog>();
        public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();
        public DbSet<PayOsOrder> PayOsOrders => Set<PayOsOrder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("citext");
            modelBuilder.HasPostgresExtension("pgcrypto");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
