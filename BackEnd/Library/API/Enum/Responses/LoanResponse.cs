namespace API.Enum.Responses
{
    public enum LoanResponse
    {
        NullObject,
        NotFound,
        InvalidDate,
        InvalidStatus,
        InvalidReturnDate,
        InvalidStatusTransition,
        BookNotFound,
        BookNotAvailable,
        Success,
        AlreadyReturned,
        CannotDelete,
        CannotExtend,
        StatusUpdated,
        LoanExpired
    }
}
