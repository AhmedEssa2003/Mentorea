using CsvHelper;
using Mentorea.Abstractions.Enums;
using Mentorea.Entities;
using System.Globalization;
using System.IO;

namespace Mentorea.Helpers
{
    public static class ReadData
    {
        public static List<Specialization> ReadCsvSpecializationData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Specialization>().ToList();
                return records;
            }
        }
        public static List<Field> ReadCsvFieldData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<FieldCsv>().ToList();
                return records.Select(x => new Field
                {
                    Id = x.Id,
                    FieldName = x.Name,
                    SpecializationId = x.specialization_id
                }).ToList();
            }

        }
        public static List<ApplicationUser> ReadCsvMenteeData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<MenteeCsv>().ToList();
                return records.Select(x=>new ApplicationUser
                {
                    Id = x.Id,
                    Name = x.Name,
                    PirthDate = x.PirthDate,
                    About = x.About,
                    ConcurrencyStamp = x.ConcurrencyStamp,
                    Email = x.Email,
                    EmailConfirmed = x.EmailConfirmed,
                    Gender = x.Gender,
                    IsDisabled = x.IsDisabled,
                    Location = x.Location,
                    LockoutEnabled = x.LockoutEnabled,
                    NormalizedEmail = x.NormalizedEmail,
                    NormalizedUserName = x.NormalizedUserName,
                    PasswordHash = x.PasswordHash,
                    SecurityStamp = x.SecurityStamp,
                    UserName = x.UserName,
                    PathPhoto = x.PathPhoto
                }
                ).ToList();
            }
        }
        public static List<ApplicationUser> ReadCsvMentorData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<MentorCsv>().ToList();
                return records.Select(x => new ApplicationUser
                {
                    Id = x.Id,
                    Name = x.Name,
                    PirthDate = x.PirthDate,
                    About = x.About,
                    ConcurrencyStamp = x.ConcurrencyStamp,
                    Email = x.Email,
                    EmailConfirmed = x.EmailConfirmed,
                    Gender = x.Gender,
                    IsDisabled = x.IsDisabled,
                    Location = x.Location,
                    LockoutEnabled = x.LockoutEnabled,
                    NormalizedEmail = x.NormalizedEmail,
                    NormalizedUserName = x.NormalizedUserName,
                    PasswordHash = x.PasswordHash,
                    SecurityStamp = x.SecurityStamp,
                    UserName = x.UserName,
                    Rate = x.Rate,
                    PriceOfSession = (int)x.PriceOfSession,
                    NumberOfSession = x.NumberOfSession,
                    NumberOfExperience = x.NumberOfExperience,
                    FieldId = x.FieldId,
                    PathPhoto = x.PathPhoto

                }
                ).ToList();
            }
        }
        public static List<IdentityUserRole<string>> ReadCsvUserRoleData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<UserRoleCsv>().ToList();
                return records.Select(x => new IdentityUserRole<string>
                {
                    RoleId = x.RoleId,
                    UserId = x.UserId
                }).ToList();
            }
        }
        public static List<MenteeFieldInterests> ReadCsvMenteeFieldInterestsData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<MenteeFieldInterestsCsv>().ToList();
                return records.Select(x => new MenteeFieldInterests
                {
                    MenteeId = x.mentee_id,
                    FieldId = x.field_id
                }).ToList();
            }
        }
        public static List<Session> ReadCsvSessionData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<SessionCsv>().ToList();
                return records.Select(x=>new Session
                {
                    Id = x.SessionId,
                    MentorId = x.MentorId,
                    MenteeId = x.MenteeId,
                    ScheduledTime = x.ScheduledTime,
                    DurationMinutes = 15,
                    WaitingTime = x.WaitingTime,
                    Price = (int)x.Price,
                    Status = SessionStatus.completed,
                    Notes = x.Notes,
                    Comment = x.Comment,
                    Rating = x.Rating,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    MentorJoinedAt = x.MentorJoinedAt,
                    MenteeJoinedAt = x.MenteeJoinedAt


                }).ToList();
            }
        }
        private class SessionCsv
        {
            public string SessionId { get; set; } = null!;
            public string MentorId { get; set; } = null!;
            public string MenteeId { get; set; } = null!;
            public DateTime ScheduledTime { get; set; }
            public int DurationMinutes { get; set; }
            public int WaitingTime { get; set; } 
            public decimal Price { get; set; } 
            public string Status { get; set; } = null!;
            public string Notes { get; set; } = null!;
            public string Comment { get; set; } = null!;
            public int Rating { get; set; } 
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public DateTime MentorJoinedAt { get; set; }
            public DateTime MenteeJoinedAt { get; set; }

        }
        private class SpecializationCsv
        {
            public string Id { get; set; } = null!;
            public string SpecializationName { get; set; } = null!;
        }
        private class MenteeFieldInterestsCsv
        {
            public string mentee_id { get; set; } = null!;
            public string field_id { get; set; } = null!;
        }
        private class UserRoleCsv
        {
            public string RoleId { get; set; } = null!;
            public string UserId { get; set; } = null!;
        }
        private class FieldCsv
        {
            public string Id { get; set; } = null!;
            public string Name { get; set; } = null!;
            public string specialization_id { get; set; } = null!;
        }
        private class MenteeCsv
        {
            public string Id { get; set; } = null!;
            public string Name { get; set; } = null!;
            public DateOnly PirthDate { get; set; }
            public string Location { get; set; } = null!;
            public Gender Gender { get; set; }
            public string About { get; set; } = null!;
            public bool IsDisabled { get; set; }
            public string UserName { get; set; } = null!;
            public string NormalizedUserName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string NormalizedEmail { get; set; } = null!;
            public bool EmailConfirmed { get; set; }
            public string PasswordHash { get; set; } = null!;
            public string SecurityStamp { get; set; } = null!;
            public string ConcurrencyStamp { get; set; } = null!;
            public bool LockoutEnabled { get; set; }
            public string PathPhoto { get; set; } = null!;
        }
        private class MentorCsv
        {
            public string Id { get; set; } = null!;
            public string Name { get; set; } = null!;
            public DateOnly PirthDate { get; set; }
            public string Location { get; set; } = null!;
            public Gender Gender { get; set; }
            public string About { get; set; } = null!;
            public bool IsDisabled { get; set; }
            public string UserName { get; set; } = null!;
            public string NormalizedUserName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string NormalizedEmail { get; set; } = null!;
            public bool EmailConfirmed { get; set; }
            public string PasswordHash { get; set; } = null!;
            public string SecurityStamp { get; set; } = null!;
            public string ConcurrencyStamp { get; set; } = null!;
            public bool LockoutEnabled { get; set; }
            public decimal Rate { get; set; }
            public decimal PriceOfSession { get; set; }
            public int NumberOfSession { get; set; }
            public int NumberOfExperience { get; set; }
            public string FieldId { get; set; } = null!;
            public string PathPhoto { get; set; } = null!;
        }
    }
}
