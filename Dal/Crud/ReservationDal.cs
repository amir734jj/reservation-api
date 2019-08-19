using AgileObjects.AgileMapper;
using Dal.Abstracts;
using Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Crud
{
    public class ReservationDal : BasicCrudDalAbstract<Reservation>, IReservationDal
    {
        private readonly EntityDbContext _dbContext;

        private readonly IMapper _mapper;

        public ReservationDal(EntityDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        protected override DbContext GetDbContext()
        {
            return _dbContext;
        }

        protected override DbSet<Reservation> GetDbSet()
        {
            return _dbContext.Reservations;
        }

        protected override IMapper Mapper()
        {
            return _mapper;
        }
    }
}