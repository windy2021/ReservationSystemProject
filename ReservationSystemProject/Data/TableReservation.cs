namespace ReservationSystemProject.Data
{
    public class TableReservation
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
    }
}