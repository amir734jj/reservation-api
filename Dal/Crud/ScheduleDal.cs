using AgileObjects.AgileMapper;
using Dal.Abstracts;
using Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Crud
{
    public class ScheduleDal : BasicCrudDalAbstract<Schedule>, IScheduleDal
    {
        private readonly EntityDbContext _dbContext;

        private readonly IMapper _mapper;

        public ScheduleDal(EntityDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        protected override DbContext GetDbContext()
        {
            return _dbContext;
        }

        protected override DbSet<Schedule> GetDbSet()
        {
            return _dbContext.Schedules;
        }

        protected override IMapper Mapper()
        {
            return _mapper;
        }
    }
}