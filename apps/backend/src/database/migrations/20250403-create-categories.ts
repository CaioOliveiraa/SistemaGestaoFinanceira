import { QueryInterface, DataTypes } from 'sequelize';
import { AllowNull } from 'sequelize-typescript';

export = {
    async up(QueryInterface: QueryInterface) {
        await QueryInterface.createTable('categories', {
            id: {
                type: DataTypes.UUID,
                allowNull: false,
                primaryKey: true,
                defaultValue: DataTypes.UUIDV4,
            },
            name: {
                type: DataTypes.STRING,
                allowNull: false,
            },
            type: {
                type: DataTypes.ENUM('income', 'expense'),
                allowNull: false,
            },
            fixed: {
                type: DataTypes.BOOLEAN,
                allowNull: false,
                defaultValue: false,
            },
            user_id: {
                type: DataTypes.UUID,
                allowNull: true,
                references: {
                    model: 'users',
                    key: 'id',
                },
                onDelete: 'CASCADE',
                onUpdate: 'CASCADE',
            },
            created_at: {
                allowNull: false,
                type: DataTypes.DATE,
                defaultValue: DataTypes.NOW,
            },
            update_at: {
                allowNull: false,
                type: DataTypes.DATE,
                defaultValue: DataTypes.NOW,
            },
        });
    },

    async down(QueryInterface: QueryInterface) {
        await QueryInterface.dropTable('categories');
    },
};
