export interface User {
    userId?: string;
    userName: string;
    userCode: string;
    displayName?: string;
    email: string;
    password: string;
    isActive: boolean;
    avatarImageFileId?: string;
    roleId: string;
}
