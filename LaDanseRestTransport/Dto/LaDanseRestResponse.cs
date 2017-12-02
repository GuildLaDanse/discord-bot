using System;

namespace LaDanseRestTransport.Dto
{  
    public class LaDanseRestResponse
    {
        public LaDanseRestResponse()
        {
            IsSuccess = true;
            ErrorResponse = null;
            Exception = null;
        }
        
        public LaDanseRestResponse(ErrorResponse errorResponse)
        {
            IsSuccess = false;
            ErrorResponse = errorResponse;
            Exception = null;
        }
        
        public LaDanseRestResponse(Exception e)
        {
            IsSuccess = false;
            ErrorResponse = null;
            Exception = Exception;
        }

        public ErrorResponse? ErrorResponse { get; }
        
        public bool IsSuccess { get; }

        public Exception Exception { get; }
    }

    public class LaDanseRestResponse<TBody> : LaDanseRestResponse
    {
        public LaDanseRestResponse() : base()
        {
        }
        
        public LaDanseRestResponse(TBody body)
        {
            Body = body;
        }
        
        public LaDanseRestResponse(ErrorResponse errorResponse) : base(errorResponse)
        {
        }
        
        public LaDanseRestResponse(Exception e) : base(e)
        {
        }
        
        public TBody Body { get; }
    }
}