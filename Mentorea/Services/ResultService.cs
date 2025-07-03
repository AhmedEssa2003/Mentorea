using Mentorea.Contracts.Common;
using Mentorea.Contracts.Response;
using Mentorea.Contracts.Users;
using Mentorea.Entities;
using Mentorea.Errors;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Mentorea.Services
{
    public class ResultService(
        MentoreaDbContext context,
        IDistributedCacheService cacheService,
        HttpClient httpClient
        ) : IResultService
    {
        private readonly MentoreaDbContext _context = context;
        private readonly IDistributedCacheService _cacheService = cacheService;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<PaginatedList<MentorProfileResponse>> GetAllMentorsAsync(RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            
            if (ShouldUseCache(requestFilters))
            {
                var cacheKey = BuildMentorCacheKey(requestFilters);
                var cachedData = await _cacheService.GetAsync<PaginatedList<MentorProfileResponse>>(cacheKey, cancellationToken);

                if (cachedData is not null)
                    return cachedData;
            }
            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        where ur.RoleId == DefaultRole.MentorRoleId
                        select   u ;

            var mentorsWithFields = from u in query
                                    join f in _context.Fields on u.FieldId equals f.Id
                                    where !u.IsDisabled && u.EmailConfirmed
                                           && (string.IsNullOrEmpty(requestFilters.SearchValue) ||
                                           u.Name.Contains(requestFilters.SearchValue) ||
                                           f.FieldName.Contains(requestFilters.SearchValue) ||
                                           (u.About != null && u.About.Contains(requestFilters.SearchValue)))
                                    select new
                                    {
                                        u.Id,
                                        u.Name,
                                        u.Email,
                                        u.PathPhoto,
                                        u.PirthDate,
                                        u.Location,
                                        u.Rate,
                                        u.PriceOfSession,
                                        u.NumberOfExperience,
                                        u.NumberOfSession,
                                        u.About,
                                        f.FieldName
                                    };
            var countNumber = from u in  mentorsWithFields
                              join s in _context.Sessions on u.Id equals s.MentorId
                              where s.Comment != null
                              group s by s.MentorId into g
                              select new
                              {
                                  g.Key,
                                  NumberOfComment = g.Count()
                              };
            var allowedSortFields = new[] { "Name", "Rate", "PriceOfSession" };
            var sortBy = allowedSortFields.Contains(requestFilters.SortBy) ? requestFilters.SortBy : "Rate";


            var finalList = mentorsWithFields
                .OrderBy($"{sortBy} {requestFilters.SortDirection}")
                .Select(x => new MentorProfileResponse(
                    x.Id,
                    x.Name,
                    x.Email!,
                    x.PathPhoto,
                    x.PirthDate,
                    x.Location,
                    x.Rate,
                    x.PriceOfSession,
                    x.NumberOfSession,
                    x.NumberOfExperience,
                    countNumber.Where(c => c.Key == x.Id).Select(c => c.NumberOfComment).FirstOrDefault(),
                    x.About,
                    x.FieldName
                ));

            var response = await PaginatedList<MentorProfileResponse>.CreateAsync(
                finalList.AsNoTracking(),
                requestFilters.PageNumber,
                requestFilters.PageSize,
                cancellationToken
            );
            if (ShouldUseCache(requestFilters))
            {
                var cacheKey = BuildMentorCacheKey(requestFilters);
                await _cacheService.SetAsync(cacheKey, response, cancellationToken);
            }
            return response;

        }
        public async Task<PaginatedList<MentorProfileResponse>> MentorsBySpecializationAsync(RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            if (ShouldUseCache(requestFilters))
            {
                var cacheKey = BuildMentorCacheKey(requestFilters);
                var cachedData = await _cacheService.GetAsync<PaginatedList<MentorProfileResponse>>(cacheKey, cancellationToken);

                if (cachedData is not null)
                    return cachedData;
            }

            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        join f in _context.Fields on u.FieldId equals f.Id
                        join s in _context.Specializations on f.SpecializationId equals s.Id
                        where r.Name == DefaultRole.Mentor
                        select new { u, s, f };

            var mentorsWithFields = from us in query
                                    where !us.u.IsDisabled && us.u.EmailConfirmed
                                    && (string.IsNullOrEmpty(requestFilters.SearchValue)
                                    || us.s.Name == requestFilters.SearchValue)
                                    select new
                                    {
                                        us.u.Id,
                                        us.u.Name,
                                        us.u.Email,
                                        us.u.PathPhoto,
                                        us.u.PirthDate,
                                        us.u.Location,
                                        us.u.Rate,
                                        us.u.PriceOfSession,
                                        us.u.NumberOfSession,
                                        us.u.NumberOfExperience,
                                        us.u.About,
                                        us.f.FieldName
                                    };
            var countNumber = from u in mentorsWithFields
                              join s in _context.Sessions on u.Id equals s.MentorId
                              where s.Comment != null
                              group s by s.MentorId into g
                              select new
                              {
                                  g.Key,
                                  NumberOfComment = g.Count()
                              };
            
            var allowedSortFields = new[] { "Name", "Rate", "PriceOfSession" };
            var sortBy = allowedSortFields.Contains(requestFilters.SortBy) ? requestFilters.SortBy : "Rate";

            var finalList = from m in mentorsWithFields
                            orderby $"{sortBy} {requestFilters.SortDirection}"
                            select new MentorProfileResponse(
                                m.Id,
                                m.Name,
                                m.Email!,
                                m.PathPhoto,
                                m.PirthDate,
                                m.Location,
                                m.Rate,
                                m.PriceOfSession,
                                m.NumberOfSession,
                                m.NumberOfExperience,
                                countNumber.Where(c => c.Key == m.Id).Select(c => c.NumberOfComment).FirstOrDefault(),
                                m.About,
                                m.FieldName
                            ); 

            var response = await PaginatedList<MentorProfileResponse>.CreateAsync(
                finalList.AsNoTracking(),
                requestFilters.PageNumber,
                requestFilters.PageSize,
                cancellationToken
            );
            if (ShouldUseCache(requestFilters))
            {
                var cacheKey = BuildMentorCacheKey(requestFilters);
                await _cacheService.SetAsync(cacheKey, response, cancellationToken);
            }

            return response;
        }
        public async Task<Result<PaginatedList<MentorProfileResponse>>> GetRecomendedMentors(string userId ,RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            var FieldIntersted = (from f in _context.Fields
                                  join fi in _context.MenteeFields on f.Id equals fi.FieldId
                                  where fi.MenteeId == userId
                                  select f.FieldName).ToListAsync(cancellationToken);
            var Body = new
            {
                mentee_id = userId,
                m_skills = await FieldIntersted
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://mentorea-api-production.up.railway.app/recommend");
            request.Content = new StringContent(JsonSerializer.Serialize(Body), System.Text.Encoding.UTF8, "application/json");
            var responseUrl = await _httpClient.SendAsync(request, cancellationToken);
            if (!responseUrl.IsSuccessStatusCode)
            {
                return Result.Failure<PaginatedList<MentorProfileResponse>>(ResultError.FailedToGetMentors);
            }
            var responseData = await responseUrl.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<RecomendationResponse>(responseData);
            var mentorIds = data!.recommendations!.Select(r => r.mentor_id).ToList();

            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        where ur.RoleId == DefaultRole.MentorRoleId
                        select u;

            var mentorsWithFields = from u in query
                                    join f in _context.Fields on u.FieldId equals f.Id
                                    where !u.IsDisabled && u.EmailConfirmed && mentorIds.Contains(u.Id)      
                                    select new
                                    {
                                        u.Id,
                                        u.Name,
                                        u.Email,
                                        u.PathPhoto,
                                        u.PirthDate,
                                        u.Location,
                                        u.Rate,
                                        u.PriceOfSession,
                                        u.NumberOfExperience,
                                        u.NumberOfSession,
                                        u.About,
                                        f.FieldName
                                    };
            var countNumber = from u in mentorsWithFields
                              join s in _context.Sessions on u.Id equals s.MentorId
                              where s.Comment != null
                              group s by s.MentorId into g
                              select new
                              {
                                  g.Key,
                                  NumberOfComment = g.Count()
                              };
            
            var finalList = mentorsWithFields
                .Select(x => new MentorProfileResponse(
                    x.Id,
                    x.Name,
                    x.Email!,
                    x.PathPhoto,
                    x.PirthDate,
                    x.Location,
                    x.Rate,
                    x.PriceOfSession,
                    x.NumberOfSession,
                    x.NumberOfExperience,
                    countNumber.Where(c => c.Key == x.Id).Select(c => c.NumberOfComment).FirstOrDefault(),
                    x.About,
                    x.FieldName
                ));

            var response = await PaginatedList<MentorProfileResponse>.CreateAsync(
                finalList.AsNoTracking(),
                requestFilters.PageNumber,
                requestFilters.PageSize,
                cancellationToken
            );

            var finalMentor = mentorIds.Select(id => response.Items.FirstOrDefault(m => m.Id == id))
                                         .Where(m => m != null)
                                         .ToList();
            var finalResponse = new PaginatedList<MentorProfileResponse>(finalMentor!, mentorIds.Count, requestFilters.PageNumber, requestFilters.PageSize, cancellationToken);
            return Result.Success(finalResponse);
        }
        private static string BuildMentorCacheKey(RequestFilters filters)
        {
            var sortBy = string.IsNullOrEmpty(filters.SortBy) ? "Rate" : filters.SortBy;
            var sortDirection = filters.SortDirection;
            var search = string.IsNullOrEmpty(filters.SearchValue) ? "all" : filters.SearchValue.ToLower();

            return $"mentors:search={search}:sort={sortBy}:direction={sortDirection}:page={filters.PageNumber}:size={filters.PageSize}";
        }
        private static bool ShouldUseCache(RequestFilters filters)
        {
            return
                filters.PageNumber >= 1 && filters.PageNumber <= 3 &&
                string.IsNullOrEmpty(filters.SearchValue) &&
                (string.IsNullOrEmpty(filters.SortBy) || filters.SortBy == "Rate") &&
                (filters.SortDirection == "DESC");
        }

    }
}
