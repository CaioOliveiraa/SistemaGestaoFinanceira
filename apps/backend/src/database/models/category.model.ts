import {
    Table,
    Column,
    Model,
    DataType,
    PrimaryKey,
    Default,
    CreatedAt,
    UpdatedAt,
    ForeignKey,
    BelongsTo,
    HasMany,
} from 'sequelize-typescript';
import { v4 as uuidv4 } from 'uuid';
import { User } from './user.model';
import { Transaction } from './transaction.model';

@Table({ tableName: 'categories' })
export class Category extends Model<Category> {
    @PrimaryKey
    @Default(uuidv4)
    @Column(DataType.UUID)
    id!: string;

    @Column(DataType.STRING)
    name!: string;

    @Column({ type: DataType.BOOLEAN, defaultValue: false })
    fixed!: boolean;

    @ForeignKey(() => User)
    @Column(DataType.UUID)
    userId!: string | null;

    @BelongsTo(() => User)
    user?: User;

    @HasMany(() => Transaction)
    transactions?: Transaction[];

    @CreatedAt
    @Column({ field: 'created_at' })
    createdAt!: Date;

    @UpdatedAt
    @Column({ field: 'updated_at' })
    updatedAt!: Date;
}
