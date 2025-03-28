namespace API.Enum.Responses
{
    public enum RepositoryStatus
    {
        Success,
        Failed,
        NullObject,
        NotFound,
        InvalidStatus,
        InvalidStatusTransition,
        InvalidId,
        InvalidRole,
        InvalidDate,
        InvalidReturnDate,
        InvalidQuantity,
        CannotDelete,
        BookNotAvailable,
        BookNotFound,
        Availaible,
        AlreadyInRole,
        RoleRemovedFailed,
        RoleUpdatedFailed,
        FailedToResetPassword
    }
}
