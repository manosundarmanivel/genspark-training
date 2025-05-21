https://www.postgresql.org/docs/current/backup.html
https://www.postgresql.org/docs/current/runtime-config-replication.html
 
 
Objective:
Create a stored procedure that inserts rental data on the primary server, and verify that changes replicate to the standby server. Add a logging mechanism to track each operation.
 
Tasks to Complete:
Set up streaming replication (if not already done):
 
Primary on port 5432
 
Standby on port 5433
 
Create a table on the primary:
 
 
CREATE TABLE rental_log (
    log_id SERIAL PRIMARY KEY,
    rental_time TIMESTAMP,
    customer_id INT,
    film_id INT,
    amount NUMERIC,
    logged_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
Ensure this table is replicated.
 
Write a stored procedure to:
 
Insert a new rental log entry
 
Accept customer_id, film_id, amount as inputs
 
Wrap logic in a transaction with error handling (BEGIN...EXCEPTION...END)
 
 
CREATE OR REPLACE PROCEDURE sp_add_rental_log(
    p_customer_id INT,
    p_film_id INT,
    p_amount NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO rental_log (rental_time, customer_id, film_id, amount)
    VALUES (CURRENT_TIMESTAMP, p_customer_id, p_film_id, p_amount);
EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error occurred: %', SQLERRM;
END;
$$;
Call the procedure on the primary:
 
 
CALL sp_add_rental_log(1, 100, 4.99);
On the standby (port 5433):
 
Confirm that the new record appears in rental_log
 
Run:SELECT * FROM rental_log ORDER BY log_id DESC LIMIT 1;
 
Add a trigger to log any UPDATE to rental_log
 
 
1. Create the rental_log table in primary server
 
CREATE TABLE rental_log (
    log_id SERIAL PRIMARY key,
    rental_time TIMESTAMP,
    customer_id INT,
    film_id INT,
    amount NUMERIC,
    logged_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
)
 
2. create the procedure in the primary server to insert the log
 
CREATE OR REPLACE PROCEDURE sp_add_rental_log(
    p_customer_id INT,
    p_film_id INT,
    p_amount NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO rental_log (rental_time, customer_id, film_id, amount)
    VALUES (CURRENT_TIMESTAMP, p_customer_id, p_film_id, p_amount);
EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error occurred: %', SQLERRM;
END;
$$;
 
3.call the procedure in primary
 
CALL sp_add_rental_log(1, 100, 4.99);
 
4.Add Trigger for Update Logging
 
CREATE TABLE rental_log_updates (
    update_id SERIAL PRIMARY KEY,
    log_id INT,
    old_amount NUMERIC,
    new_amount NUMERIC,
    updated_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
 
CREATE OR REPLACE FUNCTION log_rental_update()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO rental_log_updates (log_id, old_amount, new_amount)
    VALUES (OLD.log_id, OLD.amount, NEW.amount);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
 
 
CREATE TRIGGER trg_log_rental_update
AFTER UPDATE ON rental_log
FOR EACH ROW
WHEN (OLD.amount IS DISTINCT FROM NEW.amount)
EXECUTE FUNCTION log_rental_update();
 
 
UPDATE rental_log SET amount = 6.99 WHERE log_id = 1;
 
SELECT * FROM rental_log_updates ;
SELECT * FROM rental_log;
 
check in both servers
 
 
https://www.postgresql.org/docs/current/backup.html
https://www.postgresql.org/docs/current/runtime-config-replication.html
