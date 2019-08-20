using Api.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Api.Controllers.Api
{
    public class ScheduleController : AbstractBasicCrudController<Schedule>
    {
        private readonly IScheduleLogic _scheduleLogic;

        public ScheduleController(IScheduleLogic scheduleLogic)
        {
            _scheduleLogic = scheduleLogic;
        }
        
        protected override IBasicCrudLogic<Schedule> ResolveLogic()
        {
            return _scheduleLogic;
        }
    }
}