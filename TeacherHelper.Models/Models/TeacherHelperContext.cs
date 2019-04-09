using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TeacherHelper.Models.Models
{
    public partial class TeacherHelperContext : DbContext
    {
        public TeacherHelperContext()
        {
        }

        public TeacherHelperContext(DbContextOptions<TeacherHelperContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<AssignedTests> AssignedTests { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<Subjects> Subjects { get; set; }
        public virtual DbSet<Tests> Tests { get; set; }
        public virtual DbSet<TestsQuestions> TestsQuestions { get; set; }
        public virtual DbSet<Themes> Themes { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersSubjects> UsersSubjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(ConfigurationManager.ConnectionStrings["TeacherHelperDb"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<Answers>(entity =>
            {
                entity.ToTable("answers", "TeacherHelper");

                entity.HasIndex(e => e.QuestionId)
                    .HasName("question_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasMaxLength(13356)
                    .IsUnicode(false);

                entity.Property(e => e.QuestionId)
                    .HasColumnName("question_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("answers_ibfk_1");
            });

            modelBuilder.Entity<AssignedTests>(entity =>
            {
                entity.ToTable("assigned_tests", "TeacherHelper");

                entity.HasIndex(e => e.TestId)
                    .HasName("test_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AssignedDate).HasColumnName("assigned_date");

                entity.Property(e => e.Class)
                    .HasColumnName("class")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnName("date_created");

                entity.Property(e => e.TestId)
                    .HasColumnName("test_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.AssignedTests)
                    .HasForeignKey(d => d.TestId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("assigned_tests_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AssignedTests)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("assigned_tests_ibfk_1");
            });

            modelBuilder.Entity<Questions>(entity =>
            {
                entity.ToTable("questions", "TeacherHelper");

                entity.HasIndex(e => e.RightAnswerId)
                    .HasName("right_answer_id");

                entity.HasIndex(e => e.ThemeId)
                    .HasName("theme_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasMaxLength(13356)
                    .IsUnicode(false);

                entity.Property(e => e.IsPublic)
                    .HasColumnName("is_public")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.RightAnswerId)
                    .HasColumnName("right_answer_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ThemeId)
                    .HasColumnName("theme_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("char(30)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.RightAnswer)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.RightAnswerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("questions_ibfk_3");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("questions_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("questions_ibfk_2");
            });

            modelBuilder.Entity<Subjects>(entity =>
            {
                entity.ToTable("subjects", "TeacherHelper");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tests>(entity =>
            {
                entity.ToTable("tests", "TeacherHelper");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("author_id");

                entity.HasIndex(e => e.ThemeId)
                    .HasName("theme_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("author_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Class)
                    .HasColumnName("class")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnName("date_created");

                entity.Property(e => e.IsPublic)
                    .HasColumnName("is_public")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.ThemeId)
                    .HasColumnName("theme_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Tests)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("tests_ibfk_2");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.Tests)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("tests_ibfk_1");
            });

            modelBuilder.Entity<TestsQuestions>(entity =>
            {
                entity.HasKey(e => new { e.TestId, e.QuestionId });

                entity.ToTable("tests_questions", "TeacherHelper");

                entity.HasIndex(e => e.QuestionId)
                    .HasName("question_id");

                entity.Property(e => e.TestId)
                    .HasColumnName("test_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.QuestionId)
                    .HasColumnName("question_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.QuestionPlace)
                    .HasColumnName("question_place")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.TestsQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("tests_questions_ibfk_2");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.TestsQuestions)
                    .HasForeignKey(d => d.TestId)
                    .HasConstraintName("tests_questions_ibfk_1");
            });

            modelBuilder.Entity<Themes>(entity =>
            {
                entity.ToTable("themes", "TeacherHelper");

                entity.HasIndex(e => e.ParentThemeId)
                    .HasName("parent_theme_id");

                entity.HasIndex(e => e.PreviousThemeId)
                    .HasName("previous_theme_id");

                entity.HasIndex(e => new { e.SubjectId, e.ParentThemeId, e.Name })
                    .HasName("unique_subject_theme")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.ParentThemeId)
                    .HasColumnName("parent_theme_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PreviousThemeId)
                    .HasColumnName("previous_theme_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ParentTheme)
                    .WithMany(p => p.InverseParentTheme)
                    .HasForeignKey(d => d.ParentThemeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("themes_ibfk_2");

                entity.HasOne(d => d.PreviousTheme)
                    .WithMany(p => p.InversePreviousTheme)
                    .HasForeignKey(d => d.PreviousThemeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("themes_ibfk_3");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Themes)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("themes_ibfk_1");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users", "TeacherHelper");

                entity.HasIndex(e => e.CurrentSubjectId)
                    .HasName("current_subject_id");

                entity.HasIndex(e => e.Email)
                    .HasName("email")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("username")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CurrentSubjectId)
                    .HasColumnName("current_subject_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.IsAdmin)
                    .HasColumnName("is_admin")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.ProfilePicture)
                    .HasColumnName("profile_picture")
                    .HasColumnType("blob");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.CurrentSubject)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CurrentSubjectId)
                    .HasConstraintName("users_ibfk_1");
            });

            modelBuilder.Entity<UsersSubjects>(entity =>
            {
                entity.HasKey(e => new { e.SubjectId, e.UserId });

                entity.ToTable("users_subjects", "TeacherHelper");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.UsersSubjects)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("users_subjects_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersSubjects)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("users_subjects_ibfk_2");
            });
        }
    }
}
