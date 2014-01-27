using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace TeamBins.DataAccess
{
    public class OperationStatus
    {
       

        public int OperationID { get; set; }

        public string ExceptionMessage { set; get; }
        public string ExceptionInnerMessage { set; get; }
        public string ExceptionStackTrace { set; get; }
        public string InnerExceptionStackTrace { set; get; }
        public bool Status { set; get; }
        public string Message { set; get; }
       

        public static OperationStatus CreateFromException(string message, Exception ex)
        {
            OperationStatus opStatus = new OperationStatus
            {
                Status = false,
                Message = message
            };
            if (ex != null)
            {
                opStatus.ExceptionMessage = ex.Message;
                opStatus.ExceptionStackTrace = ex.StackTrace;
                opStatus.ExceptionInnerMessage = ex.InnerException == null ? "" : ex.InnerException.ToString();
                opStatus.InnerExceptionStackTrace = ex.InnerException == null ? "" : ex.InnerException.StackTrace;
            }
            return opStatus;

        }
        public static OperationStatus CreateFromException(string message, DbEntityValidationException ex)
        {
            OperationStatus opStatus = new OperationStatus
            {
                Status = false,
                Message = message
            };
            if (ex != null)
            {
                var validationMsgs = ex.EntityValidationErrors.SelectMany(s => s.ValidationErrors).Select(s => s.ErrorMessage);
                if (validationMsgs != null)
                {
                    var strErrMsgs = string.Join(";", validationMsgs);
                    opStatus.ExceptionMessage = ex.Message;
                    opStatus.ExceptionMessage = String.Format("{0} Validation errors : {1}", opStatus.ExceptionMessage, strErrMsgs);
                    opStatus.ExceptionStackTrace = ex.StackTrace;
                    opStatus.ExceptionInnerMessage = ex.InnerException == null ? "" : ex.InnerException.ToString();
                    opStatus.InnerExceptionStackTrace = ex.InnerException == null ? "" : ex.InnerException.StackTrace;
                }
            }
            return opStatus;

        }

    }
}
