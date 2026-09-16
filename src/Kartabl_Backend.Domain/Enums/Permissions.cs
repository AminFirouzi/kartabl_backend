namespace Kartabl_Backend.Domain.Enums;

public enum Permission
{
    DocumentRead = 1,
    DocumentCreate = 2,
    DocumentUpdate = 3,
    DocumentDelete = 4,

    
    DocumentVerify = 10,
    DocumentReject = 11,
    DocumentApprove = 12,

    // User Management
    UserRead = 20,
    UserCreate = 21,
    UserUpdate = 22,
    UserDelete = 23,

    RoleRead = 30,
    RoleManage = 31,

}