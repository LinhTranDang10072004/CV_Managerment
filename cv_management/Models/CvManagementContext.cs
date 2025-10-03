using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cv_management.Models;

public partial class CvManagementContext : DbContext
{
    public CvManagementContext()
    {
    }

    public CvManagementContext(DbContextOptions<CvManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobView> JobViews { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<PersonalInfo> PersonalInfos { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavedJob> SavedJobs { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<WorkExperience> WorkExperiences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); var configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__applicat__3BCBDCF2D1686348");

            entity.ToTable("applications");

            entity.HasIndex(e => e.CvId, "idx_applications_cv");

            entity.HasIndex(e => e.JobId, "idx_applications_job");

            entity.HasIndex(e => e.Status, "idx_applications_status");

            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("applied_at");
            entity.Property(e => e.CoverLetter).HasColumnName("cover_letter");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Cv).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_applications_cvs");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_applications_jobs");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__audit_lo__9E2397E0FFD048B3");

            entity.ToTable("audit_logs");

            entity.HasIndex(e => e.UserId, "idx_audit_logs_user");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_audit_logs_user");
        });

        modelBuilder.Entity<Certification>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("PK__certific__185D5AECB72F20CC");

            entity.ToTable("certifications");

            entity.HasIndex(e => e.CvId, "idx_certifications_cv");

            entity.Property(e => e.CertificationId).HasColumnName("certification_id");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.Issuer)
                .HasMaxLength(100)
                .HasColumnName("issuer");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.Cv).WithMany(p => p.Certifications)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_certifications_cvs");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__companie__3E267235F0F417F5");

            entity.ToTable("companies");

            entity.HasIndex(e => e.UserId, "idx_companies_user");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .HasColumnName("website");

            entity.HasOne(d => d.User).WithMany(p => p.Companies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_companies_users");
        });

        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.CvId).HasName("PK__cvs__C36883E66D8E3DE8");

            entity.ToTable("cvs", tb => tb.HasTrigger("trg_cvs_update"));

            entity.HasIndex(e => e.IsDefault, "idx_cvs_default");

            entity.HasIndex(e => e.IsPublic, "idx_cvs_public");

            entity.HasIndex(e => e.UserId, "idx_cvs_user");

            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .HasColumnName("is_default");
            entity.Property(e => e.IsPublic)
                .HasDefaultValue(true)
                .HasColumnName("is_public");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_cvs_users");
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("PK__educatio__45C0CFE77C2D3857");

            entity.ToTable("education");

            entity.HasIndex(e => e.CvId, "idx_education_cv");

            entity.Property(e => e.EducationId).HasColumnName("education_id");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.Degree)
                .HasMaxLength(100)
                .HasColumnName("degree");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Major)
                .HasMaxLength(100)
                .HasColumnName("major");
            entity.Property(e => e.SchoolName)
                .HasMaxLength(100)
                .HasColumnName("school_name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Cv).WithMany(p => p.Educations)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_education_cvs");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__jobs__6E32B6A5DDECDD24");

            entity.ToTable("jobs", tb => tb.HasTrigger("trg_jobs_update"));

            entity.HasIndex(e => e.CompanyId, "idx_jobs_company");

            entity.HasIndex(e => e.Status, "idx_jobs_status");

            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.JobType)
                .HasMaxLength(50)
                .HasColumnName("job_type");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.Requirements).HasColumnName("requirements");
            entity.Property(e => e.SalaryRange)
                .HasMaxLength(50)
                .HasColumnName("salary_range");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_jobs_companies");
        });

        modelBuilder.Entity<JobView>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PK__job_view__B5A34EE2F0E9E829");

            entity.ToTable("job_views");

            entity.HasIndex(e => e.JobId, "idx_job_views_job");

            entity.Property(e => e.ViewId).HasColumnName("view_id");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("viewed_at");

            entity.HasOne(d => d.Job).WithMany(p => p.JobViews)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_job_views_job");

            entity.HasOne(d => d.User).WithMany(p => p.JobViews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_job_views_user");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__language__804CF6B3797E0207");

            entity.ToTable("languages");

            entity.HasIndex(e => e.CvId, "idx_languages_cv");

            entity.Property(e => e.LanguageId).HasColumnName("language_id");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.LanguageName)
                .HasMaxLength(50)
                .HasColumnName("language_name");
            entity.Property(e => e.Proficiency)
                .HasMaxLength(20)
                .HasColumnName("proficiency");

            entity.HasOne(d => d.Cv).WithMany(p => p.Languages)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_languages_cvs");
        });

        modelBuilder.Entity<PersonalInfo>(entity =>
        {
            entity.HasKey(e => e.PersonalInfoId).HasName("PK__personal__0F38C331CAF33A0B");

            entity.ToTable("personal_info");

            entity.HasIndex(e => e.CvId, "idx_personal_cv");

            entity.Property(e => e.PersonalInfoId).HasColumnName("personal_info_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Summary).HasColumnName("summary");

            entity.HasOne(d => d.Cv).WithMany(p => p.PersonalInfos)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_personal_info_cvs");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__projects__BC799E1F2CDAAA02");

            entity.ToTable("projects");

            entity.HasIndex(e => e.CvId, "idx_projects_cv");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Technologies)
                .HasMaxLength(255)
                .HasColumnName("technologies");

            entity.HasOne(d => d.Cv).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_projects_cv");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CC5C2D0D29");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "UQ__roles__783254B1121626D4").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<SavedJob>(entity =>
        {
            entity.HasKey(e => e.SavedId).HasName("PK__saved_jo__04DC0EE76CC7FCFB");

            entity.ToTable("saved_jobs");

            entity.HasIndex(e => e.JobId, "idx_saved_jobs_job");

            entity.HasIndex(e => e.UserId, "idx_saved_jobs_user");

            entity.Property(e => e.SavedId).HasColumnName("saved_id");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.SavedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("saved_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Job).WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_saved_jobs_job");

            entity.HasOne(d => d.User).WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_saved_jobs_user");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__skills__FBBA83792AA60D16");

            entity.ToTable("skills");

            entity.HasIndex(e => e.CvId, "idx_skills_cv");

            entity.HasIndex(e => e.SkillName, "idx_skills_name");

            entity.Property(e => e.SkillId).HasColumnName("skill_id");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.Proficiency)
                .HasMaxLength(20)
                .HasColumnName("proficiency");
            entity.Property(e => e.SkillName)
                .HasMaxLength(100)
                .HasColumnName("skill_name");

            entity.HasOne(d => d.Cv).WithMany(p => p.Skills)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_skills_cvs");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F940EBF4E");

            entity.ToTable("users", tb => tb.HasTrigger("trg_users_update"));

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164007AFFF3").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC57256CBDF26").IsUnique();

            entity.HasIndex(e => e.Email, "idx_user_email");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__user_rol__B8D9ABA297B8183D");

            entity.ToTable("user_roles");

            entity.HasIndex(e => e.RoleId, "idx_user_roles_role");

            entity.HasIndex(e => e.UserId, "idx_user_roles_user");

            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_user_roles_roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_user_roles_users");
        });

        modelBuilder.Entity<WorkExperience>(entity =>
        {
            entity.HasKey(e => e.ExperienceId).HasName("PK__work_exp__EB216AFC88A2BD52");

            entity.ToTable("work_experience");

            entity.HasIndex(e => e.CvId, "idx_experience_cv");

            entity.Property(e => e.ExperienceId).HasColumnName("experience_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasColumnName("company_name");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Cv).WithMany(p => p.WorkExperiences)
                .HasForeignKey(d => d.CvId)
                .HasConstraintName("FK_work_experience_cvs");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
