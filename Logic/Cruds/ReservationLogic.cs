using Dal.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Cruds
{
    public class ReservationLogic : AbstractBasicCrudLogic<Reservation>, IReservationLogic
    {
        private readonly IReservationDal _reservationDal;

        public ReservationLogic(IReservationDal reservationDal)
        {
            _reservationDal = reservationDal;
        }
        
        protected override IBasicCrudDal<Reservation> ResolveDal()
        {
            return _reservationDal;
        }
    }
}