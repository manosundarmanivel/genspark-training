/*
Locking Mechanism
PostgreSQL automatically applies locks, but you can control them manually when needed.

Types of Locks

MVCC VS Locks
MVCC allows readers and writers to work together without blocking.
Locks are needed when multiple writers try to touch the same row or table.

Simple Rule of Locks
Readers don’t block each other.
Writers block other writers on the same row.


Row-Level Locking (Default Behavior) / Implicit Lock
Two Users updating the same row
-- Trans A
*/
BEGIN;
UPDATE products
SET price = 500
WHERE id = 1;
-- Trans A holds a lock on row id = 1

-- Trans B
BEGIN;
UPDATE products
SET price = 600
WHERE id = 1;

/*
Result:
B waits until A commits or rollbacks
Row Level Locking
*/

-- Table-Level Locks / Explicit Table Lock
1. ACCESS SHARE -- select
-- Allows reads and writes

2. ROW SHARE
-- SELECT ... FOR UPDATE -> lock the selected row for later update

BEGIN;
LOCK TABLE products
IN ACCESS SHARE MODE;
-- Allows other SELECTS, even INSERT/DELETE at the same time.

BEGIN;
LOCK TABLE products
IN ROW SHARE MODE;
-- SELECT .. FOR UPDATE, reads are allowed, conflicting row locks are blocked, writes allowed

3. EXCLUSIVE
-- Blocks writes (INSERT, UPDATE, DELETE) but allows reads (SELECT)

BEGIN;
LOCK TABLE products
IN EXCLUSIVE MODE;

4. ACCESS EXCLUSIVE  -- Most agressive lock 
-- Blocks everything, Used by ALTER TABLE, DROP TABLE, TRUNCATE, 
-- Internally used by DDL.


-- A
BEGIN;
LOCK TABLE products IN ACCESS EXCLUSIVE MODE;
-- Table is now fully locked!

-- B
SLEECT * FROM products;
-- B will wait until A commits or rollbacks.

-- Explicit Row Locks --> SELECT ... FOR UPDATE
-- A
BEGIN;
SELECT * FROM products
WHERE id = 1
FOR UPDATE;
-- Row id = 1 is now locked

-- B
BEGIN;
UPDATE products
SET price = 700
WHERE id = 1;
-- B is blocked until A finishes.

-- SELECT ... FOR UPDATE locks the row early so no one can change it midway.
-- Banking, Ticket Booking, Inventory Management Systems
/*
A deadlock happens when:
Transaction A waits for B
Transaction B waits for A
They both wait forever.

-- Trans A
*/
BEGIN;
UPDATE products
SET price = 500
WHERE id = 1;
-- A locks row 1

-- Trans B
BEGIN;
UPDATE products
SET price = 600
WHERE id = 2;
-- B locks row 2

-- Trans A
UPDATE products
SET price = 500
WHERE id = 2;
-- A locks row 2 (already locked by B)

-- Trans B
UPDATE products
SET price = 600
WHERE id = 1
--B locks row 1 (already locked by A)

/*
psql detects a deadlock
ERROR: deadlock detected
It automatically aborts a transaction to resolve deadlock.
*/

-- Advisory Lock / Custom Locks
-- Get a lock with ID 12345
SELECT pg_advisory_lock(12345);

-- critical ops

-- Releas the lock

---------------------------------------------------------------------------------------------------------
-- 13 May 2025 - Task


DROP TABLE IF EXISTS accounts;

CREATE TABLE accounts (
    id SERIAL PRIMARY KEY,
    name TEXT,
    balance INT
);

INSERT INTO accounts (name, balance) VALUES
('Alice', 1000),
('Bob', 1000);

-- 1. Try two concurrent updates to same row → see lock in action.

BEGIN;
UPDATE accounts SET balance = balance + 100 WHERE id = 1;
-- Do not commit yet.

BEGIN;
UPDATE accounts SET balance = balance - 50 WHERE id = 1;
-- This will wait/block until Session A commits or rolls back.


-- 2. Write a query using SELECT...FOR UPDATE and check how it locks row.
BEGIN;
SELECT * FROM accounts WHERE id = 1 FOR UPDATE;
-- Locks row with id = 1

BEGIN;
UPDATE accounts SET balance = balance + 100 WHERE id = 1;
-- Will block until Session A commits or rolls back

-- 3. Intentionally create a deadlock and observe PostgreSQL cancel one transaction.
BEGIN;
UPDATE accounts SET balance = balance + 10 WHERE id = 1;

UPDATE accounts SET balance = balance + 10 WHERE id = 2;
-- Waiting for B


BEGIN;
UPDATE accounts SET balance = balance + 10 WHERE id = 2;

UPDATE accounts SET balance = balance + 10 WHERE id = 1;
-- DEADLOCK will occur, one session will be canceled automatically.


-- 4. Use pg_locks query to monitor active locks.
SELECT pid, mode, relation::regclass, page, tuple, virtualtransaction, granted
FROM pg_locks
WHERE relation IS NOT NULL;

-- 5. Explore about Lock Modes.


-------------------------------------------------------------------------------------------------------------------------

--Trigger

create table audit_log
(audit_id serial primary key,
table_name text,
field_name text,
old_value text,
new_value text,
updated_date Timestamp default current_Timestamp)

create or replace function Update_Audit_log()
returns trigger 
as $$
begin
	Insert into audit_log(table_name,field_name,old_value,new_value,updated_date) 
	values('customer','email',OLD.email,NEW.email,current_Timestamp);
	return new;
end;
$$ language plpgsql


create trigger trg_log_customer_email_Change
before update
on customer
for each row
execute function Update_Audit_log('email');


drop trigger trg_log_customer_email_Change on customer
drop trigger trg_log_customer_last_name_Change on customer
drop table audit_log;

select * from customer order by customer_id

select * from audit_log

update customer set email = 'mary.smith@sakilacustomer.org' where customer_id = 1


-------------------------------------------------------------------------------------------------

--trigger parameter 

CREATE OR REPLACE FUNCTION update_audit_log_dynamic()
RETURNS TRIGGER AS $$
DECLARE
   col_name TEXT := TG_ARGV[0];
   tab_name TEXT := TG_ARGV[1];
   o_value TEXT;
   n_value TEXT;
BEGIN
   -- JSON to safely access dynamic column
   o_value := row_to_json(OLD)::json ->> col_name;
   n_value := row_to_json(NEW)::json ->> col_name;

   IF o_value IS DISTINCT FROM n_value THEN
      INSERT INTO audit_log(table_name, field_name, old_value, new_value, updated_date)
      VALUES(tab_name, col_name, o_value, n_value, current_timestamp);
   END IF;

   RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER trg_log_customer_email_Change
BEFORE UPDATE ON customer
FOR EACH ROW
EXECUTE FUNCTION update_audit_log_dynamic('email', 'customer');

CREATE TRIGGER trg_log_customer_last_name_Change
BEFORE UPDATE ON customer
FOR EACH ROW
EXECUTE FUNCTION update_audit_log_dynamic('last_name', 'customer');

UPDATE customer SET email = 'new.email@example.com' WHERE customer_id = 1;

UPDATE customer SET last_name = 'newlastname' WHERE customer_id = 2;

SELECT * FROM audit_log;




