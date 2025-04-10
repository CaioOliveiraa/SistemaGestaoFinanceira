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
} from 'sequelize-typescript';
import { v4 as uuidv4 } from 'uuid';
import { User } from './user.model';
import { Category } from './category.model';

@Table({ tableName: 'transactions' })
export class Transaction extends Model<Transaction> {
    @PrimaryKey
    @Default(uuidv4)
    @Column(DataType.UUID)
    id!: string;

    @ForeignKey(() => User)
    @Column(DataType.UUID)
    userId!: string;

    @BelongsTo(() => User)
    user!: User;

    @ForeignKey(() => Category)
    @Column(DataType.UUID)
    categoryId!: string;

    @BelongsTo(() => Category)
    category!: Category;

    @Column({
        type: DataType.ENUM('income', 'expense'),
        allowNull: false,
    })
    type!: 'income' | 'expense';

    @Column(DataType.DECIMAL(10, 2))
    amount!: number;

    @Column(DataType.STRING)
    description!: string;

    @Column(DataType.DATEONLY)
    date!: Date;

    @Column({ type: DataType.BOOLEAN, defaultValue: false })
    recurring!: boolean;

    @CreatedAt
    @Column({ field: 'created_at' })
    createdAt!: Date;

    @UpdatedAt
    @Column({ field: 'updated_at' })
    updatedAt!: Date;
}
