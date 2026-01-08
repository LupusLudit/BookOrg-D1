/*
Project Name: BookOrg
Author: Michal Bielina
Contact: michal.bielina@centrum.cz
Description: SQL database export containing schema definition (DDL)
             and test data (DML) for a library management system.
Target Sever: Microsoft SQL Server
*/

begin transaction;

-- Table definitions

create table genre (
id int primary key identity(1,1),
genre_name varchar(50) not null unique
);

create table author (
id int primary key identity(1,1),
author_name varchar(100) not null unique
);

create table book (
id int primary key identity(1,1),
title varchar(100) not null,
is_available bit not null default 1,
price decimal(10,2) not null check (price >= 0),
books_in_stock int not null default 0 check (books_in_stock >= 0),
check ((books_in_stock = 0 and is_available = 0) or (books_in_stock > 0))
);

create table contribution (
id int primary key identity(1,1),
book_id int not null foreign key references book(id) on update cascade on delete cascade,
author_id int not null foreign key references author(id) on update cascade on delete cascade
);

create table classification (
id int primary key identity(1,1),
book_id int not null foreign key references book(id) on update cascade on delete cascade,
genre_id int not null foreign key references genre(id) on update cascade on delete cascade
);

create table customer (
id int primary key identity(1,1),
first_name varchar(100) not null,
last_name varchar(100) not null,
email varchar(100) check (email like '%_@__%.__%')
);

create table loan (
id int primary key identity(1,1),
book_id int not null foreign key references book(id) on update cascade on delete cascade,
customer_id int not null foreign key references customer(id) on update cascade on delete cascade,
total_loan_price decimal(10,2) not null check (total_loan_price >= 0),
loan_status varchar(10) not null check (loan_status in ('active','overdue','returned')),
loan_start_date date not null,
loan_due_date date not null,
loan_returned_date date,
check (loan_due_date > loan_start_date),
check (loan_returned_date is null or loan_returned_date >= loan_start_date),
check ((loan_status = 'returned' and loan_returned_date is not null) or (loan_status in ('active','overdue') and loan_returned_date is null))
);

commit;

-- View definitions

create view view_books as
select book.id, book.title, book.price, book.is_available, book.books_in_stock, 
(select string_agg(author.author_name, ', ')
from author join contribution on author.id = contribution.author_id where contribution.book_id = book.id) as authors,
(select string_agg(genre.genre_name, ', ')
from genre join classification on genre.id = classification.genre_id where classification.book_id = book.id) as genres
from book;


create view view_ongoing_loans as
select loan.id, loan.book_id, book.title as book_title, loan.customer_id,
customer.first_name + ' ' + customer.last_name as customer_name,
loan.total_loan_price, loan.loan_status, loan.loan_start_date, loan.loan_due_date, loan.loan_returned_date
from loan
join book on loan.book_id = book.id
join customer on loan.customer_id = customer.id
where loan.loan_status in ('active', 'overdue');

create view view_returned_loans as
select loan.id, loan.book_id, book.title as book_title, loan.customer_id,
customer.first_name + ' ' + customer.last_name as customer_name,
loan.total_loan_price, loan.loan_status, loan.loan_start_date, loan.loan_due_date, loan.loan_returned_date
from loan
join book on loan.book_id = book.id
join customer on loan.customer_id = customer.id
where loan.loan_status = 'returned';

begin transaction;

-- Example data

insert into genre (genre_name) values
('Science Fiction'),
('Fantasy'),
('History'),
('Programming'),
('Philosophy');

insert into author (author_name) values
('Isaac Asimov'),
('J. R. R. Tolkien'),
('Yuval Noah Harari'),
('Robert C. Martin'),
('Plato');

insert into book (title, is_available, price, books_in_stock) values
('Foundation', 1, 19.99, 5),
('The Lord of the Rings', 1, 29.99, 3),
('Sapiens: A Brief History of Humankind', 1, 24.50, 2),
('Clean Code', 1, 34.99, 4),
('The Republic', 0, 14.99, 0);

insert into contribution (book_id, author_id) values
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);

insert into classification (book_id, genre_id) values
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);

insert into customer (first_name, last_name, email) values
('John', 'Doe', 'john.doe@example.com'),
('Jane', 'Smith', 'jane.smith@work.org'),
('Alice', 'Johnson', 'alice.j@uni.edu');

-- 1. Active Loan
insert into loan (book_id, customer_id, total_loan_price, loan_status, loan_start_date, loan_due_date, loan_returned_date) 
values (1, 1, 19.99, 'active', getdate(), dateadd(day, 14, getdate()), null);

-- 2. Overdue Loan (Start date 30 days ago, due 16 days ago)
insert into loan (book_id, customer_id, total_loan_price, loan_status, loan_start_date, loan_due_date, loan_returned_date) 
values (2, 2, 29.99, 'overdue', dateadd(day, -30, getdate()), dateadd(day, -16, getdate()), null);

-- 3. Returned Loan
insert into loan (book_id, customer_id, total_loan_price, loan_status, loan_start_date, loan_due_date, loan_returned_date) 
values (3, 3, 24.50, 'returned', dateadd(day, -60, getdate()), dateadd(day, -46, getdate()), dateadd(day, -50, getdate()));

commit;