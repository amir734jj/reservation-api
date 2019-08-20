using Api.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Api.Controllers.Api
{
    public class ReservationController : AbstractBasicCrudController<Reservation>
    {
        private readonly IReservationLogic _reservationLogic;

        public ReservationController(IReservationLogic reservationLogic)
        {
            _reservationLogic = reservationLogic;
        }
        
        protected override IBasicCrudLogic<Reservation> ResolveLogic()
        {
            return _reservationLogic;
        }
    }
}