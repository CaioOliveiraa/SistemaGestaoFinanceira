import {
    Table,
    Column,
    Model,
    DataType,
    PrimaryKey,
    Default,
    CreatedAt,
    UpdatedAt,
    Unique,
    HasMany,
} from 'sequelize-typescript';
import { v4 as uuidv4 } from 'uuid';
import { UserType } from '../../types/user.type';
import { Category } from './category.model';
import { Transaction } from './transaction.model';

@Table({ tableName: 'users' })
export class User extends Model<User> {
    @PrimaryKey
    @Default(uuidv4)
    @Column(DataType.UUID)
    id!: string;

    @Column(DataType.STRING)
    name!: string;

    @Unique
    @Column(DataType.STRING)
    email!: string;

    @Column(DataType.STRING)
    passwordHash!: string;

    @Column({
        type: DataType.ENUM(...Object.values(UserType)),
        defaultValue: UserType.COMMON,
    })
    type!: UserType;

    @Column(DataType.STRING)
    otpSecret!: string | null;

    @Column(DataType.STRING)
    oauthProvider!: string | null;

    @Column(DataType.STRING)
    oauthId!: string | null;

    @HasMany(() => Category)
    categories!: Category[];

    @HasMany(() => Transaction)
    transactions!: Transaction[];

    @CreatedAt
    @Column({ field: 'created_at' })
    createdAt!: Date;

    @UpdatedAt
    @Column({ field: 'updated_at' })
    updatedAt!: Date;
}
