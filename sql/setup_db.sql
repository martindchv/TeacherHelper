DROP DATABASE IF EXISTS `TeacherHelper`;
CREATE DATABASE `TeacherHelper`;
USE `TeacherHelper`;

CREATE TABLE `users`
(
	`id` INT PRIMARY KEY AUTO_INCREMENT,
	`username` varchar(30) NOT NULL UNIQUE,
	`name` nvarchar(40) NOT NULL,
	`email` varchar(40) NOT NULL UNIQUE,
	`password` varchar(64) NOT NULL,
	`is_admin` BOOL NOT NULL,
	`is_active` BOOL NOT NULL DEFAULT TRUE,
	`current_subject_id` INT,
	`profile_picture` BLOB
);

CREATE TABLE `subjects`
(
	`id` INT PRIMARY KEY AUTO_INCREMENT,
	`name` nvarchar(40) NOT NULL
); 

CREATE TABLE `users_subjects`
(
	`subject_id` INT,
	`user_id` INT,
	FOREIGN KEY(subject_id) REFERENCES subjects(id) ON DELETE CASCADE,
	FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE CASCADE,
	PRIMARY KEY(subject_id, user_id)
);

CREATE TABLE `themes`
(
	`id` INT PRIMARY KEY AUTO_INCREMENT,
	`name` NVARCHAR(40) NOT NULL,
	`subject_id` INT NOT NULL,
	`parent_theme_id` INT,
	`previous_theme_id` INT,
	FOREIGN KEY(subject_id) REFERENCES subjects(id) ON DELETE CASCADE,
	FOREIGN KEY(parent_theme_id) REFERENCES themes(id) ON DELETE CASCADE,
	FOREIGN KEY(previous_theme_id) REFERENCES themes(id) ON DELETE CASCADE
);

CREATE TABLE `tests`
(
	`id` INT PRIMARY KEY AUTO_INCREMENT,
	`class` nvarchar(10),
	`theme_id` INT NOT NULL,
	`author_id` INT NOT NULL,
	`is_public` BOOL NOT NULL,
	`date_created` DATETIME NOT NULL,
	FOREIGN KEY(theme_id) REFERENCES themes(id) ON DELETE CASCADE,
	FOREIGN KEY(author_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE `questions`
(
	`id` INT PRIMARY KEY AUTO_INCREMENT,
	`content` nvarchar(13356) NOT NULL,
	`type` CHAR(30) NOT NULL,
	`theme_id` INT NOT NULL,
	`is_public` BOOL NOT NULL,
	`user_id` INT NOT NULL,
	`right_answer_id` INT,
	FOREIGN KEY(theme_id) REFERENCES themes(id) ON DELETE CASCADE,
	FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE `tests_questions`
(
	`test_id` INT,
	`question_id` INT,
	FOREIGN KEY(test_id) REFERENCES tests(id) ON DELETE CASCADE,
	FOREIGN KEY(question_id) REFERENCES questions(id) ON DELETE CASCADE,
	PRIMARY KEY(test_id, question_id)
);

CREATE TABLE `answers`
(
	`id` INT PRIMARY KEY AUTO_INCREMENT,
	`content` nvarchar(13356) NOT NULL,
	`question_id` INT NOT NULL,
	FOREIGN KEY(question_id) REFERENCES questions(id) ON DELETE CASCADE
);

CREATE TABLE `assigned_tests`
(
	`id` INT PRIMARY KEY AUTO_INCREMENT,
	`user_id` INT NOT NULL,
	`test_id` INT,
	`class` nvarchar(10),
	`assigned_date` DATETIME NOT NULL,
	`date_created` DATETIME NOT NULL,
	FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE CASCADE, 
	FOREIGN KEY(test_id) REFERENCES tests(id) ON DELETE CASCADE
);

ALTER TABLE `questions`
ADD FOREIGN KEY(right_answer_id) REFERENCES answers(id) ON DELETE CASCADE;

ALTER TABLE `users`
ADD FOREIGN KEY(current_subject_id) REFERENCES subjects(id) ON DELETE SET NULL;

ALTER TABLE themes
ADD UNIQUE `unique_subject_theme`(`subject_id`, `parent_theme_id`, `name`);

ALTER TABLE tests_questions
ADD COLUMN question_place INT;