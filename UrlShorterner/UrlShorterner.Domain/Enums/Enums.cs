using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Domain.Enums
{
    public enum Enums { User, Admin }
    public enum SubscriptionTier { Standard, Plus }
    public enum SubscriptionStatus { Active, Cancelled, Expired, PastDue }
    public enum VtStatus { Unchecked, Pending, Clean, Malicious, Unresolved }
    public enum ReportStatus { Pending, Reviewed, ActionTaken, Dismissed }
    public enum ReportReason { Malicious, Phishing, Spam, Illegal, Other }
    public enum UserRole { User, Admin }
    public enum VerificationCodeType { EmailVerification, PasswordReset }
}
