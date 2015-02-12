using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SeeffProspectingAuthService
{
    [ServiceContract]
    public interface ISeeffProspectingAuthService
    {
        [OperationContract]
        ProspectingUserAuthPacket GetUserInfo(Guid userGuid);

        [OperationContract]
        decimal DebitUserBalance(decimal amount, Guid userGuid);

        [OperationContract]
        decimal CreditUserBalance(decimal amount, Guid userGuid);
    }
}
