using Fresh_Farm_Market.Model;
using Fresh_Farm_Market.Models;

namespace Fresh_Farm_Market.Service
{
    public class AuditLogService
    {
        private readonly UserDbContext userDbContext;
        public AuditLogService(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }
        public void AddAudit(AuditLog audit)
        {
            userDbContext.auditLog.Add(audit);
            userDbContext.SaveChanges();
        }
    }
}
