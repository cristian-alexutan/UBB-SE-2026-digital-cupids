using matchmaking.Domain;
using matchmaking.Repositories;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Services
{
    internal class SupportTicketService
    {
        private SupportTicketRepository TicketRepo;
        
        public SupportTicketService(SupportTicketRepository ticketRepo)
        {
            TicketRepo = ticketRepo;
        }
    
        public void CreateTicket(SupportTicket ticket)
        {
            SupportTicket checkTicket = TicketRepo.FindByEmail(ticket.Email);
            if (checkTicket != null)
            {
                throw new Exception("Active ticket already exists!");
            }
  
            ticket.IsResolved = false;
            TicketRepo.Add(ticket);
        }

        public List<SupportTicket> GetAll()
        {
            return TicketRepo.GetAll();
        }

        private void SendEmail(string email, bool isFound)
        {
            string result = isFound ? "found" : "not found";
            Console.WriteLine($"Email sent to {email}: Your partner was {result}.");
        }

        public void ResolveTicket(string email, bool isFound)
        {
            SupportTicket checkTicket = TicketRepo.FindByEmail(email);
            if (checkTicket != null)
            {   
                checkTicket.IsResolved = true;
                TicketRepo.UpdateIsSolved(checkTicket.Email, true);
                SendEmail(email, isFound);
            }
        }
    }
}
