using Dal.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Cruds
{
    public class ScheduleLogic : AbstractBasicCrudLogic<Schedule>, IScheduleLogic
    {
        private readonly IScheduleDal _scheduleDal;

        public ScheduleLogic(IScheduleDal scheduleDal)
        {
            _scheduleDal = scheduleDal;
        }
        
        protected override IBasicCrudDal<Schedule> ResolveDal()
        {
            return _scheduleDal;
        }
    }
}