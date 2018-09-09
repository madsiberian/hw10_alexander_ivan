using System.Collections.Generic;

namespace FeatureBan.Domain
{
    public interface IBoard
    {
        void AssignTicket(Ticket ticket, string playerId);
        void BlockTicket(Ticket ticket);
        Ticket GetTicketByName(string name);
        void MoveTicketForward(Ticket ticket);
        void UnblockTicket(Ticket ticket);
        IEnumerable<Ticket> OpenTickets { get; }
        IEnumerable<Ticket> TicketsInDev { get; }
        IEnumerable<Ticket> TicketsInTest { get; }
        IEnumerable<Ticket> DoneTickets { get; }
    }
}