-- Migration: Add Meeting table
-- Created: 2025-11-27
-- Bảng riêng biệt để quản lý meetings của user

CREATE TABLE `meeting` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `title` VARCHAR(255) NOT NULL COMMENT 'Tên cuộc họp',
    `description` TEXT NULL COMMENT 'Mô tả chi tiết cuộc họp',
    `start_time` DATETIME NOT NULL COMMENT 'Thời gian bắt đầu',
    `end_time` DATETIME NOT NULL COMMENT 'Thời gian kết thúc',
    `meeting_link` VARCHAR(500) NULL COMMENT 'Link tham gia meeting (Google Meet, Zoom, etc)',
    `user_id` BIGINT NOT NULL COMMENT 'User tạo và sở hữu meeting này',
    `attendees` JSON NOT NULL DEFAULT ('[]') COMMENT 'Danh sách email người tham gia dạng JSON array',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` DATETIME NULL COMMENT 'Soft delete timestamp',
    
    PRIMARY KEY (`id`),
    INDEX `idx_meeting_user` (`user_id`, `start_time`),
    INDEX `idx_meeting_deleted` (`deleted_at`),
    INDEX `idx_meeting_time` (`start_time`, `end_time`),
    
    CONSTRAINT `fk_meeting_user` 
        FOREIGN KEY (`user_id`) 
        REFERENCES `user`(`id`) 
        ON DELETE CASCADE
        
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Bảng lưu thông tin cuộc họp của user';
