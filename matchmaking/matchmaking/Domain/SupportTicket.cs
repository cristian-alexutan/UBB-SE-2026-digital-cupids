using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Domain
{
    internal class SupportTicket
    {
        public string Email { get; set; }
        public string PartnerName { get; set; }
        public string MarriageCertificatePath { get; set; }
        public string PartnerPhotoPath { get; set; }
        public bool IsResolved { get; set; }

        public SupportTicket(string email, string partnerName, string marriageCertificatePath, string partnerPhotoPath, bool isResolved)
        {
            Email = email;
            PartnerName = partnerName;
            MarriageCertificatePath = marriageCertificatePath;
            PartnerPhotoPath = partnerPhotoPath;
            IsResolved = isResolved;
        }
    }
}
