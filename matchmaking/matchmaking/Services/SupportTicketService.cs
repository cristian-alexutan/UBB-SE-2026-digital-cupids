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

        public List<SupportTicket> GetAllUnresolved()
        {
            List<SupportTicket> allTickets = TicketRepo.GetAll();
            List<SupportTicket> unresolvedTickets = allTickets.Where(t => !t.IsResolved).ToList();
            return unresolvedTickets;
        }

        private void SendEmail(string email, bool isFound)
        {
            string result = isFound ? "SPOUSE FOUND" : "SPOUSE NOT FOUND";

            using var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(App.Email, App.Password)
            };

            var message = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(App.Email),
                Subject = "Spouse Checker Result",
                Body = result
            };

            message.To.Add(email);
            client.Send(message);
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
