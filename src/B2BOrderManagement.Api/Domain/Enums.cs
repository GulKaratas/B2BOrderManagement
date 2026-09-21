namespace B2BOrderManagement.Api.Domain.Entities;

public enum SupplierStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3
}

public enum ProductStatus
{
    Active = 1,
    Inactive = 2,
    Discontinued = 3
}

public enum CustomerStatus
{
    Active = 1,
    Inactive = 2
}

public enum OrderStatus
{
    Draft = 1,
    Submitted = 2,
    Confirmed = 3,
    Rejected = 4,
    Shipped = 5,
    Cancelled = 6
}

public enum IntegrationStatus
{
    Success = 1,
    Failed = 2
}
