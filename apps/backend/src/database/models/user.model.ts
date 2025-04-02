import {
    Table,
    Column,
    Model,
    DataType,
    PrimaryKey,
    Default,
    CreatedAt,
    UpdatedAt,
} from 'sequelize-typescript';
import { v4 as uuidv4 } from 'uuid';

export enum UserType {
    ADMIN = 'admin',
    COMUM = 'comum',
}

@Table({ tableName: 'users' })
export class User extends Model<User> {
    @PrimaryKey
    @Default(uuidv4)
    @Column(DataType.UUID)
    id!: string;

    @Column(DataType.STRING)
    nome!: string;

    @Column({
        type: DataType.STRING,
        unique: true,
    })
    email!: string;

    @Column({
        type: DataType.STRING,
        field: 'senha_hash',
    })
    senhaHash!: string;

    @Column({
        type: DataType.ENUM(...Object.values(UserType)),
        defaultValue: UserType.COMUM,
    })
    tipo!: UserType;

    @Column({
        type: DataType.STRING,
        allowNull: true,
        field: 'otp_secret',
    })
    otpSecret!: string | null;

    @Column({
        type: DataType.STRING,
        allowNull: true,
        field: 'oauth_provider',
    })
    oauthProvider!: string | null;

    @Column({
        type: DataType.STRING,
        allowNull: true,
        field: 'oauth_id',
    })
    oauthId!: string | null;

    @CreatedAt
    @Column({ field: 'created_at' })
    createdAt!: Date;

    @UpdatedAt
    @Column({ field: 'updated_at' })
    updatedAt!: Date;
}
