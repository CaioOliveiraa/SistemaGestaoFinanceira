export interface UserResponseDto {
    id: string;
    name: string;
    email: string;
    type: 'Admin' | 'Common';
}
