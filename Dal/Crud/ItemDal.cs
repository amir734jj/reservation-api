using AgileObjects.AgileMapper;
using Dal.Abstracts;
using Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Crud
{
    public class ItemDal : BasicCrudDalAbstract<Item>, IItemDal
    {
        private readonly EntityDbContext _dbContext;

        private readonly IMapper _mapper;

        public ItemDal(EntityDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        protected override DbContext GetDbContext()
        {
            return _dbContext;
        }

        protected override DbSet<Item> GetDbSet()
        {
            return _dbContext.Items;
        }

        protected override IMapper Mapper()
        {
            return _mapper;
        }
    }
}