namespace LaDanseSiteConnector.Dto
{  
    public class LaDanseRestResponse
    {
        public LaDanseRestResponse()
        {
            IsSuccess = true;
            ErrorResponse = null;
        }
        
        public LaDanseRestResponse(ErrorResponse errorResponse)
        {
            IsSuccess = false;
            ErrorResponse = errorResponse;
        }

        public ErrorResponse? ErrorResponse { get; }
        
        public bool IsSuccess { get; }
    }

    public class LaDanseRestResponse<TBody> : LaDanseRestResponse
    {
        public LaDanseRestResponse(TBody body)
        {
            Body = body;
        }
        
        public LaDanseRestResponse(ErrorResponse errorResponse) : base(errorResponse)
        {
        }
        
        public TBody Body { get; }
    }
}