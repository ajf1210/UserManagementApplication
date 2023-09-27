using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UserManagementApplication.Models;

namespace UserManagementApplication.Components
{
    public class DBManager : DbContext
    {
        public DbSet<UserInfo> Users { get; set; }

        public DBManager() 
        {
            this.Database.EnsureCreated();
            if(!this.Users.Any()) // 테이블에 데이터가 존재하지 않을 경우 초기 데이터 생성.
                Init();
        }

        /// <summary>
        /// 데이터베이스 초기 데이터 생성 Method
        /// </summary>
        public void Init()
        {
            this.Users?.AddRange(new Models.UserInfo[]
            {
                new Models.UserInfo { Name = "User1", Age = 25, Contact = "010-1111-1111", PreparationStatus = true },
                new Models.UserInfo { Name = "User2", Age = 30, Contact = "010-2222-2222", PreparationStatus = true },
                new Models.UserInfo { Name = "User3", Age = 35, Contact = "010-3333-3333", PreparationStatus = true },
                new Models.UserInfo { Name = "User4", Age = 40, Contact = "010-4444-4444", PreparationStatus = true },
                new Models.UserInfo { Name = "User5", Age = 45, Contact = "010-5555-5555", PreparationStatus = true },
                new Models.UserInfo { Name = "User6", Age = 50, Contact = "010-6666-6666", PreparationStatus = true },
                new Models.UserInfo { Name = "User7", Age = 55, Contact = "010-7777-7777", PreparationStatus = true },
                new Models.UserInfo { Name = "User8", Age = 60, Contact = "010-8888-8888", PreparationStatus = true },
                new Models.UserInfo { Name = "User9", Age = 65, Contact = "010-9999-9999", PreparationStatus = true },
                new Models.UserInfo { Name = "User10", Age = 70, Contact = "010-0000-0000", PreparationStatus = true}
            });

            this.SaveChanges();
        }
        /// <summary>
        /// SQLite DB 연동 설정.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=sqlitedb.db", option => 
            {
                option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// DB Table 연동 설정
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>().ToTable("UsersDatabase", "UsersTable");
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(k => k.UserId); // UserId 기본키 설정
                entity.Property(k => k.UserId).ValueGeneratedOnAdd(); // UserId 삽입 시 AutoIncrement 설정
            });
            base.OnModelCreating(modelBuilder);
        }

    }
}
