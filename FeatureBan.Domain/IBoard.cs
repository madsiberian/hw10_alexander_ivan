namespace FeatureBan.Domain
{
    public interface IBoard
    {
        void AssignTicket(Ticket ticket, string playerId);
        void BlockTicket(Ticket ticket);
        Ticket GetOpenTicket();
        Ticket GetTicketByName(string name);
        void MoveTicketForward(Ticket ticket);
        void UnblockTicket(Ticket ticket);
    }
}