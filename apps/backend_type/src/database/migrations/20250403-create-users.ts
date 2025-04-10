'use strict';

import { QueryInterface, DataTypes } from 'sequelize';
import { UserType } from '../../types/user.type';

export = {
    async up(QueryInterface: QueryInterface) {
        await QueryInterface.createTable('users', {
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
            email: {
                type: DataTypes.STRING,
                allowNull: false,
                unique: true,
            },
            passwordHash: {
                type: DataTypes.STRING,
                allowNull: false,
                field: 'password_hash',
            },
            type: {
                type: DataTypes.ENUM(...Object.values(UserType)),
                allowNull: false,
                defaultValue: UserType.COMMON,
            },
            otpSecret: {
                type: DataTypes.STRING,
                allowNull: true,
                field: 'otp_secret',
            },
            oauthProvider: {
                type: DataTypes.STRING,
                allowNull: true,
                field: 'oauth_provider',
            },
            oauthId: {
                type: DataTypes.STRING,
                allowNull: true,
                field: 'oauth_id',
            },
            createdAt: {
                type: DataTypes.DATE,
                allowNull: false,
                defaultValue: DataTypes.NOW,
            },
            updatedAt: {
                type: DataTypes.DATE,
                allowNull: false,
                defaultValue: DataTypes.NOW,
            },
        });
    },
    async down(QueryInterface: QueryInterface) {
        await QueryInterface.dropTable('users');
    },
};
